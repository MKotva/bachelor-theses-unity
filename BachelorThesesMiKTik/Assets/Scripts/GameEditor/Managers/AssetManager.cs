using Assets.Core.GameEditor.DTOS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Managers
{
    public class AssetManager<T, TSource, TAsset, TController> : Singleton<T> where T : MonoBehaviour 
        where TSource : AssetSourceDTO where TController : MonoBehaviour 
    {
        public Dictionary<string, Dictionary<int, TController>> Controllers { get; private set; }
        public Dictionary<string, TAsset> Assets { get; private set; }
        public Dictionary<string, TSource> Sources { get; private set; }
        protected override void Awake()
        {
            Controllers = new Dictionary<string, Dictionary<int, TController>>();
            Assets = new Dictionary<string, TAsset>();
            Sources = new Dictionary<string, TSource>();
            base.Awake();
        }

        public ManagerDTO Get()
        {
            return new ManagerDTO(Sources.Values.ToArray());
        }

        //public async Task Set(ManagerDTO managerDTO)
        //{
        //    var names = Sources.Keys.ToList();
        //    foreach (var name in names)
        //    {
        //        RemoveAnimation(name);
        //    }

        //    var tasks = new List<Task<bool>>();
        //    foreach (var data in managerDTO.Sources)
        //    {
        //        if (!( data is TSource ))
        //        {
        //            ErrorOutputManager.Instance.ShowMessage("Data loading error, data might be corrupted!", "Animations Manager");
        //            continue;
        //        }
        //        tasks.Add(AddAnimation((TSource) data));
        //    }
        //    await Task.WhenAll(tasks);
        //}

        //#region AnimationMethods
        //public async Task<bool> AddAnimation(TSource animationData)
        //{
        //    var name = animationData.Name;
        //    if (Sources.ContainsKey(name))
        //    {
        //        ErrorOutputManager.Instance.ShowMessage($"Animation with given name: {name} already exists!", "Animations Manager");
        //        return false;
        //    }

        //    var customAnim = await AnimationLoader.LoadAnimation(animationData);
        //    if (customAnim != null)
        //    {
        //        Assets.Add(name, customAnim);
        //        Sources.Add(name, animationData);
        //        return true;
        //    }
        //    return false;
        //}

        //public async Task EditAnimation(string name, TSource animationData)
        //{
        //    if (!Sources.ContainsKey(name))
        //    {
        //        return;
        //    }

        //    var customAnim = await AnimationLoader.LoadAnimation(animationData as AnimationSourceDTO);
        //    if (customAnim != null)
        //    {
        //        var tasset = customAnim;
        //        Assets[name] = ;
        //        Sources[name] = animationData;

        //        if (!Controllers.ContainsKey(name))
        //        {
        //            return;
        //        }

        //        foreach (var anim in Controllers[name].Values)
        //        {
        //            anim.EditCutomAnimation(customAnim);
        //        }
        //    }
        //}

        //public void RemoveAnimation(string name)
        //{
        //    if (!Sources.ContainsKey(name))
        //    {
        //        return;
        //    }

        //    foreach (var frame in Assets[name].Frames)
        //    {
        //        Destroy(frame.Sprite);
        //    }

        //    if (Controllers.ContainsKey(name))
        //    {
        //        foreach (var controller in Controllers[name].Values)
        //        {
        //            controller.RemoveAnimation();
        //        }
        //    }

        //    Assets.Remove(name);
        //    Sources.Remove(name);
        //    Controllers.Remove(name);
        //}
        //#endregion

        //#region ControllerMethods
        //public bool AddActiveController(string name, TController controller)
        //{
        //    if (Assets.ContainsKey(name))
        //    {
        //        if (Controllers.ContainsKey(name))
        //        {
        //            var instanceID = controller.GetInstanceID();
        //            if (ContainsActiveController(name, instanceID))
        //                return false;

        //            Controllers[name].Add(instanceID, controller);
        //        }
        //        else
        //        {
        //            Controllers.Add(name, new Dictionary<int, TController>
        //            {
        //                { controller.GetInstanceID(), controller }
        //            });
        //        }

        //        return true;
        //    }
        //    return false;
        //}

        //public bool ContainsActiveController(string name, int instanceID)
        //{
        //    if (Controllers[name].ContainsKey(instanceID))
        //        return true;
        //    return false;
        //}

        //public bool RemoveActiveController(string name, AnimationsController controller)
        //{
        //    if (Controllers.ContainsKey(name))
        //    {
        //        var id = controller.GetInstanceID();
        //        if (Controllers[name].ContainsKey(id))
        //        {
        //            Controllers[name].Remove(id);
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //public bool ContainsName(string name)
        //{
        //    return Assets.ContainsKey(name);
        //}

        //#endregion
    }
}
