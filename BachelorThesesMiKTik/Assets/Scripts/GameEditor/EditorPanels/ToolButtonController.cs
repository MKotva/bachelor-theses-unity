using Assets.Scenes.GameEditor.Core.EditorActions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.EditorPanels
{
    public class ToolButtonController : MonoBehaviour
    {
        [SerializeField] string ActionName;
        [SerializeField] ToolPanelController PanelController;

        private Color selectionColor;
        private Color originalColor;
        private EditorActionBase _action;

        private void Awake()
        {
            originalColor = new Color(0.7882353f, 0.9921569f, 0.9764706f, 1);
            selectionColor = new Color(0.6886792f, 0.6729145f, 0.6594428f, 1);

            var type = Type.GetType("Assets.Scenes.GameEditor.Core.EditorActions." + ActionName);
            if (type != null)
            {
                _action = (EditorActionBase) Activator.CreateInstance(type);
            }
            else
            {
                _action = new MoveAction();
            }
        }

        /// <summary>
        /// On button click event handler.
        /// </summary>
        public void OnClick()
        {
            PanelController.HandleButtonClick(this, _action);
        }

        /// <summary>
        /// Changes color of button to selectection color (to be clear its clicked)
        /// </summary>
        public void ChangeStateToClicked()
        {
            originalColor = GetComponent<Image>().color;
            GetComponent<Image>().color = selectionColor;
        }

        /// <summary>
        /// Restores color of button to original color.
        /// </summary>
        public void ChangeStateToUnclicked()
        {
            GetComponent<Image>().color = originalColor;
        }
    }
}
