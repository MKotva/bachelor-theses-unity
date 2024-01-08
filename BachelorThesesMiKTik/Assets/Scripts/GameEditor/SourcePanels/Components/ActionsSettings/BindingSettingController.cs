using Assets.Core.GameEditor.DTOS.SourcePanels;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings
{
    public class BindingSettingController : MonoBehaviour
    {
        [SerializeField] GameObject SourcePanel;
        [SerializeField] GameObject Parent;

        private Stack<BindingSourcePanelController> instances;
        private List<string> actualActions;

        private void Awake()
        {
            instances = new Stack<BindingSourcePanelController>();
            actualActions = new List<string> { "None", "Create code" };
        }

        public void OnAdd()
        {
            instances.Push(CreatePanel());
        }

        public void OnRemove() 
        {
            var instance = instances.Pop();
            Destroy(instance.gameObject);
        }

        public List<ActionBindDTO> GetBindings()
        {
            var bindings = new List<ActionBindDTO>();
            foreach (var panel in instances) 
            {
                if (panel.TryGet(out var actionBind))
                {
                    bindings.Add(actionBind);
                }
            }
            return bindings;
        }

        public void SetBindings(List<ActionBindDTO> bindings)
        {
            foreach(var binding in bindings)
            {
                var panel = CreatePanel();
                panel.Set(binding);
                instances.Push(panel);
            }
        }

        /// <summary>
        /// Sets all avalible actions to binding panels.
        /// </summary>
        /// <param name="actions"></param>
        public void SetActions(List<string> actions)
        {
            var defaultactions = new List<string> {"None", "Create code"};
            defaultactions.AddRange(actions);
            foreach (var instance in instances) 
            {
                instance.SetActions(defaultactions);
            }
            actualActions = defaultactions;
        }

        /// <summary>
        /// If some Binding source panel try to change his key bind, this handler is called.
        /// Checks if any other panel does not have same keys binded. If yes returns false, else true.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private bool OnBindingChange(List<KeyCode> bindings, int ID)
        {
            foreach(var instance in instances) 
            {
                if (instance.ID == ID || instance.BindingKeys.Count != bindings.Count) 
                    continue;                

                bool eq = true;
                for(int i = 0; i < instance.BindingKeys.Count; i++)
                {
                    if (instance.BindingKeys[i] != bindings[i])
                    {
                        eq = false;
                        break;
                    }
                }

                if (eq)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// If some Binding source panel try to change his value, this handler is called.
        /// Checks if any other panel does not have same action binded. If yes returns false, else true.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private bool OnActionChange(string action, int ID)
        {
            foreach (var instance in instances)
            {
                if (instance.ID == ID)
                {
                    continue;
                }
                
                if(instance.Action ==  action)
                    return false;
            }
            return true;
        }

        private BindingSourcePanelController CreatePanel()
        {
            var newRow = Instantiate(SourcePanel, Parent.transform).GetComponent<BindingSourcePanelController>();
            newRow.SetActions(actualActions);
            newRow.ID = instances.Count;
            newRow.BindingSet += OnBindingChange;
            newRow.ActionChange += OnActionChange;
            return newRow;
        }
    }
}
