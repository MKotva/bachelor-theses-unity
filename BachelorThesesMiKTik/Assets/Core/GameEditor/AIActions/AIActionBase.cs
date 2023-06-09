using UnityEngine;

namespace Assets.Scenes.GameEditor.Core.AIActions
{
    public abstract class AIActionBase : MonoBehaviour
    {
        public abstract void DoAction(GameObject performer);
    }
}
