using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Scripts.GameEditor.Audio;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.OutputControllers;
using Assets.Scripts.GameEditor.SourcePanels;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp.Managers
{
    public class AudioManagerController : MonoBehaviour
    {
        [SerializeField] GameObject AnimationsView;
        [SerializeField] GameObject NameButton;
        [SerializeField] GameObject AudioCreatorPrefab;
        [SerializeField] GameObject AudioEditPrefab;
        [SerializeField] OutputController OutputConsole;

        private List<NamePanelController> lines;
        private List<string> selected;

        public void OnPlay()
        {
            if (!AudioManager.Instance.OnPlay(GetSelectedNames()))
            {
                OutputConsole.ShowMessage("No object with this clip present in the scene!");
            }
            else
            {
                OutputConsole.DisposeMessage();
            }
        }

        public void OnPause()
        {
            if (!AudioManager.Instance.OnPause(GetSelectedNames()))
            {
                OutputConsole.ShowMessage("No object with this clip present in the scene!");
            }
            else
            {
                OutputConsole.DisposeMessage();
            }
        }

        public void OnResume()
        {
            if (!AudioManager.Instance.OnResume(GetSelectedNames()))
            {
                OutputConsole.ShowMessage("No object with this clip present in the scene!");
            }
            else
            {
                OutputConsole.DisposeMessage();
            }
        }

        public void OnRestart()
        {
            if (!AudioManager.Instance.OnRestart(GetSelectedNames()))
            {
                OutputConsole.ShowMessage("No object with this clip present in the scene!");
            }
            else
            {
                OutputConsole.DisposeMessage();
            }
        }

        public void OnCreate()
        {
            var controller = Instantiate(AudioCreatorPrefab, gameObject.transform).GetComponent<AudioLoaderController>();
            controller.onExit += LoadTable;
        }

        public void OnEdit()
        {
            selected = GetSelectedNames();
            if (selected.Count == 1 && AudioManager.Instance.ContainsName(selected[0]))
            {
                var controller = Instantiate(AudioEditPrefab, gameObject.transform).GetComponent<AudioEditorController>();
                controller.Initialize(AudioManager.Instance.AudioData[selected[0]]);
                controller.OnSave += Edit;
            }
            else if (selected.Count != 1)
            {
                var controller = Instantiate(AudioEditPrefab, gameObject.transform).GetComponent<AudioEditorController>();
                controller.OnSave += EditAll;
            }
            else
            {
                OutputConsole.ShowMessage("Invalid clip name!");
                return;
            }
            OutputConsole.DisposeMessage();
        }

        public void OnDelete()
        {
            var names = GetSelectedNames();
            foreach (var name in names)
            {
                var instance = AudioManager.Instance;
                if(instance != null) 
                {
                    instance.RemoveClip(name);
                }
            }
            LoadTable();
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

            foreach (var name in AudioManager.Instance.AudioData.Keys)
            {
                var buttonPrefab = Instantiate(NameButton, AnimationsView.transform)
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
            foreach(var line in lines)
            {
                if(line.IsSelected)
                {
                    toggled.Add(line.PanelName);
                }
            }
            return toggled;
        }

        private void EditAll(AudioSourceDTO source)
        {
            if (selected.Count == 0)
            {
                foreach (var controllerGroup in AudioManager.Instance.AudioControllers.Values)
                {
                    foreach(var controller in controllerGroup.Values)
                        controller.EditAudio(source);
                }
            }
            else
            { 
                foreach(var name in selected)
                {
                    if (AudioManager.Instance.AudioControllers.ContainsKey(name))
                    {
                        foreach(var controller in AudioManager.Instance.AudioControllers[name].Values)
                            controller.EditAudio(source);
                    }
                }
            }
        }

        private void Edit(AudioSourceDTO source)
        {
            if (AudioManager.Instance.AudioControllers.ContainsKey(selected[0])) 
            {
                foreach(var controller in AudioManager.Instance.AudioControllers[selected[0]].Values)
                    controller.EditAudio(source);
            }
        }
        #endregion
    }
}
