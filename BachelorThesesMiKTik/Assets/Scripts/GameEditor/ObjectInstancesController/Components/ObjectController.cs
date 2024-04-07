using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.ObjectInstancesController
{
    public class ObjectController : MonoBehaviour, IObjectController
    {
        public string Name { get; private set; }
        public Dictionary<System.Type, IObjectController> Components { get; private set; }

        private Vector3 positon;

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
            positon = gameObject.transform.position;
            foreach(var component in Components.Values)
            {
                component.Enter();
            }
        }

        public void Exit()
        {
            gameObject.transform.position = positon;
            foreach(var component in Components.Values)
            {
                component.Exit();
            }
        }


        private void Awake()
        {
            Components = new Dictionary<System.Type, IObjectController>();
            EditorController.Instance.AddActiveObject(gameObject.GetInstanceID(), this);
        }

        private void OnDestroy()
        {
            var editorInstance = EditorController.Instance;
            if(editorInstance != null)
                editorInstance.RemoveActiveObject(gameObject.GetInstanceID());
        }
    }
}
