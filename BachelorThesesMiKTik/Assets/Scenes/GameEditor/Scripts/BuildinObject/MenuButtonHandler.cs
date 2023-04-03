using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonHandler : MonoBehaviour
{
  [SerializeField] public ItemData BuildingItem;
  Button _button;

  void Awake()
  {
    _button = GetComponent<Button>();
    _button.onClick.AddListener(ButtonAction);
  }

  void ButtonAction()
  {
    GameObject workspace = GameObject.Find("Grid");
    workspace.GetComponent<GridController>().SetPrefab(BuildingItem.Prefab);
  }

  public void SetBuildingItem(ItemData data)
  {
    BuildingItem = data;
  }
}
