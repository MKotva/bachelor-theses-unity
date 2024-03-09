using Assets.Scripts.GameEditor.Audio;
using System.Collections.Generic;


namespace Assets.Scripts.GameEditor.Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        public Dictionary<string, AudioController> AudioControllers { get; private set; }

        protected override void Awake()
        {
            AudioControllers = new Dictionary<string, AudioController>();
            base.Awake();
        }

        public bool AddActivePlayer(string name, AudioController controller)
        {
            if (!AudioControllers.ContainsKey(name))
            {
                AudioControllers.Add(name, controller);
                return true;
            }
            return false;
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
            return AudioControllers.ContainsKey(name);
        }

        public bool OnPlay(List<string> names)
        {
            return OnRestart(names);
        }

        public bool OnPause(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controller in AudioControllers.Values)
                {
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

                    AudioControllers[name].Pause();
                }
                return true;
            }
        }

        public bool OnResume(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controller in AudioControllers.Values)
                {
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

                    AudioControllers[name].Resume();
                }
                return true;
            }
        }

        public bool OnRestart(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controller in AudioControllers.Values)
                {
                    controller.ResetClip();
                }
                return true;
            }
            else
            {
                foreach(var name in names) 
                {
                    if(!AudioControllers.ContainsKey(name))
                        return false;

                    AudioControllers[name].ResetClip();
                }
                return true;
            }
        }
    }
}
