using Assets.Core.GameEditor.CodeEditor.EnviromentObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentHandlers
{
    static class EnviromentController
    {
        private static Dictionary<string, Type> types = LoadControllers(typeof(EnviromentObject));

        /// <summary>
        /// Returns all known enviroment objects names.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetControllerNames()
        {
            return types.Keys.ToList();
        }

        /// <summary>
        /// If there is enviroment object with given name, returns instace in out parameter
        /// and true. Else returns null and false.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryGetInstance(string name, out object instance)
        {
            if(types.ContainsKey(name)) 
            {
                instance = types[name].Instantiate();
                return true;
            }
            instance = null;
            return false;
        }

        /// <summary>
        /// If there is enviroment object with given name, returns Type in out parameter
        /// and true. Else returns null and false.
        /// </summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool TryGetType(string name, out Type type) 
        {
            if(types.ContainsKey(name)) 
            {
                type = types[name];
                return true;
            }
            type = null;
            return false;
        }

        //TODO: Check if this reference if ok!
        /// <summary>
        /// The body of this method was found on : https://copyprogramming.com/howto/c-get-all-classes-that-inherit-class
        /// </summary>
        /// <param name="myType"></param>
        /// <returns></returns>
        private static Dictionary<string, Type> LoadControllers(Type myType)
        {
            var types = Assembly.GetAssembly(myType).GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(myType));
            
            var dic = new Dictionary<string, Type>();
            foreach (var type in types) 
            {
                dic.Add(type.Name, type);
            }
            return dic;
        } 
    }
}
