using Assets.Core.GameEditor;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.Colliders
{
    public class PositionSoucePanelController : MonoBehaviour
    {
        [SerializeField] TMP_InputField SizeX;
        [SerializeField] TMP_InputField SizeY;

        public delegate void EditHandler();
        public event EditHandler OnEdit;

        public Vector2 GetPoint()
        {
            return new Vector2(MathHelper.GetFloat(SizeX.text, 0f), MathHelper.GetFloat(SizeY.text, 0f));
        }

        public void SetPoint(Vector2 point) 
        {
            SizeX.text = point.x.ToString();
            SizeY.text = point.y.ToString();
        }

        private void Awake()
        {
            SizeX.onEndEdit.AddListener(InvokeOnEdit);
            SizeY.onEndEdit.AddListener(InvokeOnEdit);
        }

        private void InvokeOnEdit(string change)
        {
            OnEdit?.Invoke();
        }
    }
}
