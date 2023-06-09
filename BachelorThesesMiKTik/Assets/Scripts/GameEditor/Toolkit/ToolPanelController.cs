using Assets.Scenes.GameEditor.Core.EditorActions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPanelController : MonoBehaviour
{
    [SerializeField] MapCanvasController MapCanvasController;

    private ToolButtonController _activeButtonController;

    public void HandleButtonClick(ToolButtonController buttonController, EditorActionBase action)
    {
        if (_activeButtonController == buttonController)
        {
            SetDefault();
            return;
        }
        if(_activeButtonController != null)
            _activeButtonController.ChangeStateToUnclicked();

        _activeButtonController = buttonController;
        _activeButtonController.ChangeStateToClicked();
        MapCanvasController.SetAction(action);
    }

    private void SetDefault()
    {
        _activeButtonController.ChangeStateToUnclicked();
        _activeButtonController = null;
        MapCanvasController.SetDefaultAction();
    }

}
