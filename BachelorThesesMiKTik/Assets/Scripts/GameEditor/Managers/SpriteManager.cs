using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Managers
{
    public class SpriteManager : Singleton<SpriteManager>
    {
        public Dictionary<string, List<SpriteController>> SpriteControllers { get; private set; }
        public Dictionary<string, Sprite> Sprites { get; private set; }
        public Dictionary<string, AssetSourceDTO> SpriteData { get; private set; }
        protected override void Awake()
        {
            SpriteControllers = new Dictionary<string, List<SpriteController>>();
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
            var names = SpriteData.Keys.ToList();
            foreach(var name in names) 
            {
                RemoveSprite(name);
            }

            foreach(var data in managerDTO.Sources) 
            {
                await AddSprite(data);
            }
        }

        #region SpriteMethods
        public async Task AddSprite(AssetSourceDTO spriteData)
        {
            var name = spriteData.Name;
            if (SpriteData.ContainsKey(name))
            {
                ErrorOutputManager.Instance.ShowMessage("Sprite with the same name already exists!", "Sprite manager");
                return;
            }

            var sprite = await SpriteLoader.LoadSprite(spriteData.URL);
            if (sprite != null)
            {
                Sprites.Add(name, sprite);
                SpriteData.Add(name, spriteData);
            }
        }

        public async void EditSprite(AssetSourceDTO spriteData)
        {
            var name = spriteData.Name;
            if (!SpriteData.ContainsKey(name))
            {
                ErrorOutputManager.Instance.ShowMessage("Sprite with given name does not exists!", "Sprite manager");
                return;
            }

            var sprite = await SpriteLoader.LoadSprite(spriteData.URL);
            if (sprite != null)
            {
                SpriteData[name] = spriteData;
                Sprites[name] = sprite;

                if (!SpriteControllers.ContainsKey(name))
                    return;

                foreach(var controller in SpriteControllers[name])
                {
                    controller.EditSprite(sprite);
                }
            }
        }
        public void RemoveSprite(string name)
        {
            if (!SpriteData.ContainsKey(name))
            {
                ErrorOutputManager.Instance.ShowMessage("Sprite with given name does not exists!", "Sprite manager");
                return;
            }

            if (SpriteControllers.ContainsKey(name))
            {
                foreach (var controller in SpriteControllers[name])
                {
                    controller.DeleteSprite();
                }
            }

            Destroy(Sprites[name]);
            Sprites.Remove(name);
            SpriteData.Remove(name);
            SpriteControllers.Remove(name);
        }

        public void SetSprite(GameObject ob, SourceDTO source)
        {
            var name = source.Name;
            if (!Sprites.ContainsKey(name))
                return;

            SpriteController controller;
            if (!ob.TryGetComponent(out controller))
                controller = ob.AddComponent<SpriteController>();

            controller.SetSprite(Sprites[name], source.XSize, source.YSize);
            AddActiveObject(name, controller);
        }
        #endregion
        #region ControllerMethods
        public bool AddActiveObject(string name, SpriteController controller)
        {
            if (Sprites.ContainsKey(name))
            {
                if (SpriteControllers.ContainsKey(name))
                {
                    SpriteControllers[name].Add(controller);
                    return true;
                }
                else
                {
                    SpriteControllers.Add(name, new List<SpriteController> { controller });
                }
            }
            return false;
        }

        public bool RemoveActiveObject(string name)
        {
            if (SpriteControllers.ContainsKey(name))
            {
                SpriteControllers.Remove(name);
                return true;
            }
            return false;
        }

        public bool ContainsName(string name)
        {
            return Sprites.ContainsKey(name);
        }
        #endregion
    }
}
