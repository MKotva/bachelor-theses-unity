using System;
using System.Collections.Generic;

namespace Assets.Scenes.GameEditor.Core.DTOS
{
    [Serializable]
    public class MapDataDTO
    {
        public List<MapObjectDTO> mapObjects;
        public MapDataDTO() 
        {
            mapObjects = new List<MapObjectDTO>();
        }
    }
}
