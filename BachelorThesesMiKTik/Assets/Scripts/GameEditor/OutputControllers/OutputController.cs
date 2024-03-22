using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.OutputControllers
{
    public class OutputController : MonoBehaviour
    {
        [SerializeField] TMP_Text OutputConsole;
        
        public void ShowMessage(string message)
        {
            OutputConsole.text = message;
        }

        public void DisposeMessage()
        {
            OutputConsole.text = "";
        }

        private void Start()
        {
            var errorOutputManager = ErrorOutputManager.Instance;
            if (errorOutputManager != null) 
            {
                var instanceID = gameObject.GetInstanceID().ToString();
                errorOutputManager.AddOnShowListener(instanceID, ShowMessage);
                errorOutputManager.AddDisposeListerer(instanceID, DisposeMessage);
            }
        }

        private void OnDestroy()
        {
            var errorOutputManager = ErrorOutputManager.Instance;
            if (errorOutputManager != null)
            {
                var instanceID = gameObject.GetInstanceID().ToString();
                errorOutputManager.RemoveListener(instanceID);
                errorOutputManager.RemoveDisposeListener(instanceID);
            }
        }
    }
}
