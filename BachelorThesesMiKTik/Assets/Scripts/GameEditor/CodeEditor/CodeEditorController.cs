using System;
using TMPro;
using UnityEngine;
using Assets.Core.GameEditor.CodeEditor;

namespace Assets.Scripts.GameEditor.CodeEditor
{
    public class CodeEditorController : MonoBehaviour
    {
        [SerializeField] TMP_InputField Code;
        [SerializeField] uint UpdateRate;

        private uint lastUpdate = 0;


        private void Update()
        {
            if(lastUpdate != UpdateRate)
            {
                lastUpdate++;
                return;
            }

            SyntaxHighlight.ColorKeyWords(Code.textComponent);
            lastUpdate = 0;
        }
    }
}
