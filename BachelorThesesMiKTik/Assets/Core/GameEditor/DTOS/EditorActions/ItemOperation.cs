
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.EditorActions
{
    public class ItemOperation : JournalActionDTO
    {
        public Dictionary<Vector3, int> Items;

        public ItemOperation(ActionPerformer performer, Dictionary<Vector3, int> items) : base(performer)
        {
            if(items == null)
                Items = new Dictionary<Vector3, int>();
            else
                Items = items;
        }
    }
}
