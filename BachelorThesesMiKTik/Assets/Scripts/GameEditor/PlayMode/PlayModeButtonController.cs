using UnityEngine;

namespace Assets.Scripts.GameEditor.PlayMode
{
    public class PlayModeButtonController : MonoBehaviour
    {
        public void OnClick()
        {
            var instance = EditorController.Instance;
            if(instance != null)
                EditorController.Instance.DisplayPlayMode();
        }
    }
}
