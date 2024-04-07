using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.Components;
using Assets.Core.GameEditor.DTOS.Managers;
using Assets.Scenes.GameEditor.Core.DTOS;
using Assets.Scripts.GameEditor.ItemView;
using Assets.Scripts.GameEditor.Managers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assets.Core.GameEditor.Serializers
{
    public static class GameDataSerializer
    {
        /// <summary>
        /// Creates new GameDataDTO objects and sets it based on actual game state.
        /// </summary>
        /// <returns></returns>
        public static GameDataDTO Serialize()
        {
            var gameData = new GameDataDTO();
            gameData.Managers = GetManagers();
            gameData.Items = GetItems();
            gameData.MapObjects = GetMapObjects();
            gameData.BackgroundDTO = GetBackgroundSetting();
            return gameData;
        }

        /// <summary>
        /// Sets actual game status based on given GameDataDTO.
        /// </summary>
        /// <param name="gameData">Stored game status</param>
        /// <returns></returns>
        public static async Task Deserialize(GameDataDTO gameData)
        {
            await SetManagers(gameData.Managers);
            SetItems(gameData.Items);
            SetObjectToMap(gameData.MapObjects);
            SetBackground(gameData.BackgroundDTO);
        }

        public static ManagersDTO GetManagers()
        {
            var spriteData = SpriteManager.Instance.Get();
            var animationData = AnimationsManager.Instance.Get();
            var audioData = AudioManager.Instance.Get();

            return new ManagersDTO(spriteData, animationData, audioData);
        }

        public static async Task SetManagers(ManagersDTO managers)
        {
            var tasks = new List<Task>
            {
                SpriteManager.Instance.Set(managers.SpritesManager),
                AnimationsManager.Instance.Set(managers.AnimationManager),
                AudioManager.Instance.Set(managers.AudioManager)
            };

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// </summary>
        /// <returns>List of ItemDTO representing all created objects</returns>
        private static List<ItemDTO> GetItems()
        {
            var dtos = new List<ItemDTO>();
            foreach (var item in GameItemController.Instance.Items.Values)
            {
                if (item.Components != null)
                    dtos.Add(new ItemDTO(item));
            }
            return dtos;
        }

        /// <summary>
        /// Creates game items based on given itemDTOS.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private static void SetItems(List<ItemDTO> items)
        {
            GameItemController.Instance.ClearItems();
            foreach (var item in items)
            {
                var newItem = new ItemData();

                foreach (var component in item.Components)
                {
                    SetComponent(component, newItem);
                }

                GameItemController.Instance.AddItem(newItem);
            }
        }

        /// <summary>
        /// Sets components of newly created items.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="newItem"></param>
        /// <returns></returns>
        private static void SetComponent(CustomComponent component, ItemData newItem)
        {
            component.Set(newItem);
            newItem.Components.Add(component);
        }

        /// <summary>
        /// </summary>
        /// <returns>List of MapObjectDTO representing all instances of objects in map.</returns>
        private static List<MapObjectDTO> GetMapObjects()
        {
            var map = MapCanvas.Instance;
            var mapObjects = new List<MapObjectDTO>();

            foreach (var id in map.Data.Keys)
            {
                foreach (var position in map.Data[id].Keys)
                {
                    mapObjects.Add(new MapObjectDTO(id, position));
                }
            }
            return mapObjects;
        }

        /// <summary>
        /// Creates instances of items on position given by MapObjectDTO.
        /// </summary>
        /// <param name="mapObjects"></param>
        private static void SetObjectToMap(List<MapObjectDTO> mapObjects)
        {
            var map = MapCanvas.Instance;
            map.MapJournal.Clear();
            map.EraseMap();
            map.UnselectAll();

            foreach (var obj in mapObjects)
            {
                var item = GameItemController.Instance.Items[obj.Id];
                map.Paint(item, obj.Position);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>BackgroundDTO representing actual bacground state.</returns>
        private static BackgroundDTO GetBackgroundSetting()
        {
            var instance = BackgroundController.Instance;
            if (instance != null)
                return new BackgroundDTO(instance.Sources, instance.AudioSource);

            return new BackgroundDTO();
        }

        /// <summary>
        /// Sets background based on given BackgroundDTO.
        /// </summary>
        /// <param name="background"></param>
        /// <returns></returns>
        private static void SetBackground(BackgroundDTO background)
        {
            var instance = BackgroundController.Instance;
            if (instance != null)
            {
                instance.SetBackground(background.LayersSources);
                instance.SetAudioSource(background.AudioSource);
            }
        }
    }
}
