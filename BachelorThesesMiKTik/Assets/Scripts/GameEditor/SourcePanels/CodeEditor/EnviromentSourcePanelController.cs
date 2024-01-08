using Assets.Core.GameEditor.CodeEditor.EnviromentHandlers;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.SimpleCompiler.Exceptions;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels
{
    public class EnviromentSourcePanelController : MonoBehaviour
    {
        [SerializeField] TMP_InputField NameField;
        [SerializeField] TMP_Dropdown TypeDropDown;

        /// <summary>
        /// Sets source panel to reflect given input.
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="obj"></param>
        public void Set(EnviromentObjectDTO obj)
        {
            NameField.text = obj.Alias;
            for(int i = 0; i < TypeDropDown.options.Count; i++)
            {
                var option = TypeDropDown.options[i];
                if(option.text == obj.TypeName) 
                {
                    TypeDropDown.value = i;
                }
            }
        }
        
        /// <summary>
        /// Gets selected enviroment object from dropdown menu and alias(name used in code) from
        /// input text field.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CompilationException"></exception>
        public EnviromentObjectDTO Get()
        {
            if(!CheckNaming(NameField.text))
            {
                throw new CompilationException($"Enviroment exception! Object {TypeDropDown.options[TypeDropDown.value]}" +
                                               $"has invalid name! Allowed name contains at least one character and numbers");
            }

            return new EnviromentObjectDTO(NameField.text, TypeDropDown.options[TypeDropDown.value].text);
        }

        private bool CheckNaming(string input)
        {
            return Regex.IsMatch(input, @"^[0-9]*[a-zA-Z]+[0-9]*$");
        }

        private void Awake()
        {
            var options = EnviromentController.GetControllerNames();
            TypeDropDown.AddOptions(options);
        }
    }
}
