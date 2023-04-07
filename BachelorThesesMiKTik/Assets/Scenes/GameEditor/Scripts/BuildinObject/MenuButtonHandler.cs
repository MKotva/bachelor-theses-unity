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
    SetActualItemPreview();
  }

  public void SetBuildingItem(ItemData data)
  {
    BuildingItem = data;
  }

  private void SetActualItemPreview()
  {
    GameObject actualImageView = GameObject.Find("SelectedItemImage");
    Image img = actualImageView.GetComponent<Image>();
    Sprite sprite = BuildingItem.Prefab.GetComponent<SpriteRenderer>().sprite;
    img.sprite = sprite;
  }
}
