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

            foreach (var data in managerDTO.Sources)
            {
                if (!( data is AudioSourceDTO ))
                {
                    ErrorOutputManager.Instance.ShowMessage("Data loading error, data might be corrupted!");
                    continue;
                }
                await AddAudioClip((AudioSourceDTO)data);
            }
        }

        #region AudioClipMethods
        public async Task AddAudioClip(AudioSourceDTO audioSourceDTO)
        {
            var name = audioSourceDTO.Name;
            if (AudioData.ContainsKey(name))
            {
                return;
            }

            var result = await AudioLoader.LoadAudioClip(audioSourceDTO.URL);
            if (result)
            {
                AudioClips.Add(name, result);
                AudioData.Add(name, audioSourceDTO);
            }
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

        public void SetAudioClip(GameObject ob, SourceDTO source)
        {
            var name = source.Name;
            if (!AudioClips.ContainsKey(name))
                return;

            AudioController controller;
            if (!ob.TryGetComponent(out controller))
                controller = ob.AddComponent<AudioController>();

            controller.SetAudioClip(name);
            AddActivePlayer(name, controller);
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
        #endregion

        public void AddActivePlayer(string name, AudioController controller)
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

        public bool RemoveActivePlayer(string name)
        {
            if (AudioControllers.ContainsKey(name))
            {
                AudioControllers.Remove(name);
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
    }
}
