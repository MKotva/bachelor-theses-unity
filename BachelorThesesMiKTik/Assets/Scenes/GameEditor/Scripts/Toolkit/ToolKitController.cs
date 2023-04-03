using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolKitController : MonoBehaviour
{
  enum CanvasType 
  {
    None,
    ItemMenu
  }

  [SerializeField] GameObject ItemMenuCanvas;

  CanvasType _last;

  public void OnItemsButtonClick()
  {

    if (_last != CanvasType.None)
    {
      if(_last == CanvasType.ItemMenu) 
      {
        SetNonActive();
        return;
      }
      SetNonActive();
    }
    ItemMenuCanvas.SetActive(true);
    _last = CanvasType.ItemMenu;
  }

  void SetNonActive()
  {
    if (_last == CanvasType.ItemMenu)
      ItemMenuCanvas.SetActive(false);

    _last = CanvasType.None;
  }
}
