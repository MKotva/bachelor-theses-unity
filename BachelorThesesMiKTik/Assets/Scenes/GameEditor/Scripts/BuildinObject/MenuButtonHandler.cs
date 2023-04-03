using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonHandler : MonoBehaviour
{
  [SerializeField] public ItemData BuildingItem;
  [SerializeField] public GameObject WorkSpace; //TODO: Rework, possibly suboptimal.
  Button _button;

  void Awake()
  {
    _button = GetComponent<Button>();
    _button.onClick.AddListener(ButtonAction);
  }

  void ButtonAction()
  {
    WorkSpace.GetComponent<GridController>().SetPrefab(BuildingItem.Prefab);
  }

  public void SetBuildingItem(ItemData data)
  {
    BuildingItem = data;
  }
}
