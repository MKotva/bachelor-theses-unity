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

        /// <summary>
        /// </summary>
        /// <returns>Actual state of manager in ManagerDTO</returns>
        public ManagerDTO Get()
        {
            return new ManagerDTO(AnimationData.Values.ToArray());
        }

        /// <summary>
        /// Sets actual state of manager based on ManagerDTO.
        /// </summary>
        /// <param name="managerDTO"></param>
        /// <returns></returns>
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
                    OutputManager.Instance.ShowMessage("Data loading error, data might be corrupted!", "Animations Manager");
                    continue;
                }
                tasks.Add(AddAnimation((AnimationSourceDTO) data));
            }
            await Task.WhenAll(tasks);
        }

        #region AnimationMethods

        /// <summary>
        /// Loads animation based on given AnimationDTO (if there is not animation with same name).
        /// Animation and her DTO are than stored in Animations and AnimationsData.
        /// </summary>
        /// <param name="animationData"></param>
        /// <returns></returns>
        public async Task<bool> AddAnimation(AnimationSourceDTO animationData)
        {
            var name = animationData.Name;
            if(AnimationData.ContainsKey(name)) 
            {
                OutputManager.Instance.ShowMessage($"Animation with given name: {name} already exists!", "Animations Manager");
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

        /// <summary>
        /// Returns first image of animation as preview.
        /// </summary>
        /// <param name="name">Animation name.</param>
        /// <returns>If animation exist, returns sprite. Otherwise null.</returns>
        public Sprite GetAnimationPreview(string name)
        {
            if (!Animations.ContainsKey(name))
                return null;

            return Animations[name].Frames[0].Sprite;
        }

        /// <summary>
        /// Rewrites animation of given name with new one, based on AnimaitonDTO.
        /// </summary>
        /// <param name="name">Name of old animation</param>
        /// <param name="animationData">New animation desription.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Removes animation with given name and all connected objects from manager data -> 
        /// (SourceDTO and all controller with this name.)
        /// </summary>
        /// <param name="name"></param>
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

        /// <summary>
        /// Sets animation to a given object. If object has no Animation controller, method will add one.
        /// This controller is than added to registered controllers in this manager.
        /// </summary>
        /// <param name="ob">Object to be set.</param>
        /// <param name="source">Reference to animation with scaling.</param>
        /// <param name="shouldLoop">Should animation loop?</param>
        /// <param name="onAwake">Should be animation played after set?</param>
        public void SetAnimation(GameObject ob, SourceReference source, bool shouldLoop = true, bool onAwake = true)
        {
            AnimationsController controller;
            if (!ob.TryGetComponent(out controller))
                controller = ob.AddComponent<AnimationsController>();

            SetAnimation(controller, source, shouldLoop, onAwake);
        }

        /// <summary>
        /// Sets animation to a given animation controller. If object has no Animation controller, method will add one.
        /// This controller is than added to registered controllers in this manager.
        /// </summary>
        /// <param name="ob">Object to be set.</param>
        /// <param name="source">Reference to animation with scaling.</param>
        /// <param name="shouldLoop">Should animation loop?</param>
        /// <param name="onAwake">Should be animation played after set?</param>
        public void SetAnimation(AnimationsController controller, SourceReference source, bool shouldLoop = true, bool onAwake = true)
        {
            if (!Animations.ContainsKey(source.Name))
                return;

            var instanceID = controller.GetInstanceID();
            if (ContainsActiveController(source.Name, instanceID))
            {
                AnimationControllers[source.Name][instanceID].ResetClip();
                return;
            }

            var sourceReference = controller.SourceReference;
            if (sourceReference != null)
            {
                if(sourceReference.Name != "")
                    RemoveActiveController(sourceReference.Name, controller);
            }

            controller.SetCustomAnimation(Animations[source.Name], source, shouldLoop, onAwake);
            AddActiveController(source.Name, controller);
        }

        public CustomAnimation GetAnimation(string name)
        {
            if (Animations.ContainsKey(name))
            {
                return Animations[name];
            }
            return null;
        }

        #endregion

        #region ControllerMethods
        /// <summary>
        /// Adds animations controller as registered controller to manager as pair(animation name, controller)
        /// </summary>
        /// <param name="name">Animation name</param>
        /// <param name="controller">Animation controller</param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks if manager registers controller with given instance id 
        /// of animation with given name.
        /// </summary>
        /// <param name="name">Animation name</param>
        /// <param name="instanceID">Animation controller instance id.</param>
        /// <returns></returns>
        public bool ContainsActiveController(string name, int instanceID) 
        {
            if (!AnimationControllers.ContainsKey(name))
                return false;

            if (AnimationControllers[name].ContainsKey(instanceID))
                return true;
            return false;
        }

        /// <summary>
        /// Removes controller from registered controllers.
        /// </summary>
        /// <param name="name">Animation name</param>
        /// <param name="controller">Animation controller</param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks if there is animation with given name.
        /// </summary>
        /// <param name="name">Animation name.</param>
        /// <returns></returns>
        public bool ContainsName(string name)
        {
            return Animations.ContainsKey(name);
        }

        /// <summary>
        /// Plays all registered controllers with given animation names.
        /// </summary>
        /// <param name="names">List of animation names.</param>
        /// <returns></returns>
        public bool OnPlay(List<string> names)
        {
            return OnRestart(names);
        }

        /// <summary>
        /// Pauses all registered controllers with given animation names.
        /// </summary>
        /// <param name="names">List of animation names.</param>
        /// <returns></returns>
        public bool OnPause(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controllers in AnimationControllers.Values)
                {
                    foreach (var controler in controllers.Values)
                    {
                        controler.IsManualyPaused = true;
                        controler.Pause();
                    }
                }
                return true;
            }
            else
            {
                foreach (var name in names)
                {
                    if (!AnimationControllers.ContainsKey(name))
                        return false;

                    foreach (var controler in AnimationControllers[name].Values)
                    {
                        controler.IsManualyPaused = true;
                        controler.Pause();
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Resumes all paused registered controllers with given animation names.
        /// </summary>
        /// <param name="names">List of animation names.</param>
        /// <returns></returns>
        public bool OnResume(List<string> names)
        {
            if (names.Count == 0)
            {
                foreach (var controllers in AnimationControllers.Values)
                {
                    foreach (var controler in controllers.Values)
                    {
                        controler.Resume();
                    }
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
                    {
                        controller.Resume();
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Pauses all registered controllers with given animation names.
        /// </summary>
        /// <param name="names">List of animation names.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Stops all registered controllers with given animation names.
        /// </summary>
        /// <param name="names">List of animation names.</param>
        /// <returns></returns>
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
