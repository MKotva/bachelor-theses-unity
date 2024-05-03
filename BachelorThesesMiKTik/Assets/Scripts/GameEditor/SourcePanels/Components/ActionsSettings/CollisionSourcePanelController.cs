using Assets.Core.GameEditor.DTOS.SourcePanels;
using Assets.Core.SimpleCompiler;
using Assets.Scripts.GameEditor.CodeEditor;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.Toolkit;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings
{
    public class CollisionSourcePanelController : MonoBehaviour
    {
        [SerializeField] private MultiselectDropdownController MultiselectController;
        [SerializeField] private GameObject CodeEditor;

        private List<string> Groups;
        private GameObject ParentCanvas;
        private CodeEditorPopupController codeController;
        private SimpleCode handler;
        private void Awake()
        {
            Groups = ItemManager.Instance.Groups.Keys.ToList();
            var itemSelection = ItemManager.Instance.ItemsNameIdPair.Keys.ToList();
            itemSelection.AddRange(Groups);

            MultiselectController.SetOptions(itemSelection);
            ParentCanvas = GameManager.Instance.PopUpCanvas.gameObject;
        }

        /// <summary>
        /// Returs data from panel in COllisionDTO.
        /// </summary>
        /// <returns>If error, returns null</returns>
        public CollisionDTO Get()
        {
            var selected = MultiselectController.Get();
            if (handler == null || selected.Count == 0)
            {
                //ErrorOutputManager.Instance.ShowMessage($"There is no assigned code (action) for collision with {name}");
                return null;
            }

            var selectedItems = new List<string>();
            var selectedGroups = new List<string>();
            foreach(var item in selected)
            {
                if(Groups.Contains(item))
                    selectedGroups.Add(item);
                else
                    selectedItems.Add(item);
            }

            return new CollisionDTO(selectedItems, selectedGroups, handler);
        }

        /// <summary>
        /// Sets collision panel with data from CollisionDTO.
        /// </summary>
        /// <param name="data"></param>
        public void Set(CollisionDTO data) 
        {
            var selected = new List<string>(data.ObjectsNames);
            selected.AddRange(data.GroupsNames);
            MultiselectController.SetSelected(selected);
            handler = data.Handler;
        }

        /// <summary>
        /// Handles Edit Code click by displaying Code Editor panel. Also connects exit hadler,
        /// which returns compiled code.
        /// </summary>
        public void ShowEditor()
        {
            codeController = Instantiate(CodeEditor, ParentCanvas.transform).GetComponent<CodeEditorPopupController>(); //TODO: Check the transform.
            if(handler != null) 
            {
                codeController.Initialize(handler);
            }
            codeController.onExit += OnEditorExit;
        }

        public void OnEditorExit()
        {
            if(codeController.CompilationCode != null) 
            {
                handler = codeController.CompilationCode;
            }
        }
    }
}
