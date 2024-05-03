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
        public List<ItemDTO> Prototypes;
        public List<MapObjectDTO> MapObjects;
        public BackgroundDTO BackgroundSetting;

        public GameDataDTO() 
        {
            Managers = new ManagersDTO();
            Prototypes = new List<ItemDTO>();
            MapObjects = new List<MapObjectDTO>();
            BackgroundSetting = new BackgroundDTO();
        }
    }
}
