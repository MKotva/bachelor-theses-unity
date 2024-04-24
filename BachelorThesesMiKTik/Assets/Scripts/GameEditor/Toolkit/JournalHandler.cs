using Assets.Core.GameEditor.Journal;
using Assets.Scenes.GameEditor.Core.EditorActions;
using System.Collections;
using System.Collections.Generic;
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
            undo.Performer(undo.Parameters);
        }
    }

    public void OnRedoClick() 
    {
        var undo = _journal.GetRedoAction();
        if (undo != null)
        {
            undo.Performer(undo.Parameters);
        }
    }
}
