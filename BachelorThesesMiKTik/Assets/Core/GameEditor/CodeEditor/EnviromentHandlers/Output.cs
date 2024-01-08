using Assets.Core.GameEditor.Attributes;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentObjects
{
    public class Output : EnviromentObject
    {
        public override void SetInstance(GameObject instance) { }

        [CodeEditorAttribute("Prints given number(element) to console(InfoPanel)", "(num element)")]
        public void PrintTest(float element)
        {
            InfoPanelController.Instance.ShowMessage(element.ToString(), "Console");
        }

        [CodeEditorAttribute("Prints given bool(element) to console(InfoPanel)", "(bool element)")]
        public void PrintTest(bool element)
        {
            InfoPanelController.Instance.ShowMessage(element.ToString(), "Console");
        }

        [CodeEditorAttribute("Prints given string(element) to console(InfoPanel)", "(string element)")]
        public void PrintTest(string element)
        {
            InfoPanelController.Instance.ShowMessage(element, "Console");
        }

        [CodeEditorAttribute("Prints given number(element) to console(InfoPanel) if given condition is true", "(num element, bool condition)")]
        public void PrintTest(float element, bool condition)
        {
            if (condition)
                InfoPanelController.Instance.ShowMessage(element.ToString(), "Console");
        }

        [CodeEditorAttribute("Prints given bool(element) to console(InfoPanel) if given condition is true", "(bool element, bool condition)")]
        public void PrintTest(bool element, bool condition)
        {
            if (condition)
                InfoPanelController.Instance.ShowMessage(element.ToString(), "Console");
        }

        [CodeEditorAttribute("Prints given string(element) to console(InfoPanel) if given condition is true", "(string element, bool condition)")]
        public void PrintTest(string element, bool condition)
        {
            if (condition)
                InfoPanelController.Instance.ShowMessage(element, "Console");
        }
    }
}
