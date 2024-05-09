using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Scripts.GameEditor.Controllers;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Assets.Scripts.GameEditor.Managers
{
    public class SpriteManager : Singleton<SpriteManager>
    {
        public Dictionary<string, Dictionary<int, SpriteController>> SpriteControllers { get; private set; }
        public Dictionary<string, Sprite> Sprites { get; private set; }
        public Dictionary<string, AssetSourceDTO> SpriteData { get; private set; }
        protected override void Awake()
        {
            SpriteControllers = new Dictionary<string, Dictionary<int, SpriteController>>();
            Sprites = new Dictionary<string, Sprite>();
            SpriteData = new Dictionary<string, AssetSourceDTO>();
            base.Awake();
        }

        public ManagerDTO Get()
        {
            return new ManagerDTO(SpriteData.Values.ToArray());
        }

        public async Task Set(ManagerDTO managerDTO)
        {
            if (SpriteData != null)
            {
                var names = SpriteData.Keys.ToList();
                foreach (var name in names)
                {
                    RemoveSprite(name);
                }

            }

            var tasks = new List<Task<bool>>();
            foreach(var data in managerDTO.Sources) 
            {
                tasks.Add(AddSprite(data));
            }
            
            await Task.WhenAll(tasks);
        }

        #region SpriteMethods
        public async Task<bool> AddSprite(AssetSourceDTO spriteData)
        {
            var name = spriteData.Name;
            if (SpriteData.ContainsKey(name))
            {
                OutputManager.Instance.ShowMessage("Sprite with the same name already exists!", "Sprite manager");
                return false;
            }

            var sprite = await SpriteLoader.LoadSprite(spriteData);
            if (sprite != null)
            {
                Sprites.Add(name, sprite);
                SpriteData.Add(name, spriteData);
                return true;
            }
            OutputManager.Instance.ShowMessage($"Sprite with given name: {spriteData.Name} could not be loaded!", "Sprite manager");
            return false;
        }

        public async void EditSprite(AssetSourceDTO spriteData)
        {
            var name = spriteData.Name;
            if (!SpriteData.ContainsKey(name))
            {
                OutputManager.Instance.ShowMessage("Sprite with given name does not exists!", "Sprite manager");
                return;
            }

            var sprite = await SpriteLoader.LoadSprite(spriteData);
            if (sprite != null)
            {
                SpriteData[name] = spriteData;
                Sprites[name] = sprite;

                if (!SpriteControllers.ContainsKey(name))
                    return;

                foreach(var controller in SpriteControllers[name].Values)
                {
                    controller.EditSprite(sprite);
                }
            }
        }
        public void RemoveSprite(string name)
        {
            if (!SpriteData.ContainsKey(name))
            {
                OutputManager.Instance.ShowMessage("Sprite with given name does not exists!", "Sprite manager");
                return;
            }

            if (SpriteControllers.ContainsKey(name))
            {
                foreach (var controller in SpriteControllers[name].Values)
                {
                    controller.DeleteSprite();
                }
            }

            Destroy(Sprites[name]);
            Sprites.Remove(name);
            SpriteData.Remove(name);
            SpriteControllers.Remove(name);
        }

        public void SetSprite(GameObject ob, SourceReference source)
        {
            SpriteController controller;
            if (!ob.TryGetComponent(out controller))
                controller = ob.AddComponent<SpriteController>();

            SetSprite(controller, source);
        }

        public void SetSprite(SpriteController controller, SourceReference source)
        {
            var name = source.Name;
            if (!Sprites.ContainsKey(name))
                return;

            RemoveAnimationController(controller.gameObject);

            controller.SetSprite(Sprites[name], source);
            AddActiveController(name, controller);
        }

        public Sprite GetSprite(string name) 
        {
            if(Sprites.ContainsKey(name))
            {
                return Sprites[name];
            }
            return null;
        }

        #endregion
        #region ControllerMethods
        public bool AddActiveController(string name, SpriteController controller)
        {
            if (Sprites.ContainsKey(name))
            {
                if (SpriteControllers.ContainsKey(name))
                {
                    var instanceID = controller.GetInstanceID();
                    if (ContainsActiveController(name, instanceID))
                        return false;

                    SpriteControllers[name].Add(instanceID, controller);
                }
                else
                {
                    SpriteControllers.Add(name, new Dictionary<int, SpriteController>
                    {
                        { controller.GetInstanceID(), controller }
                    });
                }

                return true;
            }
            return false;
        }

        public bool RemoveActiveController(string name, SpriteController controller)
        {
            if (SpriteControllers.ContainsKey(name))
            {
                var id = controller.GetInstanceID();
                if (SpriteControllers[name].ContainsKey(id))
                {
                    SpriteControllers[name].Remove(id);
                    return true;
                }
            }
            return false;
        }

        public bool ContainsActiveController(string name, int instanceID)
        {
            if (SpriteControllers[name].ContainsKey(instanceID))
                return true;
            return false;
        }

        public bool ContainsName(string name)
        {
            return Sprites.ContainsKey(name);
        }
        #endregion

        private void RemoveAnimationController(GameObject gameOb)
        {
            if (gameOb.TryGetComponent<AnimationsController>(out var animation))
            {
                if (animation.SourceReference == null)
                    return;

                animation.Stop();
                var manager = AnimationsManager.Instance;
                if (manager != null)
                    manager.RemoveActiveController(animation.SourceReference.Name, animation);
            }
        }
    }
}
