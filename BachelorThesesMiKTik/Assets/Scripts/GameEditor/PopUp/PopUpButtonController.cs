using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp
{
    public class PopUpButtonController : MonoBehaviour
    {
        [SerializeField] GameObject PopUpCanvas;
        [SerializeField] GameObject ToolkitCanvasPopUp;
        [SerializeField] GameObject PopUpWindow;

        public void OnCreate()
        {
            ToolkitCanvasPopUp.SetActive(true);
            PopUpCanvas.SetActive(true);
            Instantiate(PopUpWindow, PopUpCanvas.transform);
        }
    }
}
