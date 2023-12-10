using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComponentData", menuName = "Component scriptable")]
public class ComponentData : ScriptableObject
{
    public string Name;
    public GameObject Prefab;

    public static ComponentData CreateInstance(string name, GameObject prefab)
    {
        var data = ScriptableObject.CreateInstance<ComponentData>();

        data.Name = name;
        data.Prefab = prefab;

        return data;
    }
}
