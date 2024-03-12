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

            EditorController.Instance.PausePlayMode();
            controller.exitHandler += ExitHandler;
        }

        public void ExitHandler()
        {
            EditorController.Instance.StartPlayMode();
        }
    }
}
