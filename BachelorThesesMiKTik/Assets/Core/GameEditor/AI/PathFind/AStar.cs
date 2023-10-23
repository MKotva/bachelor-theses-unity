using Assets.Core.GameEditor.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scenes.GameEditor.Core.AIActions;
using Assets.Scripts.GameEditor.AI.PathFind;

namespace Assets.Scripts.GameEditor.AI
{
    public class AStar : IAIPathFinder
    {
        private Vector3 _startPosition;
        private Vector3 _endPosition;

        private List<Node> _activeNodes;
        private List<AIActionBase> _actions;
        private Dictionary<Vector3, Node> _closedNodes;

        public List<AgentActionDTO> FindPath(Vector3 startPosition, Vector3 endPosition, List<AIActionBase> actions)
        {
            Initialize(startPosition, endPosition, actions);

            while (_activeNodes.Count > 0)
            {
                var actualNode = _activeNodes.First();
                _activeNodes.Remove(actualNode);

                if (actualNode.Position == endPosition)
                {
                    return BackTrackActions(actualNode);
                }

                _closedNodes.Add(actualNode.Position, actualNode);
                var reacheableNodes = GetWalkableNodes(actualNode);

                foreach (var node in reacheableNodes)
                {
                    //We have already visited this tile so we don't need to do so again!
                    if (_closedNodes.ContainsKey(node.Position))
                        continue;

                    if (!TryAdjustCost(node))
                        _activeNodes.Add(node);
                }
                _activeNodes = _activeNodes.OrderBy(node => node.CostDistance).ToList();
            }

            InfoPanelController.Instance.ShowMessage("No path found!");
            return null;
        }

        private bool TryAdjustCost(Node node)
        {
            foreach (var activeNode in _activeNodes)
            {
                if (activeNode.Position == node.Position)
                {
                    if (activeNode.CostDistance > node.CostDistance)
                    {
                        _activeNodes.Remove(activeNode);
                        _activeNodes.Add(node);
                    }
                    return true;
                }
            }
            return false;
        }
        private void Initialize(Vector3 startPosition, Vector3 endPosition, List<AIActionBase> actions)
        {
            _activeNodes = new List<Node>();

            _startPosition = startPosition;
            _endPosition = endPosition;
            _actions = actions;

            _activeNodes.Add(new Node(_startPosition, _endPosition));
            _closedNodes = new Dictionary<Vector3, Node>();
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

            foreach(var action in _actions)
                action.GetPossibleActions(actualNode.Position).ForEach(action => actionResults.Add(action));


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

