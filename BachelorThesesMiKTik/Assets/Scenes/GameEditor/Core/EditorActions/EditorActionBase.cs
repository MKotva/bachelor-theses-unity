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
    public class EditorActionBase
    {
        public MapCanvasController context;

        public EditorActionBase(MapCanvasController context) 
        {
            this.context = context;
        }

        public virtual void OnMouseDown(MouseButton key) {}

        public virtual void OnMouseUp() { }

        public virtual void OnUpdate(Vector3 mousePosition) { }

        public virtual void OnKeyDown(Key key) { }
        public virtual void OnKeyUp() { }
    }
}
