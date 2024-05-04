using Assets.Core.GameEditor.Attributes;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Scripts.GameEditor.AI;
using Assets.Scripts.GameEditor.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentObjects
{
    public class AIControl : EnviromentObject
    {
        private AIObjectController agent;
        private ItemManager itemManager;
        private EditorCanvas editor;

        public override bool SetInstance(GameObject instance) 
        { 
            if(!instance.TryGetComponent(out agent))
            {
                return false;
            }

            itemManager = ItemManager.Instance;
            if(itemManager == null)
                return false;

            editor = EditorCanvas.Instance;
            if (editor == null)
                return false;

            return true;
        }

        public AIControl()
        {
            itemManager = ItemManager.Instance;
            editor = EditorCanvas.Instance;
        }
        
        [CodeEditorAttribute("Moves actual object to closest object with given name(objectName), if its possible, with use of given actions.", "( string objectName)")]
        public void MoveToClosest(string name)
        {
            var itemInstances = GetElementInstances(name);
            var minPos = GetClosestPos(itemInstances);
            agent.MoveTo(minPos);
        }

        [CodeEditorAttribute("Moves actual object to n-th object with given name(objectName), if its possible, with use of given actions.", "( string objectName)")]
        public void MoveToNth(string name, int n)
        {
            var itemInstances = GetElementInstances(name);
            var maxPos = GetNthPos(itemInstances, n);
            agent.MoveTo(maxPos);

        }

        [CodeEditorAttribute("Moves actual object to farest object with given name(objectName), if its possible, with use of given actions.", "( string objectName)")]
        public void MoveToFarest(string name)
        {
            var itemInstances = GetElementInstances(name);
            var maxPos = GetFarestPos(itemInstances);
            agent.MoveTo(maxPos);
        }

        [CodeEditorAttribute("Moves actual object to given position(xPos, yPos), if its possible, with use of given actions.", "(num xPos, num YPos)")]
        public void MoveToPosition(float x, float y)
        {
            agent.MoveTo(new Vector3(x, y)); //TODO: Check if possition is valid! Like not in the wall
        }

        [CodeEditorAttribute("Checks if object with given name(objectName) is in given range(distance)", "(string objectName, num distance)")]
        public bool IsInRange(string name, float distance)
        {
            var agentPos = agent.transform.position;
            if(itemManager.TryFindIdByName(name, out var endpointId))
                return editor.Data[endpointId].Any(x => Vector3.Distance(agentPos, x.Key) < distance);
            return false;
        }

        [CodeEditorAttribute("Moves actual object to closest object with given name(objectName)" +
            " in given range(distance)", "(string objectName, num distance)")]
        public void MoveToClosestInRange(string name, float distance)
        {
            var itemInstances = GetElementInstances(name);
            var inRange = GetElementsInRange(itemInstances, distance);
            var minPos = GetClosestPos(inRange);
            agent.MoveTo(minPos);
        }

        [CodeEditorAttribute("Moves actual object to n-th object with given name(objectName)" +
    " in given range(distance)", "(string objectName, num distance)")]
        public void MoveToNthInRange(string name, int n, float distance)
        {
            var itemInstances = GetElementInstances(name);
            var inRange = GetElementsInRange(itemInstances, distance);
            var nthPos = GetNthPos(inRange, n);
            agent.MoveTo(nthPos);
        }

        [CodeEditorAttribute("Moves actual object to farest object with given name(objectName)" +
    " in given range(distance)", "(string objectName, num distance)")]
        public void MoveToFarestInRange(string name, float distance)
        {
            var itemInstances = GetElementInstances(name);
            var inRange = GetElementsInRange(itemInstances, distance);
            var maxPos = GetFarestPos(inRange);
            agent.MoveTo(maxPos);
        }

        [CodeEditorAttribute("Based on motion actions on this object, performs random action.")]
        public void PerformRandomAction()
        {
            agent.PerformRandomAction();
        }

        [CodeEditorAttribute("Returns number of queued actions.")]
        public float ActualQueuedActionsCount()
        {
            return agent.ActionsToPerform.Count();
        }

        #region PRIVATE
        private Vector3 GetClosestPos(Dictionary<Vector3, GameObject> data)
        {
            var agentPos = agent.transform.position;
            //Due to lazy evaluation, the orderby will be O(n)
            return data.OrderByDescending(x => Vector3.Distance(agentPos, x.Key)).First().Key; //TODO: Check if possition is valid! Like not in the wall
        }

        private Vector3 GetNthPos(Dictionary<Vector3, GameObject> data, int n)
        {
            var agentPos = agent.transform.position;
            //Due to lazy evaluation, the orderby will be O(n)
            var maxPos = data.OrderBy(x => Vector3.Distance(agentPos, x.Key)); //TODO: Check if possition is valid! Like not in the wall

            var count = maxPos.Count();
            if (count < n)
                throw new RuntimeException($"There is not enough elements to find nth element. Count {count}");

            return maxPos.ElementAt(n).Key;
        }

        private Vector3 GetFarestPos(Dictionary<Vector3, GameObject> data)
        {
            var agentPos = agent.transform.position;
            //Due to lazy evaluation, the orderby will be O(n)
            return data.OrderByDescending(x => Vector3.Distance(agentPos, x.Key)).First().Key; //TODO: Check if possition is valid! Like not in the wall
        }

        private Dictionary<Vector3, GameObject> GetElementInstances(string name)
        {
            if(itemManager.TryFindIdByName(name, out var endpointId))
                if (editor.Data.ContainsKey(endpointId))
                {
                    return editor.Data[endpointId];
                }
            throw new RuntimeException($"There is no element with name {name}!");
        }

        private Dictionary<Vector3, GameObject> GetElementsInRange(Dictionary<Vector3, GameObject> instances, float distance)
        {
            var agentPos = agent.transform.position;
            return (Dictionary<Vector3, GameObject>)instances.Where(x => Vector3.Distance(agentPos, x.Key) < distance);
        }
        #endregion
    }
}
