using Assets.Core.GameEditor.DTOS;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.AIActions.Movement
{
    public class MoveHelper
    {
        private float sin;
        private float iterations;

        private Rigidbody2D rigidbody;
        private float speed;
        private Vector2 startPosition;
        private Vector2 endPosition;

        public MoveHelper(Rigidbody2D rigid, float speed, Vector2 startPosition, Vector2 endPosition) 
        {
            sin = 0;
            iterations = 0;

            this.rigidbody = rigid;
            this.speed = speed;
            this.startPosition = startPosition;
            this.endPosition = endPosition;
        }

        public bool TranslationTick(GameObject performer, EditorCanvas map, float deltaTime)
        {
            if (map.GetCellCenterPosition(performer.transform.position) != endPosition)
            {
                sin += deltaTime * ( speed / 50);
                sin = Mathf.Clamp(sin, 0, Mathf.PI);

                var x = 0.6f * Mathf.Sin(sin - Mathf.PI / 2f) + 0.5f;
                if(x != 0)
                    performer.transform.position = Vector2.Lerp(startPosition, endPosition, x);

                iterations++;
                if (iterations > 1000000)
                    return true;

                return false;
            }
            return true;
        }

        public static bool CheckIfStaysOnGround(GameObject gameObject)
        {
            var position = (Vector2) gameObject.transform.position;
            var collider = gameObject.GetComponent<Collider2D>();
            var colliderSize = collider.bounds.size.y;

            var hits = Physics2D.RaycastAll(position, Vector2.down, colliderSize);
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                    return true;
            }
            return false;
        }

        public static Vector2 FindContinuousPath(AgentActionDTO firstAction, Queue<AgentActionDTO> actions)
        {
            var actualAction = firstAction;
            var actionType = firstAction.ActionParameters;
            
            while(actions.Count > 0)
            { 
                var action = actions.Peek();
                if(action.ActionParameters != actionType)
                {  
                    return actualAction.EndPosition;
                }

                actions.Dequeue();
                actualAction = action;
            }

            return actualAction.EndPosition;
        }
    }
}
