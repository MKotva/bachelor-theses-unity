using Assets.Core.GameEditor.DTOS;
using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;
using Assets.Scripts.GameEditor.SourcePanels;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp.CodeEditor
{
    class GlobalVariableController : MonoBehaviour
    {
        [SerializeField] GameObject SourceLinePrefab;
        [SerializeField] GameObject ContentView;

        private Stack<GlobalVariableSourcePanelController> lines = new Stack<GlobalVariableSourcePanelController>();


        /// <summary>
        /// Checks all line instances and gets the data from them.
        /// </summary>
        /// <returns></returns>
        public List<GlobalVariableDTO> Get()
        {
            var variables = new List<GlobalVariableDTO>();
            foreach (var instance in lines)
            {
                var ob = instance.Get();
                variables.Add(ob);
            }
            return variables;
        }

        /// <summary>
        /// Sets new lines based on given global variable objects objects.
        /// </summary>
        /// <returns></returns>
        public void Set(List<GlobalVariableDTO> globalVariables)
        {
            foreach (var env in globalVariables)
            {
                var line = AddLine();
                line.Set(env);
                lines.Push(line);
            }
        }

        public void OnAddClick()
        {
            lines.Push(AddLine());
        }

        public void OnRemoveClick()
        {
            if (lines.Count > 0)
            {
                var line = lines.Pop();
                Destroy(line.gameObject);
            }
        }

        private GlobalVariableSourcePanelController AddLine()
        {
            return Instantiate(SourceLinePrefab, ContentView.transform).GetComponent<GlobalVariableSourcePanelController>();
        }
    }
}
