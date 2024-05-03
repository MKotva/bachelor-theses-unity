using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Assets.Scenes.GameEditor.Core.EditorActions;
using Assets.Core.GameEditor.Journal;
using Assets.Scripts.GameEditor;
using Assets.Scripts.GameEditor.Map;
using Assets.Scripts.GameEditor.Managers;

public class EditorCanvas : Singleton<EditorCanvas>
{
    [SerializeField] public Camera CameraObj;
    [SerializeField] public Grid GridLayout;
    [SerializeField] public Marker Marker;
    [SerializeField] public bool IsRecording;
    [SerializeField] private int JournalCapacity;

    public ItemData ActualPrefab
    {
        get { return ItemManager.Instance.ActualSelectedItem; }
    }

    public Dictionary<Vector3, (GameObject, bool)> Selected;
    public Dictionary<int, Dictionary<Vector3, GameObject>> Data;
    public Journal MapJournal;

    private EditorActionBase actualAction;
    private EditorActionBase defaultAction;
    private EditorInputActions mapActions;
    private InputAction _keyPressed;
    private InputAction _mousePressed;

    private bool _isActionAllowed;


    #region PUBLIC
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
        actualAction = action;
    }

    public void SetDefaultAction()
    {
        actualAction = defaultAction;
    }

    public void UnselectAll()
    {
        foreach (var item in Selected)
        {
            if (item.Value.Item2)
            {
                Erase(item.Value.Item1);
            }
            else
            {
                Marker.UnMarkObject(item.Value.Item1);
            }
        }
        Selected = new Dictionary<Vector3, (GameObject, bool)>();
    }


    public void ReplaceData(Vector3 oldPosition, Vector3 newPosition, GameObject newValue)
    {
        foreach (var group in Data)
        {
            var groupMembers = group.Value;
            if (groupMembers.ContainsKey(oldPosition))
            {
                groupMembers.Remove(oldPosition);
                if (ContainsObjectAtPosition(newPosition, out int id))
                {
                    Destroy(Data[id][newPosition]);
                    RemoveFromData(newPosition);
                }
                groupMembers.Add(newPosition, newValue);
                return;
            }
        }
    }

    public bool IsPositionInCameraView(Vector3 position)
    {
        var sceneMinPosition = CameraObj.ScreenToWorldPoint(new Vector3(0, 0));
        var sceneMaxPosition = CameraObj.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

        if (sceneMaxPosition.x < position.x || sceneMinPosition.x > position.x ||
           sceneMaxPosition.y < position.y || sceneMinPosition.y > position.y)
            return false;
        return true;
    }

    public bool ContainsObjectAtPosition(Vector3 position)
    {
        foreach (var group in Data.Values)
        {
            if (group.ContainsKey(position))
                return true;
        }
        return false;
    }

    public bool ContainsObjectAtPosition(Vector3 position, out int id)
    {
        foreach(var prototypeId in Data.Keys)
        {
            if (Data[prototypeId].ContainsKey(position))
            {
                id = prototypeId;
                return true;
            }
        }

        id = 0;
        return false;
    }

    public bool ContainsObjectAtPosition(Vector3 position, out GameObject ob)
    {
        foreach (var group in Data.Values)
        {
            if (group.ContainsKey(position))
            {
                ob = group[position];
                return true;             
            }
        }

        ob = null;
        return false;
    }


    //TODO: Rework this, make with layers.
    public bool ContainsBlockingObjectAtPosition(Vector3 position)
    {
        foreach (var group in Data.Values)
        {
            if (group.ContainsKey(position))
            {
                if (group[position].layer == LayerMask.GetMask("Box"))
                    return true;
            }
        }
        return false;
    }

    public void RemoveFromData(Vector3 position)
    {
        foreach(var id in Data.Keys)
        {
            if (Data[id].ContainsKey(position))
            {
                Data[id].Remove(position);
                if(Data[id].Count == 0)
                    Data.Remove(id);

                return;
            }
        }
    }

    public void RemoveGroupFromData(int id)
    {
        if (Data.ContainsKey(id))
        {
            foreach (var ob in Data[id].Values)
            {
                Destroy(ob);
            }
            Data.Remove(id);
        }
    }

    /// <summary>
    /// Returns group of object instances with the same id (same prefab).
    /// If group is not found, returns empty dictionary(empty group).
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns></returns>
    public Dictionary<Vector3, GameObject> GetGroup(int groupId)
    {
        if (Data.ContainsKey(groupId))
        {
            return Data[groupId];
        }
        return new Dictionary<Vector3, GameObject>();
    }

    public bool TryGetID(Vector3 position, out int prototypeId)
    {
        foreach(var id in Data.Keys)
        {
            if (Data[id].ContainsKey(position))
            {
                prototypeId = id;
                return true;
            }
        }

        prototypeId = 0;
        return false;
    }

    public GameObject Paint(ItemData item, Vector3 position)
    {
        var newInstance = CreateInstance(item, position);
        InsertToData(item, newInstance, position);
        return newInstance;
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
        foreach (var item in Data)
        {
            foreach (var gameObj in item.Value)
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
    #endregion

    #region PRIVATE
    protected override void Awake()
    {
        base.Awake();

        MapJournal = new Journal(JournalCapacity);

        mapActions = new EditorInputActions();
        _keyPressed = mapActions.Player.KeyPressed;
        _keyPressed.started += ctx => OnKeyPress();
        _keyPressed.canceled += ctx => OnKeyRelease();

        _mousePressed = mapActions.Player.MousePressed;
        _mousePressed.started += ctx => OnMousePress();
        _mousePressed.canceled += ctx => OnMouseRelease();

        defaultAction = new MoveAction();
        actualAction = defaultAction;

        Selected = new Dictionary<Vector3, (GameObject, bool)>();
        Data = new Dictionary<int, Dictionary<Vector3, GameObject>>();
    }

    private void Update()
    {
        if (_isActionAllowed)
            actualAction.OnUpdate(GetWorldMousePosition());
    }

    private MouseButton GetPressedMouseButton()
    {
        if (Input.GetMouseButtonDown(0))
            return MouseButton.LeftMouse;

        if (Input.GetMouseButtonDown(1))
            return MouseButton.RightMouse;

        return MouseButton.MiddleMouse;
    }

    private GameObject CreateInstance(ItemData item, Vector3 position)
    {
        GameObject newInstance = Instantiate(item.Prefab, position, Quaternion.identity, GridLayout.transform);
        if (item.Components != null)
        {
            foreach (var comp in item.Components)
            {
                comp.SetInstance(item, newInstance);
            }
        }
        return newInstance;
    }

    private void InsertToData(ItemData item, GameObject gameobject, Vector3 position)
    {
        if (Data.ContainsKey(item.Id))
        {
            Data[item.Id].Add(position, gameobject);
        }
        else
        {
            var itemGroup = new Dictionary<Vector3, GameObject>
            {
                { position, gameobject }
            };
            Data.Add(item.Id, itemGroup);
        }
    }

    private void OnMousePress()
    {
        if (Input.mousePosition.y < 830 && Input.mousePosition.y > 0
            && Input.mousePosition.x > 0 && Input.mousePosition.x < 1920)
        {
            _isActionAllowed = true;
            actualAction.OnMouseDown(GetPressedMouseButton());
        }
    }

    private void OnMouseRelease()
    {
        if (_isActionAllowed)
        {
            _isActionAllowed = false;
            actualAction.OnMouseUp();
        }
    }

    private void OnKeyPress()
    {
        _isActionAllowed = true;
        actualAction.OnKeyDown(Key.LeftShift);
    }

    private void OnKeyRelease()
    {
        _isActionAllowed = false;
        actualAction.OnKeyUp();
    }
    #endregion
}
