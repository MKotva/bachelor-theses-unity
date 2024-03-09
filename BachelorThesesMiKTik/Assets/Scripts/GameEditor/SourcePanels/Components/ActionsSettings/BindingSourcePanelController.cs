using Assets.Core.GameEditor.DTOS.SourcePanels;
using Assets.Core.SimpleCompiler;
using Assets.Scripts.GameEditor.CodeEditor;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels
{
    public class BindingSourcePanelController : MonoBehaviour
    {
        [SerializeField] TMP_Text ButtonText;
        [SerializeField] TMP_Dropdown ActionSelection;
        [SerializeField] GameObject CodeEditor;
        [SerializeField] GameObject EditButton;

        public List<KeyCode> BindingKeys { get; set; } = new List<KeyCode>();
        public string Action { get; set; } = "None";
        public int ID { get; set; }


        public delegate bool KeySetHandler(List<KeyCode> binding, int panelId);
        public delegate bool ActionChangeHandler(string action, int panelID);

        public event KeySetHandler BindingSet;
        public event ActionChangeHandler ActionChange;

        private GameObject parent;
        private bool IsClicked;
        private List<KeyCode> binding;
        private CodeEditorPopupController codeEditorPopup;
        private SimpleCode actionCode;

        public void SetActions(List<string> actions)
        {
            ActionSelection.ClearOptions();
            ActionSelection.AddOptions(actions);
        }

        public void OnBindClick()
        {
            ButtonText.text = "Awaiting press";
            binding = new List<KeyCode>();
            IsClicked = true;
        }

        /// <summary>
        /// Sets given binding for action.
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="option"></param>
        public void Set(ActionBindDTO binding) 
        {
            SetBinding(binding.Binding);
            
            if (binding.ActionCode != null)
                actionCode = binding.ActionCode;

            for(int i  = 0; i < ActionSelection.options.Count; i++) 
            {
                if(ActionSelection.options[i].text == binding.ActionType)
                {
                    ActionSelection.value = i;
                }
            }
        }

        /// <summary>
        /// If binding source panel has setted keys and action, returns ActionBindDTO
        /// as out parameter. Else returs false and null in out parameter.
        /// </summary>
        /// <param name="actionBindDTO"></param>
        /// <returns></returns>
        public bool TryGet(out ActionBindDTO actionBindDTO)
        {
            actionBindDTO = null;
            if(BindingKeys.Count != 0 && Action != "None")
            {
                if (Action == "Create code")
                {
                    if (actionCode == null)
                        return false;
                    actionBindDTO = new ActionBindDTO(BindingKeys, Action, actionCode);
                }
                else
                {
                    actionBindDTO = new ActionBindDTO(BindingKeys, Action);
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// Creates editor and initialize it with previous code, if the code was created.
        /// </summary>
        public void CreateEditor()
        {
            codeEditorPopup = Instantiate(CodeEditor, parent.transform).GetComponent<CodeEditorPopupController>();
            codeEditorPopup.onExit += OnCodeEditorExit;
            if (actionCode != null)
                codeEditorPopup.Initialize(actionCode);
        }

        #region PRIVATE
        private void Awake()
        {
            parent = EditorController.Instance.PopUpCanvas.gameObject;
            ActionSelection.onValueChanged.AddListener(delegate { OnActionChange(); });
        }

        private void Update()
        {
            if (IsClicked) 
            {
                GetBinding();
            }
        }

        /// <summary>
        /// This method is called from Update() if assign button is clicked.
        /// Until user press a key, it will idle. While user press a key, it will read all
        /// input keys and bind them together until user release all keys. If the same
        /// combination of keys is already used, user has to change it.
        /// </summary>
        private void GetBinding()
        {
            var bindings = new List<KeyCode>();
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(code))
                {
                    bindings.Add(code);
                }
            }

            if (bindings.Count > 0)
            {
                foreach (KeyCode code in bindings)
                {
                    if (!binding.Contains(code))
                    {
                        binding.Add(code);
                    }
                }
            }
            else
            {
                if (binding.Count > 0)
                {
                    if (!BindingSet.Invoke(binding, ID))
                    {
                        ButtonText.text = "Used keycode!";
                        
                    }
                    else
                    {
                        SetBinding(binding);
                    }
                    IsClicked = false;
                }
            }
        }


        /// <summary>
        /// This method is onActionChange handler of dropdown menu.
        /// If changed value is "Create code", the code editor is Instanciated.
        /// Else if action is already assinged, error will raise.
        /// </summary>
        private void OnActionChange() 
        {
            if (ActionSelection.value == 0)
            {
                Action = "None";
            }
            
            var valueAction = ActionSelection.options[ActionSelection.value].text;
            Action = valueAction;
            if (valueAction == "Create code")
            {
                EditButton.SetActive(true);
                CreateEditor();
            }
            else
            {
                if(EditButton.activeInHierarchy)
                    EditButton.SetActive(false);
                
                if (!ActionChange.Invoke(valueAction, ID))
                {
                    ActionSelection.value = 0;
                    ButtonText.text = "Assingned action!";
                }
            }
        }

        /// <summary>
        /// Merges all keyCode to one string separeted by " + ".
        /// In case of bindings.Cout == 1 returns only the KeyCode.ToString()
        /// </summary>
        /// <param name="bindings"></param>
        private void SetBinding(List<KeyCode> bindings)
        {
            string newText = "";
            if (bindings.Count == 1)
            {
                newText = bindings[0].ToString();
            }
            else
            {
                for(int i = 0; i < bindings.Count - 1; i++)
                {
                    newText += $"{bindings[i]} +";
                }
                newText += $"{bindings[bindings.Count -1]}";
            }
            ButtonText.text = newText;
            BindingKeys = bindings;
        }


        private void OnCodeEditorExit()
        {
            actionCode = codeEditorPopup.CompilationCode;
        }
        #endregion
    }
}
