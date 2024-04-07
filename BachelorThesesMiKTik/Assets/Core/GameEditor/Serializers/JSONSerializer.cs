using Assets.Scenes.GameEditor.Core.DTOS;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Assets.Core.GameEditor.Serializers
{
    public static class JSONSerializer
    {
        /// <summary>
        /// Loads GameData from file on given path.
        /// </summary>
        /// <param name="path">Path to file with game data.</param>
        /// <param name="result">Out parameter with deserialized game data.</param>
        /// <returns>True if data were succesfully read and stored to GameDataDTO. Otherwise false.</returns>
        public static bool Deserialize(string path, out GameDataDTO result)
        {
            var data = ReadData(path);
            if (data != "")
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                try
                {
                    result = JsonConvert.DeserializeObject<GameDataDTO>(data, settings);
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorOutputManager.Instance.ShowMessage($"JSON parsing error! Description: {ex.Message}");
                }
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Serializes GameDataDTO to json and saves it to file on given path.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <returns>True if data were succesfully serialized to file. Otherwise false.</returns>
        public static bool Serialize(GameDataDTO data, string path)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new IgnorePropertiesResolver("normalized")
            };

            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented, settings);
                File.WriteAllText(path, json);
                return true;
            }
            catch (Exception ex) 
            {
                ErrorOutputManager.Instance.ShowMessage($"Error during saving procces! Description: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reads all data from file on given path if exists.
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>File content in string.</returns>
        private static string ReadData(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    return File.ReadAllText(path);
                }
                catch (IOException ex)
                {
                    ErrorOutputManager.Instance.ShowMessage($"File loading error! Description: {ex}");
                }
            }
            ErrorOutputManager.Instance.ShowMessage($"File loading error! Path does not exists!");
            return "";
        }
    }

    /// <summary>
    /// This code was found on https://code-maze.com/csharp-exclude-properties-from-json-serialization/
    /// TODO: Reference properly!!
    /// </summary>
    public class IgnorePropertiesResolver : DefaultContractResolver
    {
        private readonly HashSet<string> _ignoreProps;
        public IgnorePropertiesResolver(params string[] propNamesToIgnore)
        {
            _ignoreProps = new HashSet<string>(propNamesToIgnore);
        }
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (_ignoreProps.Contains(property.PropertyName))
            {
                property.ShouldSerialize = _ => false;
            }
            return property;
        }
    }
}
