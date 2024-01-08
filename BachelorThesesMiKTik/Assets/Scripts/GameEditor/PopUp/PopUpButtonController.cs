using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp
{
    public class PopUpButtonController : MonoBehaviour
    {
        [SerializeField] GameObject PopUpCanvas;
        [SerializeField] GameObject BackgroundPopUp;

        public void OnClick()
        {
            PopUpCanvas.SetActive(true);
            BackgroundPopUp.SetActive(true);
        }
    }
}
