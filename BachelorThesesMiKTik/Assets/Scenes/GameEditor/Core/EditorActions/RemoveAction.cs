using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.EditorActions
{
    internal class RemoveAction : EditorActionBase
    {
        bool _isMouseDown;

        public RemoveAction(GridController context) : base(context) { }
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
            if (_isMouseDown)
            {
                Remove(mousePosition);
            }
        }


        private void Remove(Vector3 position)
        {
            var worldCellPosition = context.GetCellCenterPosition(position);
            if (context.Selected.ContainsKey(worldCellPosition))
            {
                RemoveSelection();
            }

            GameObject objectAtPos = context.GetObjectAtPosition(worldCellPosition);
            if (objectAtPos)
            {
                context.Erase(objectAtPos, worldCellPosition);
            }
        }

        private void RemoveSelection()
        {
            foreach(var position in context.Selected.Keys) 
            {
                context.Erase(context.Selected[position], position);
                context.Selected[position] = context.CreateMarkAtPosition(position);
            }

            _isMouseDown = false;
        }
    }
}