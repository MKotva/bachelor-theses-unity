using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Toolkit.MultiselectDropdown
{
    public class SelectionPanelController : MonoBehaviour
    {
        [SerializeField] TMP_Text ButtonHeader;
        [SerializeField] TMP_Text Toggle;
        public string Name { get; private set; }
        public bool IsClicked { get; private set; }

        public void Set(string name, bool deafultState = false)
        {
            Name = name;
            ButtonHeader.text = name;

            if (deafultState != IsClicked)
            {
                SwitchState();
            }
        }

        public void SwitchState()
        {
            SetState(!IsClicked);
        }

        public void SetState(bool state)
        {
            if (state)
            {
                IsClicked = true;
                Toggle.text = "X";
            }
            else
            {
                IsClicked = false;
                Toggle.text = "";
            }
        }
    }
}
