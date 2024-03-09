using Assets.Scripts.GameEditor;
using TMPro;
using UnityEngine;

public class PlayModePanelController : MonoBehaviour
{
    [SerializeField] TMP_Text OutputConsole;

    public void OnPlayClick()
    {
        EditorController.Instance.StartPlayMode();
    }

    public void OnPause(bool pause)
    {
        EditorController.Instance.PausePlayMode();
    }

    public void OnExitClick(bool exit)
    {
        EditorController.Instance.ExitPlayMode();

    }
}
