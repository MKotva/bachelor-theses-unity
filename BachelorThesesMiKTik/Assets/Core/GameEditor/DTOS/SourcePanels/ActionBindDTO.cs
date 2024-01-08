using Assets.Core.SimpleCompiler;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.SourcePanels
{
    [Serializable]
    public class ActionBindDTO
    {
        public List<KeyCode> Binding;
        public string ActionType;
        public SimpleCode ActionCode;

        public ActionBindDTO(List<KeyCode> binding, string action, SimpleCode code = null) 
        {
            Binding = binding;
            ActionType = action;
            ActionCode = code;
        }
    }
}
