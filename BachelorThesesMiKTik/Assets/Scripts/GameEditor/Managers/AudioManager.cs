using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Scripts.GameEditor.Audio;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        public Dictionary<string, List<AudioController>> AudioControllers { get; private set; }
        public Dictionary<string, AudioClip> AudioClips { get; private set; }
        public Dictionary<string, AudioSourceDTO> AudioData { get; private set; }

        protected override void Awake()
        {
            AudioControllers = new Dictionary<string, List<AudioController>>();
            AudioClips = new Dictionary<string, AudioClip>();
            AudioData = new Dictionary<string, AudioSourceDTO>();
            base.Awake();
        }

        public ManagerDTO Get()
        {
            return new ManagerDTO(AudioData.Values.ToArray());
        }

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
                    ErrorOutputManager.Instance.ShowMessage("Data loading error, data might be corrupted!", "Audio manager");
                    continue;
                }
                tasks.Add(AddAudioClip((AudioSourceDTO)data));
            }
            await Task.WhenAll(tasks);
        }

        #region AudioClipMethods
        public async Task<bool> AddAudioClip(AudioSourceDTO audioSourceDTO)
        {
            var name = audioSourceDTO.Name;
            if (AudioData.ContainsKey(name))
            {
                ErrorOutputManager.Instance.ShowMessage($"Audio clip with given name: {name} already exists!", "Audio manager");
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

        public void EditAudioClip(AudioSourceDTO audioSourceDTO) 
        {
            if (!AudioData.ContainsKey(name))
            {
                return;
            }

            AudioData[name] = audioSourceDTO;
            if (AudioControllers.ContainsKey(name))
            {
                foreach (var controller in AudioControllers[name])
                {
                    controller.EditAudio(audioSourceDTO);
                }
            }
        }

        public void RemoveClip(string name)
        {
            if (!AudioData.ContainsKey(name))
            {
                return;
            }

            if (AudioControllers.ContainsKey(name))
            {
                foreach (var controller in AudioControllers[name])
                {
                    controller.RemoveClip();
                }
            }

            Destroy(AudioClips[name]);
            AudioClips.Remove(name);
            AudioData.Remove(name);
            AudioControllers.Remove(name);
        }

        public void SetAudioClip(GameObject ob, SourceReference source, bool playOnAwake = true)
        {
            AudioController controller;
            if (!ob.TryGetComponent(out controller))
                controller = ob.AddComponent<AudioController>();

            SetAudioClip(controller, source, playOnAwake);
        }

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
        public void AddActiveController(string name, AudioController controller)
        {
            if (!AudioControllers.ContainsKey(name))
            {
                AudioControllers.Add(name, new List<AudioController> { controller });
            }
            else
            {
                AudioControllers[name].Add(controller);
            }
        }

        public bool RemoveActiveController(string name, AudioController controller)
        {
            if (AudioControllers.ContainsKey(name))
            {
                var id = controller.GetInstanceID();
                var controllers = AudioControllers[name];
                for (int i = 0; i < controllers.Count; i++)
                {
                    if (id == controllers[i].GetInstanceID())
                    {
                        controllers.RemoveAt(i);
                    }

                }
                return true;
            }
            return false;
        }

        public bool ContainsName(string name)
        {
            return AudioClips.ContainsKey(name);
        }

        public bool OnPlay(List<string> names)
        {
            return OnRestart(names);
        }

        public bool OnPause(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controllerGroup in AudioControllers.Values)
                {
                    foreach(var controller in controllerGroup)
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
                    
                    foreach (var controller in AudioControllers[name])
                        controller.Pause();
                }
                return true;
            }
        }

        public bool OnResume(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controllerGroup in AudioControllers.Values)
                {
                    foreach (var controller in controllerGroup)
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

                    foreach (var controller in AudioControllers[name])
                        controller.Resume();
                }
                return true;
            }
        }

        public bool OnRestart(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controllerGroup in AudioControllers.Values)
                {
                    foreach (var controller in controllerGroup)
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

                    foreach (var controller in AudioControllers[name])
                        controller.ResetClip();
                }
                return true;
            }
        }

        public bool OnStop(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controllerGroup in AudioControllers.Values)
                {
                    foreach (var controller in controllerGroup)
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

                    foreach (var controller in AudioControllers[name])
                        controller.StopClip();
                }
                return true;
            }
        }
        #endregion
    }
}
