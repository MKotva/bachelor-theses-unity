using Assets.Core.GameEditor.Components.Colliders;
using Assets.Core.SimpleCompiler;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Assets.Scripts.GameEditor.Entiti
{
    public class ColliderController : MonoBehaviour, IObjectController
    {
        public string Name { get; private set; }
        public string GroupName { get; private set; }
        public Collider2D ObjectCollider { get; private set; }
        public ColliderComponent ColliderSettings { get; private set; }

        private List<(HashSet<string>, HashSet<string>, SimpleCode)> handlers;

        public void Initialize(string name, string groupName, ColliderComponent colliderSetting)
        {
            Name = name;
            GroupName = groupName;
            ColliderSettings = colliderSetting;

            foreach (var handler in ColliderSettings.Colliders)
            {
                var namesSet = new HashSet<string>();
                var groupSet = new HashSet<string>();

                foreach (var names in handler.ObjectsNames)
                {
                    namesSet.Add(names);
                }
                foreach(var group in handler.GroupsNames)
                {
                    groupSet.Add(group);
                }

                handlers.Add((namesSet, groupSet, handler.Handler));
            }
        }

        #region IObjectMethods
        public void Play()
        {
            ObjectCollider.enabled = true;
        }

        public void Pause()
        {
            ObjectCollider.enabled = false;
        }

        public void Enter() {}

        public void Exit() 
        {
            ObjectCollider.enabled = false;
        }

        #endregion

        public bool ContainsHandler(string name)
        {
            foreach (var handler in handlers)
            {
                if (handler.Item1.Contains(name))
                {
                    return true;
                }
                else if (handler.Item2.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }

        //TODO: Consider construction of data structure for faster name check: MB: List<Dictionary<string, Code>>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<ColliderController>(out var controller))
            {
                foreach (var handler in handlers)
                {
                    if (handler.Item1.Contains(controller.Name))
                    {
                        handler.Item3.Execute(gameObject);
                    }
                    else if (handler.Item2.Contains(controller.Name))
                    {
                        handler.Item3.Execute(gameObject);
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent<ColliderController>(out var controller))
            {
                foreach (var handler in handlers)
                {
                    if (handler.Item1.Contains(controller.Name))
                    {
                        handler.Item3.Execute(gameObject);
                    }
                    else if (handler.Item2.Contains(controller.Name))
                    {
                        handler.Item3.Execute(gameObject);
                    }
                }
            }
        }

        private void Awake()
        {
            ColliderSettings = new ColliderComponent();
            handlers = new List<(HashSet<string>, HashSet<string>, SimpleCode)>();

            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(ColliderController), this);
            }

            Collider2D collider;
            if (!TryGetComponent(out collider))
            {
                collider = gameObject.AddComponent<BoxCollider2D>();
                ObjectCollider.isTrigger = false;
            }

            ObjectCollider = collider;
            ObjectCollider.enabled = false;
        }
    }
}
