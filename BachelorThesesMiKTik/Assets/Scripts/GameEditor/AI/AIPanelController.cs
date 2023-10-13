using Assets.Core.GameEditor.DTOS;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class AIPanelController : MonoBehaviour
    {
        public MapCanvasController MapController;

        private Vector3 startPosition;
        private Vector3 endPosition;

        private Vector3 sceneMinPosition;
        private Vector3 sceneMaxPosition;
        private List<GameObject> Markers;


        public void OnShowWalkableTilesClick()
        {
                DestroyAllMarkers();
                ShowWalkableTiles();
        }

        public void OnShowPosibleActions()
        {
            Initialize();
            var selectedObject = MapController.GetObjectAtPosition(startPosition);
            if (selectedObject != null)
            {
                AIAgent aiController;
                if (selectedObject.TryGetComponent(out aiController))
                {
                    foreach (var action in aiController.AI.Actions)
                    {
                        action.PrintReacheables(selectedObject.transform.position).ForEach(marker => Markers.Add(marker));
                    }
                }
            }
        }

        public void OnShowJumps()
        {
            Initialize();
            var selectedObject = MapController.GetObjectAtPosition(startPosition);
            if (selectedObject != null)
            {
                AIAgent aiController;
                if (selectedObject.TryGetComponent(out aiController))
                {
                    foreach (var action in aiController.AI.Actions)
                    {
                        if (action is JumpAIAction)
                            Markers = ( (JumpAIAction) action ).PrintAllPossibleJumps(selectedObject.transform.position);
                    }
                }
            }
        }

        public void OnFindPathClick()
        {
            Initialize();
            var path =  FindPath(startPosition, endPosition);

            if (path != null) 
            {
                PrintPath(path);
            }
        }

        public void OnFindPathAndPerform()
        {
            Initialize();
            var selectedObject = MapController.GetObjectAtPosition(startPosition);
            var path = FindPath(startPosition, endPosition);

            foreach(var action in path)
            {
                action.Performer(action);
            }
        }

        public void OnCleanClick()
        {
            DestroyAllMarkers();
        }

        #region PRIVATE

        private void Awake()
        {
            Markers = new List<GameObject>();
        }

        private void Initialize()
        {
            DestroyAllMarkers();
            GetScreneBoundaries();
            SetStart();
            SetEnd();
        }

        private List<AgentActionDTO> FindPath(Vector3 startPosition, Vector3 endPosition)
        {
            var selectedObject = MapController.GetObjectAtPosition(startPosition);
            if (selectedObject != null)
            {
                var astar = new AStar();

                AIAgent aiController;
                if (selectedObject.TryGetComponent(out aiController))
                {
                    return astar.FindPath(selectedObject.transform.position, endPosition, aiController.AI.Actions);
                }
            }
            return null;
        }

        private void PrintPath(List<AgentActionDTO> path)
        {
            foreach (var action in path)
            {
                action.PrintingPerformer(action).ForEach(marker => Markers.Add(marker));
            }
        }

        private void GetScreneBoundaries()
        {
            sceneMinPosition = MapController.CameraObj.ScreenToWorldPoint(new Vector3(0, 0));
            sceneMaxPosition = MapController.CameraObj.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        }

        private void SetStart()
        {
            if (MapController.Data.ContainsKey(0))
            {
                foreach (var position in MapController.Data[0].Keys)
                {
                    if (MapController.Selected.ContainsKey(position))
                    {
                        if (IsPositionInBoundaries(position))
                        {
                            startPosition = position;
                        }
                    }
                }
            }
        }

        private void SetEnd()
        {
            if (MapController.Data.ContainsKey(1))
            {
                foreach (var endPoint in MapController.Data[1])
                {
                    if (IsPositionInBoundaries(endPoint.Key))
                    {
                        endPosition = endPoint.Key;
                    }
                }
            }
        }
        private bool IsPositionInBoundaries(Vector3 position)
        {
            if (sceneMaxPosition.x < position.x || sceneMinPosition.x > position.x ||
               sceneMaxPosition.y < position.y || sceneMinPosition.y > position.y)
                return false;
            return true;
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
                    if (position == endPosition)
                        continue;

                    var upperNeighbourPosition = MapController.GetCellCenterPosition(new Vector3(position.x, position.y + cellSize.y));
                    if (MapController.ContainsObjectAtPosition(upperNeighbourPosition) || item.Value.layer != 7)
                        continue;

                    Markers.Add(MapController.CreateMarkAtPosition(upperNeighbourPosition));
                }
            }
            Markers.Add(MapController.CreateMarkAtPosition(endPosition));
        }

        private void DestroyAllMarkers()
        {
            foreach (var mark in Markers)
            {
                MapController.DestroyMark(mark);
            }
            Markers.Clear();
        }

        #endregion
    }
}