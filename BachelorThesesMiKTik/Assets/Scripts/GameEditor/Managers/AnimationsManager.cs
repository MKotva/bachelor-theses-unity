using Assets.Core.GameEditor.Animation;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Scripts.GameEditor.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Managers
{
    public class AnimationsManager : Singleton<AnimationsManager>
    {
        public Dictionary<string, List<AnimationsController>> AnimationControllers { get; private set; }
        public Dictionary<string, CustomAnimation> Animations { get; private set; }
        public Dictionary<string, AnimationSourceDTO> AnimationData { get; private set; }
        protected override void Awake()
        {
            AnimationControllers = new Dictionary<string, List<AnimationsController>>();
            Animations = new Dictionary<string, CustomAnimation>();
            AnimationData = new Dictionary<string, AnimationSourceDTO>();
            base.Awake();
        }

        public ManagerDTO Get()
        {
            return new ManagerDTO(AnimationData.Values.ToArray());
        }

        public async Task Set(ManagerDTO managerDTO)
        {
            var names = AnimationData.Keys.ToList();
            foreach (var name in names)
            {
                RemoveAnimation(name);
            }

            foreach (var data in managerDTO.Sources)
            {
                if (!( data is AnimationSourceDTO ))
                {
                    ErrorOutputManager.Instance.ShowMessage("Data loading error, data might be corrupted!");
                    continue;
                }
                await AddAnimation((AnimationSourceDTO) data);
            }
        }

        public async Task AddAnimation(AnimationSourceDTO animationData)
        {
            var name = animationData.Name;
            if(AnimationData.ContainsKey(name)) 
            {
                return;
            }
            
            var customAnim = await AnimationLoader.LoadAnimation(animationData);
            if(customAnim != null) 
            { 
                Animations.Add(name, customAnim);
                AnimationData.Add(name, animationData);
            }
        }

        public void RemoveAnimation(string name)
        {
            if (!AnimationData.ContainsKey(name))
            {
                return;
            }

            foreach (var frame in Animations[name].Frames)
            {
                Destroy(frame.Sprite);
            }

            if (AnimationControllers.ContainsKey(name))
            {
                foreach (var controller in AnimationControllers[name])
                {
                    controller.RemoveAnimation();
                }
            }

            Animations.Remove(name);
            AnimationData.Remove(name);
            AnimationControllers.Remove(name);
        }

        public void SetAnimation(GameObject ob, SourceDTO source, bool shouldLoop = true, bool onAwake = true)
        {
            var name = source.Name;
            if (!Animations.ContainsKey(name))
                return;

            AnimationsController controller;
            if (!ob.TryGetComponent(out controller))
                controller = ob.AddComponent<AnimationsController>();

            controller.SetCustomAnimation(Animations[name], shouldLoop, onAwake, source.XSize, source.YSize);
            AddActivePlayer(name, controller);
        }

        public async Task EditAnimation(string name, AnimationSourceDTO animationData)
        {
            if (!AnimationData.ContainsKey(name))
            {
                return;
            }

            var customAnim = await AnimationLoader.LoadAnimation(animationData);
            if (customAnim != null)
            {
                Animations[name] =  customAnim;
                AnimationData[name] = animationData;

                if(!AnimationControllers.ContainsKey(name)) 
                {
                    return;
                }

                foreach(var anim in AnimationControllers[name])
                {
                    anim.EditCutomAnimation(customAnim);
                }
            }
        }

        public Sprite GetAnimationPreview(string name)
        {
            if (!Animations.ContainsKey(name))
                return null;

            return Animations[name].Frames[0].Sprite;
        }

        public bool AddActivePlayer(string name, AnimationsController controller)
        {
            if (Animations.ContainsKey(name))
            {
                if(AnimationControllers.ContainsKey(name)) 
                {
                    AnimationControllers[name].Add(controller);
                    return true;
                }
                else
                {
                    AnimationControllers.Add(name, new List<AnimationsController> { controller });
                }
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
            return Animations.ContainsKey(name);
        }

        public bool OnPlay(List<string> names)
        {
            return OnRestart(names);
        }

        public bool OnPause(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controllers in AnimationControllers.Values)
                {
                    foreach (var controller in controllers)
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

                    foreach(var controller in AnimationControllers[name])
                        controller.Pause();
                }
                return true;
            }
        }

        public bool OnResume(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controllers in AnimationControllers.Values)
                {
                    foreach (var controller in controllers)
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

                    foreach (var controller in AnimationControllers[name])
                        controller.Resume();
                }
                return true;
            }
        }

        public bool OnRestart(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controllers in AnimationControllers.Values)
                {
                    foreach (var controller in controllers)
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

                    foreach (var controller in AnimationControllers[name])
                        controller.ResetClip();
                }
                return true;
            }
        }
    }
}
