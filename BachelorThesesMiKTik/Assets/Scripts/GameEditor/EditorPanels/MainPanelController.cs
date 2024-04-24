using UnityEngine;

namespace Assets.Scripts.GameEditor.EditorPanels
{
    public class MainPanelController : MonoBehaviour
    {
        private GameObject _activePanel;

        private void Awake()
        {
            _activePanel = null;
        }

        /// <summary>
        /// Changes actual tool panel to given one.
        /// </summary>
        /// <param name="panel"></param>
        public void ShowPanel(GameObject panel)
        {
            if (_activePanel != null)
            {
                _activePanel.SetActive(false);
            }
            _activePanel = panel;
            _activePanel.SetActive(true);
        }
    }
}
