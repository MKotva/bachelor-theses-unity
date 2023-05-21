using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.IO;
using System;
using Assets.Scenes.GameEditor.Core.DTOS;
using static UnityEditor.Progress;
using UnityEditor.Rendering;
using UnityEngine.UIElements;
using System.Linq;
using Assets.Scenes.GameEditor.Core.EditorActions;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem.Controls;

public class GridController : MonoBehaviour
{
    //TODO: Restructuralize (Some field not needed.) Also change to props.
    [SerializeField] public GameObject Parent;
    [SerializeField] public GameObject MarkerPrefab;

    [SerializeField] public ItemData ActualPrefab;
    [SerializeField] public Grid GridLayout;

    [SerializeField] public List<ItemData> AllAvalibleItems;

    public Color originalColor = Color.white;
    public int selectedId = -1;

    public Dictionary<Vector3, (GameObject, bool)> Selected;
    public Dictionary<int, Dictionary<Vector3, GameObject>> Data;


    private EditorActionBase _actualAction;
    private EditorActionBase _defaultAction;
    private EditorInputActions _editorActions;
    private InputAction _keyPressed;
    private InputAction _mousePressed;

    private bool _isActionAllowed;

    private void Awake()
    {
        _editorActions = new EditorInputActions();
        _keyPressed = _editorActions.Player.KeyPressed;

        _keyPressed.started += ctx =>
        {
            _isActionAllowed = true;
            _actualAction.OnKeyDown(Key.LeftShift);
        };
        _keyPressed.canceled += ctx =>
        {
            _isActionAllowed = false;
            _actualAction.OnKeyUp();
        };

        _mousePressed = _editorActions.Player.MousePressed;
        _mousePressed.started += ctx =>
        {
            _isActionAllowed = true;
            _actualAction.OnMouseDown(GetPressedMouseButton());
        };
        _mousePressed.canceled += ctx =>
        {
            _isActionAllowed = false;
            _actualAction.OnMouseUp();
        };

        _defaultAction = new MoveAction(this);
        _actualAction = _defaultAction;

        Selected = new Dictionary<Vector3, (GameObject, bool)>();
        Data = new Dictionary<int, Dictionary<Vector3, GameObject>>();
    }

    private void OnEnable()
    {
        _keyPressed.Enable();
        _mousePressed.Enable();
    }

    private void OnDisable()
    {
        _keyPressed?.Disable();
        _mousePressed?.Disable();
    }

    private void FixedUpdate()
    {
        if (_isActionAllowed)
            _actualAction.OnUpdate(GetWorldMousePosition());
    }

    private MouseButton GetPressedMouseButton()
    {
        if (Input.GetMouseButtonDown(0))
            return MouseButton.LeftMouse;

        if (Input.GetMouseButtonDown(1))
            return MouseButton.RightMouse;

        return MouseButton.MiddleMouse;
    }

    public void SetAction(EditorActionBase action)
    {
        _actualAction = action;
    }

    public void SetDefaultAction()
    {
        _actualAction = _defaultAction;
    }

    public void SetPrefab(int prefabId)
    {
        ActualPrefab = AllAvalibleItems[prefabId];
    }

    public GameObject CreateMarkAtPosition(Vector3 position)
    {
        return Paint(MarkerPrefab, Parent, GridLayout, position);
    }

    public void UnMarkPosition(Vector3 position)
    {
        Erase(GetObjectAtPosition(position));
    }

    public void MarkObject(GameObject gameObject)
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(2f, 0f, 0f, 0.7f);
    }

    public void UnMarkObject(GameObject gameObject)
    {
        gameObject.GetComponent<Renderer>().material.color = originalColor;
    }

    public void InsertToData(Vector3 position, GameObject gameobject)
    {
        if (Data.ContainsKey(ActualPrefab.Id))
        {
            Data[ActualPrefab.Id].Add(position, gameobject);
        }
        else
        {
            var itemGroup = new Dictionary<Vector3, GameObject>
            {
                    { position, gameobject }
            };
            Data.Add(ActualPrefab.Id, itemGroup);
        }
    }

    public void RemoveFromData(Vector3 position)
    {
        if (Data.ContainsKey(ActualPrefab.Id))
        {
            Data[ActualPrefab.Id].Remove(position);
        }
    }

    public void RemoveFromData(GameObject gameobject)
    {
        if (Data.ContainsKey(ActualPrefab.Id))
        {
            var item = Data[ActualPrefab.Id].First(kvp => kvp.Value == gameObject);
            Data[ActualPrefab.Id].Remove(item.Key);
        }
    }

    public void RemoveGroupFromData(int id)
    {
        if (Data.ContainsKey(id))
        {
            Data.Remove(id);
        }
    }

    public GameObject Paint(GameObject prefab, GameObject parentObject, Grid grid, Vector3 position)
    {
        if (ActualPrefab)
        {
            GameObject newInstance = TileBase.Instantiate(prefab, position, Quaternion.identity, parentObject.transform);
            newInstance.transform.SetParent(parentObject.transform);
            return newInstance;
        }
        return null;
    }

    public void Erase(GameObject gameobject)
    {
        Destroy(gameobject);
    }

    public void Erase(GameObject gameobject, Vector3 position)
    {
        RemoveFromData(position);
        Destroy(gameobject);
    }


    /// <summary>
    /// Get GameObject at given world cell center position.
    /// </summary>
    [field: Tooltip("Get GameObject at given world cell center position.")]
    public GameObject GetObjectAtPosition(Vector3 cellCenter)
    {
        foreach (var group in Data.Values)
        {
            if (group.ContainsKey(cellCenter))
                return group[cellCenter];
        }
        return null;
    }

    public Vector3 GetWorldMousePosition()
    {
        Vector2 m_pos = Input.mousePosition;
        Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(m_pos);
        return new Vector3(worldMousePos.x, worldMousePos.y);
    }

    public Vector3 GetCellCenterPosition(Vector3 mousePosition)
    {
        return GridLayout.GetCellCenterWorld(GridLayout.WorldToCell(mousePosition));
    }

    public void SaveMap()
    {
        var _mapData = new MapDataDTO();
        foreach (var id in Data.Keys)
        {
            foreach (var position in Data[id].Keys)
            {
                _mapData.mapObjects.Add(new MapObjectDTO(id, position));
            }
        }

        string json = JsonUtility.ToJson(_mapData);

        using (var sw = new StreamWriter("MapSaveTest.json"))
        {
            sw.Write(json);
        }
    }

    public void LoadMap()
    {
        string map = "";
        using (var sr = new StreamReader("MapSaveTest.json"))
        {
            map = sr.ReadToEnd();
        }

        var _mapData = JsonUtility.FromJson<MapDataDTO>(map);
        foreach (var obj in _mapData.mapObjects)
        {
            ActualPrefab = AllAvalibleItems[obj.Id];
            var newObject = Paint(ActualPrefab.Prefab, Parent, GridLayout, obj.Position);
            InsertToData(obj.Position, newObject);
        }
    }
}
