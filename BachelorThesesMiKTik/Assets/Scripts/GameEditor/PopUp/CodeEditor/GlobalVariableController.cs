using Assets.Core.GameEditor.DTOS;
using Assets.Scripts.GameEditor.SourcePanels;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp.CodeEditor
{
    class GlobalVariableController : MonoBehaviour
    {
        [SerializeField] GameObject SourceLinePrefab;
        [SerializeField] GameObject ContentView;

        private List<GlobalVariableSourcePanelController> lines = new List<GlobalVariableSourcePanelController>();


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
                lines.Add(line);
            }
        }

        public void OnAddClick()
        {
            lines.Add(AddLine());
        }

        private GlobalVariableSourcePanelController AddLine()
        {
            var line = Instantiate(SourceLinePrefab, ContentView.transform);
            line.GetComponent<SourcePanelController>().onDestroyClick += DestroyPanel;
            return line.GetComponent<GlobalVariableSourcePanelController>();

        }

        private void DestroyPanel(int id)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].GetInstanceID() == id)
                {
                    Destroy(lines[i]);
                    lines.RemoveAt(i);
                }
            }
        }
    }
}
