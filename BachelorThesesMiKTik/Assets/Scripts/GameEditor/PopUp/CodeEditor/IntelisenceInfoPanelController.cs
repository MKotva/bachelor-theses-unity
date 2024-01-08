using Assets.Core.GameEditor.Attributes;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp.CodeEditor
{
    public class IntelisenceInfoPanelController : MonoBehaviour
    {
        [SerializeField] TMP_Text Description;
        [SerializeField] TMP_Text Params;


        public void Set(CodeEditorAttribute codeEditorAttribute)
        {
            Description.text = codeEditorAttribute.Description;
            Params.text = codeEditorAttribute.Arguments;
        }

        public void OnExitClick()
        {
            Destroy(gameObject);
        }
    }
}
