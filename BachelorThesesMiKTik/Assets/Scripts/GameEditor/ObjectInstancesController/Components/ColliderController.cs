﻿using Assets.Core.GameEditor.Components.Colliders;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Entiti
{
    public class ColliderController : MonoBehaviour, IObjectController
    {
        public string Name;
        private BoxCollider2D boxCollider;
        private ColliderComponent colliderSettings;
        private PolygonCollider2D polygonCollider;

        public void Initialize(string name, ColliderComponent colliderSetting)
        {
            Name = name;
            colliderSettings = colliderSetting;
        }

        public void Play()
        {
            boxCollider.enabled = true;
        }

        public void Pause()
        {
            boxCollider.enabled = false;
        }

        public void Enter() {}

        public void Exit() 
        {
            boxCollider.enabled = false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<ColliderController>(out var controller))
            {
                foreach (var handler in colliderSettings.Colliders)
                {
                    if(handler.ObjectName == controller.Name) 
                    {
                        handler.Handler.Execute(gameObject);
                    }
                }
            }
        }

        private void Awake()
        {
            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(ColliderController), this);
                if (!TryGetComponent(out boxCollider))
                    boxCollider = gameObject.AddComponent<BoxCollider2D>();
                boxCollider.enabled = false;
            }
        }
    }
}
