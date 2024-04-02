using Assets.Scripts.GameEditor.Toolkit.MultiselectDropdown;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Toolkit
{
    public class MultiselectDropdownController : MonoBehaviour
    {
        [SerializeField] TMP_Text PreviewButton;
        [SerializeField] GameObject SelectionButtonPreafab;
        [SerializeField] GameObject SelectionView;
        [SerializeField] GameObject ContentView;

        private bool isPreviewActive;
        private List<SelectionPanelController> itemOptions = new ();

        public void SetSelected(List<string> options)
        {
            if (options.Count == 0)
                return;

            foreach (var item in itemOptions) 
            {
                item.SetState(false);
                foreach (var option in options)
                {
                    if(item.Name == option)
                        item.SetState(true);
                }
            }
        }

        public void SetOptions(List<string> options)
        {
            Clear();

            foreach (var option in options)
            {
                var controller = Instantiate(SelectionButtonPreafab, ContentView.transform)
                    .GetComponent<SelectionPanelController>();
                controller.Set(option);
                itemOptions.Add(controller);
            }
        }

        public List<string> Get()
        {
            var selected = new List<string>();
            foreach(var controller in itemOptions)
            {
                if(controller.IsClicked)
                    selected.Add(controller.Name);
            }
            return selected;
        }

        public void ShowPreview()
        {
            if(isPreviewActive)
            {
                SelectionView.SetActive(false);
                isPreviewActive = false;
                SetSelectButtonText();
            }
            else
            {
                SelectionView.SetActive(true);
                isPreviewActive=true;
            }
        }

        private void SetSelectButtonText()
        {
            var selected = Get();
            if (selected.Count == 0)
                PreviewButton.text = "Items";
            else
                PreviewButton.text = string.Join(", ", selected);
        }

        private void Clear()
        {
            if(itemOptions.Count > 0)
            {
                foreach (var controller in itemOptions)
                {
                    Destroy(controller.gameObject);
                }
                itemOptions.Clear();
            }
        }
    }
}
