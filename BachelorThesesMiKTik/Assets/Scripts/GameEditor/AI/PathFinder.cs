using Assets.Core.GameEditor.DTOS;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

namespace Assets.Scripts.GameEditor.AI
{
    public class PathFinder : MonoBehaviour
    {
        public MapCanvasController MapController;

        public GameObject player;
        public GameObject TrajectoryMarker;

        private Vector3 startPosition;
        private Vector3 endPosition;

        private Vector3 sceneMinPosition;
        private Vector3 sceneMaxPosition;
        private List<GameObject> Markers;

        private bool IsShowingWalkableTiles;
        private bool IsShowingPath;

        public void OnShowWalkableTilesClick()
        {
            if (IsShowingWalkableTiles)
            {
                DestroyAllMarkers();
                IsShowingWalkableTiles = false;
            }
            else
            {
                ShowWalkableTiles();
                IsShowingWalkableTiles = true;
            }
        }

        public void OnShowJumpable()
        {
            Initialize();
            var selectedObject = MapController.GetObjectAtPosition(startPosition);
            if (selectedObject != null)
            {
                var jumpHandler = new JumpAIAction(MapController, selectedObject);
                jumpHandler.PrintJumpables().ForEach(marker => Markers.Add(marker));
            }
        }

        public void OnFindPathClick()
        {
            if (IsShowingPath)
            {
                FindPath();
                IsShowingPath = false;
            }
            else
            {
                ShowWalkableTiles();
                IsShowingPath = true;
            }
        }

        public void OnCleanClick()
        {
            DestroyAllMarkers();
        }

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

        private void FindPath()
        {
            Initialize();
            var selectedObject = MapController.GetObjectAtPosition(startPosition);
            if (selectedObject != null)
            {
                var astar = new AStar(MapController, selectedObject);
                var path = astar.FindPath(endPosition);
                if (path != null)
                {
                    PrintPath(path);
                }
            }
        }

        private void PrintPath(List<AgentActionDTO> path)
        {
            foreach (var action in path) 
            {
                action.PrinttingPerformer(action.StartPosition, action.PositionActionParameter).ForEach(marker => Markers.Add(marker));
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
    }
}