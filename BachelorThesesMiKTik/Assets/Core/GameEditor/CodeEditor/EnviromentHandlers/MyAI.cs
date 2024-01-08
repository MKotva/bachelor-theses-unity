using Assets.Core.GameEditor.Attributes;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Scripts.GameEditor.AI;
using Assets.Scripts.GameEditor.ItemView;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentObjects
{
    public class MyAI : EnviromentObject
    {
        public AIAgent Agent { get; set; }
        private GameItemController itemController;
        private Editor editor;

        public override void SetInstance(GameObject instance) { }

        public MyAI()
        {
            itemController = GameItemController.Instance;
            editor = Editor.Instance;
        }
        
        [CodeEditorAttribute("Moves actual object to closest object with given name(objectName), if its possible, with use of given actions.", "( string objectName)")]
        public void MoveToClosest(string name)
        {
            var itemInstances = GetElementInstances(name);
            var minPos = GetClosestPos(itemInstances);
            Agent.MoveTo(minPos);
        }

        [CodeEditorAttribute("Moves actual object to n-th object with given name(objectName), if its possible, with use of given actions.", "( string objectName)")]
        public void MoveToNth(string name, int n)
        {
            var itemInstances = GetElementInstances(name);
            var maxPos = GetNthPos(itemInstances, n);
            Agent.MoveTo(maxPos);

        }

        [CodeEditorAttribute("Moves actual object to farest object with given name(objectName), if its possible, with use of given actions.", "( string objectName)")]
        public void MoveToFarest(string name)
        {
            var itemInstances = GetElementInstances(name);
            var maxPos = GetFarestPos(itemInstances);
            Agent.MoveTo(maxPos);
        }

        [CodeEditorAttribute("Moves actual object to given position(xPos, yPos), if its possible, with use of given actions.", "(num xPos, num YPos)")]
        public void MoveToPosition(float x, float y)
        {
            Agent.MoveTo(new Vector3(x, y)); //TODO: Check if possition is valid! Like not in the wall
        }

        [CodeEditorAttribute("Checks if object with given name(objectName) is in given range(distance)", "(string objectName, num distance)")]
        public bool IsInRange(string name, float distance)
        {
            var agentPos = Agent.transform.position;
            if(itemController.TryFindIdByName(name, out var endpointId))
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
            Agent.MoveTo(minPos);
        }

        [CodeEditorAttribute("Moves actual object to n-th object with given name(objectName)" +
    " in given range(distance)", "(string objectName, num distance)")]
        public void MoveToNthInRange(string name, int n, float distance)
        {
            var itemInstances = GetElementInstances(name);
            var inRange = GetElementsInRange(itemInstances, distance);
            var nthPos = GetNthPos(inRange, n);
            Agent.MoveTo(nthPos);
        }

        [CodeEditorAttribute("Moves actual object to farest object with given name(objectName)" +
    " in given range(distance)", "(string objectName, num distance)")]
        public void MoveToFarestInRange(string name, float distance)
        {
            var itemInstances = GetElementInstances(name);
            var inRange = GetElementsInRange(itemInstances, distance);
            var maxPos = GetFarestPos(inRange);
            Agent.MoveTo(maxPos);
        }

        public void UseWeapon(string name, string target)
        {

        }

        #region PRIVATE
        private Vector3 GetClosestPos(Dictionary<Vector3, GameObject> data)
        {
            var agentPos = Agent.transform.position;
            //Due to lazy evaluation, the orderby will be O(n)
            return data.OrderByDescending(x => Vector3.Distance(agentPos, x.Key)).First().Key; //TODO: Check if possition is valid! Like not in the wall
        }

        private Vector3 GetNthPos(Dictionary<Vector3, GameObject> data, int n)
        {
            var agentPos = Agent.transform.position;
            //Due to lazy evaluation, the orderby will be O(n)
            var maxPos = data.OrderBy(x => Vector3.Distance(agentPos, x.Key)); //TODO: Check if possition is valid! Like not in the wall

            var count = maxPos.Count();
            if (count < n)
                throw new RuntimeException($"There is not enough elements to find nth element. Count {count}");

            return maxPos.ElementAt(n).Key;
        }

        private Vector3 GetFarestPos(Dictionary<Vector3, GameObject> data)
        {
            var agentPos = Agent.transform.position;
            //Due to lazy evaluation, the orderby will be O(n)
            return data.OrderByDescending(x => Vector3.Distance(agentPos, x.Key)).First().Key; //TODO: Check if possition is valid! Like not in the wall
        }

        private Dictionary<Vector3, GameObject> GetElementInstances(string name)
        {
            if(itemController.TryFindIdByName(name, out var endpointId))
                if (editor.Data.ContainsKey(endpointId))
                {
                    return editor.Data[endpointId];
                }
            throw new RuntimeException($"There is no element with name {name}!");
        }

        private Dictionary<Vector3, GameObject> GetElementsInRange(Dictionary<Vector3, GameObject> instances, float distance)
        {
            var agentPos = Agent.transform.position;
            return (Dictionary<Vector3, GameObject>)instances.Where(x => Vector3.Distance(agentPos, x.Key) < distance);
        }
        #endregion
    }
}
