using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp
{
    public class PopUpButtonController : MonoBehaviour
    {
        [SerializeField] GameObject PopUpCanvas;
        [SerializeField] GameObject PopUpWindow;

        public void OnCreate()
        {
            var instance = Instantiate(PopUpWindow, PopUpCanvas.transform);
        }
    }
}
