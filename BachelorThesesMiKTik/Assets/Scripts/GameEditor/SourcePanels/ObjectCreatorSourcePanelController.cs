using Assets.Scripts.GameEditor.SourcePanels.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreatorSourcePanelController : MonoBehaviour
{
    [SerializeField] GameObject prefab;

    List<ICreatorComponent> components;

    public void OnCreateClick()
    {
        var ob = Instantiate(prefab);

        foreach (var component in components)
        { 
            component.SetObject(ob);
        } 
    }
}
