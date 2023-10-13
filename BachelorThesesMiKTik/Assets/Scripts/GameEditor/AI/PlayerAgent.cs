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
    }

    private void FixedUpdate()
    {
        if(AI.Actions.Count > 0) 
        {
            OnAction();
        }
    }


    public override void Simulate()
    {
        throw new System.NotImplementedException();
    }

    public override void PrintSimulation()
    {
        throw new System.NotImplementedException();
    }
}
