using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using Assets.Scripts.GameEditor.AI;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : AIAgent
{
    private void Awake()
    {
        context = GameObject.Find("MapPanel").GetComponent<MapCanvasController>();
        var actions = new List<AIActionBase>()
        {
            new JumpAIAction(context, gameObject),
            new MoveAIAction(context, gameObject)
        };

        AI = new AIObject(gameObject, actions);
        pathFinder = new AStar();
    }

    private void FixedUpdate()
    {
            OnAction();
    }

    private Vector3 FindDestination()
    {
        if (context.Data.ContainsKey(1))
        {
            foreach (var endPoint in context.Data[1])
            {
                if (context.IsPositionInBoundaries(endPoint.Key))
                {
                    return endPoint.Key;
                }
            }
        }
        return Vector3.zero; //TODO: Proper failure, no suitable endpoint.
    }

    public override void Simulate()
    {
        var path = pathFinder.FindPath(gameObject.transform.position, FindDestination(), AI.Actions);
        if (path != null) 
        {
            EnqueAction(path);
        }
    }

    public override List<GameObject> PrintSimulation()
    {
        var path = pathFinder.FindPath(gameObject.transform.position, FindDestination(), AI.Actions);
        return AgentActionDTO.Print(path);
    }

    public override List<GameObject> PrintPossibleActions()
    {
        List<GameObject> markers = new List<GameObject>();
        foreach(var action in AI.Actions) 
        {
            action.PrintReacheables(gameObject.transform.position).ForEach(x => markers.Add(x));
        }
        return markers;
    }
}
