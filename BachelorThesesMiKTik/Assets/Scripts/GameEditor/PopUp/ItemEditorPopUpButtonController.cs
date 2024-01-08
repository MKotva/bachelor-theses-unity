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
    }
}
