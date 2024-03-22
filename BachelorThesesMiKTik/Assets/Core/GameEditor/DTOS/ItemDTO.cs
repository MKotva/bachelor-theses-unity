using Assets.Core.GameEditor.Components;
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
        [SerializeReference] public List<CustomComponent> Components;

        public ItemDTO() 
        {
            Components = new List<CustomComponent>();
        }

        public ItemDTO(ItemData item) 
        {
            Name = item.ShownName;
            ID = item.Id;
            Components = item.Components;
        }
    }
}
