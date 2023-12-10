using Assets.Core.GameEditor.CodeEditor;
using Assets.Core.GameEditor.Compile;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.ExpressionEvaluator;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.CodeEditor
{
    public class CodeEditorController : MonoBehaviour
    {
        [SerializeField] TMP_InputField Code;
        [SerializeField] uint UpdateRate;

        private uint lastUpdate = 0;
        private Compiler compiler;
        private string text;

        private void Awake()
        {
            compiler = new Compiler();

            text = @" 
num value = 4 / 2
num value2 = 2.178448 + value * 3
bool boolean = true && false
bool boolean1 = 1 > 2 && 1 != 3

if value < value2
    value = -3
    if boolean
        boolean = true
    else
        value = 0
    fi
fi

num i = 2
num result = 1
while i <= 150
    result = result * i
    i = i + 1
end

num value8 = transform.mass.something + 1
transform.mass.something = 1 + 3
num value7 = call ( )

# string str = ""Hello world!""
";

        }

        private void Update()
        {
            if(lastUpdate != UpdateRate)
            {
                lastUpdate++;
                return;
            }

            SyntaxHighlight.ColorKeyWords(Code.textComponent);
            var code = compiler.CompileCode(Code.text.Split(Environment.NewLine));
            code.Execute();
            lastUpdate = 0;
        }
    }
}
