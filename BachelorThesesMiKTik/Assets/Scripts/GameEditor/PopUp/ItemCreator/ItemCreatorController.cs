using Assets.Scripts.GameEditor.SourcePanels.Components;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ItemCreatorController : PopUpController
{
    [SerializeField] private GameObject ParentObject;
    [SerializeField] private TMP_Dropdown ComponentDropDown;
    [SerializeField] private ComponentData StaticComponent;
    [SerializeField] private List<ComponentData> ComponentsData;
    [SerializeField] private ObjectCreatorSourcePanelController ObjectCreator;

    private Dictionary<string, ObjectComponent> active;
    private bool IsEditing;


    /// <summary>
    /// Button click handler. Adds component by selected name in dropdown.
    /// </summary>
    public void OnAddComponent()
    {
        if (ComponentDropDown.value >= ComponentsData.Count)
        {
            //TODO: Exception
            return;
        }

        AddComponent(ComponentsData[ComponentDropDown.value]);
    }

    /// <summary>
    /// Button click handler. Trigers item creating/editing method with given components.
    /// </summary>
    public void OnSetClick()
    {
        var result = false;
        if (IsEditing)
        {
            result = ObjectCreator.EditItem(active.Values.ToList());
        }
        else
        {
            result = ObjectCreator.CreateItem(active.Values.ToList());
        }

        if (result) 
            OnExitClick();
    }


    /// <summary>
    /// Adds component to editor if editor does no already contains component of same type.
    /// </summary>
    /// <param name="data">Component scriptable object with data.</param>
    public void AddComponent(ComponentData data)
    {
        if (!active.ContainsKey(data.Name) && CheckComponentCollision(data))
        {
            var component = CreateComponentPanel(data);
            active.Add(component.name, component);
        }
    }

    /// <summary>
    /// Sets object editor based on created object.
    /// </summary>
    public void EditObject(ItemData item)
    {
        IsEditing = true;
        foreach(var component in item.Components) 
        {
            if (component.ComponentName == StaticComponent.Name)
            {
                active["General Setting"].SetComponent(component);
                continue;
            }

            foreach(var componentData in ComponentsData) 
            {
                if(component.ComponentName == componentData.Name) 
                {
                    var componentPanel = CreateComponentPanel(componentData);
                    componentPanel.SetComponent(component);
                    active.Add(componentPanel.name, componentPanel);
                }
            }
        }
    }

    #region PRIVATE
    protected override void Awake()
    {
        base.Awake();
        DropDownSet(ComponentsData);
        active = new Dictionary<string, ObjectComponent>();
        AddComponent(StaticComponent);
    }

    /// <summary>
    /// Sets component dropdown. Adds options based on given ComponentData list in init.
    /// </summary>
    /// <param name="data">Component data init list.</param>
    private void DropDownSet(List<ComponentData> data)
    {
        var options = new List<string>();
        foreach (var compoment in data)
        {
            options.Add(compoment.Name);
        }
        ComponentDropDown.ClearOptions();
        ComponentDropDown.AddOptions(options);
    }

    /// <summary>
    /// Callback method for each component at destroy.
    /// </summary>
    /// <param name="componentName">Component name</param>
    private void DestroyComponentPanel(string componentName)
    {
        Destroy(active[componentName].gameObject);
        active.Remove(componentName);
    }

    /// <summary>
    /// Instantiates new component based on given component data.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private ObjectComponent CreateComponentPanel(ComponentData data)
    {
        var component = Instantiate(data.Prefab, ParentObject.transform).GetComponent<ObjectComponent>();
        component.name = data.Name;
        component.SetExitMethod(DestroyComponentPanel);
        return component;
    }

    private bool CheckComponentCollision(ComponentData data)
    {
        if ((data.Name == "AI Control" && active.ContainsKey("Player Control")) ||
            (data.Name == "Player Control") && active.ContainsKey("AI Control"))
        {
                OutputManager.Instance.ShowMessage("You can not add AI and Player components at the same time.");
                return false;
        }
        return true;
    }

    #endregion
}
