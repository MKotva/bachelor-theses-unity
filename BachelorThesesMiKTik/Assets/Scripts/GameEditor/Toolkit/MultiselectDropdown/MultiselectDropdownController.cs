using Assets.Scripts.GameEditor.Toolkit.MultiselectDropdown;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Toolkit
{
    public class MultiselectDropdownController : MonoBehaviour
    {
        [SerializeField] TMP_Text PreviewButton;
        [SerializeField] GameObject ViewMenuPanel;
        [SerializeField] GameObject ContentView;

        private Dictionary<string, bool> checkedOptions;
        private SelectionMenuController instance;

        /// <summary>
        /// Sets state of dropdown options with given data. Marks them as selected if exists.
        /// </summary>
        /// <param name="options">Selected options</param>
        public void SetSelected(List<string> options)
        {
            if (options.Count == 0)
                return;

            var keys = checkedOptions.Keys.ToList();
            foreach (var item in keys) 
            {
                if(checkedOptions.ContainsKey(item))
                    checkedOptions[item] = false;
            }

            foreach (var option in options)
            {
                if (checkedOptions.ContainsKey(option))
                {
                    checkedOptions[option] = true;
                    PreviewButton.text = string.Join(", ", options);
                }
            }
        }

        /// <summary>
        /// Sets the set of options in dropdown menu.
        /// </summary>
        /// <param name="options"></param>
        public void SetOptions(List<string> options)
        {
            Clear();

            foreach (var option in options)
            {
                checkedOptions.Add(option, false);
            }
        }

        /// <summary>
        /// Returns selected options in dropown panel.
        /// </summary>
        /// <returns></returns>
        public List<string> Get()
        {
            var selected = new List<string>();
            foreach (var key in checkedOptions.Keys)
            {
                if (checkedOptions[key])
                    selected.Add(key);
            }
            return selected;
        }

        /// <summary>
        /// Gets all selected options and displays them in preview panel.
        /// </summary>
        public void ShowPreview()
        {
            if(instance == null)
            {
                var gameInstance = GameManager.Instance;
                if (gameInstance == null)
                    return;

                if (gameInstance.PopUpCanvas == null)
                    return;

                instance = Instantiate(ViewMenuPanel, gameInstance.PopUpCanvas.transform)
                    .GetComponent<SelectionMenuController>();

                instance.SetOptions(checkedOptions);
                instance.onExit += MenuExit;
            }
        }

        private void Awake()
        {
            PreviewButton.text = "Items";

            if(checkedOptions == null)
                checkedOptions = new Dictionary<string, bool>();
        }

        ///// <summary>
        ///// Gets all selected options and displays them in preview panel. If no option
        ///// is selected, then the default text is displayed.
        ///// </summary>
        //private void SetSelectButtonText()
        //{
        //    var selected = Get();
        //    if (selected.Count == 0)
        //        PreviewButton.text = "Items";
        //    else
        //        PreviewButton.text = string.Join(", ", selected);
        //}

        /// <summary>
        /// Clears all options in dropdown.
        /// </summary>
        private void Clear()
        {
            checkedOptions = new Dictionary<string, bool>();
        }

        private void MenuExit()
        {
            SetSelected(instance.Get());
            instance = null;
        }
    }
}
