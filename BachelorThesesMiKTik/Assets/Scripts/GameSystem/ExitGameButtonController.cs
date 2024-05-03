using Assets.Scripts.GameEditor.PopUp;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GameSystem
{
    public class ExitGameButtonController : MonoBehaviour
    {
        [SerializeField] public GameObject ConfirmationPrefab;
        [SerializeField] public Canvas Canvas;

        public void OnExitClick()
        {
            var instance = Instantiate(ConfirmationPrefab, Canvas.transform);
            var controller = instance.GetComponent<ExitConfirmationPopUp>();
            controller.ShowMessage("Exit confrimation", "Are you sure, that you want to leave?");
            controller.OnExit += ResultHandler;
        }


        public void ResultHandler(bool result)
        {
            if (result)
            {
                SceneManager.LoadScene("MenuScene");
            }
        }
    }
}
