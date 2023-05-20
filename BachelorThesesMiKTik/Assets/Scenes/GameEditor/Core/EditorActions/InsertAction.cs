using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.EditorActions
{
    public class InsertAction : EditorActionBase
    {
        bool _isMouseDown;

        public InsertAction(GridController context) : base(context) {}
        public override void OnMouseDown(MouseButton key) 
        {
            if (key == MouseButton.LeftMouse)
                _isMouseDown = true;
        }

        public override void OnMouseUp() 
        {
            _isMouseDown = false;
        }

        public override void OnUpdate(Vector3 mousePosition) 
        {
            if(_isMouseDown) 
            {
                Insert(mousePosition);
            }
        }

        private void Insert(Vector3 position)
        {
            var worldCellPosition = context.GetCellCenterPosition(position);
            if (context.Selected.ContainsKey(worldCellPosition))
            {
                InsertSquare();
            }

            GameObject objectAtPos = context.GetObjectAtPosition(worldCellPosition);
            if (objectAtPos == null)
            {
                var newObject = context.Paint(context.GridLayout, context.Parent, worldCellPosition);
                context.InsertToData(worldCellPosition, newObject);
            }
        }

        private void InsertSquare()
        {
            foreach(var position in context.Selected.Keys)
            {
                if (PrefabUtility.GetCorrespondingObjectFromSource(context.Selected[position]) == context.MarkerPrefab) //TODO: Check selection prefab.
                {
                    context.Erase(context.Selected[position]);
                    
                    var newObject = context.Paint(context.GridLayout, context.Parent, position);
                    context.MarkObject(newObject);
                    context.Selected[position] = newObject;
                    context.InsertToData(position, newObject);
                }
            }

            _isMouseDown = false; //TODO: Check if this is good idea...
        }
    }
}
