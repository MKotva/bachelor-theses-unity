using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels
{
    public class SourcePanelController : MonoBehaviour
    {
        public delegate void DestroyHandler(int id);
        public event DestroyHandler onDestroy;

        public void OnDestroyClick()
        {
            if (onDestroy != null)
                onDestroy.Invoke(gameObject.GetInstanceID());

            Destroy(gameObject);
        }
    }
}
