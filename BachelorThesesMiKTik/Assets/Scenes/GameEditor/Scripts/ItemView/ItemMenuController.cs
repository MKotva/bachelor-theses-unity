using System.Collections;
using System.Collections.Generic;
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

  MenuType _lastShown = MenuType.Box;

  public void HandleBoxClick()
  {
    if (_lastShown != MenuType.Box && _lastShown != MenuType.None)
      SetNonActive();
      
    BoxMenu.SetActive(true);
    _lastShown = MenuType.Box;
  }

  public void HandleTrapClick()
  {
    if (_lastShown != MenuType.Traps && _lastShown != MenuType.None)
      SetNonActive();

    TrapsMenu.SetActive(true);
    _lastShown = MenuType.Traps;
  }

  public void HandleDecorationsClick()
  {
    if (_lastShown != MenuType.Decorations && _lastShown != MenuType.None)
      SetNonActive();

    DecorationsMenu.SetActive(true);
    _lastShown = MenuType.Decorations;
  }

  public void HandleEviromentClick()
  {

  }

  void SetNonActive()
  {
     if(_lastShown == MenuType.Box)
      BoxMenu.SetActive(false);
     else if(_lastShown == MenuType.Traps)
      TrapsMenu.SetActive(false);
     else if(_lastShown == MenuType.Decorations)
      DecorationsMenu.SetActive(false);

    _lastShown = MenuType.None;
  }
}
