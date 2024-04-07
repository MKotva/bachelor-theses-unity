using Assets.Core.GameEditor.Animation;
using Assets.Core.GameEditor.CodeEditor.EnviromentHandlers;
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
        public Dictionary<string, Dictionary<int, AnimationsController>> AnimationControllers { get; private set; }
        public Dictionary<string, CustomAnimation> Animations { get; private set; }
        public Dictionary<string, AnimationSourceDTO> AnimationData { get; private set; }
        protected override void Awake()
        {
            AnimationControllers = new Dictionary<string, Dictionary<int, AnimationsController>>();
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

            var tasks = new List<Task<bool>>();
            foreach (var data in managerDTO.Sources)
            {
                if (!( data is AnimationSourceDTO ))
                {
                    ErrorOutputManager.Instance.ShowMessage("Data loading error, data might be corrupted!", "Animations Manager");
                    continue;
                }
                tasks.Add(AddAnimation((AnimationSourceDTO) data));
            }
            await Task.WhenAll(tasks);
        }

        #region AnimationMethods
        public async Task<bool> AddAnimation(AnimationSourceDTO animationData)
        {
            var name = animationData.Name;
            if(AnimationData.ContainsKey(name)) 
            {
                ErrorOutputManager.Instance.ShowMessage($"Animation with given name: {name} already exists!", "Animations Manager");
                return false;
            }
            
            var customAnim = await AnimationLoader.LoadAnimation(animationData);
            if(customAnim != null) 
            { 
                Animations.Add(name, customAnim);
                AnimationData.Add(name, animationData);
                return true;
            }
            return false;
        }

        public Sprite GetAnimationPreview(string name)
        {
            if (!Animations.ContainsKey(name))
                return null;

            return Animations[name].Frames[0].Sprite;
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
                Animations[name] = customAnim;
                AnimationData[name] = animationData;

                if (!AnimationControllers.ContainsKey(name))
                {
                    return;
                }

                foreach (var anim in AnimationControllers[name].Values)
                {
                    anim.EditCutomAnimation(customAnim);
                }
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
                foreach (var controller in AnimationControllers[name].Values)
                {
                    controller.RemoveAnimation();
                }
            }

            Animations.Remove(name);
            AnimationData.Remove(name);
            AnimationControllers.Remove(name);
        }

        public void SetAnimation(GameObject ob, SourceReference source, bool shouldLoop = true, bool onAwake = true)
        {
            AnimationsController controller;
            if (!ob.TryGetComponent(out controller))
                controller = ob.AddComponent<AnimationsController>();

            SetAnimation(controller, source, shouldLoop, onAwake);
        }

        public void SetAnimation(AnimationsController controller, SourceReference source, bool shouldLoop = true, bool onAwake = true)
        {
            if (!Animations.ContainsKey(source.Name))
                return;

            if (ContainsActiveController(source.Name, controller.GetInstanceID()))
                return;

            var sourceReference = controller.SourceReference;
            if (sourceReference.Name != "")
            {
                RemoveActiveController(sourceReference.Name, controller);
            }

            controller.SetCustomAnimation(Animations[source.Name], source, shouldLoop, onAwake);
            AddActiveController(source.Name, controller);
        }
        #endregion

        #region ControllerMethods
        public bool AddActiveController(string name, AnimationsController controller)
        {
            if (Animations.ContainsKey(name))
            {
                if (AnimationControllers.ContainsKey(name))
                {
                    var instanceID = controller.GetInstanceID();
                    if (ContainsActiveController(name, instanceID))
                        return false;
                    
                    AnimationControllers[name].Add(instanceID, controller);
                }
                else
                {
                    AnimationControllers.Add(name, new Dictionary<int, AnimationsController> 
                    { 
                        { controller.GetInstanceID(), controller } 
                    });
                }
                
                return true;
            }
            return false;
        }

        public bool ContainsActiveController(string name, int instanceID) 
        {
            if (AnimationControllers[name].ContainsKey(instanceID))
                return true;
            return false;
        }

        public bool RemoveActiveController(string name, AnimationsController controller)
        {
            if (AnimationControllers.ContainsKey(name))
            {
                var id = controller.GetInstanceID();
                if (AnimationControllers[name].ContainsKey(id))
                {
                    AnimationControllers[name].Remove(id);
                    return true;
                }
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
                    foreach (var controller in controllers.Values)
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

                    foreach(var controller in AnimationControllers[name].Values)
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
                    foreach (var controller in controllers.Values)
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

                    foreach (var controller in AnimationControllers[name].Values)
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
                    foreach (var controller in controllers.Values)
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

                    foreach (var controller in AnimationControllers[name].Values)
                        controller.ResetClip();
                }
                return true;
            }
        }

        public bool OnStop(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controllers in AnimationControllers.Values)
                {
                    foreach (var controller in controllers.Values)
                        controller.Stop();
                }
                return true;
            }
            else
            {
                foreach (var name in names)
                {
                    if (!AnimationControllers.ContainsKey(name))
                        return false;

                    foreach (var controller in AnimationControllers[name].Values)
                        controller.Stop();
                }
                return true;
            }
        }
        #endregion

    }
}
