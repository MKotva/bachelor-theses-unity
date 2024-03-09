using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Scripts.GameEditor.Audio;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.SourcePanels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp.Managers
{
    public class AudioManagerController : MonoBehaviour
    {
        [SerializeField] TMP_Text OutputConsole;
        [SerializeField] GameObject AnimationsView;
        [SerializeField] GameObject NameButton;
        [SerializeField] GameObject AudioSettingPrefab;

        private List<NamePanelController> lines;
        private List<string> selected;

        private void Awake()
        {
            lines = new List<NamePanelController>();

            foreach (var name in AudioManager.Instance.AudioControllers.Keys)
            {
                var buttonPrefab = Instantiate(NameButton, AnimationsView.transform)
                                    .GetComponent<NamePanelController>();
                buttonPrefab.PanelName = name;
                lines.Add(buttonPrefab);
            }
        }
        public void OnPlay()
        {
            if (!AudioManager.Instance.OnPlay(GetSelectedNames()))
            {
                OutputConsole.text = "Invalid clip name!";
            }
            OutputConsole.text = "";
        }

        public void OnPause()
        {
            if (!AudioManager.Instance.OnPause(GetSelectedNames()))
            {
                OutputConsole.text = "Invalid clip name!";
            }
            else
            {
                OutputConsole.text = "";
            }
        }

        public void OnResume()
        {
            if (!AudioManager.Instance.OnResume(GetSelectedNames()))
            {
                OutputConsole.text = "Invalid clip name!";
            }
            else
            {
                OutputConsole.text = "";
            }
        }

        public void OnRestart()
        {
            if (!AudioManager.Instance.OnRestart(GetSelectedNames()))
            {
                OutputConsole.text = "Invalid clip name!";
            }
            else
            {
                OutputConsole.text = "";
            }
        }

        public void OnEdit()
        {
            selected = GetSelectedNames();
            if (selected.Count == 1 && AudioManager.Instance.ContainsName(selected[0]))
            {
                var controller = Instantiate(AudioSettingPrefab, gameObject.transform).GetComponent<AudioEditorController>();
                controller.Initialize(AudioManager.Instance.AudioControllers[selected[0]].AudioSourceDTO);
                controller.OnSave += Edit;
            }
            else if (selected.Count != 1)
            {
                var controller = Instantiate(AudioSettingPrefab, gameObject.transform).GetComponent<AudioEditorController>();
                controller.OnSave += EditAll;
            }
            else
            {
                OutputConsole.text = "Invalid name!";
                return;
            }
            OutputConsole.text = "";
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
                foreach (var controller in AudioManager.Instance.AudioControllers.Values)
                {
                    controller.EditAudio(source);
                }
            }
            else
            { 
                foreach(var name in selected)
                {
                    if (AudioManager.Instance.AudioControllers.ContainsKey(name))
                    {
                        var controller = AudioManager.Instance.AudioControllers[name];
                        controller.EditAudio(source);
                    }
                }
            }
        }

        private void Edit(AudioSourceDTO source)
        {
            if (AudioManager.Instance.AudioControllers.ContainsKey(selected[0])) 
            {
                var controller = AudioManager.Instance.AudioControllers[selected[0]];
                controller.EditAudio(source);
            }
        }
    }
}
