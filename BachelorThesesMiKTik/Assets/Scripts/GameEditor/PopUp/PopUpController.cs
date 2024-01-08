using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpController : MonoBehaviour
{
    public virtual void OnExitClick()
    {
        Destroy(gameObject);
    }
}
