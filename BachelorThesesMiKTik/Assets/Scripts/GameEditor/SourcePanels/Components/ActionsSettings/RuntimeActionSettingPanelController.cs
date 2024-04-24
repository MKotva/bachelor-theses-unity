using Assets.Core.SimpleCompiler;
using Assets.Scripts.GameEditor.CodeEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings
{
    public class RuntimeActionSettingPanelController : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown ActionSelection;
        [SerializeField] Button EditButton;
        [SerializeField] GameObject EditorPrefab;

        public SimpleCode ActionCode { get; private set; }
        private CodeEditorPopupController controller;
        private GameObject PopUp;

        public void OnEditClick()
        {
            ShowEditor();
            if(ActionCode != null) 
            {
                controller.Initialize(ActionCode);
            }
        }

        public void SetPanel(SimpleCode actionCode)
        {
            ActionCode = actionCode;
        }

        #region PRIVATE

        private void Awake()
        {
            PopUp = GameManager.Instance.PopUpCanvas.gameObject;
            ActionSelection.onValueChanged.AddListener(delegate { OnValueChanged(); });
            EditButton.gameObject.SetActive(false);
        }

        private void OnValueChanged()
        {
            if (ActionSelection.options[ActionSelection.value].text == "Create code")
            {
                EditButton.gameObject.SetActive(true);
                ShowEditor();
            }
            else
            {
                EditButton.gameObject.SetActive(false);
            }
        }

        private void ShowEditor()
        {
            controller = Instantiate(EditorPrefab, PopUp.transform).GetComponent<CodeEditorPopupController>();
            controller.onExit += OnEditorExit;
        }

        private void OnEditorExit()
        {
            ActionCode = controller.CompilationCode;
        }
        #endregion
    }
}
