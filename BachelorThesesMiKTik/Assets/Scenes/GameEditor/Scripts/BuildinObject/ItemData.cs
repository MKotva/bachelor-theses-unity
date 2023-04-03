using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemData", menuName = "JumpSystem")]
public class ItemData : ScriptableObject
{
  public enum CategoryType { 
    None, 
    Boxes, 
    Traps, 
    Decorations,
    Tools
  }

  public string ShownName;
  public GameObject Prefab;
  public Vector2 Size;
  public Vector2 Center;
  public CategoryType Category;
}
