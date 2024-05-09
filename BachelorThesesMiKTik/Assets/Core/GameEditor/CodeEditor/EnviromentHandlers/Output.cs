using Assets.Core.GameEditor.Attributes;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentObjects
{
    public class Output : EnviromentObject
    {
        private OutputManager output;

        public override bool SetInstance(GameObject instance) 
        {
            output = OutputManager.Instance;
            if (output == null)
                return false;
            return true;
        }

        [CodeEditorAttribute("Prints given number(element) to console(InfoPanel)", "(num element)")]
        public void PrintNum(float element)
        {
            OutputManager.Instance.ShowMessage(element.ToString(), "Console", 1000);
        }

        [CodeEditorAttribute("Prints given bool(element) to console(InfoPanel)", "(bool element)")]
        public void PrintBool(bool element)
        {
            OutputManager.Instance.ShowMessage(element.ToString(), "Console", 1000);
        }

        [CodeEditorAttribute("Prints given string(element) to console(InfoPanel)", "(string element)")]
        public void Print(string element)
        { 
            OutputManager.Instance.ShowMessage(element, "Console", 1000);
        }

        [CodeEditorAttribute("Prints given number(element) to console(InfoPanel) if given condition is true", "(num element, bool condition)")]
        public void PrintNum(float element, bool condition)
        {
            if (condition)
                OutputManager.Instance.ShowMessage(element.ToString(), "Console", 1000);
        }

        [CodeEditorAttribute("Prints given bool(element) to console(InfoPanel) if given condition is true", "(bool element, bool condition)")]
        public void PrintBool(bool element, bool condition)
        {
            if (condition)
                OutputManager.Instance.ShowMessage(element.ToString(), "Console", 1000);
        }

        [CodeEditorAttribute("Prints given string(element) to console(InfoPanel) if given condition is true", "(string element, bool condition)")]
        public void Print(string element, bool condition)
        {
            if (condition)
                OutputManager.Instance.ShowMessage(element, "Console", 1000);
        }
    }
}
