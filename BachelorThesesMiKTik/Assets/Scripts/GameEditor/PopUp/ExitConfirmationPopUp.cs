using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp
{
    public class ExitConfirmationPopUp : MonoBehaviour
    {
        [SerializeField] TMP_Text Header;
        [SerializeField] TMP_Text InfoPanel;

        public delegate void ResultHandler(bool result);
        public event ResultHandler OnExit;

        public void ShowMessage(string header, string message)
        {
            Header.text = header;
            InfoPanel.text = message;
        }

        public void OnConfirmClick()
        {
            InvokeHandlers(true);
            Destroy(gameObject);
        }

        public void OnCancelClick() 
        {
            InvokeHandlers(false);
            Destroy(gameObject);
        }

        private void InvokeHandlers(bool result)
        {
            if (OnExit == null)
                return;

            OnExit.Invoke(result);
        }
    }
}
