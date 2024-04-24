using Assets.Scripts.GameEditor.PopUp;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GameEditor.Toolkit
{
    public class ExitButtonController : MonoBehaviour
    {
        [SerializeField] public GameObject ConfirmationPrefab;

        public void OnClick()
        {
            var instance = Instantiate(ConfirmationPrefab, GameManager.Instance.PopUpCanvas.transform);
            var controller = instance.GetComponent<ExitConfirmationPopUp>();
            controller.ShowMessage("Exit confrimation", "Are you sure, that you saved your project?");
            controller.OnExit += ResultHandler;
        }


        public void ResultHandler(bool result)
        {
            if(result) 
            {
                SceneManager.LoadScene("MenuScene");
            }
        }

    }
}
