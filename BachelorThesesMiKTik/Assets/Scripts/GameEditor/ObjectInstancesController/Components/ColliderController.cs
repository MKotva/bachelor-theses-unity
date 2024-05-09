using Assets.Core.GameEditor.Components.Colliders;
using Assets.Core.SimpleCompiler;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Entiti
{
    public class ColliderController : MonoBehaviour, IObjectController
    {
        [SerializeField] string InitName;
        [SerializeField] string InitGroupName;

        public string Name { get; private set; }
        public string GroupName { get; private set; }
        public Collider2D ObjectCollider { get; private set; }
        public ColliderComponent ColliderSettings { get; private set; }
        public ContactPoint2D[] ContactPoints { get; private set; }


        private List<(HashSet<string>, HashSet<string>, SimpleCode)> handlers;
        private bool IsEnabled = false;

        public void Initialize(string name, string groupName, ColliderComponent colliderSetting)
        {
            Name = name;
            GroupName = groupName;
            ColliderSettings = colliderSetting;

            foreach (var handler in ColliderSettings.Colliders)
            {
                if(handler == null)
                    continue;

                if(handler.Handler == null)
                    continue;

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

                handlers.Add((namesSet, groupSet, new SimpleCode(handler.Handler)));
            }
        }

        #region IObjectMethods
        public void Play()
        {
            IsEnabled = true;
            ObjectCollider.enabled = true;
        }

        public void Pause()
        {
            IsEnabled = false; 
            ObjectCollider.enabled = false;
        }

        public void Enter() 
        {
            IsEnabled = true;
            ObjectCollider.enabled = true;

            foreach (var handler in handlers)
            {
                if(handler.Item3 != null)
                {
                    handler.Item3.ResetContext();
                }
            }
        }

        public void Exit() 
        {
            IsEnabled = false;
            ObjectCollider.enabled = false;
        }

        #endregion

        public bool ContainsContactPoint(Vector2 normal)
        {
            if (ContactPoints == null)
                return false;

            foreach (var point in ContactPoints)
            {
                var pointNormal = point.normal;
                if (Mathf.Abs(normal.x - pointNormal.x) < float.Epsilon &&
                    Mathf.Abs(normal.y - pointNormal.y) < float.Epsilon)
                {
                    return true;
                }
            }

            return false;
        }

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
            if (!IsEnabled)
                return;

            ContactPoints = collision.contacts;
            if (collision.gameObject.TryGetComponent<ColliderController>(out var controller))
            {
                foreach (var handler in handlers)
                {
                    if (handler.Item1.Contains(controller.Name))
                    {
                        handler.Item3.Execute(gameObject);
                    }
                    else if (handler.Item2.Contains(controller.GroupName))
                    {
                        handler.Item3.Execute(gameObject);
                    }
                }
            }

            ContactPoints = null;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsEnabled)
                return;

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
            DefaultPrefabInit();
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
        }

        private void DefaultPrefabInit()
        {
            if(InitName != null && InitGroupName != null) 
            {
                if(InitName != "" && InitGroupName != "")
                {

                    if(Name == null && GroupName == null) 
                    {
                        Name = InitName;
                        GroupName = InitGroupName;
                    }
                    
                    if(Name == "" && GroupName == "")
                    {
                        Name = InitName;
                        GroupName= InitGroupName;
                    }
                }
            }
        }
    }
}
