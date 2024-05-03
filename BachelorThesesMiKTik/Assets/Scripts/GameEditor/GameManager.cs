using Assets.Scripts.GameEditor.ObjectInstancesController;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameEditor
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] public GameObject SuccesPanel;
        [SerializeField] public GameObject RestartPanel;
        [SerializeField] public Canvas ToolkitCanvas;
        [SerializeField] public Canvas PopUpCanvas;
        [SerializeField] public Canvas PlayModeCanvas;

        public Dictionary<int, IObjectController> ActiveObjects { get; set; }
        public Dictionary<int, GameObject> ActivePlayers { get; set; }
        public bool IsInPlayMode { get; set; }
        public float LowestYPoint { get; set; }


        public void AddActiveObject(int id, IObjectController obj)
        {
            if (!ActiveObjects.ContainsKey(id))
                ActiveObjects.Add(id, obj);
        }

        public void RemoveActiveObject(int id)
        {
            if (ActiveObjects.ContainsKey(id))
                ActiveObjects.Remove(id);
        }

        public void AddPlayer(int id, GameObject obj)
        {
            if (!ActivePlayers.ContainsKey(id))
                ActivePlayers.Add(id, obj);
        }

        public void RemovePlayer(int id)
        {
            if (ActiveObjects.ContainsKey(id))
            {
                ActivePlayers.Remove(id);
                if (ActivePlayers.Count == 0)
                {
                    ShowGameFail();
                }
            }
        }

        #region PlayMode
        /// <summary>
        /// If is in PlayMode, reenables toolkit panel
        /// </summary>
        public void EnableEditor()
        {
            PopUpCanvas.gameObject.SetActive(false);
            ToolkitCanvas.gameObject.SetActive(true);
            PopUpCanvas.gameObject.SetActive(true);
        }

        /// <summary>
        /// If is in PlayMode, disables toolkit panel
        /// </summary>
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
            EnterGame();
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


        public void RestartGame()
        {
            ExitGame();
            EnterGame();
            StartGame();
        }

        public void EnterGame()
        {
            foreach (var obj in ActiveObjects.Values)
                obj.Enter();
        }

        public void StartGame()
        {
            foreach (var obj in ActiveObjects.Values)
                obj.Play();

            if (ActivePlayers.Count == 0)
                OutputManager.Instance.ShowMessage("Warning! No active player present.");

            FindLowestPoint();
        }

        public void PauseGame()
        {
            foreach (var obj in ActiveObjects.Values)
                obj.Pause();
        }

        public void ExitGame()
        {
            foreach (var obj in ActiveObjects.Values)
                obj.Exit();

            ActivePlayers.Clear();
        }

        public void ShowGameFail()
        {
            PauseGame();
            Instantiate(RestartPanel, PopUpCanvas.transform);
        }

        public void ShowGameSucces()
        {
            PauseGame();
            Instantiate(SuccesPanel, PopUpCanvas.transform);
        }

        #region Private

        /// <summary>
        /// Finds the lowest object in map for fall checks.
        /// </summary>
        private void FindLowestPoint()
        {
            var editorCanvas = EditorCanvas.Instance;
            if (editorCanvas != null)
            {
                var fistGroup = editorCanvas.Data.Keys.First();
                LowestYPoint = editorCanvas.Data[fistGroup].Keys.First().y;
                foreach(var ob in editorCanvas.Data.Values)
                {
                    foreach(var position in ob.Keys)
                    {
                        if(position.y < LowestYPoint)
                            LowestYPoint = position.y;
                    }
                }
            }
            else
            {
                LowestYPoint = -1000;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            ActiveObjects = new Dictionary<int, IObjectController>();
            ActivePlayers = new Dictionary<int, GameObject> { };
        }
        #endregion
    }
}
