using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Scripts.GameEditor.Audio;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.GameEditor.Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] public AudioMixerGroup MixerGroup;
        public Dictionary<string, Dictionary<int,AudioController>> AudioControllers { get; private set; }
        public Dictionary<string, AudioClip> AudioClips { get; private set; }
        public Dictionary<string, AudioSourceDTO> AudioData { get; private set; }

        protected override void Awake()
        {
            AudioControllers = new Dictionary<string, Dictionary<int, AudioController>>();
            AudioClips = new Dictionary<string, AudioClip>();
            AudioData = new Dictionary<string, AudioSourceDTO>();
            base.Awake();
        }

        /// <summary>
        /// </summary>
        /// <returns>Actual state of manager in ManagerDTO</returns>
        public ManagerDTO Get()
        {
            return new ManagerDTO(AudioData.Values.ToArray());
        }

        /// <summary>
        /// Sets actual state of manager based on ManagerDTO.
        /// </summary>
        /// <param name="managerDTO"></param>
        /// <returns></returns>
        public async Task Set(ManagerDTO managerDTO)
        {
            var names = AudioData.Keys.ToList();
            foreach (var name in names)
            {
                RemoveClip(name);
            }

            var tasks = new List<Task<bool>>();
            foreach (var data in managerDTO.Sources)
            {
                if (!( data is AudioSourceDTO ))
                {
                    OutputManager.Instance.ShowMessage("Data loading error, data might be corrupted!", "Audio manager");
                    continue;
                }
                tasks.Add(AddAudioClip((AudioSourceDTO)data));
            }
            await Task.WhenAll(tasks);
        }

        #region AudioClipMethods

        /// <summary>
        /// Loads audio clip based on given AudioSourceDTO (if there is not clip with same name).
        /// Clip and his DTO are than stored in AudioClips and AudioData.
        /// </summary>
        /// <param name="audioSourceDTO"></param>
        /// <returns></returns>
        public async Task<bool> AddAudioClip(AudioSourceDTO audioSourceDTO)
        {
            var name = audioSourceDTO.Name;
            if (AudioData.ContainsKey(name))
            {
                OutputManager.Instance.ShowMessage($"Audio clip with given name: {name} already exists!", "Audio manager");
                return false;
            }

            var result = await AudioLoader.LoadAudioClip(audioSourceDTO.URL);
            if (result)
            {
                AudioClips.Add(name, result);
                AudioData.Add(name, audioSourceDTO);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Rewrites audio clip of given name with new one, based on AudioSourceDTO.
        /// </summary>
        /// <param name="name">Name of old audio clip</param>
        /// <param name="audioSourceDTO">New audio clip desription.</param>
        /// <returns></returns>
        public void EditAudioClip(AudioSourceDTO audioSourceDTO) 
        {
            if (!AudioData.ContainsKey(name))
            {
                return;
            }

            AudioData[name] = audioSourceDTO;
            if (AudioControllers.ContainsKey(name))
            {
                foreach (var controller in AudioControllers[name].Values)
                {
                    controller.EditAudio(audioSourceDTO);
                }
            }
        }

        /// <summary>
        /// Removes audio clip with given name and all connected objects from manager data -> 
        /// (SourceDTO and all controller with this name.)
        /// </summary>
        /// <param name="name">Audio clip name.</param>
        public void RemoveClip(string name)
        {
            if (!AudioData.ContainsKey(name))
            {
                return;
            }

            if (AudioControllers.ContainsKey(name))
            {
                foreach (var controller in AudioControllers[name].Values)
                {
                    controller.RemoveClip();
                }
            }

            Destroy(AudioClips[name]);
            AudioClips.Remove(name);
            AudioData.Remove(name);
            AudioControllers.Remove(name);
        }

        /// <summary>
        /// Sets audio clip to a given object. If object has no audio controller, method will add one.
        /// This controller is than added to registered controllers in this manager.
        /// </summary>
        /// <param name="ob">Object to be set.</param>
        /// <param name="source"></param>
        /// <param name="playOnAwake">Should be clip played after set?</param>
        public void SetAudioClip(GameObject ob, SourceReference source, bool playOnAwake = true)
        {
            AudioController controller;
            if (!ob.TryGetComponent(out controller))
                controller = ob.AddComponent<AudioController>();

            SetAudioClip(controller, source, playOnAwake);
        }

        /// <summary>
        /// Sets audio clip to a given audio controller. If object has no audio controller, method will add one.
        /// This controller is than added to registered controllers in this manager.
        /// </summary>
        /// <param name="ob">Object to be set.</param>
        /// <param name="source"></param>
        /// <param name="playOnAwake">Should be clip played after set?</param>
        public void SetAudioClip(AudioController controller, SourceReference source, bool playOnAwake = true)
        {
            var name = source.Name;
            if (!AudioClips.ContainsKey(name))
                return;

            controller.SetAudioClip(name, playOnAwake);
            AddActiveController(name, controller);
        }
        #endregion

        #region ControllerMethods

        /// <summary>
        /// Adds audio controller as registered controller to manager as pair(clip name, controller)
        /// </summary>
        /// <param name="name">Audio clip name</param>
        /// <param name="controller">Audio controller</param>
        /// <returns></returns>
        public bool AddActiveController(string name, AudioController controller)
        {
            if (AudioClips.ContainsKey(name))
            {
                if (AudioControllers.ContainsKey(name))
                {
                    var instanceID = controller.GetInstanceID();
                    if (ContainsActiveController(name, instanceID))
                        return false;

                    AudioControllers[name].Add(instanceID, controller);
                }
                else
                {
                    AudioControllers.Add(name, new Dictionary<int, AudioController>
                    {
                        { controller.GetInstanceID(), controller }
                    });
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if manager registers controller with given instance id 
        /// of audio clip with given name.
        /// </summary>
        /// <param name="name">Audio clip name</param>
        /// <param name="instanceID">Audio controller instance id.</param>
        /// <returns></returns>
        public bool ContainsActiveController(string name, int instanceID)
        {
            if (AudioControllers[name].ContainsKey(instanceID))
                return true;
            return false;
        }

        /// <summary>
        /// Removes controller from registered controllers.
        /// </summary>
        /// <param name="name">Audio clip name</param>
        /// <param name="controller">Audio controller</param>
        /// <returns></returns>
        public bool RemoveActiveController(string name, AudioController controller)
        {
            if (AudioControllers.ContainsKey(name))
            {
                var id = controller.GetInstanceID();
                if (AudioControllers[name].ContainsKey(id))
                {
                    AudioControllers[name].Remove(id);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if there is audio clip with given name.
        /// </summary>
        /// <param name="name">Audio clip name.</param>
        /// <returns></returns>
        public bool ContainsName(string name)
        {
            return AudioClips.ContainsKey(name);
        }

        /// <summary>
        /// Plays all registered controllers with given audio clip names.
        /// </summary>
        /// <param name="names">List of audio clip names.</param>
        /// <returns></returns>
        public bool OnPlay(List<string> names)
        {
            return OnRestart(names);
        }

        /// <summary>
        /// Pauses all registered controllers with given audio clip names.
        /// </summary>
        /// <param name="names">List of audio clip names.</param>
        /// <returns></returns>
        public bool OnPause(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controllerGroup in AudioControllers.Values)
                {
                    foreach(var controller in controllerGroup.Values)
                        controller.Pause();
                }
                return true;
            }
            else
            {
                foreach (var name in names)
                {
                    if (!AudioControllers.ContainsKey(name))
                        return false;
                    
                    foreach (var controller in AudioControllers[name].Values)
                        controller.Pause();
                }
                return true;
            }
        }

        /// <summary>
        /// Resumes all paused registered controllers with given audio clip names.
        /// </summary>
        /// <param name="names">List of audio clip names.</param>
        /// <returns></returns>
        public bool OnResume(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controllerGroup in AudioControllers.Values)
                {
                    foreach (var controller in controllerGroup.Values)
                        controller.Resume();
                }
                return true;
            }
            else
            {
                foreach (var name in names)
                {
                    if (!AudioControllers.ContainsKey(name))
                        return false;

                    foreach (var controller in AudioControllers[name].Values)
                        controller.Resume();
                }
                return true;
            }
        }

        /// <summary>
        /// Pauses all registered controllers with given audio clip names.
        /// </summary>
        /// <param name="names">List of audio clip names.</param>
        /// <returns></returns>
        public bool OnRestart(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controllerGroup in AudioControllers.Values)
                {
                    foreach (var controller in controllerGroup.Values)
                        controller.ResetClip();
                }
                return true;
            }
            else
            {
                foreach (var name in names)
                {
                    if (!AudioControllers.ContainsKey(name))
                        return false;

                    foreach (var controller in AudioControllers[name].Values)
                        controller.ResetClip();
                }
                return true;
            }
        }

        /// <summary>
        /// Stops all registered controllers with given audio clip names.
        /// </summary>
        /// <param name="names">List of audio clip names.</param>
        /// <returns></returns>
        public bool OnStop(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controllerGroup in AudioControllers.Values)
                {
                    foreach (var controller in controllerGroup.Values)
                        controller.StopClip();
                }
                return true;
            }
            else
            {
                foreach (var name in names)
                {
                    if (!AudioControllers.ContainsKey(name))
                        return false;

                    foreach (var controller in AudioControllers[name].Values)
                        controller.StopClip();
                }
                return true;
            }
        }
        #endregion
    }
}
