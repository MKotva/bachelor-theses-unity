using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.OutputControllers;
using Assets.Scripts.GameEditor.SourcePanels;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp.Managers
{
    public class SpriteManagerController : MonoBehaviour
    {
        [SerializeField] GameObject SpriteView;
        [SerializeField] GameObject NameButton;
        [SerializeField] GameObject SpriteCreator;
        [SerializeField] GameObject SpriteEditor;
        [SerializeField] OutputController OutputController;

        private List<NamePanelController> lines;
        private List<string> selected;

        public void OnCreateClick()
        {
            var controller = Instantiate(SpriteCreator, EditorController.Instance.PopUpCanvas.transform)
                                    .GetComponent<SpriteLoaderPopUpController>();
            controller.onExit += LoadTable;
        }

        public void OnDeleteClick()
        {
            var names = GetSelectedNames();
            foreach (var name in names)
            {
                SpriteManager.Instance.RemoveSprite(name);
            }
            LoadTable();
        }

        public void OnEditClick()
        {
            var names = GetSelectedNames();
            if (names.Count != 1)
            {
                OutputController.ShowMessage("You can select only one sprite for edit!");
            }
            else
            {
                var instance = SpriteManager.Instance;
                if (instance == null)
                    return;

                var controller = Instantiate(SpriteEditor, EditorController.Instance.PopUpCanvas.transform)
                                    .GetComponent<SpriteEditorPopUpController>();
                controller.SetData(instance.SpriteData[names[0]]);
            }
        }

        #region PRIVATE
        private void Awake()
        {
            lines = new List<NamePanelController>();
            LoadTable();
        }

        private void LoadTable()
        {
            ClearTable();

            foreach (var name in SpriteManager.Instance.SpriteData.Keys)
            {
                var buttonPrefab = Instantiate(NameButton, SpriteView.transform)
                                    .GetComponent<NamePanelController>();
                buttonPrefab.PanelName = name;
                lines.Add(buttonPrefab);
            }
        }

        private void ClearTable()
        {
            foreach (var line in lines)
            {
                Destroy(line.gameObject);
            }

            lines.Clear();
        }

        private List<string> GetSelectedNames()
        {
            var toggled = new List<string>();
            foreach (var line in lines)
            {
                if (line.IsSelected)
                {
                    toggled.Add(line.PanelName);
                }
            }
            return toggled;
        }
        #endregion
    }
}
