using TMPro;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class PopUpInfoPanel : MonoBehaviour
    {
        [SerializeField] public TMP_Text text;

        public delegate void ExitHandler();
        public event ExitHandler onExit;

        private void Awake() { }

        public virtual void OnExitClick()
        {
            if (onExit != null)
                onExit.Invoke();
            Destroy(gameObject);
        }
    }
}
