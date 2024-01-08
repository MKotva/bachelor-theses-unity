using Assets.Core.GameEditor.DTOS.SourcePanels;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Entiti
{
    public class CollisionController : MonoBehaviour
    {
        public string Name { get; private set; }
        private List<CollisionDTO> handlers;

        public void Set(string name, List<CollisionDTO> collisions)
        {
            Name = name;
            handlers = collisions;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<CollisionController>(out var controller))
            {
                foreach (var handler in handlers)
                {
                    if(handler.ObjectName == controller.Name) 
                    {
                        handler.Handler.Execute(gameObject);
                    }
                }
            }
        }
    }
}
