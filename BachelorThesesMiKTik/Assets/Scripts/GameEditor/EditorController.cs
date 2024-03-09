using UnityEngine;

namespace Assets.Scripts.GameEditor
{
    public class EditorController : Singleton<EditorController>
    {
        [SerializeField] public Canvas ToolkitCanvas;
        [SerializeField] public Canvas PopUpCanvas;
        [SerializeField] public Canvas PlayModeCanvas;

        public delegate void PlayModeChangeHandler();
        public event PlayModeChangeHandler PlayModeEnter;
        public event PlayModeChangeHandler PlayModePause;
        public event PlayModeChangeHandler PlayModeExit;
        public bool IsInPlayMode { get; private set; } 

        public void EnableEditor()
        {
            ToolkitCanvas.gameObject.SetActive(true);
            PopUpCanvas.gameObject.SetActive(true);
        }

        public void DisableEditor()
        {
            ToolkitCanvas.gameObject.SetActive(false);
            PopUpCanvas.gameObject.SetActive(false);
        }    


        public void DisplayPlayMode() 
        {
            if (IsInPlayMode) 
            {
                return;
            }

            DisableEditor();
            PlayModeCanvas.gameObject.SetActive(true);
            IsInPlayMode = true;

        }

        public void StartPlayMode() 
        {
            if (!IsInPlayMode)
            {
                return;
            }
            PlayModeEnter?.Invoke();
        }

        public void PausePlayMode() 
        {
            if (!IsInPlayMode)
            {
                return;
            }
            PlayModePause?.Invoke();
        }

        public void ExitPlayMode()
        {
            if (!IsInPlayMode)
            {
                return;
            }
            EnableEditor();
            PlayModeExit?.Invoke();
            PlayModeCanvas.gameObject.SetActive(false);
            IsInPlayMode = false;
        }
    }
}
