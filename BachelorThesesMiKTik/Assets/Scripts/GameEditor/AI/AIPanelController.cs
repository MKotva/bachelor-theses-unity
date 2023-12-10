using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class AIPanelController : MonoBehaviour
    {
        private AIAgent agent;
        private List<GameObject> markers;
        private Editor editor;

        public void OnShowWalkableTilesClick()
        {
            DestroyAllMarkers();
            ShowWalkableTiles();
        }

        public void OnShowPosibleActions()
        {
            Initialize();

            if (agent == null)
            {
                InfoPanelController.Instance.ShowMessage("Selected object does not contain AIAgent component.");
                return;
            }
            markers = agent.PrintPossibleActions();
        }

        public void OnShowJumps()
        {
            Initialize();

            if(agent == null) 
            {
                InfoPanelController.Instance.ShowMessage("Selected object does not contain AIAgent component.");
                return;
            }

            foreach (var action in agent.AI.Actions)
            {
                if (action is JumpAIAction)
                    markers = ( (JumpAIAction) action ).PrintAllPossibleJumps(agent.transform.position);
            }
        }

        public void OnPrintSimulation()
        {
            Initialize();
            if (agent == null)
            {
                InfoPanelController.Instance.ShowMessage("Selected object does not contain AIAgent component.");
                return;
            }

            markers = agent.PrintSimulation();
        }

        public void OnSimulate()
        {
            Initialize();
            if (agent == null)
            {
                InfoPanelController.Instance.ShowMessage("Selected object does not contain AIAgent component.");
                return;
            }
            agent.Simulate();
        }

        public void OnCleanClick()
        {
            DestroyAllMarkers();
        }

        #region PRIVATE

        private void Awake()
        {
            markers = new List<GameObject>();
            editor = Editor.Instance;
        }

        private void Initialize()
        {
            DestroyAllMarkers();
            agent = FindSelectedObject();
        }

        private AIAgent FindSelectedObject()
        {
            if (editor.Data.ContainsKey(0))
            {
                foreach (var position in editor.Data[9].Keys) //TODO: Change so it will not be hardcoded.(Item Id selection)
                {
                    if (editor.Selected.ContainsKey(position))
                    {
                        if (editor.IsPositionInBoundaries(position))
                        {
                            var selectedObject = editor.GetObjectAtPosition(position);

                            AIAgent agent;
                            if (selectedObject.TryGetComponent(out agent))
                            {
                                return agent;
                            }
                        }
                    }
                }
            }
            return null; //TODO: Fix, proper fail. Also rework the method to be more exact in selection of object.
        }

        private void ShowWalkableTiles()
        {
            Initialize();

            var cellSize = editor.GridLayout.cellSize;
            foreach (var row in editor.Data.Values)
            {
                foreach (var item in row)
                {
                    var position = item.Key;
                    var upperNeighbourPosition = editor.GetCellCenterPosition(new Vector3(position.x, position.y + cellSize.y));
                    if (editor.ContainsObjectAtPosition(upperNeighbourPosition) || item.Value.layer != 7)
                        continue;

                    markers.Add(editor.CreateMarkAtPosition(upperNeighbourPosition));
                }
            }
        }

        private void DestroyAllMarkers()
        {
            foreach (var mark in markers)
            {
                editor.DestroyMark(mark);
            }
            markers.Clear();
        }

        #endregion
    }
}