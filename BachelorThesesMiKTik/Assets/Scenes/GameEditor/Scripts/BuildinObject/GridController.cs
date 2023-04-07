using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;

public class GridController : MonoBehaviour
{
  enum ActionType
  {
    None,
    Insert,
    Remove
  }

  //TODO: Restructuralize (Some field not needed.) Also change to props.
  [SerializeField] GameObject Grid;
  [SerializeField] GameObject Parent;
  [SerializeField] GameObject _actualPrefab;
  [SerializeField] Grid GridLayout;


  EditorInputActions _editorActions;
  InputAction _multiAction;
  InputAction _insertAction;
  InputAction _removeAction;

  bool _multiActionEnabled;
  ActionType _actualAction;
  
  
  //Vector3 _mouseStartPos;
  //Vector3 _mouseEndPos;

  private void Awake()
  {
    _editorActions = new EditorInputActions();

    _multiAction = _editorActions.Player.MultiAction;
    _multiAction.started += ctx => _multiActionEnabled = true;
    _multiAction.canceled += ctx => _multiActionEnabled = false;

    _insertAction = _editorActions.Player.AddItem;
    _insertAction.started += ctx => _actualAction = ActionType.Insert;
    _insertAction.canceled += ctx => _actualAction = ActionType.None;

    _removeAction = _editorActions.Player.RemoveItem;
    _removeAction.started += ctx => _actualAction = ActionType.Remove;
    _removeAction.canceled += ctx => _actualAction = ActionType.None;

  }

  private void OnEnable()
  {
    _multiAction.Enable();
    _insertAction.Enable();
    _removeAction.Enable();

  }

  private void OnDisable()
  {
    _multiAction.Disable();
    _insertAction.Disable();
    _removeAction.Disable();
  }

  private void FixedUpdate()
  {
    PerformAction();
    //if (_multiActionEnabled)
    //{
    //  InsertItem();
    //}
  }

  private void PerformAction()
  {
    switch (_actualAction)
    {
      case ActionType.Insert: 
        InsertItem();
        break;
      case ActionType.Remove: 
        this.RemoveItem();
        break;
    }
  }


  public void SetPrefab(GameObject prefab)
  {
    _actualPrefab = prefab;
  }

  private void InsertItem() 
  {
    Vector3 position = GetWorldMousePosition();
    GameObject objectAtPos = GetObjectAtPosition(position);

    if (objectAtPos == null)
    {
      Paint(GridLayout, Parent, position);
    }
  }

  private void RemoveItem()
  {
    Vector3 position = GetWorldMousePosition();
    GameObject objectAtPos = GetObjectAtPosition(position);

    if (objectAtPos)
    {
      Destroy(objectAtPos);
    }
  }

  private void Paint(Grid grid, GameObject brushTarget, Vector3 position)
  {
    if (_actualPrefab)
    {
      GameObject newInstance = TileBase.Instantiate(_actualPrefab, grid.GetCellCenterWorld(grid.WorldToCell(position)), Quaternion.identity, brushTarget.transform);
      newInstance.transform.SetParent(brushTarget.transform);
    }
  }


  private GameObject GetObjectAtPosition(Vector3 position)
  {
    RaycastHit2D hit = Physics2D.Raycast(position, new Vector3(0, 0, 1));
    if (hit)
    {
      return hit.collider.gameObject;
    }
    return null;
  }

  private Vector3 GetWorldMousePosition()
  {
    Vector2 m_pos = Input.mousePosition;
    Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(m_pos);
    return new Vector3(worldMousePos.x, worldMousePos.y);
  }
}
