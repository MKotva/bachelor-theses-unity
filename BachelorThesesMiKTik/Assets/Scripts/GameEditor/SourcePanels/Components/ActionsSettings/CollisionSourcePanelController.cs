using Assets.Core.GameEditor.DTOS.SourcePanels;
using Assets.Core.SimpleCompiler;
using Assets.Scripts.GameEditor.CodeEditor;
using Assets.Scripts.GameEditor.ItemView;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings
{
    public class CollisionSourcePanelController : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown ObjectSelection;
        [SerializeField] private GameObject CodeEditor;


        private SimpleCode handler;
        private void Awake()
        {
            ObjectSelection.ClearOptions();
            ObjectSelection.AddOptions(GameItemController.Instance.ItemsNameIdPair.Keys.ToList());
        }

        public CollisionDTO Get()
        {
            var name = ObjectSelection.options[ObjectSelection.value].text;
            if (handler == null)
            {
                InfoPanelController.Instance.ShowMessage($"There is no assigned code (action) for collision with {name}");
                return new CollisionDTO(name, new SimpleCode("", null, null));
            }
            return new CollisionDTO(name, handler);
        }

        public void Set(CollisionDTO data) 
        {
            SetName(data.ObjectName);
            handler = data.Handler;
        }

        public void ShowEditor()
        {
            var controller = Instantiate(CodeEditor, transform).GetComponent<CodeEditorPopupController>(); //TODO: Check the transform.
            if(handler != null) 
            {
                controller.Initialize(handler);
            }
        }

        private void SetName(string name)
        {
            for(int i = 0; i < ObjectSelection.options.Count; i++) 
            {
                if (ObjectSelection.options[i].text == name)
                {
                    ObjectSelection.value = i;
                }
            }
        }
    }
}
