using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ItemScriptable")]
public class ItemData : ScriptableObject
{
    public string ShownName;
    public Sprite ShownImage;
    public GameObject Prefab;
    public int Id;

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
