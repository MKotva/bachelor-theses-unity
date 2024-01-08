using Assets.Core.GameEditor.Attributes;
using Assets.Core.GameEditor.CodeEditor.EnviromentHandlers;
using Assets.Core.GameEditor.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Assets.Core.GameEditor.CodeEditor
{
    public static class Intelisence
    {
        public static List<IntelisenceSuggestionDTO> TryFindIntelisence(string text, int caretPos, List<EnviromentObjectDTO> enviromentObjects)
        {
            var path = ExtractPath(text, caretPos);
            return FindSuggestions(path, enviromentObjects);
        }

        /// <summary>
        /// From given enviroment objects (connected object with the code), the method will try to find
        /// all suggestions.
        /// 
        /// If enviroment objects does not contain object with name from path, no suggestion will be returned.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="enviromentObjects"></param>
        /// <returns></returns>
        private static List<IntelisenceSuggestionDTO> FindSuggestions(string path, List<EnviromentObjectDTO> enviromentObjects)
        {
            var suggestions = new List<IntelisenceSuggestionDTO>();
            if (TryGetPathMembers(path, out var members))
            {
                Type enviromentType = null;
                if (!TryFindEnviromentObject(members[0], enviromentObjects, out enviromentType))
                    return suggestions;

                var acting = FindActingObject(members, enviromentType);
                if (acting != null)
                {
                    suggestions = GetAllSuggestions(acting);
                }
            }
            return suggestions;
        }

        /// <summary>
        /// Finds all suggestions(Props or Methods marked with CodeEditor attribute) on "Acting object".
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static List<IntelisenceSuggestionDTO> GetAllSuggestions(Type type)
        {
            var suggestions = new List<IntelisenceSuggestionDTO>();

            foreach (var property in type.GetProperties())
            {
                var attributes = property.GetCustomAttributes<CodeEditorAttribute>();
                if (attributes.Count() >= 1)
                    suggestions.Add(new IntelisenceSuggestionDTO(property.Name, attributes.First()));
            }
            foreach (var method in type.GetMethods())
            {
                var attributes = method.GetCustomAttributes<CodeEditorAttribute>();
                if (attributes.Count() >= 1)
                    suggestions.Add(new IntelisenceSuggestionDTO(method.Name, attributes.First()));
            }
            return suggestions;
        }

        /// <summary>
        /// Method goes through the splitted path (by SplitPath()) and finds 
        /// the "Acting object" on which type the suggestions will be searched.
        /// 
        /// If any member of the path can not be found, method will rise CompilationEx.
        /// </summary>
        /// <param name="pathMembers"></param>
        /// <returns></returns>
        /// <exception cref="CompilationException"></exception>
        private static Type FindActingObject(string[] pathMembers, Type initialType)
        {
            var actualObject = initialType;
            for (int i = 1; i < pathMembers.Count(); i++)
            {
                if (TryGetProperty(actualObject, pathMembers[1], out var propertyInfo))
                    actualObject = propertyInfo.PropertyType;
                else
                    return null;
            }
            return actualObject;
        }

        /// <summary>
        /// Checks the type and finds property  which has the same name. 
        ///
        /// If no such property are found, compilation error is raised.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="CompilationException"></exception>
        private static bool TryGetProperty(Type type, string name, out PropertyInfo result)
        {
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (property.Name == name)
                {
                    result = property;
                    return true;
                }
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Splits given path (e.g. name.name./ name.property) by "."
        /// If path has less than 1 members returns false
        /// </summary>
        /// <param name="path"></param>
        /// <param name="members">out parameter with path members</param>
        /// <returns></returns>
        private static bool TryGetPathMembers(string path, out string[] members)
        {
            members = path.Split('.');
            if (members.Length < 1)
                return false;
            return true;
        }

        /// <summary>
        /// Finds the path in the text from text component with use of actual caret position. 
        /// The path must be separated by spaces or begin/end of line.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caretPos"></param>
        /// <returns></returns>
        private static string ExtractPath(string text, int caretPos)
        {
            for (int i = caretPos - 1; i >= 0; i--)
            {
                if (char.IsWhiteSpace(text[i]))
                {
                    return text.Substring(i + 1, caretPos - i - 1);
                }
            }
            return text.Substring(0, caretPos);
        }

        /// <summary>
        /// Tryes to find connected enviroment object alias with given name. Then returns its name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enviroments"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool TryFindEnviromentObject(string name, List<EnviromentObjectDTO> enviroments, out Type result)
        {
            foreach(var enviromentObject in enviroments)
            {
                if(enviromentObject.Alias == name && 
                   EnviromentController.TryGetType(enviromentObject.TypeName, out var type))
                {
                    result = type;
                    return true;
                }
            }
            result = null;
            return false;
        }
    }
}
