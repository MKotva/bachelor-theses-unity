﻿using Assets.Scripts.GameEditor.Audio;
using Assets.Scripts.GameEditor.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.ObjectInstancesController
{
    public class ObjectController : MonoBehaviour, IObjectController
    {
        public string Name { get; private set; }

        public float HP { get; set; }
        public float Score { get; set; }

        public Dictionary<System.Type, IObjectController> Components { get; private set; }

        private Vector3 positon;
        private Quaternion rotation;
        private bool IsDying;
        private string AnimationName = "";
        private string AudioName = "";

        private AnimationsController animationsController;
        private AudioController audioController;

        public void AddComponent(IObjectController component)
        {
            var type = component.GetType();
            if(!Components.ContainsKey(type))
            {
                Components.Add(type, component);
            }
        }

        public void Set(string name)
        {
            Name = name;
        }

        public void Kill(bool shouldFinishAnimation, bool shouldFinishAudio)
        {
            IsDying = true;
            if (shouldFinishAnimation)
            {
                if(gameObject.TryGetComponent(out AnimationsController controller))
                {
                    controller.StopAfterFinishingLoop();
                    animationsController = controller;
                    AnimationName = animationsController.SourceReference.Name;
                }
            }

            if(shouldFinishAudio) 
            {
                if (gameObject.TryGetComponent(out AudioController controller))
                {
                    controller.StopAfterFinishingLoop();
                    audioController = controller;
                    AudioName = audioController.AudioSourceDTO.Name;
                }
            }
        }

        public void Play()
        {
            foreach (var component in Components.Values) 
            {
                component.Play();
            }
        }

        public void Pause()
        {
            foreach(var component in Components.Values)
            {
                component.Pause();
            }
        }

        public void Enter()
        {
            HP = 100f;
            Score = 0;

            positon = gameObject.transform.position;
            rotation = gameObject.transform.rotation;

            animationsController = null;
            audioController = null;
            IsDying = false;

            foreach (var component in Components.Values)
            {
                component.Enter();
            }
        }

        public void Exit()
        {
            foreach (var component in Components.Values)
            {
                component.Exit();
            }

            IsDying = false;
            gameObject.SetActive(true);
            gameObject.transform.position = positon;
            gameObject.transform.rotation = rotation;
        }


        private void Awake()
        {
            Components = new Dictionary<System.Type, IObjectController>();
            
            var gameManager = GameManager.Instance;
            if(gameManager != null )
                gameManager.AddActiveObject(gameObject.GetInstanceID(), this);
        }

        private void FixedUpdate()
        {
            if (IsDying == true)
            {
                if (animationsController != null)
                {
                    if (!animationsController.HasFinished() &&
                        animationsController.SourceReference.Name == AnimationName)
                        return;
                }

                if(audioController != null)
                {
                    if (!audioController.HasFinished() &&
                        audioController.AudioSourceDTO.Name == AudioName)
                        return;
                }


                gameObject.SetActive(false);

                var gameManager = GameManager.Instance;
                if (gameManager != null)
                    gameManager.RemovePlayer(gameObject.GetInstanceID());
            }
        }

        private void OnDestroy()
        {
            var gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                gameManager.RemoveActiveObject(gameObject.GetInstanceID());
            }
        }
    }
}
