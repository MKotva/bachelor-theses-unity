using Assets.Core.GameEditor.DTOS.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class ItemDTO
    {
        public string Name;
        public int ID;
        [SerializeReference] public List<ComponentDTO> Components;

        public ItemDTO() 
        {
            Components = new List<ComponentDTO>();
        }

        public ItemDTO(ItemData item) 
        {
            Name = item.ShownName;
            ID = item.Id;
            Components = item.Components;
        }
    }
}
