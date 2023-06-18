using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Core.GameEditor.AI
{
    public abstract class AIAgentBase
    {
        public abstract AgentActionDTO GetReacheablePosition(Vector3 position);
        public abstract void PerformAction(string parameters);
    }
}
