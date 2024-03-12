using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Components;
using Assets.Scenes.GameEditor.Core.DTOS;
using Assets.Scripts.GameEditor.ItemView;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.Core.GameEditor.Serializers
{
    public static class GameDataSerializer
    {
        /// <summary>
        /// Creates new GameDataDTO objects and sets it based on actual game state.
        /// </summary>
        /// <returns></returns>
        public static GameDataDTO GetGameDTO()
        {
            var gameData = new GameDataDTO();
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
        public static async Task SetGame(GameDataDTO gameData)
        {
            await SetItems(gameData.Items);
            SetObjectToMap(gameData.MapObjects);
            await SetBackground(gameData.BackgroundDTO);
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
        private static async Task SetItems(List<ItemDTO> items)
        {
            var orderedItems = items.OrderBy(x => x.ID);
            foreach (var item in orderedItems)
            {
                var newItem = ItemData.CreateInstance();

                var tasks = new List<Task>();
                foreach (var component in item.Components)
                {
                    tasks.Add(SetComponent(component, newItem));
                }
                await Task.WhenAll(tasks);
                GameItemController.Instance.AddItem(newItem);
            }
        }

        /// <summary>
        /// Sets components of newly created items.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="newItem"></param>
        /// <returns></returns>
        private static async Task SetComponent(ComponentDTO component, ItemData newItem)
        {
            await component.Set(newItem);
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
            var bg = new BackgroundDTO();
            foreach (var layer in BackgroundController.Instance.BackgroundLayers)
            {
                //There is default background setted or error so no save!
                if (layer.Info == null)
                    return new BackgroundDTO();

                bg.LayersSources.Add(layer.Info);
            }
            return bg;
        }

        /// <summary>
        /// Sets background based on given BackgroundDTO.
        /// </summary>
        /// <param name="background"></param>
        /// <returns></returns>
        private static async Task SetBackground(BackgroundDTO background)
        {
            await BackgroundController.Instance.SetBackground(background.LayersSources);
        }
    }
}
