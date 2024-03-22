using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataSource", menuName = "Item DataSource")]
public class ItemDataSource : ScriptableObject
{
    [SerializeField] public string Name;
    [SerializeField] public string Group;
    [SerializeField] public GameObject Prefab;
}
