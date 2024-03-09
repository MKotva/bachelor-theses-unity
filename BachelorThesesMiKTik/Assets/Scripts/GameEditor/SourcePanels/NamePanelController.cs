using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.SourcePanels
{
    public class NamePanelController : MonoBehaviour
    {
        [SerializeField] Toggle SelectToggle;
        [SerializeField] TMP_Text Name;

        public string PanelName
        {
            get { return Name.text; }
            set { Name.text = value; }
        }

        public bool IsSelected
        {
            get { return SelectToggle.isOn; }
            set { SelectToggle.isOn = value; }
        }
    }
}
