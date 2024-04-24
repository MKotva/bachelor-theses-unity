using Assets.Scripts.GameEditor.PopUp;
using Assets.Scripts.GameEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GameSystem
{
    public class ExitGameButtonController : MonoBehaviour
    {
        [SerializeField] public GameObject ConfirmationPrefab;

        public void OnExitClick()
        {
            var instance = Instantiate(ConfirmationPrefab, GameManager.Instance.PopUpCanvas.transform);
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
