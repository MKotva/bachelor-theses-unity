using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAIController : MonoBehaviour
{
    private void Awake()
    {
        var rigid = GetComponent<Rigidbody2D>();
        rigid.isKinematic = false;
    }
}
