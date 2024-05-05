using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Assets.Core.GameEditor.CodeEditor;
using Assets.Core.SimpleCompiler;
using UnityEngine.EventSystems;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Scripts.GameEditor.PopUp.CodeEditor;
using System.Collections;
using System;

namespace Assets.Scripts.GameEditor.CodeEditor
{
    public class CodeEditorPopupController : PopUpController
    {
        [SerializeField] TMP_InputField Code;
        [SerializeField] TMP_Text OutputConsole;
        [SerializeField] GameObject IntelisencePrefab;
        [SerializeField] EnviromentPanelController enviroment;
        [SerializeField] GlobalVariableController globalVarController;
        [SerializeField] uint UpdateRate;

        public SimpleCode CompilationCode { get; private set; }

        private List<EnviromentObjectDTO> enviromentObjects;
        private List<GlobalVariableDTO> globalVariables;
        private GameObject intelisenceInstance;
        private bool IsInvokedByMethod; //This indicates if OnTextChange event was raised by user or method.
        private bool IsCompilationRunning;

        private uint lastUpdate = 0;
        private SimpleCode lastCompilation;

        public void Initialize(SimpleCode code)
        {
            CompilationCode = code;
            lastCompilation = code;

            Code.text = code.Code;

            LogErrorConsole(code.ErrorOutput);
            LogOutPut(code.Output);

            enviroment.Set(code.EnviromentObjects);
            globalVarController.Set(code.GlobalVariables);
        }

        /// <summary>
        /// Handles Save button click by returning storing last succesfull build to CompilationCode
        /// property, if exists. 
        /// </summary>
        public void OnSaveClick()
        {
            if (IsCompilationRunning)
                return;

            if (lastCompilation == null)
            {
                StartCoroutine(TryCompile((succes, code) => 
                {
                    if(succes)
                    {
                        lastCompilation = code;
                    }

                    LogOutPut("Compilation finished");
                }));
            }

            CompilationCode = lastCompilation;
            LogOutPut("Code saved");
        }

        /// <summary>
        /// Handles Build button click by compiling the code. 
        /// </summary>
        public void OnBuildClick()
        {
            if (IsCompilationRunning)
                return;

            StartCoroutine(TryCompile((succes, code) =>
            {
                if (succes)
                {
                    lastCompilation = code;
                }

                LogOutPut("Compilation finished");
            }));
        }

        /// <summary>
        /// Tryes to compile actual code in editor.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IEnumerator TryCompile(Action<bool, SimpleCode> onComplete)
        {
            IsCompilationRunning = true;

            ClearConsole();
            LogOutPut("Compiling...");

            if (LoadDependencies())
            {
                var compilaton = new SimpleCode(Code.text, enviromentObjects, globalVariables);
                var task = compilaton.CompileAsync();
                yield return new WaitUntil(() => task.IsCompleted);

                if (compilaton.ErrorOutput != "")
                {
                    LogErrorConsole(compilaton.ErrorOutput);
                }
                else
                {
                    LogOutPut(compilaton.Output);
                    LogErrorConsole(compilaton.ErrorOutput);

                    onComplete.Invoke(true, compilaton);
                    IsCompilationRunning = false;
                    yield break;
                }
            }

            onComplete.Invoke(true, null);
            IsCompilationRunning = false;
            yield break;
        }

        #region PRIVATE
        protected override void Awake()
        {
            base.Awake();
            Code.onValueChanged.AddListener(OnTextChanged);
        }

        private void Update()
        {
            if (lastUpdate != UpdateRate)
            {
                lastUpdate++;
                return;
            }

            //TODO: Think about adding live compile

            SyntaxHighlight.ColorKeyWords(Code.textComponent);
            lastUpdate = 0;
        }

        /// <summary>
        /// Loads enviroment objects and global variables setted in editor and check their naming,
        /// for collisions.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CompilationException"></exception>
        private bool LoadDependencies()
        {
            try
            {
                globalVariables = globalVarController.Get();
                enviromentObjects = GetEnviroment();
            }
            catch (CompilerException ex)
            {
                LogErrorConsole($"{ex.Message}\n");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Logs error to console with red coloring.
        /// </summary>
        /// <param name="message"></param>
        private void LogErrorConsole(string message)
        {
            OutputConsole.text += $"<color=\"red\">{message}\n";
        }

        private void LogOutPut(string message)
        {
            OutputConsole.text += $"<color=\"black\">{message}\n";
        }

        private void ClearConsole()
        {
            OutputConsole.text = string.Empty;
        }

        /// <summary>
        /// This is event for code editor text change. This method will look at last added character.
        /// If last character is '.' intelisence is search is invoked.
        /// </summary>
        /// <param name="text"></param>
        private void OnTextChanged(string text)
        {
            if (IsInvokedByMethod)
                return;

            if (text.Length == 0 || Code.caretPosition == 0)
            {
                DestroyIntelisence();
                return;
            }

            var lastCharIndex = Code.caretPosition - 1;
            if (text[lastCharIndex] == '.')
            {
                IntelisenceShow(text, lastCharIndex);
            }
            else
            {
                DestroyIntelisence();
            }
        }

        /// <summary>
        /// Handles click to intelisence window, by adding suggestion after '.' character.
        /// </summary>
        /// <param name="suggestion"></param>
        private void OnIntelisenceClick(string suggestion)
        {
            IsInvokedByMethod = true;
            Code.text = Code.text.Insert(Code.caretPosition, suggestion);
            DestroyIntelisence();

            Code.caretPosition = Code.text.Length;
            EventSystem.current.SetSelectedGameObject(Code.gameObject, null);
            IsInvokedByMethod = false;
        }

        /// <summary>
        /// Disposes intelicense panel.
        /// </summary>
        private void DestroyIntelisence()
        {
            if (intelisenceInstance != null)
            {
                Destroy(intelisenceInstance);
                intelisenceInstance = null;
            }
        }

        /// <summary>
        /// Loads enviroment object for code context.
        /// </summary>
        /// <returns></returns>
        private List<EnviromentObjectDTO> GetEnviroment()
        {
            try
            {
                return enviroment.Get();
            }
            catch (CompilerException ex)
            {
                LogErrorConsole($"{ex.Message}\n");
            }
            return new List<EnviromentObjectDTO>();
        }

        /// <summary>
        /// Shows intelisence window, if suggestion are not empty or if the charInfo of text
        /// is accesible.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        private void IntelisenceShow(string text, int index)
        {
            var suggestions = Intelisence.TryFindIntelisence(text, index, GetEnviroment());
            if (suggestions.Count == 0)
                return;

            if (TryGetCharPosition(index, out var charPos))
            {
                intelisenceInstance = Instantiate(IntelisencePrefab, transform);
                var rect = intelisenceInstance.GetComponent<RectTransform>().rect;
                var position = new Vector3(charPos.x + ( rect.width / 2 ), charPos.y - ( rect.height / 2 ), charPos.z - 1);
                
                var controller = intelisenceInstance.GetComponent<IntelisenceController>();
                controller.ShowIntelisence(suggestions, OnIntelisenceClick, position);
            }
        }

        /// <summary>
        /// If charinfo is accesible, gets the positon where Intelisence should be summoned.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool TryGetCharPosition(int index, out Vector3 position)
        {
            Code.textComponent.ForceMeshUpdate();
            try
            {
                TMP_CharacterInfo charInfo = Code.textComponent.textInfo.characterInfo[index];
                position = Code.textComponent.transform.TransformPoint(charInfo.bottomRight);
            }
            catch
            {
                position = Vector3.zero;
                return false;
            }

            return true;
        }
        #endregion
    }
}
