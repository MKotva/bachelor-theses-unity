using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.OutputControllers;
using Assets.Scripts.GameEditor.PopUp.AssetLoaders;
using Assets.Scripts.GameEditor.SourcePanels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp.Managers
{
    public class AnimationsManagerController : MonoBehaviour
    {
        [SerializeField] GameObject AnimationsView;
        [SerializeField] GameObject NameButton;
        [SerializeField] GameObject AnimationCreator;
        [SerializeField] GameObject AnimationEditor;
        [SerializeField] OutputController OutputConsole;

        private List<NamePanelController> lines;


        public void OnPlay()
        {
            var names = GetSelectedNames();
            if (!AnimationsManager.Instance.OnPlay(names))
            {
                OutputConsole.ShowMessage("No object with this animation present in the scene!");
            }
            else
            {
                OutputConsole.DisposeMessage();
            }
        }

        public void OnPause()
        {
            var names = GetSelectedNames();
            if (!AnimationsManager.Instance.OnPause(names))
            {
                OutputConsole.ShowMessage("No object with this animation present in the scene!");
            }
            else
            {
                OutputConsole.DisposeMessage();
            }
        }

        public void OnResume()
        {
            var names = GetSelectedNames();
            if (!AnimationsManager.Instance.OnResume(names))
            {
                OutputConsole.ShowMessage("No object with this animation present in the scene!");
            }
            else
            {
                OutputConsole.DisposeMessage();
            }
        }

        public void OnRestart()
        {
            var names = GetSelectedNames();
            if (!AnimationsManager.Instance.OnRestart(names))
            {
                OutputConsole.ShowMessage("No object with this animation present in the scene!");
            }
            else
            {
                OutputConsole.DisposeMessage();
            }
        }

        public void OnCreate()
        {
            var controller = Instantiate(AnimationCreator, EditorController.Instance.PopUpCanvas.transform)
                                    .GetComponent<AnimationCreatorPopUpController>();
            controller.onExit += LoadTable;
        }

        public void OnEdit()
        {
            var names = GetSelectedNames();
            if(names.Count != 1)
            {
                OutputConsole.ShowMessage("You can select only one animation for edit!");
            }
            else 
            {
                var instance = AnimationsManager.Instance;
                if (instance == null)
                    return;

                var controller = Instantiate(AnimationEditor, EditorController.Instance.PopUpCanvas.transform)
                                    .GetComponent<AnimationEditorPopUpController>();
                controller.SetData(instance.AnimationData[names[0]]);
            }
        }

        public void OnDelete()
        {
            var names = GetSelectedNames();
            foreach(var name in names) 
            {
                AnimationsManager.Instance.RemoveAnimation(name);
            }
            LoadTable();
        }

        #region PRIVATE
        private void Awake()
        {
            lines = new List<NamePanelController>();
            LoadTable();
        }

        private void ClearTable()
        {
            foreach (var line in lines)
            {
                Destroy(line.gameObject);
            }

            lines.Clear();
        }

        private void LoadTable()
        {
            ClearTable();

            foreach (var name in AnimationsManager.Instance.AnimationData.Keys)
            {
                var buttonPrefab = Instantiate(NameButton, AnimationsView.transform)
                                    .GetComponent<NamePanelController>();
                buttonPrefab.PanelName = name;
                lines.Add(buttonPrefab);
            }
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
