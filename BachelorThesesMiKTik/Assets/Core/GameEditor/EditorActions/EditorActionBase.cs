using Assets.Core.GameEditor.DTOS;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.EditorActions
{
    public class EditorActionBase
    {
        public MapCanvas map;
        internal JournalActionDTO lastActionRecord;
        internal JournalActionDTO lastActionRecordReverse;

        public EditorActionBase() 
        {
            map = MapCanvas.Instance;
        }

        public virtual void OnMouseDown(MouseButton key) {}
        public virtual void OnMouseUp() {}
        public virtual void OnUpdate(Vector3 mousePosition) {}
        public virtual void OnKeyDown(Key key) {}
        public virtual void OnKeyUp() {}
        public virtual void PerformAction(string action) {}

        public virtual JournalActionDTO GetLastActionRecord() 
        {
            return lastActionRecord;
        }

        public virtual JournalActionDTO GetLastActionRecordReverse()
        {
            return lastActionRecordReverse;
        }
    }
}
