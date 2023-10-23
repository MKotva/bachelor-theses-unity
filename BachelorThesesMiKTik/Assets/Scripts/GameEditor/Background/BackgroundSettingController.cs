using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class BackgroundSettingController : MonoBehaviour
{
    [SerializeField] GameObject BackgroundController;

    private BackgroundController backgroundController;

    private void Awake()
    {
        backgroundController = BackgroundController.GetComponent<BackgroundController>();
    }

    public void OnButtonSe1lection()
    {
        var test = new List<string>() 
        {
            @"C:\Users\mkotv\Pictures\rayTrace.png\"
        };

        backgroundController.SetImagesAsync(test);
    }
}
