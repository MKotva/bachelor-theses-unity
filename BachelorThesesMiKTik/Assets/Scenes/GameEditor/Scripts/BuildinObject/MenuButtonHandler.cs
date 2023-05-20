using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonHandler : MonoBehaviour
{
    [SerializeField] public ItemData BuildingItem;

    private Button _button;
    private GridController _gridController;
    private GameObject _viewBox;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ButtonAction);
    }

    void ButtonAction()
    {
        _gridController.SetPrefab(BuildingItem.Id);
        SetActualItemPreview();
    }

    public void SetBuildingItem(ItemData data, GridController controller, GameObject viewBox)
    {
        BuildingItem = data;
        _gridController = controller;
        _viewBox = viewBox;
    }

    public void SetActualItemPreview()
    {
        Image img = _viewBox.GetComponent<Image>();
        Sprite sprite = BuildingItem.Prefab.GetComponent<SpriteRenderer>().sprite;
        img.sprite = sprite;
    }
}
