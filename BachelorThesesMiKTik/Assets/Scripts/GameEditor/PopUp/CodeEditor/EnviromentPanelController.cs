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
        
        private List<EnviromentSourcePanelController> lines = new List<EnviromentSourcePanelController>();

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
                lines.Add(line);
            }
        }

        public void OnAddClick()
        {
            lines.Add(AddLine());
        }

        /// <summary>
        /// Adds new evniroment class panel to view.
        /// </summary>
        /// <returns></returns>
        private EnviromentSourcePanelController AddLine()
        {
            var line = Instantiate(SourceLinePrefab, ContentView.transform);
            line.GetComponent<SourcePanelController>().onDestroyClick += DestroyPanel;
            return line.GetComponent<EnviromentSourcePanelController>();
        }

        /// <summary>
        /// Destroyes enviroment panel based on given instance id.
        /// </summary>
        /// <param name="id"></param>
        private void DestroyPanel(int id)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].gameObject.GetInstanceID() == id)
                {
                    Destroy(lines[i].gameObject);
                    lines.RemoveAt(i);
                }
            }
        }
    }
}
