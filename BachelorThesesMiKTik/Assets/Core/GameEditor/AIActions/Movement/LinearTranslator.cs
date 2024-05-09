using Assets.Core.GameEditor.DTOS;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.AIActions.Movement
{
    public class LinearTranslator
    {
        private float sin;
        private float speed;
        private Vector2 startPosition;
        private Vector2 endPosition;

        public LinearTranslator(float speed, Vector2 startPosition, Vector2 endPosition) 
        {
            sin = 0;
            this.speed = speed;
            this.startPosition = startPosition;
            this.endPosition = endPosition;
        }

        /// <summary>
        /// Lineary translates performer object from start position to end position.
        /// </summary>
        /// <param name="performer">Performer</param>
        /// <param name="map"></param>
        /// <param name="deltaTime">Time since last translation tick.</param>
        /// <returns></returns>
        public bool TranslationTick(GameObject performer, EditorCanvas map, float deltaTime)
        {
            if (map.GetCellCenterPosition(performer.transform.position) != endPosition)
            {
                sin += deltaTime * ( speed / 15);
                sin = Mathf.Clamp(sin, 0, Mathf.PI);

                var x = 0.5f * Mathf.Sin(sin - Mathf.PI / 2f) + 0.5f;
                if(x >= float.Epsilon)
                {
                    var newPos = Vector2.Lerp(startPosition, endPosition, x);
                    if(newPos != startPosition)
                    { 
                        performer.transform.position = newPos;
                    }
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Baseded on given start action, tryes to find sequence of queued agent action with same action
        /// code -> Same Direction. Founded sequence is than merged with start action.
        /// </summary>
        /// <param name="firstAction">Start action</param>
        /// <param name="actions">Queued actions</param>
        /// <returns></returns>
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
