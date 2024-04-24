using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp
{
    public class PlayModePopUpButtonPrefab : MonoBehaviour
    {
        [SerializeField] GameObject PopUpCanvas;
        [SerializeField] GameObject PopUpWindow;

        public void OnCreate()
        {
            var controller = Instantiate(PopUpWindow, PopUpCanvas.transform)
                            .GetComponent<PopUpController>();

            GameManager.Instance.PausePlayMode();
            controller.onExit += ExitHandler;
        }

        public void ExitHandler()
        {
            GameManager.Instance.StartPlayMode();
        }
    }
}
