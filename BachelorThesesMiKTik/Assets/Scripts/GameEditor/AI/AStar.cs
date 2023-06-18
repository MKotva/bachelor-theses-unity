using Assets.Core.GameEditor.DTOS;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.GameEditor.AI
{
    public class AStar
    {
        public MapCanvasController MapController;

        private GameObject _player;

        private Vector3 _startPosition;
        private Vector3 _endPosition;

        private PriorityQueue<Node, double> ActiveNodes;
        private Dictionary<Vector3, Node> ClosedNodes;


        private JumpAIAction jumpAction;
        private MoveAIAction moveAIAction;

        public AStar(MapCanvasController controller, GameObject player)
        {
            MapController = controller;
            _player = player;

            jumpAction = new JumpAIAction(MapController, _player);
            moveAIAction = new MoveAIAction(MapController);
        }

        public void FindPath(Vector3 endPosition)
        {
            Initialize(endPosition);

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

                    if(!TryAdjustCost(node))
                        ActiveNodes.Enqueue(node, node.CostDistance);
                }
            }

            Debug.Log("No Path Found!");
        }
        
        private bool TryAdjustCost(Node node)
        {
            foreach (var activeNode in ActiveNodes.UnorderedItems)
            {
                if (activeNode.Element.Position == node.Position)
                {
                    if (activeNode.Element.CostDistance > node.CostDistance)
                    {
                        activeNode.Element.Cost = node.Cost;
                        activeNode.Element.Distance = node.Distance;
                    }
                    return true;
                }
            }
            return false;
        }
        private void Initialize(Vector3 endPositon)
        {
            ActiveNodes = new PriorityQueue<Node, double>();

            _startPosition = _player.transform.position;
            _endPosition = endPositon;

            ActiveNodes.Enqueue(new Node(_startPosition, _endPosition), 0);
            ClosedNodes = new Dictionary<Vector3, Node>();
        }

        private void BackTrack(Node actualNode)
        {
            var tile = actualNode;
            while (true)
            {
                MapController.CreateMarkAtPosition(tile.Position);
                tile = tile.Parent;
                if (tile == null)
                {
                    return;
                }
            }
        }

        private List<Node> GetWalkableNodes(Node actualNode)
        {
            var newNodes = new List<Node>();
            var actionResults = new List<AgentActionDTO>();

            //TODO:Replace
            actionResults.Add(moveAIAction.GetReacheablePosition(actualNode.Position));
            actionResults.Add(jumpAction.GetReacheablePosition(actualNode.Position));


            foreach (var actionResult in actionResults)
            {
                for (int i = 0; i < actionResult.ReachablePositions.Count; i++)
                {
                    var position = actionResult.ReachablePositions[i];

                    var node = new Node
                    {
                        Cost = actualNode.Cost + actionResult.Cost,
                        Distance = Math.Abs(_endPosition.x - position.x) + Math.Abs(_endPosition.y - position.y),
                        Parent = actualNode,
                        Position = position,
                        ActionRecord = actionResult.PositionActionParameters[i]
                    };

                    newNodes.Add(node);
                }
            }
            return newNodes;
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
    public string ActionRecord { get; set; }

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

