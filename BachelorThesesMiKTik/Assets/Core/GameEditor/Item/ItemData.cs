using Assets.Core.GameEditor.Components;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using System.Collections.Generic;
using UnityEngine;

public class ItemData
{
    public string ShownName;
    public string GroupName;
    public Sprite ShownImage;
    public GameObject Prefab;
    public List<CustomComponent> Components;
    public int Id;

    public ItemData()
    {
        Prefab = CreatePrefab();
        Components = new List<CustomComponent>();
    }

    public ItemData(string shownName, string groupName, int id)
    {
        Prefab = CreatePrefab();
        ShownName = shownName;
        GroupName = groupName;
        var texture = new Texture2D(32, 16);
        ShownImage = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        Components = new List<CustomComponent>();
        Id = id;
    }

    public ItemData(ItemDataSource source) 
    {
        ShownName = source.Name;
        GroupName = source.Group;
        Prefab = source.Prefab;

        SpriteRenderer renderer;
        if(!Prefab.TryGetComponent(out renderer)) 
        {
            renderer = Prefab.AddComponent<SpriteRenderer>();
        }

        ShownImage = renderer.sprite;
        Id = ShownName.GetHashCode();
    }
    public void Destroy()
    {
        GameObject.Destroy(Prefab);
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

    private GameObject CreatePrefab()
    {
        var prefab = new GameObject("CustomObjectPrefab");
        prefab.AddComponent<SpriteRenderer>();
        prefab.AddComponent<ObjectController>();
        return prefab;
    }
}
