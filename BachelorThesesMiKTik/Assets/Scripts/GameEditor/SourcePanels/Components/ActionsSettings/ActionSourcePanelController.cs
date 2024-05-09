using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS.Action;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components
{
    public abstract class ActionSourcePanelController : MonoBehaviour
    {
        public abstract ActionDTO GetAction();
        public abstract void SetAction(ActionDTO actions);
        public abstract List<string> GetActionTypes();
        public virtual bool TryParse(string text, out float result)
        {
            if(MathHelper.GetFloat(text, out result))
            {
                return true;
            }

            OutputManager.Instance.ShowMessage("Invalid input type in actionsParsing!");
            return false;
        }
    }
}
