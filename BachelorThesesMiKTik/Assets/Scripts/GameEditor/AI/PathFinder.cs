using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.GameEditor.AI
{
    public class PathFinder : MonoBehaviour
    {
        public MapCanvasController MapController;

        private Vector3 startPosition;
        private Vector3 endPosition;

        private Vector3 sceneMinPosition;
        private Vector3 sceneMaxPosition;

        private Dictionary<Vector3, Node> AllAvaliblePositions;
        private PriorityQueue<Node, double> ActiveNodes;
        private Dictionary<Vector3, Node> ClosedNodes;
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

        public void FindPath()
        {
            Initialize();

            while (ActiveNodes.Count > 0)
            {
                var actualNode = ActiveNodes.Dequeue();

                if (actualNode.Position == endPosition)
                {
                    BackTrack(actualNode);
                    return;
                }

                ClosedNodes.Add(actualNode.Position, actualNode);
                var reacheableNodes = GetWalkableNodes(actualNode);

                foreach (var node in reacheableNodes)
                {
                    //We have already visited this tile so we don't need to do so again!
                    if (ClosedNodes.ContainsKey(node.Position))
                        continue;

                    foreach (var activeNode in ActiveNodes.UnorderedItems)
                    {
                        if (activeNode.Element.Position == node.Position)
                        {
                            var existingTile = ActiveNodes.Dequeue();
                            if (existingTile.CostDistance > node.CostDistance)
                            {
                                ActiveNodes.Enqueue(node, node.CostDistance);
                            }
                        }
                        continue;
                    }
                    ActiveNodes.Enqueue(node, node.CostDistance);
                }
            }

            Debug.Log("No Path Found!");
        }

        private void Awake()
        {
            AllAvaliblePositions = new Dictionary<Vector3, Node>();
            ActiveNodes = new PriorityQueue<Node, double>();
            ClosedNodes = new Dictionary<Vector3, Node>();
            Markers = new List<GameObject>();
        }

        private void Initialize()
        {
            DestroyAllMarkers();
            GetScreneBoundaries();
            SetStart();
            SetEnd();
            SetAllAvaliblePositions();

            ActiveNodes = new PriorityQueue<Node, double>();
            ActiveNodes.Enqueue(new Node(startPosition, endPosition), 0);

            ClosedNodes = new Dictionary<Vector3, Node>();
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
                foreach (var startPoint in MapController.Data[0])
                {
                    if (IsPositionInBoundaries(startPoint.Key))
                    {
                        startPosition = startPoint.Key;
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

        private void SetAllAvaliblePositions()
        {
            AllAvaliblePositions = new Dictionary<Vector3, Node>();

            var cellSize = MapController.GridLayout.cellSize;
            foreach (var row in MapController.Data.Values)
            {
                foreach (var item in row)
                {
                    var position = item.Key;
                    var upperNeighbourPosition = MapController.GetCellCenterPosition(new Vector3(position.x, position.y + cellSize.y));
                    if (MapController.ContainsObjectAtPosition(upperNeighbourPosition) || item.Value.layer != 0)
                        continue;

                    AllAvaliblePositions.Add(upperNeighbourPosition, new Node(upperNeighbourPosition, endPosition));
                }
            }
            AllAvaliblePositions.Add(endPosition, new Node(endPosition, endPosition));
        }

        private bool IsPositionInBoundaries(Vector3 position)
        {
            if (sceneMaxPosition.x < position.x || sceneMinPosition.x > position.x ||
               sceneMaxPosition.y < position.y || sceneMinPosition.y > position.y)
                return false;
            return true;
        }

        private void BackTrack(Node actualNode)
        {
            var tile = actualNode;
            while (true)
            {
                Markers.Add(MapController.CreateMarkAtPosition(tile.Position));
                tile = tile.Parent;
                if (tile == null)
                {
                    return;
                }
            }
        }

        private List<Node> GetWalkableNodes(Node actualNode)
        {
            var cellSize = MapController.GridLayout.cellSize;

            var newPositions = new Vector3[]
            {
                MapController.GetCellCenterPosition(new Vector3(actualNode.Position.x + cellSize.x, actualNode.Position.y)), //Right
                MapController.GetCellCenterPosition(new Vector3(actualNode.Position.x + cellSize.x, actualNode.Position.y - cellSize.y)), //LowerRight
                MapController.GetCellCenterPosition(new Vector3(actualNode.Position.x + cellSize.x, actualNode.Position.y + cellSize.y)), //UpperRight

                MapController.GetCellCenterPosition(new Vector3(actualNode.Position.x - cellSize.x, actualNode.Position.y)), //Left
                MapController.GetCellCenterPosition(new Vector3(actualNode.Position.x - cellSize.x, actualNode.Position.y - cellSize.y)), //LowerLeft;
                MapController.GetCellCenterPosition(new Vector3(actualNode.Position.x - cellSize.x, actualNode.Position.y - cellSize.y)) //UpperLeft;
            };

            var newNodes = new List<Node>();
            foreach (var position in newPositions)
            {
                if (AllAvaliblePositions.ContainsKey(position))
                {
                    var node = new Node
                    {
                        Position = position,
                        Parent = actualNode,
                        Cost = actualNode.Cost + 1,
                        Distance = Math.Abs(endPosition.x - position.x) + Math.Abs(endPosition.y - position.y)
                    };
                    newNodes.Add(node);
                }
            }
            return newNodes;
        }

        private void ShowWalkableTiles()
        {
            Initialize();
            foreach (var item in AllAvaliblePositions)
            {
                Markers.Add(MapController.CreateMarkAtPosition(item.Key));
            }
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

class Node
{
    public double Cost { get; set; }
    public double Distance { get; set; }
    public double CostDistance => Cost + Distance;
    public Node Parent { get; set; }
    public Vector3 Position { get; set; }

    public Node() { }

    public Node(Vector3 actualPosition, Vector3 endPosition)
    {
        Position = actualPosition;
        Distance = Math.Abs(endPosition.x - actualPosition.x) + Math.Abs(endPosition.y - actualPosition.y);
    }

    public void SetDistance(Vector3 endPosition)
    {
        this.Distance = Math.Abs(endPosition.x - Position.x) + Math.Abs(endPosition.y - Position.y);
    }
}