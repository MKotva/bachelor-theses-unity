using Assets.Core.GameEditor.DTOS;
using System;
using System.Collections.Generic;

namespace Assets.Scenes.GameEditor.Core.DTOS
{
    [Serializable]
    public class GameDataDTO
    {
        public List<ItemDTO> Items;
        public List<MapObjectDTO> MapObjects;
        public BackgroundDTO BackgroundDTO;

        public GameDataDTO() 
        {
            Items = new List<ItemDTO>();
            MapObjects = new List<MapObjectDTO>();
            BackgroundDTO = new BackgroundDTO();
        }
    }
}
