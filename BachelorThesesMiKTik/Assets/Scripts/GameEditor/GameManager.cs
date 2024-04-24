using Assets.Scripts.GameEditor.ObjectInstancesController;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] public Canvas ToolkitCanvas;
        [SerializeField] public Canvas PopUpCanvas;
        [SerializeField] public Canvas PlayModeCanvas;

        public bool IsInPlayMode { get; set; }
        private Dictionary<int, IObjectController> activeObjects;

        #region PlayMode
        public void AddActiveObject(int id, IObjectController obj) 
        {
            if(!activeObjects.ContainsKey(id))
                activeObjects.Add(id, obj);
        }

        public void RemoveActiveObject(int id) 
        {
            if (activeObjects.ContainsKey(id))
                activeObjects.Remove(id);
        }

        public void EnableEditor()
        {
            PopUpCanvas.gameObject.SetActive(false);
            ToolkitCanvas.gameObject.SetActive(true);
            PopUpCanvas.gameObject.SetActive(true);
        }

        public void DisableEditor()
        {
            ToolkitCanvas.gameObject.SetActive(false);
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

            foreach (var obj in activeObjects.Values)
                obj.Enter();
        }

        public void StartPlayMode() 
        {
            if (!IsInPlayMode)
            {
                return;
            }

            StartGame();
        }

        public void PausePlayMode() 
        {
            if (!IsInPlayMode)
            {
                return;
            }
            
            PauseGame();
        }

        public void ExitPlayMode()
        {
            if (!IsInPlayMode)
            {
                return;
            }
            EnableEditor();
            PlayModeCanvas.gameObject.SetActive(false);
            IsInPlayMode = false;

            ExitGame();
        }

        #endregion

        public void StartGame()
        {
            foreach (var obj in activeObjects.Values)
                obj.Play();
        }

        public void PauseGame()
        {
            foreach (var obj in activeObjects.Values)
                obj.Pause();
        }

        public void ExitGame()
        {
            foreach (var obj in activeObjects.Values)
                obj.Exit();
        }

        protected override void Awake()
        {
            base.Awake();
            activeObjects = new Dictionary<int, IObjectController>();
        }
    }
}
