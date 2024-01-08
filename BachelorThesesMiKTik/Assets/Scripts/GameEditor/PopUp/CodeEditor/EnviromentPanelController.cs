using Assets.Core.GameEditor.DTOS;
using Assets.Scripts.GameEditor.SourcePanels;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.CodeEditor
{
    public class EnviromentPanelController : MonoBehaviour
    {
        [SerializeField] GameObject SourceLinePrefab;
        [SerializeField] GameObject ContentView;
        
        private Stack<EnviromentSourcePanelController> lines = new Stack<EnviromentSourcePanelController>();

        /// <summary>
        /// Checks all line instances and gets the data from them.
        /// </summary>
        /// <returns></returns>
        public List<EnviromentObjectDTO> Get()
        {
            var enviroment = new List<EnviromentObjectDTO>();
            foreach(var instance in lines) 
            {
                enviroment.Add(instance.Get());
            }
            return enviroment;
        }

        /// <summary>
        /// Sets new lines based on given enviroment objects.
        /// </summary>
        /// <returns></returns>
        public void Set(List<EnviromentObjectDTO> enviromentObjects)
        {
            foreach (var env in enviromentObjects)
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

        private EnviromentSourcePanelController AddLine()
        {
            return Instantiate(SourceLinePrefab, ContentView.transform).GetComponent<EnviromentSourcePanelController>();
        }
    }
}
