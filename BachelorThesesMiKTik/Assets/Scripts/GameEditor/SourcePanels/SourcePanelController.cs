using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels
{
    public class SourcePanelController : MonoBehaviour
    {
        public delegate void DestroyHandler(int id);
        public event DestroyHandler onDestroyClick;

        public void OnDestroyClick()
        {
            if (onDestroyClick != null)
                onDestroyClick.Invoke(gameObject.GetInstanceID());
        }
    }
}
