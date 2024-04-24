using Assets.Scenes.GameEditor.Core.EditorActions;
using UnityEngine;

namespace Assets.Scripts.GameEditor.EditorPanels
{
    public class ToolPanelController : MonoBehaviour
    {
        private ToolButtonController activeButtonController;

        /// <summary>
        /// If pressed button is the same as active button, button is deactivated.
        /// Otherwise the button is activated. This button also becomes new active button.
        /// </summary>
        /// <param name="buttonController">Button controller of pressed button.</param>
        /// <param name="action">Proper editor action of this button.</param>
        public void HandleButtonClick(ToolButtonController buttonController, EditorActionBase action)
        {
            if (activeButtonController == buttonController)
            {
                SetDefault();
                return;
            }
            if (activeButtonController != null)
                activeButtonController.ChangeStateToUnclicked();

            activeButtonController = buttonController;
            activeButtonController.ChangeStateToClicked();
            EditorCanvas.Instance.SetAction(action);
        }

        /// <summary>
        /// Changes actual state of tool panel to default -> Change active button
        /// to uncklicked and set active button to null. Also change actual canvas action
        /// to default action.
        /// </summary>
        private void SetDefault()
        {
            activeButtonController.ChangeStateToUnclicked();
            activeButtonController = null;
            EditorCanvas.Instance.SetDefaultAction();
        }
    }
}
