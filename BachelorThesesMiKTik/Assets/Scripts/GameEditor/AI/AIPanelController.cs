using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class AIPanelController : MonoBehaviour
    {
        public MapCanvasController MapController;
        private AIAgent agent;
        private List<GameObject> markers;


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
        }

        private void Initialize()
        {
            DestroyAllMarkers();
            agent = FindSelectedObject();
        }

        private AIAgent FindSelectedObject()
        {
            if (MapController.Data.ContainsKey(0))
            {
                foreach (var position in MapController.Data[0].Keys)
                {
                    if (MapController.Selected.ContainsKey(position))
                    {
                        if (MapController.IsPositionInBoundaries(position))
                        {
                            var selectedObject = MapController.GetObjectAtPosition(position);

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

            var cellSize = MapController.GridLayout.cellSize;
            foreach (var row in MapController.Data.Values)
            {
                foreach (var item in row)
                {
                    var position = item.Key;
                    var upperNeighbourPosition = MapController.GetCellCenterPosition(new Vector3(position.x, position.y + cellSize.y));
                    if (MapController.ContainsObjectAtPosition(upperNeighbourPosition) || item.Value.layer != 7)
                        continue;

                    markers.Add(MapController.CreateMarkAtPosition(upperNeighbourPosition));
                }
            }
        }

        private void DestroyAllMarkers()
        {
            foreach (var mark in markers)
            {
                MapController.DestroyMark(mark);
            }
            markers.Clear();
        }

        #endregion
    }
}