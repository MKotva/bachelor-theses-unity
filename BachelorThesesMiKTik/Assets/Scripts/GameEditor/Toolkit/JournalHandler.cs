using Assets.Core.GameEditor.Journal;
using UnityEngine;

public class JournalHandler : MonoBehaviour
{
    private Journal _journal;

    private void Start()
    {
        if(_journal == null)
            _journal = EditorCanvas.Instance.MapJournal;
    }

    public void OnUndoClick()
    {
        var undo = _journal.GetUndoAction();
        if (undo != null) 
        {
            undo.Performer(undo);
        }
    }

    public void OnRedoClick() 
    {
        var redo = _journal.GetRedoAction();
        if (redo != null)
        {
            redo.Performer(redo);
        }
    }
}
