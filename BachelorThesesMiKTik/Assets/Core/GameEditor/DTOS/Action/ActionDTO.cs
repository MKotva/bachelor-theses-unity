using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.Action
{
    public abstract class ActionDTO
    {
        public string Name;
        public abstract List<ActionBase> GetAction(GameObject instance);
    }
}
