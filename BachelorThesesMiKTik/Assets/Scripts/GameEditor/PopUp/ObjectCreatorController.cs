using Assets.Core.GameEditor.DTOS;
using Assets.Scripts.GameEditor.ObjectEditors;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectCreatorController : MonoBehaviour
{
    [SerializeField] public GameObject DropDown;
    [SerializeField] public GameObject BoxEditor;
    [SerializeField] public GameObject TrapEditor;
    [SerializeField] public GameObject DecorationEditor;
    [SerializeField] public GameObject EntitiEditor;


    private GameObject active;

    private TMP_Dropdown menu;

    void Start()
    {
        menu = DropDown.GetComponent<TMP_Dropdown>();
        menu.onValueChanged.AddListener(delegate
        {
            ChangeField();
        });
    }

    public void OnCreateClick()
    {
       active.GetComponent<ObjectEditorController>().OnCreate();
    }

    private void ChangeField()
    {
        switch(menu.value)
        {
            case 0:
                SwitchActive(BoxEditor);
                break;
            case 1:
                SwitchActive(TrapEditor);
                break; 
            case 2:
                SwitchActive(DecorationEditor);
                break;
            case 3:
                SwitchActive(EntitiEditor);
                break;
        }
    }

    private void SwitchActive(GameObject activated)
    {
        activated.SetActive(true);
        active.SetActive(false);
        active = activated;
    }
}
