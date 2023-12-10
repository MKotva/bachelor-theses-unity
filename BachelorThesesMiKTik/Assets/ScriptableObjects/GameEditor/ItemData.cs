using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ItemScriptable")]
public class ItemData : ScriptableObject
{
    public string ShownName;
    public string GroupName;
    public Sprite ShownImage;
    public GameObject Prefab;
    public int Id;

    public static ItemData CreateInstance(string shownName, string groupName, Sprite shownImage, GameObject prefab, int id)
    {
        var data = ScriptableObject.CreateInstance<ItemData>();
        
        data.ShownName = shownName;
        data.GroupName = groupName;
        data.ShownImage = shownImage;
        data.Prefab = prefab;
        data.Id = id;

        return data;
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
