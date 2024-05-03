using UnityEngine;

namespace Assets.Scripts.GameEditor.PlayMode
{
    class PlayModeCanvasController : MonoBehaviour
    {
        [SerializeField] private  PlayModePanelController controller;
        private void OnEnable()
        {
            controller.Initialize();
        }
    }
}
