using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemMenuController : MonoBehaviour
{
    enum MenuType
    {
        None,
        Box,
        Traps,
        Decorations
    }

    [SerializeField] GameObject BoxMenu;
    [SerializeField] GameObject TrapsMenu;
    [SerializeField] GameObject DecorationsMenu;
    [SerializeField] GameObject SelectionInputField;

    public List<GameObject> Groups;

    private GameObject _active;

    public void ShowGroup(GameObject group)
    {
        if(_active != null)
            _active.SetActive(false);
        
        _active = group;
        _active.SetActive(true);
    }

    public void OnSelect()
    {
        GameObject groupWithMinDistance = null;
        int minDistance = int.MaxValue;

        var name = SelectionInputField.GetComponent<TMP_InputField>().text;
        foreach(var group in Groups)
        { 
            var controller = group.GetComponentInChildren<ViewportController>();

            int distance = int.MaxValue;
            if(controller.SelectBySearch(name, out distance))
            {
                ShowGroup(group);
                return;
            }

            if (minDistance > distance)
            {
                groupWithMinDistance = group;
                minDistance = distance;
            }
        }
        ShowGroup(groupWithMinDistance);
    }
}
