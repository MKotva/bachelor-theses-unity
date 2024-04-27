using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.EditorActions
{
    public class PositionOperation : JournalActionDTO
    {
        public List<Vector3> Positions;

        public PositionOperation(ActionPerformer performer, List<Vector3> position) : base(performer)
        {
            if(position == null)
                Positions = new List<Vector3>();
            else
                Positions = position;
        }
    }
}
