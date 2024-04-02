using Assets.Core.GameEditor.DTOS.SourcePanels;
using Assets.Core.SimpleCompiler;
using Assets.Scripts.GameEditor.CodeEditor;
using Assets.Scripts.GameEditor.ItemView;
using Assets.Scripts.GameEditor.Toolkit;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
            Groups = GameItemController.Instance.GroupViews.Keys.ToList();
            var itemSelection = GameItemController.Instance.ItemsNameIdPair.Keys.ToList();
            itemSelection.AddRange(Groups);

            MultiselectController.SetOptions(itemSelection);
            ParentCanvas = EditorController.Instance.PopUpCanvas.gameObject;
        }

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

        public void Set(CollisionDTO data) 
        {
            var selected = new List<string>(data.ObjectsNames);
            selected.AddRange(data.GroupsNames);
            MultiselectController.SetSelected(selected);
            handler = data.Handler;
        }

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
