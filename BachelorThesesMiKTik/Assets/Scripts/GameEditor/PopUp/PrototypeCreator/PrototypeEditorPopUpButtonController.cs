using Assets.Scripts.GameEditor.Managers;
using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp
{
    public class PrototypeEditorPopUpButtonController : MonoBehaviour
    {
        [SerializeField] Canvas PopUpCanvas;
        [SerializeField] GameObject ItemEditorPrefab;

        public void OnCreate()
        {
            Instantiate(ItemEditorPrefab, PopUpCanvas.transform);
        }

        public void OnEdit()
        {
            var actual = PrototypeManager.Instance.ActualSelectedItem;
            if (PrototypeManager.Instance.CheckIfItemIsDefault(actual.ShownName))
            {
                OutputManager.Instance.ShowMessage($"You can not edit item {actual.ShownName} because it is default item.");
                return;
            }

            var itemEditor = Instantiate(ItemEditorPrefab, PopUpCanvas.transform).GetComponent<PrototypeCreatorController>();
            itemEditor.EditObject(actual);
        }

        public void OnDelete()
        {
            var instace = PrototypeManager.Instance;
            if (instace != null) 
            {
                instace.RemoveItem(instace.ActualSelectedItem);
            }
        }
    }
}
