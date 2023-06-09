using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ActualItemViewController : MonoBehaviour
{
  public void SetImage(Sprite actualSprite)
  {
    GetComponent<Image>().sprite = actualSprite;
  }
}
