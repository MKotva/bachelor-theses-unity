using Assets.Core.GameEditor.DTOS.Components;
using Assets.Scripts.GameEditor.SourcePanels.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "ItemData", menuName = "ItemScriptable")]
public class ItemData : ScriptableObject
{
    public string ShownName;
    public string GroupName;
    public Sprite ShownImage;
    public GameObject Prefab;
    public List<ComponentDTO> Components;
    public int Id;

    public static ItemData CreateInstance()
    {
        var data = ScriptableObject.CreateInstance<ItemData>();
        data.Prefab = ((GameObject) Resources.Load("CustomObjectPrefab"));
        data.Components = new List<ComponentDTO>();
        return data;
    }

    public static ItemData CreateInstance(string shownName, string groupName, int id)
    {
        var data = ScriptableObject.CreateInstance<ItemData>();
        
        data.ShownName = shownName;
        data.GroupName = groupName;
        data.Prefab = new GameObject("Random");
        var texture = new Texture2D(32, 16);
        data.ShownImage = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        data.Components = new List<ComponentDTO>();
        data.Id = id;
        return data;
    }

    public static void DestroyInstance(ItemData data)
    {
        ScriptableObject.Destroy(data.Prefab);
    }

    public Sprite GetImage()
    {
        if (ShownImage == null && Prefab != null)
        {
            SpriteRenderer renderer;
            if (!Prefab.TryGetComponent(out renderer))
                Prefab.GetComponentInChildren<SpriteRenderer>();

            if (renderer != null)
                ShownImage = renderer.sprite;
        }
        return ShownImage;
    }
}
