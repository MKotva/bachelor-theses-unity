﻿using Assets.Core.GameEditor.Serializers;
using Assets.Scenes.GameEditor.Core.DTOS;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public CameraController Camera { get; set; }
        public Dictionary<int, IObjectController> ActiveObjects { get; set; }
        public Dictionary<int, GameObject> ActivePlayers { get; set; }
        public bool IsInPlayMode { get; set; }
        public float LowestYPoint { get; set; }

        #region ObjectManagment
        public void AddActiveObject(int id, IObjectController obj)
        {
            if (ActiveObjects == null)
                return;

            if (!ActiveObjects.ContainsKey(id))
                ActiveObjects.Add(id, obj);
        }

        public void RemoveActiveObject(int id)
        {
            if (ActiveObjects == null)
                return;

            if (ActiveObjects.ContainsKey(id))
                ActiveObjects.Remove(id);
        }

        public void AddPlayer(int id, GameObject obj)
        {
            if (ActiveObjects == null)
                return;

            if (!ActivePlayers.ContainsKey(id))
                ActivePlayers.Add(id, obj);
        }

        public void RemovePlayer(int id)
        {
            if (ActivePlayers == null)
                return;

            if (ActivePlayers.ContainsKey(id))
            {
                ActivePlayers.Remove(id);
                Camera.SetFollowTransform();

                if (ActivePlayers.Count == 0)
                {
                    ShowGameFail();
                }
            }
        }
        #endregion

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

        #region GameManagement
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

            Camera.Enter();
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
            Camera.Exit();

            foreach (var obj in ActiveObjects.Values)
                obj.Exit();

            ActivePlayers.Clear();
            OutputManager.Instance.ClearMessages();
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

        public void Clear()
        {
            ActivePlayers = new Dictionary<int, GameObject> { };
            ActiveObjects = new Dictionary<int, IObjectController> { };
        }

        #endregion

        public async Task LoadLevelFromGame(string path)
        {
            Clear();
            await LoadDataHandler.LoadMap(path);
            IsInPlayMode = true;
            EnterGame();
            StartGame();
        }

        public async Task LoadLevel(GameDataDTO data)
        {
            Clear();
            await GameDataSerializer.Deserialize(data);
            IsInPlayMode = true;
            EnterGame();
            StartGame();
        }

        #region Private

        /// <summary>
        /// Finds the lowest object in map for fall checks.
        /// </summary>
        private void FindLowestPoint()
        {
            var editorCanvas = EditorCanvas.Instance;
            if (editorCanvas == null)
            {
                LowestYPoint = -1000;
            }
            else if (editorCanvas.Data.Count == 0)
            {
                LowestYPoint = -1000;
            }
            else
            {
                var fistGroup = editorCanvas.Data.Keys.First();
                LowestYPoint = editorCanvas.Data[fistGroup].Keys.First().y;
                foreach (var ob in editorCanvas.Data.Values)
                {
                    foreach (var position in ob.Keys)
                    {
                        if (position.y < LowestYPoint)
                            LowestYPoint = position.y;
                    }
                }
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
