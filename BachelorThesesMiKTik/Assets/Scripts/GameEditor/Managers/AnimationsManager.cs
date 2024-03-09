using Assets.Scripts.GameEditor.Controllers;
using System.Collections.Generic;

namespace Assets.Scripts.GameEditor.Managers
{
    public class AnimationsManager : Singleton<AnimationsManager>
    {
        public Dictionary<string, AnimationsController> AnimationControllers { get; private set; }

        protected override void Awake()
        {
            AnimationControllers = new Dictionary<string, AnimationsController>();
            base.Awake();
        }

        public bool AddActivePlayer(string name, AnimationsController controller)
        {
            if (!AnimationControllers.ContainsKey(name))
            {
                AnimationControllers.Add(name, controller);
                return true;
            }
            return false;
        }

        public bool RemoveActivePlayer(string name)
        {
            if (AnimationControllers.ContainsKey(name))
            {
                AnimationControllers.Remove(name);
                return true;
            }
            return false;
        }

        public bool ContainsName(string name)
        {
            return AnimationControllers.ContainsKey(name);
        }

        public bool OnPlay(List<string> names)
        {
            return OnRestart(names);
        }

        public bool OnPause(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controller in AnimationControllers.Values)
                {
                    controller.Pause();
                }
                return true;
            }
            else
            {
                foreach (var name in names)
                {
                    if (!AnimationControllers.ContainsKey(name))
                        return false;
                    AnimationControllers[name].Pause();
                }
                return true;
            }
        }

        public bool OnResume(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controller in AnimationControllers.Values)
                {
                    controller.Resume();
                }
                return true;
            }
            else
            {
                foreach (var name in names)
                {
                    if (!AnimationControllers.ContainsKey(name))
                        return false;
                    AnimationControllers[name].Resume();
                }
                return true;
            }
        }

        public bool OnRestart(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controller in AnimationControllers.Values)
                {
                    controller.ResetClip();
                }
                return true;
            }
            else
            {
                foreach (var name in names)
                {
                    if (!AnimationControllers.ContainsKey(name))
                        return false;
                    AnimationControllers[name].ResetClip();
                }
                return true;
            }
        }
    }
}
