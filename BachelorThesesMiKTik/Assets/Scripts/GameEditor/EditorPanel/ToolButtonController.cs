using Assets.Scenes.GameEditor.Core.EditorActions;
using System;
using UnityEngine;
using UnityEngine.UI;

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

    public void OnClick()
    {
        PanelController.HandleButtonClick(this, _action);
    }

    public void ChangeStateToClicked() 
    {
        originalColor = GetComponent<Image>().color;
        GetComponent<Image>().color = selectionColor;
    }

    public void ChangeStateToUnclicked()
    {
        GetComponent<Image>().color = originalColor;
    }
}
