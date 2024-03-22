using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Managers;
using System;
using System.Collections.Generic;

namespace Assets.Scenes.GameEditor.Core.DTOS
{
    [Serializable]
    public class GameDataDTO
    {
        public ManagersDTO Managers;
        public List<ItemDTO> Items;
        public List<MapObjectDTO> MapObjects;
        public BackgroundDTO BackgroundDTO;

        public GameDataDTO() 
        {
            Managers = new ManagersDTO();
            Items = new List<ItemDTO>();
            MapObjects = new List<MapObjectDTO>();
            BackgroundDTO = new BackgroundDTO();
        }
    }
}
