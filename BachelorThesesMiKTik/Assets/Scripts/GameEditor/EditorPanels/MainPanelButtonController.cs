using UnityEngine;

namespace Assets.Scripts.GameEditor.EditorPanels
{
    public class MainPanelButtonController : MonoBehaviour
    {
        [SerializeField] public GameObject MainPanel;
        [SerializeField] public GameObject PanelToShow;

        private MainPanelController _controller;

        private void Awake()
        {
            _controller = MainPanel.GetComponent<MainPanelController>();
        }

        /// <summary>
        /// Button click event handler. Calls method for changing actual tool panel
        /// to panel stored in this script.
        /// </summary>
        public void OnClick()
        {
            _controller.ShowPanel(PanelToShow);
        }
    }
}
