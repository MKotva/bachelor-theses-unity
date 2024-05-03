using Assets.Core.GameEditor.AIActions;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.ObjectInstancesController.Components.Entiti;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class AIPanelController : MonoBehaviour
    {
        private ActionsAgent agent;
        private List<GameObject> markers;
        private EditorCanvas map;

        public void OnShowWalkableTilesClick()
        {
            DestroyAllMarkers();
            ShowWalkableTiles();
        }

        public void OnShowPosibleActions()
        {
            TryInitialize();
            markers = agent.PrintPossibleActions();
        }

        public void OnShowJumps()
        {
            if (!TryInitialize())
                return;

            foreach (var action in agent.Actions)
            {
                if (action is JumpAction)
                    markers = ((JumpAction) action).PrintAllPossibleJumps(agent.transform.position);
                else if(action is ChargeableJumpAction)
                    markers = ((ChargeableJumpAction) action).PrintAllPossibleJumps(agent.transform.position);
            }
        }

        public void OnPrintSimulation()
        {
            if (!TryInitialize())
                return;

            if (TryFindEndPosition("Finish", out var endpoint))
                markers = agent.PrintMoveTo(endpoint);
        }

        public void OnSimulate()
        {
            if (!TryInitialize())
                return;

            if(TryFindEndPosition("Finish", out var endpoint))
                agent.MoveTo(endpoint);
        }

        public void OnCleanClick()
        {
            DestroyAllMarkers();
        }
        #region PRIVATE

        private void Awake()
        {
            markers = new List<GameObject>();
            map = EditorCanvas.Instance;
        }

        private bool TryInitialize()
        {
            DestroyAllMarkers();
            if (TryFindSelectedObject(out agent))
                return true;

            OutputManager.Instance.ShowMessage("Selected object does not contain AIAgent component.");
            return false;
        }

        private bool TryFindSelectedObject(out ActionsAgent aIAgent)
        {
            if (map.Selected.Count == 1)
            {
                var selectedObject = map.GetObjectAtPosition(map.Selected.Keys.First());
                if (selectedObject.TryGetComponent(out ActionsAgent agent))
                {
                    aIAgent = agent;
                    return true;
                }
            }
            aIAgent = null;
            return false;
        }

        private bool TryFindEndPosition(string name, out Vector3 endPosition)
        {
            if (ItemManager.Instance.TryFindIdByName(name, out int edpointId))
            {
                if (map.Data.ContainsKey(edpointId))
                {
                    endPosition = map.Data[edpointId].Keys.First();
                    return true;
                }
            }

            endPosition = Vector3.zero;
            return false;
        }


        //TODO : ADD if is walkable option to ItemData
        private void ShowWalkableTiles()
        {
            TryInitialize();

            var cellSize = map.GridLayout.cellSize;
            foreach (var row in map.Data.Values)
            {
                foreach (var item in row)
                {
                    var position = item.Key;
                    var upperNeighbourPosition = map.GetCellCenterPosition(new Vector3(position.x, position.y + cellSize.y));
                    if (map.ContainsObjectAtPosition(upperNeighbourPosition) || item.Value.layer != 7)
                        continue;

                    markers.Add(map.Marker.CreateMarkAtPosition(upperNeighbourPosition));
                }
            }
        }

        private void DestroyAllMarkers()
        {
            foreach (var mark in markers)
            {
                map.Marker.DestroyMark(mark);
            }
            markers.Clear();
        }

        #endregion
    }
}