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
    EditorActionBase _actualAction;

    //TODO: Restructuralize (Some field not needed.) Also change to props.
    [SerializeField] public GameObject Parent;
    [SerializeField] public GameObject MarkerPrefab;

    [SerializeField] public ItemData _actualObject;
    [SerializeField] public Grid GridLayout;

    [SerializeField] public List<ItemData> AllAvalibleItems;


    private EditorInputActions _editorActions;
    private InputAction _keyPressed;
    private InputAction _mousePressed;

    public Color originalColor = Color.white;
    public int selectedId = -1;


    public Dictionary<Vector3, GameObject> Selected;
    public Dictionary<int, Dictionary<Vector3, GameObject>> Data;


    private void Awake()
    {
        _editorActions = new EditorInputActions();
        _keyPressed = _editorActions.Player.KeyPressed;
        _keyPressed.started += ctx => _actualAction.OnKeyDown(((KeyControl)ctx.control).keyCode);
        _keyPressed.canceled += ctx => _actualAction.OnKeyUp();

        _mousePressed = _editorActions.Player.MousePressed;
        _mousePressed.started += ctx => _actualAction.OnMouseDown(GetPressedMouseButton());
        _mousePressed.canceled += ctx => _actualAction.OnMouseUp();

        Data = new Dictionary<int, Dictionary<Vector3, GameObject>>();
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void FixedUpdate()
    {
        if (_actualAction != null)
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

    public void SetPrefab(int prefabId)
    {
        _actualObject = AllAvalibleItems[prefabId];
    }

    public GameObject CreateMarkAtPosition(Vector3 position)
    {
        return Paint(GridLayout, MarkerPrefab, position);
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
        if (Data.ContainsKey(_actualObject.Id))
        {
            Data[_actualObject.Id].Add(position, gameobject);
        }
        else
        {
            var itemGroup = new Dictionary<Vector3, GameObject>
            {
                    { position, gameobject }
            };
            Data.Add(_actualObject.Id, itemGroup);
        }
    }

    public void RemoveFromData(Vector3 position)
    {
        if (Data.ContainsKey(_actualObject.Id))
        {
            Data[_actualObject.Id].Remove(position);
        }
    }

    public void RemoveFromData(GameObject gameobject)
    {
        if (Data.ContainsKey(_actualObject.Id))
        {
            var item = Data[_actualObject.Id].First(kvp => kvp.Value == gameObject);
            Data[_actualObject.Id].Remove(item.Key);
        }
    }

    public void RemoveGroupFromData(int id)
    {
        if (Data.ContainsKey(id))
        {
            Data.Remove(id);
        }
    }

    public GameObject Paint(Grid grid, GameObject brushTarget, Vector3 position)
    {
        if (_actualObject)
        {
            GameObject newInstance = TileBase.Instantiate(_actualObject.Prefab, position, Quaternion.identity, brushTarget.transform);
            newInstance.transform.SetParent(brushTarget.transform);
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
            _actualObject = AllAvalibleItems[obj.Id];
            var newObject = Paint(GridLayout, Parent, obj.Position);
            InsertToData(obj.Position, newObject);
        }
    }
}
