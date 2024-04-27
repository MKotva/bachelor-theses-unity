using Assets.Core.GameEditor.DTOS;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.EditorActions
{
    public class EditorActionBase
    {
        public EditorCanvas map;
        internal JournalActionDTO lastActionRecord; //Record of performed action.
        internal JournalActionDTO lastActionRecordReverse; //Record of reverse action of performed action.

        public EditorActionBase() 
        {
            map = EditorCanvas.Instance;
        }

        public virtual void OnMouseDown(MouseButton key) {}
        public virtual void OnMouseUp() {}
        public virtual void OnUpdate(Vector3 mousePosition) {}
        public virtual void OnKeyDown(Key key) {}
        public virtual void OnKeyUp() {}
        public virtual void PerformAction(string action) {}

 
        internal void SaveRecord(JournalActionDTO lastAction, JournalActionDTO redoAction) 
        { 
            if(map.IsRecording && map.MapJournal != null)
            {
                map.MapJournal.Record(lastAction, redoAction);
            }
        }
    }
}
