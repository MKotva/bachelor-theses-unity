using Assets.Scripts.GameEditor.SourcePanels.Components;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ObjectCreatorController : MonoBehaviour
{
    [SerializeField] private GameObject ParentObject;
    [SerializeField] private TMP_Dropdown DropDown;
    [SerializeField] private ComponentData StaticComponent;
    [SerializeField] private List<ComponentData> Components;
    [SerializeField] private ObjectCreatorSourcePanelController ObjectCreator;

    private Dictionary<string, GameObject> active;

    private void Awake()
    {
        active = new Dictionary<string, GameObject>();
        AddComponent(StaticComponent);
    }

    public void OnAddComponent()
    {
        if (DropDown.value >= Components.Count)
        {
            //TODO: Exception
            return;
        }

        AddComponent(Components[DropDown.value]);
    }
    public void OnCreateClick()
    {
        ObjectCreator.Create(active.Values.ToList());
    }


    public void AddComponent(ComponentData data)
    {
        if (!active.ContainsKey(data.Name))
        {
            var component = Instantiate(data.Prefab, ParentObject.transform);
            component.name = data.Name;
            component.GetComponent<ObjectComponent>().SetExitMethod(DestroyComponent);

            active.Add(component.name, component);
            SetComponentPositions();
        }
    }

    private void SetComponentPositions()
    {
        var compoments = active.Values.ToList();
        var actualHeight = 330f;

        foreach (var compoment in compoments)
        {
            var rect = compoment.GetComponent<RectTransform>();

            var halfHeight = rect.sizeDelta.y / 2;
            actualHeight = actualHeight - halfHeight;
            rect.localPosition = new Vector3(0, actualHeight, 0);
            actualHeight = actualHeight - halfHeight - 5;
        }

    }

    private void DestroyComponent(string componentName)
    {
        Destroy(active[componentName]);
        active.Remove(componentName);
        SetComponentPositions();
    }
}
