using Assets.Scripts.GameEditor.SourcePanels.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.ObjectInstancesController
{
    public class ObjectController : MonoBehaviour
    {
        public string Name { get; private set; }
        public List<ObjectComponent> Components { get; private set; }
        private bool IsActive;

        public void Set(string name, List<ObjectComponent> components)
        {
            Name = name;
            Components = components;
        }

        public void SetActive(bool isActive)
        {

        }
    }
}
