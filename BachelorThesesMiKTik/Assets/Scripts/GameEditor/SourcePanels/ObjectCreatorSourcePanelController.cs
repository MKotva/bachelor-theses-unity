using Assets.Scripts.GameEditor.ItemView;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using Assets.Scripts.GameEditor.SourcePanels.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectCreatorSourcePanelController : MonoBehaviour
{
    private bool hasPassed;
    public async Task CreateItem(List<ObjectComponent> components)
    {
        InfoPanelController.Instance.AddOnAddListener("ObjectCreate", ErrorHandler, "ObjectCreate");
        var newItem = await CreateNewPrefab (components);
        if (hasPassed)
        {
            GameItemController.Instance.AddItem(newItem);
        }
        InfoPanelController.Instance.RemoveListener("ObjectCreate");
    }

    public async Task EditItem(List<ObjectComponent> components)
    {
        InfoPanelController.Instance.AddOnAddListener("ObjectCreate", ErrorHandler, "ObjectCreate");
        var newItem = await CreateEditingPrefab(components);
        if (hasPassed)
        {
            GameItemController.Instance.EditActualSelectedItem(GameItemController.Instance.ActualSelectedItem, newItem);
        }
        else
        {

        }    
        InfoPanelController.Instance.RemoveListener("ObjectCreate");
    }

    #region PRIVATE

    /// <summary>
    /// Creates new item scriptableObject and calls method for applying given components on it.
    /// Than checks if there was any error raised during applying components and if new item has
    /// not same name as existing items. If failes returns null.
    /// </summary>
    /// <param name="components"></param>
    /// <returns></returns>
    private async Task<ItemData> CreateNewPrefab(List<ObjectComponent> components) 
    {
        hasPassed = true;
        var item = ItemData.CreateInstance("Test", "Boxes", 0);
        await ApplyComponents(item, components);
        if (CheckName(item) && hasPassed)
        {
            var objectController = item.Prefab.AddComponent<ObjectController>();
            objectController.Set(item.ShownName, components);
            return item;
        }
        ItemData.DestroyInstance(item);
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
    private async Task<ItemData> CreateEditingPrefab(List<ObjectComponent> components)
    {
        hasPassed = true;
        var item = ItemData.CreateInstance("Test", "Boxes", 0);
        await ApplyComponents(item, components);
        if (hasPassed)
        {
            var objectController = item.Prefab.AddComponent<ObjectController>();
            objectController.Set(item.ShownName, components);
            return item;
        }
        ItemData.DestroyInstance(item);
        return null;
    }

    /// <summary>
    /// Applyes given components on given item.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="components"></param>
    /// <returns></returns>
    private async Task ApplyComponents(ItemData item, List<ObjectComponent> components)
    {
        var tasks = new List<Task>();
        foreach (var component in components)
            tasks.Add(ApplyComponent(item, component));

        await Task.WhenAll(tasks);
        
    }

    /// <summary>
    /// Extracts component from given component source panel and sets it on given item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="component"></param>
    /// <returns></returns>
    private async Task ApplyComponent(ItemData item, ObjectComponent component)
    {
        var comp = await component.GetComponent();
        if (comp != null) 
        {
            await comp.Set(item);
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
        if (GameItemController.Instance.ItemsNameIdPair.ContainsKey(item.ShownName))
        {
            InfoPanelController.Instance.ShowMessage($"Invalid item name {item.ShownName}, name is already used!", "ObjectCreate");
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
