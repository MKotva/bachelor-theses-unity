using Assets.Scenes.GameEditor.Core.EditorActions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolButtonController : MonoBehaviour
{
    [SerializeField] string ActionName;
    [SerializeField] ToolPanelController PanelController;

    private Color _selectionColor;
    private EditorActionBase _action;

    private void Awake()
    {
        _selectionColor = Color.cyan;

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

    public void OnClick()
    {
        PanelController.HandleButtonClick(this, _action);
    }

    public void ChangeStateToClicked() 
    {
        GetComponent<Image>().color = _selectionColor;
    }

    public void ChangeStateToUnclicked()
    {
        GetComponent<Image>().color = Color.white;
    }
}
