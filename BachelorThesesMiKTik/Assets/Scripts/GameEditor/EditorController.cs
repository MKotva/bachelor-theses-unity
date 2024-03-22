using Assets.Scripts.GameEditor.ObjectInstancesController;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor
{
    public class EditorController : Singleton<EditorController>
    {
        [SerializeField] public Canvas ToolkitCanvas;
        [SerializeField] public Canvas PopUpCanvas;
        [SerializeField] public Canvas PlayModeCanvas;

        public bool IsInPlayMode { get; private set; }
        private Dictionary<int, IObjectController> activeObjects;
        
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

            foreach (var obj in activeObjects.Values)
                obj.Play();
        }

        public void PausePlayMode() 
        {
            if (!IsInPlayMode)
            {
                return;
            }

            foreach (var obj in activeObjects.Values)
                obj.Pause();
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

            foreach (var obj in activeObjects.Values)
                obj.Exit();
        }

        protected override void Awake()
        {
            activeObjects = new Dictionary<int, IObjectController>();
        }
    }
}
