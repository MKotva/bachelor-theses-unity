using Assets.Core.GameEditor.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static Assets.Core.GameEditor.DTOS.JournalActionDTO;
using static Assets.Core.GameEditor.DTOS.AgentActionDTO;

namespace Assets.Scripts.GameEditor.AI
{
    public class AStar
    {
        public MapCanvasController MapController;

        private GameObject _player;

        private Vector3 _startPosition;
        private Vector3 _endPosition;

        private List<Node> ActiveNodes;
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

        public List<AgentActionDTO> FindPath(Vector3 endPosition)
        {
            Initialize(endPosition);

            while (ActiveNodes.Count > 0)
            {
                var actualNode = ActiveNodes.First();
                ActiveNodes.Remove(actualNode);

                if (actualNode.Position == endPosition)
                {
                    return BackTrackActions(actualNode);
                }

                ClosedNodes.Add(actualNode.Position, actualNode);
                var reacheableNodes = GetWalkableNodes(actualNode);

                foreach (var node in reacheableNodes)
                {
                    //We have already visited this tile so we don't need to do so again!
                    if (ClosedNodes.ContainsKey(node.Position))
                        continue;

                    if (!TryAdjustCost(node))
                        ActiveNodes.Add(node);
                }
                ActiveNodes = ActiveNodes.OrderBy(node => node.CostDistance).ToList();
            }

            Debug.Log("No Path Found!");
            return null;
        }

        private bool TryAdjustCost(Node node)
        {
            foreach (var activeNode in ActiveNodes)
            {
                if (activeNode.Position == node.Position)
                {
                    if (activeNode.CostDistance > node.CostDistance)
                    {
                        ActiveNodes.Remove(activeNode);
                        ActiveNodes.Add(node);
                    }
                    return true;
                }
            }
            return false;
        }
        private void Initialize(Vector3 endPositon)
        {
            ActiveNodes = new List<Node>();

            _startPosition = _player.transform.position;
            _endPosition = endPositon;

            ActiveNodes.Add(new Node(_startPosition, _endPosition));
            ClosedNodes = new Dictionary<Vector3, Node>();
        }

        private List<AgentActionDTO> BackTrackActions(Node actualNode)
        {
            Stack<AgentActionDTO> stack = new Stack<AgentActionDTO>();

            var node = actualNode;
            while (true)
            {
                if (node.Parent == null)
                {
                    break;
                }

                stack.Push(node.ActionRecord);
                node = node.Parent;
            }

            if (stack.Count == 0)
                return null; //TODO: Exception.

            return stack.ToList();
        }

        private List<Node> GetWalkableNodes(Node actualNode)
        {
            var newNodes = new List<Node>();
            var actionResults = new List<AgentActionDTO>();

            moveAIAction.GetReacheablePosition(actualNode.Position).ForEach(action => actionResults.Add(action));
            jumpAction.GetReacheablePosition(actualNode.Position).ForEach(action => actionResults.Add(action));


            foreach (var actionResult in actionResults)
            {
                var position = actionResult.EndPosition;
                var node = new Node
                {
                    Cost = actualNode.Cost + actionResult.Cost,
                    Distance = Math.Abs(_endPosition.x - position.x) + Math.Abs(_endPosition.y - position.y),
                    Parent = actualNode,
                    Position = position,
                    ActionRecord = actionResult                  
                };

                newNodes.Add(node);
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
    public AgentActionDTO ActionRecord { get; set; }

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

