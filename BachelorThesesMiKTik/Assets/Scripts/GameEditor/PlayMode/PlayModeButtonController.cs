using UnityEngine;

namespace Assets.Scripts.GameEditor.PlayMode
{
    public class PlayModeButtonController : MonoBehaviour
    {
        public void OnClick()
        {
            var instance = GameManager.Instance;
            if(instance != null)
                GameManager.Instance.DisplayPlayMode();
        }
    }
}
