using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.ObjectInstancesController
{
    public class ObjectController : MonoBehaviour, IObjectController
    {
        public string Name { get; private set; }
        public Dictionary<System.Type, IObjectController> Components { get; private set; }

        public void AddComponent(IObjectController component)
        {
            var type = component.GetType();
            if(!Components.ContainsKey(type))
            {
                Components.Add(type, component);
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

        private void Awake()
        {
            Components = new Dictionary<System.Type, IObjectController>();
            EditorController.Instance.PlayModeEnter += Play;
            EditorController.Instance.PlayModePause += Pause;
            EditorController.Instance.PlayModeExit += Pause;
        }
    }
}
