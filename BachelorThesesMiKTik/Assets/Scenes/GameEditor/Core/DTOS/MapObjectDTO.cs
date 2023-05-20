using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.DTOS
{

    [Serializable]
    public class MapObjectDTO
    {
        public int Id;
        public Vector3 Position;

        public MapObjectDTO(int id, Vector3 position) 
        {
            Id = id;
            Position = position;
        }
    }
}
