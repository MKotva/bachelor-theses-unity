using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using Assets.Scripts.GameEditor.SourcePanels.Components;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreatorSourcePanelController : MonoBehaviour
{
    private bool hasPassed;
    public bool CreateItem(List<ObjectComponent> components)
    {
        OutputManager.Instance.AddOnAddListener("ObjectCreate", ErrorHandler, "ObjectCreate");
        var newItem = CreateNewPrefab (components);
        if (hasPassed)
        {
            ItemManager.Instance.AddItem(newItem);
            return true;
        }

        OutputManager.Instance.RemoveListener("ObjectCreate");
        return false;
    }

    public bool EditItem(List<ObjectComponent> components)
    {
        OutputManager.Instance.AddOnAddListener("ObjectCreate", ErrorHandler, "ObjectCreate");
        var newItem = CreateEditingPrefab(components);
        if (hasPassed)
        {
            ItemManager.Instance.EditActualSelectedItem(ItemManager.Instance.ActualSelectedItem, newItem);
            return true;
        }  

        OutputManager.Instance.RemoveListener("ObjectCreate");
        return false;
    }

    #region PRIVATE

    /// <summary>
    /// Creates new item scriptableObject and calls method for applying given components on it.
    /// Than checks if there was any error raised during applying components and if new item has
    /// not same name as existing items. If failes returns null.
    /// </summary>
    /// <param name="components"></param>
    /// <returns></returns>
    private ItemData CreateNewPrefab(List<ObjectComponent> components) 
    {
        hasPassed = true;
        var item = new ItemData("Test", "Boxes", 0);
        ApplyComponents(item, components);
        if (CheckName(item) && hasPassed)
        {
            var objectController = item.Prefab.AddComponent<ObjectController>();
            objectController.Set(item.ShownName);
            return item;
        }
        item.Destroy();
        return null;
    }

    /// <summary>
    /// Creates new item scriptableObject and calls method for applying given components on it.
    /// Than checks if there was any error raised during applying components. This method doest
    /// not check name for uniqueness, because this object is supposed to replace existing item.
    /// If failes returns null.
    /// </summary>
    /// <param name="components"></param>
    /// <returns></returns>
    private ItemData CreateEditingPrefab(List<ObjectComponent> components)
    {
        hasPassed = true;
        var item = new ItemData("Test", "Boxes", 0);
        ApplyComponents(item, components);
        if (hasPassed)
        {
            var objectController = item.Prefab.AddComponent<ObjectController>();
            objectController.Set(item.ShownName);
            return item;
        }
        item.Destroy();
        return null;
    }

    /// <summary>
    /// Applyes given components on given item.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="components"></param>
    /// <returns></returns>
    private void ApplyComponents(ItemData item, List<ObjectComponent> components)
    {
        foreach (var component in components)
            GetComponent(item, component);

        foreach (var component in item.Components)
            component.Set(item);
    }

    /// <summary>
    /// Extracts component from given component source panel and sets it on given item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="component"></param>
    /// <returns></returns>
    private void GetComponent(ItemData item, ObjectComponent component)
    {
        var comp = component.GetComponent();
        if (comp != null) 
        {
            item.Components.Add(comp);
        }
    }

    /// <summary>
    /// Checks if item name is unique
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private bool CheckName(ItemData item)
    {
        if (ItemManager.Instance.ItemsNameIdPair.ContainsKey(item.ShownName))
        {
            OutputManager.Instance.ShowMessage($"Invalid item name {item.ShownName}, name is already used!", "ObjectCreate");
            return false;
        }
        return true;
    }

    private void ErrorHandler(string message)
    {
        hasPassed = false;
    }
    #endregion
}
