using Assets.Core.GameEditor.DTOS;
using Assets.Core.SimpleCompiler.Enums;
using Assets.Core.SimpleCompiler.Exceptions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels
{
    public class GlobalVariableSourcePanelController : MonoBehaviour
    {
        [SerializeField] TMP_InputField NameField;
        [SerializeField] TMP_InputField ValueField;
        [SerializeField] TMP_Dropdown TypeDropDown;


        private void Awake()
        {
            TypeDropDown.AddOptions(new List<string>
            {
                "String",
                "Bool",
                "Numberic"
            });
        }

        /// <summary>
        /// Sets source panel to reflect given input.
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="obj"></param>
        public void Set(GlobalVariableDTO variable)
        {
            NameField.text = variable.Alias;
            TypeDropDown.value = SetValueType(variable.Type);
            ValueField.text = variable.Value;
        }

        /// <summary>
        /// Gets selected global variableType from dropdown menu, default value and alias(name used in code) from
        /// input text field.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CompilationException"></exception>
        public GlobalVariableDTO Get()
        {
            if (!CheckNaming(NameField.text))
            {
                throw new CompilationException($"Enviroment exception! Object {TypeDropDown.options[TypeDropDown.value]}" +
                                               $"has invalid name! Allowed name contains at least one character and numbers");
            }

            return new GlobalVariableDTO(NameField.text, ValueField.text, GetValueType());
        }

        private bool CheckNaming(string input)
        {
            return Regex.IsMatch(input, @"^[0-9]*[a-zA-Z]+[0-9]*$");
        }

        /// <summary>
        /// Parses value from dropdown to ValueType enum.
        /// </summary>
        /// <returns></returns>
        private ValueType GetValueType()
        {
            switch (TypeDropDown.value)
            {
                case 0: return ValueType.String;
                case 1: return ValueType.Boolean;
                default: return ValueType.Numeric;
            }
        }

        /// <summary>
        /// Parses ValueType to dropdown option value
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private int SetValueType(ValueType type)
        {
            switch(type) 
            {
                case ValueType.String:
                    return 0;
                case ValueType.Boolean: 
                    return 1;
                default: 
                    return 2;
            } 
        }
    }
}
