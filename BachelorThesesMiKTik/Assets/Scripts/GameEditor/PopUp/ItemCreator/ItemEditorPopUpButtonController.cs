using Assets.Scripts.GameEditor.Managers;
using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp
{
    public class ItemEditorPopUpButtonController : MonoBehaviour
    {
        [SerializeField] Canvas PopUpCanvas;
        [SerializeField] GameObject ItemEditorPrefab;

        public void OnCreate()
        {
            Instantiate(ItemEditorPrefab, PopUpCanvas.transform);
        }

        public void OnEdit()
        {
            var actual = ItemManager.Instance.ActualSelectedItem;
            if (ItemManager.Instance.CheckIfItemIsDefault(actual.ShownName))
            {
                ErrorOutputManager.Instance.ShowMessage($"You can not edit item {actual.ShownName} because it is default item.");
                return;
            }

            var itemEditor = Instantiate(ItemEditorPrefab, PopUpCanvas.transform).GetComponent<ItemCreatorController>();
            itemEditor.EditObject(actual);
        }

        public void OnDelete()
        {
            var instace = ItemManager.Instance;
            if (instace != null) 
            {
                instace.RemoveItem(instace.ActualSelectedItem);
            }
        }
    }
}
