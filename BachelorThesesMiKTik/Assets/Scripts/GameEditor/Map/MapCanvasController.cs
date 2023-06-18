using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.IO;
using System;
using Assets.Scenes.GameEditor.Core.DTOS;
using UnityEngine.UIElements;
using System.Linq;
using Assets.Scenes.GameEditor.Core.EditorActions;
using Assets.Core.GameEditor.Journal;

public class MapCanvasController : MonoBehaviour
{
    //TODO: Restructuralize (Some field not needed.) Also change to props.
    [SerializeField] public GameObject Parent;
    [SerializeField] public GameObject MarkerPrefab;
    [SerializeField] public GameObject MarkerDotPrefab;
    [SerializeField] public Camera CameraObj;
    [SerializeField] public Grid GridLayout;
    [SerializeField] public ItemData ActualPrefab;
    [SerializeField] public List<ItemData> AllAvalibleItems;
    [SerializeField] public bool IsRecording;
    [SerializeField] public int JournalCapacity;
    [SerializeField] public List<string> BlockingObjectTags;

    public Color originalColor = Color.white;
    public int selectedId = -1;
    public Dictionary<Vector3, (GameObject, bool)> Selected;
    public Dictionary<int, Dictionary<Vector3, GameObject>> Data;
    public Journal MapJournal;

    private EditorActionBase _actualAction;
    private EditorActionBase _defaultAction;
    private EditorInputActions _editorActions;
    private InputAction _keyPressed;
    private InputAction _mousePressed;

    private bool _isActionAllowed;

    private void Awake()
    {
        MapJournal = new Journal(JournalCapacity);

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
            if (Input.mousePosition.y < 830 && Input.mousePosition.y > 0 
                && Input.mousePosition.x > 0 && Input.mousePosition.x < 1920)
            {
                _isActionAllowed = true;
                _actualAction.OnMouseDown(GetPressedMouseButton());
            }
        };
        _mousePressed.canceled += ctx =>
        {
            if (Input.mousePosition.y < 830 && Input.mousePosition.y > 0
                && Input.mousePosition.x > 0 && Input.mousePosition.x < 1920)
            {
                _isActionAllowed = false;
                _actualAction.OnMouseUp();
                if (IsRecording)
                {
                    MapJournal.Record(_actualAction.GetLastActionRecord(), _actualAction.GetLastActionRecordReverse());
                }
            }
        };

        _defaultAction = new MoveAction(this);
        _actualAction = _defaultAction;

        Selected = new Dictionary<Vector3, (GameObject, bool)>();
        Data = new Dictionary<int, Dictionary<Vector3, GameObject>>();
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

    public void OnEnable()
    {
        _keyPressed.Enable();
        _mousePressed.Enable();
    }

    public void OnDisable()
    {
        _keyPressed?.Disable();
        _mousePressed?.Disable();
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

    public GameObject CreateMarkAtPosition(GameObject markerPrefab, Vector3 position)
    {
        return Paint(markerPrefab, Parent, GridLayout, position);
    }

    public void DestroyMark(GameObject marker)
    {
        Erase(marker);
    }

    public void UnSelectAll()
    {
        foreach(var item in Selected)
        {
            if (item.Value.Item2)
            {
                Erase(item.Value.Item1);
            }
            else
            {
                UnMarkObject(item.Value.Item1);
            }
        }
        Selected = new Dictionary<Vector3, (GameObject, bool)>();
    }

    public void MarkObject(GameObject gameObject)
    {
        Renderer renderer;
        if(!gameObject.TryGetComponent(out renderer))
           renderer = gameObject.GetComponentInChildren<Renderer>();

        renderer.material.color = new Color(2f, 0f, 0f, 0.7f);
    }

    public void UnMarkObject(GameObject gameObject)
    {
        Renderer renderer;
        if (!gameObject.TryGetComponent(out renderer))
            renderer = gameObject.GetComponentInChildren<Renderer>();

        renderer.material.color = originalColor;
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

    public void ReplaceData(Vector3 oldKey, Vector3 newKey, GameObject newValue)
    {
        foreach(var group in Data)
        {
            var groupMembers = group.Value;
            if (groupMembers.ContainsKey(oldKey))
            {
                groupMembers.Remove(oldKey);
                if (groupMembers.ContainsKey(newKey))
                {
                    Erase(groupMembers[newKey]);
                    groupMembers[newKey] = newValue;
                }
                else
                {
                    groupMembers.Add(newKey, newValue);
                }
                return;
            }
        }
    }

    public bool ContainsObjectAtPosition(Vector3 position)
    {
        foreach(var group in Data.Values)
        {
            if (group.ContainsKey(position))
                return true;
        }
        return false;
    }

    public bool ContainsObjectAtPosition(Vector3 position, int[] layers)
    {
        foreach (var group in Data.Values)
        {
            if (group.ContainsKey(position))
            {
                var obj = group[position];
                foreach( var layer in layers)
                {
                    if(obj.layer == layer)
                        return true;
                }
            }
        }
        return false;
    }


    //TODO: Rework this, make with layers.
    public bool ContainsBlockingObjectAtPosition(Vector3 position)
    {
        foreach (var group in Data.Values)
        {
            if (group.ContainsKey(position))
            {
                if (BlockingObjectTags.Contains(group[position].tag))
                    return true;
            }
        }
        return false;
    }

    public void RemoveFromData(Vector3 position)
    {
        foreach(var group in Data.Values)
        {
            if(group.ContainsKey(position))
                group.Remove(position);
        }
    }

    //TODO: Fix this, cannot search by ActualPrefab ID!!!!
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

    public void EraseMap()
    {
        foreach(var item in Data)
        {
            foreach(var gameObj in item.Value)
            {
                Erase(gameObj.Value);
            }
        }
        Data = new Dictionary<int, Dictionary<Vector3, GameObject>>();
    }

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
        Vector2 worldMousePos = CameraObj.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(worldMousePos.x, worldMousePos.y);
    }

    public Vector3 GetCellCenterPosition(Vector3 mousePosition)
    {
        return GridLayout.GetCellCenterWorld(GridLayout.WorldToCell(mousePosition));
    }

    public void SaveMap(string path)
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

        using (var sw = new StreamWriter(path))
        {
            sw.Write(json);
        }
    }

    public void LoadMap(string path)
    {
        string map = "";
        using (var sr = new StreamReader(path))
        {
            map = sr.ReadToEnd();
        }

        var _mapData = JsonUtility.FromJson<MapDataDTO>(map);
        if(_mapData != null)
        {
            EraseMap();
            foreach (var obj in _mapData.mapObjects)
            {
                ActualPrefab = AllAvalibleItems[obj.Id];
                var newObject = Paint(ActualPrefab.Prefab, Parent, GridLayout, obj.Position);
                InsertToData(obj.Position, newObject);
            }
        }
    }
}
