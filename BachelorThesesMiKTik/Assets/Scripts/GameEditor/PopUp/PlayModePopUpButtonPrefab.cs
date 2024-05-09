using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp
{
    public class PlayModePopUpButtonPrefab : MonoBehaviour
    {
        [SerializeField] GameObject PopUpCanvas;
        [SerializeField] GameObject PopUpWindow;
        [SerializeField] PlayModePanelController PlayModeControler;

        public void OnCreate()
        {
            var controller = Instantiate(PopUpWindow, PopUpCanvas.transform)
                            .GetComponent<PopUpController>();

            if (GameManager.Instance.IsInPlayMode)
            {
                GameManager.Instance.PausePlayMode();
                PlayModeControler.SwitchPlayPauseButton(true);
            }
            controller.onExit += ExitHandler;
        }

        public void ExitHandler()
        {
        }
    }
}
