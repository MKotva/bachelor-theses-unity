using Assets.Scenes.GameEditor.Core.DTOS;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unity.VisualScripting;

namespace Assets.Core.GameEditor.Serializers
{
    public static class JSONSerializer
    {
        public static bool Deserialize(string path, out GameDataDTO result)
        {
            var data = ReadData(path);
            if (data == String.Empty)
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
                    InfoPanelController.Instance.ShowMessage($"JSON parsing error! Description: {ex.Message}");
                }
            }
            result = null;
            return false;
        }

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
                InfoPanelController.Instance.ShowMessage($"Error during saving procces! Description: {ex.Message}");
                return false;
            }
        }

        private static string ReadData(string path)
        {
            if (!File.Exists(path))
            {
                try
                {
                    return File.ReadAllText(path);
                }
                catch (IOException ex)
                {
                    InfoPanelController.Instance.ShowMessage($"File loading error! Description: {ex}");
                }
            }
            InfoPanelController.Instance.ShowMessage($"File loading error! Path does not exists!");
            return String.Empty;
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
