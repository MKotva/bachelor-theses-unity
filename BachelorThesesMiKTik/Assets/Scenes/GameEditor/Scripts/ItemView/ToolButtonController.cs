using Assets.Scenes.GameEditor.Core.EditorActions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolButtonController : MonoBehaviour
{
    [SerializeField] string ActionName;
    [SerializeField] GameObject Grid;

    private EditorActionBase _action;
    private GridController _controller;
    private bool _clicked;
    private void Awake()
    {
        _controller = Grid.GetComponent<GridController>();

        var type = Type.GetType("Assets.Scenes.GameEditor.Core.EditorActions." + ActionName);
        if (type != null)
        {
            _action = (EditorActionBase) Activator.CreateInstance(type, new object[] { _controller });
        }
        else
        {
            _action = new MoveAction(_controller);
        }
    }

    public void OnClick()
    {
        if( _clicked)
        {
            _clicked = false;
            _controller.SetDefaultAction();
        }
        else
        {
            _clicked = true;
            _controller.SetAction(_action);
        }
    }
}
