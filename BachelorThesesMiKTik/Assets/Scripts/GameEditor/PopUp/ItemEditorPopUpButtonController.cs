using Assets.Scripts.GameEditor.ItemView;
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
            var itemEditor = Instantiate(ItemEditorPrefab, PopUpCanvas.transform).GetComponent<ObjectCreatorController>();
            itemEditor.EditActualObject();
        }

        public void OnDelete()
        {
            var instace = GameItemController.Instance;
            if (instace != null) 
            {
                instace.RemoveItem(instace.ActualSelectedItem);
            }
        }
    }
}
