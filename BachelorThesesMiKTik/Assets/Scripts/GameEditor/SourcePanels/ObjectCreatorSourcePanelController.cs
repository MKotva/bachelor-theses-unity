using Assets.Scripts.GameEditor.ItemView;
using Assets.Scripts.GameEditor.SourcePanels.Components;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ObjectCreatorSourcePanelController : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    public async Task Create(List<GameObject> components)
    {
        var ob = Instantiate(prefab);
        var item = ItemData.CreateInstance("Test", "Boxes", ob.GetComponent<SpriteRenderer>().sprite, ob, 0);

        var tasks = new List<Task>(); 
        foreach (var component in components)
        {
            if(component.TryGetComponent(out ObjectComponent controller))
                tasks.Add(controller.SetItem(item));
        }

        await Task.WhenAll(tasks);
        GameItemController.Instance.AddItem(item);
    }
}
