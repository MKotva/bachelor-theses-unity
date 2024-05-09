using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Toolkit.MultiselectDropdown
{
    public class SelectionMenuController : PopUpController
    {
        [SerializeField] GameObject SelectionButtonPreafab;
        [SerializeField] GameObject ContentView;

        public List<SelectionPanelController> itemOptions = new();


        /// <summary>
        /// Sets the set of options in dropdown menu.
        /// </summary>
        /// <param name="options"></param>
        public void SetOptions(Dictionary<string, bool> options)
        {
            foreach (var option in options)
            {
                var controller = Instantiate(SelectionButtonPreafab, ContentView.transform)
                    .GetComponent<SelectionPanelController>();

                controller.Set(option.Key, option.Value);
                itemOptions.Add(controller);
            }
        }

        /// <summary>
        /// Returns selected options in dropown panel.
        /// </summary>
        /// <returns></returns>
        public List<string> Get()
        {
            var selected = new List<string>();
            foreach (var controller in itemOptions)
            {
                if (controller.IsClicked)
                    selected.Add(controller.Name);
            }
            return selected;
        }
    }
}
