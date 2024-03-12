using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.SourcePanels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp.Managers
{
    public class AnimationsManagerController : MonoBehaviour
    {
        [SerializeField] TMP_Text OutputConsole;
        [SerializeField] GameObject AnimationsView;
        [SerializeField] GameObject NameButton;

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
            var names = GetSelectedNames();
            if (!AnimationsManager.Instance.OnPlay(names))
            {
                OutputConsole.text = "Invalid clip name!";
            }
            OutputConsole.text = "";
        }

        public void OnPause()
        {
            var names = GetSelectedNames();
            if (!AnimationsManager.Instance.OnPause(names))
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
            var names = GetSelectedNames();
            if (!AnimationsManager.Instance.OnResume(names))
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
            var names = GetSelectedNames();
            if (!AnimationsManager.Instance.OnRestart(names))
            {
                OutputConsole.text = "Invalid clip name!";
            }
            else
            {
                OutputConsole.text = "";
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
    }
}
