using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpController : MonoBehaviour
{
    [SerializeField] GameObject Canvas;
    public virtual void OnExitClick()
    {
        gameObject.SetActive(false);
        Canvas.SetActive(false);
    }
}
