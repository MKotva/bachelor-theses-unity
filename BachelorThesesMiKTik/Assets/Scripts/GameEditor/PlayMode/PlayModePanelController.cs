using Assets.Scripts.GameEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayModePanelController : MonoBehaviour
{
    [SerializeField] Button PlayButton;
    [SerializeField] TMP_Text ButtonText;
    [SerializeField] TMP_Text OutputConsole;

    private bool IsPlaying;

    public void OnPlayClick()
    {
        SwitchPlayPauseButton(IsPlaying);

        if (IsPlaying)
            GameManager.Instance.StartPlayMode();
        else
            GameManager.Instance.PausePlayMode();

    }

    public void OnExitClick()
    {
        IsPlaying = false;
        SwitchPlayPauseButton(true);
        GameManager.Instance.ExitPlayMode();
    }

    public void Initialize()
    {
        SwitchPlayPauseButton(true);
        OutputConsole.text = "";

        var outputManager = OutputManager.Instance;
        if(outputManager != null)
            outputManager.ClearMessages();
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
        var instance = OutputManager.Instance;
        if (instance != null)
            instance.RemoveListener("Debug panel");
    }
}
 