using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
    public class GameOutputConsole : MonoBehaviour
    {
        [SerializeField] TMP_Text OutputConsole;
        [SerializeField] SpriteRenderer Background;
        [SerializeField] Color DisplayColor;

        /// <summary>
        /// Displayes given message to console.
        /// </summary>
        public void ShowMessage(string message)
        {
            OutputConsole.text = message;
            Background.color = DisplayColor;
        }

        /// <summary>
        /// Clears text field of console.
        /// </summary>
        public void DisposeMessage()
        {
            OutputConsole.text = "";
            Background.color = new Color(0, 0, 0, 0);
        }

        /// <summary>
        /// Connects this controller to OutputManager.
        /// </summary>
        private void Start()
        {
            var errorOutputManager = OutputManager.Instance;
            if (errorOutputManager != null)
            {
                var instanceID = gameObject.GetInstanceID().ToString();
                errorOutputManager.AddOnShowListener(instanceID, ShowMessage);
                errorOutputManager.AddDisposeListerer(instanceID, DisposeMessage);
            }
        }

        /// <summary>
        /// Disconnect this controller from OutputManager.
        /// </summary>
        private void OnDestroy()
        {
            var errorOutputManager = OutputManager.Instance;
            if (errorOutputManager != null)
            {
                var instanceID = gameObject.GetInstanceID().ToString();
                errorOutputManager.RemoveListener(instanceID);
                errorOutputManager.RemoveDisposeListener(instanceID);
            }
        }
    }
}
