using Assets.Scripts.GameEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayModePanelController : MonoBehaviour
{
    [SerializeField] TMP_Text OutputConsole;
    [SerializeField] Button PlayButton;
    [SerializeField] TMP_Text ButtonText;

    private bool IsPlaying;

    public void OnPlayClick()
    {
        SwitchPlayPauseButton(IsPlaying);

        if (IsPlaying)
            EditorController.Instance.StartPlayMode();
        else
            EditorController.Instance.PausePlayMode();

    }

    public void OnExitClick()
    {
        EditorController.Instance.ExitPlayMode();
    }

    private void Awake()
    {
        var instance = ErrorOutputManager.Instance;
        if(instance != null )
            instance.AddOnShowListener("Debug panel", ErrorHandler);
    }

    private void ErrorHandler(string message)
    {
        OutputConsole.text = message;
    }

    private void SwitchPlayPauseButton(bool isPlaying)
    {
        if (isPlaying)
        {
            PlayButton.image.color = new Color(0.5803922f, 0.8980392f, 0.7138003f, 1);
            ButtonText.text = "Play";
            IsPlaying = false;
        }
        else 
        {
            PlayButton.image.color = new Color(0.8980392f, 0.6008394f, 0.5803922f, 1);
            ButtonText.text = "Pause";
            IsPlaying = true;
        }

    }

    private void OnDestroy()
    {
        var instance = ErrorOutputManager.Instance;
        if (instance != null)
            instance.RemoveListener("Debug panel");
    }
}
