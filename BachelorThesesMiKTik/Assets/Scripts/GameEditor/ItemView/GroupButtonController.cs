using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupButtonController : MonoBehaviour
{
    [SerializeField] GameObject ItemMenu;
    [SerializeField] GameObject Group;

    private ItemMenuController _itemMenuController;

    private void Awake()
    {
        _itemMenuController = ItemMenu.GetComponent<ItemMenuController>();
        _itemMenuController.Groups.Add(Group);
    }

    public void OnClick()
    {
        _itemMenuController.ShowGroup(Group);
    }
}
