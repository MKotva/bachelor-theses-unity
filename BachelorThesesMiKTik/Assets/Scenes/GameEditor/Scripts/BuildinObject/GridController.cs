using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridController : MonoBehaviour
{
  [SerializeField] GameObject Grid;
  [SerializeField] GameObject Parent;
  [SerializeField] GameObject _actualPrefab;
  [SerializeField] Grid GridLayout;

  public void SetPrefab(GameObject prefab)
  {
    _actualPrefab = prefab;
  }

  public void OnMouseLeftClick()
  {
    Vector2 m_pos = Input.mousePosition;
    Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(m_pos);

    Vector3 position = new Vector3(worldMousePos.x, worldMousePos.y);
    Paint(GridLayout, Parent, position);
  }

  public void Paint(Grid grid, GameObject brushTarget, Vector3 position)
  {
    if (_actualPrefab != null)
    {
      GameObject newInstance = Instantiate(_actualPrefab, grid.GetCellCenterWorld(grid.WorldToCell(position)), Quaternion.identity);
      newInstance.transform.SetParent(brushTarget.transform);
    }
  }
}
