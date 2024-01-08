using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Assets.Scenes.GameEditor.Core.EditorActions;
using Assets.Core.GameEditor.Journal;
using Assets.Scripts.GameEditor.ItemView;
using Assets.Scripts.GameEditor;

public class Editor : Singleton<Editor>
{
    //TODO: Restructuralize (Some field not needed.) Also change to props.
    [SerializeField] public GameObject Parent;
    [SerializeField] public GameObject MarkerPrefab;
    [SerializeField] public GameObject MarkerDotPrefab;
    [SerializeField] public Camera CameraObj;
    [SerializeField] public Grid GridLayout;
    [SerializeField] public bool IsRecording;
    [SerializeField] public int JournalCapacity;
    [SerializeField] public List<string> BlockingObjectTags;

    public ItemData ActualPrefab
    {
        get { return GameItemController.Instance.ActualSelectedItem; }
    }

    public Color originalColor = Color.white;
    public Dictionary<Vector3, (GameObject, bool)> Selected;
    public Dictionary<int, Dictionary<Vector3, GameObject>> Data;
    public Journal MapJournal;

    private EditorActionBase _actualAction;
    private EditorActionBase _defaultAction;
    private EditorInputActions _editorActions;
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
        _actualAction = action;
    }

    public void SetDefaultAction()
    {
        _actualAction = _defaultAction;
    }

    public GameObject CreateMarkAtPosition(Vector3 position)
    {
        return TileBase.Instantiate(MarkerPrefab, position, Quaternion.identity, GridLayout.transform);
    }

    public List<GameObject> CreateMarkAtPosition(List<Vector3> positions)
    {
        var markers = new List<GameObject>();
        foreach (Vector3 position in positions)
            markers.Add(CreateMarkAtPosition(position));

        return markers;
    }

    public GameObject CreateMarkAtPosition(GameObject markerPrefab, Vector3 position)
    {
        return TileBase.Instantiate(markerPrefab, position, Quaternion.identity, GridLayout.transform); ;
    }

    public List<GameObject> CreateMarkAtPosition(GameObject markerPrefab, List<Vector3> positions)
    {
        var markers = new List<GameObject>();
        foreach (Vector3 position in positions)
            markers.Add(CreateMarkAtPosition(markerPrefab, position));

        return markers;
    }

    public GameObject CreateMarkAtPosition(GameObject markerPrefab, Vector3 position, Color color)
    {
        var marker = TileBase.Instantiate(markerPrefab, position, Quaternion.identity, GridLayout.transform); ;
        marker.GetComponent<Renderer>().material.color = color;
        return marker;
    }

    public void DestroyMark(GameObject marker)
    {
        Erase(marker);
    }

    public void UnSelectAll()
    {
        foreach (var item in Selected)
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
        if (!gameObject.TryGetComponent(out renderer))
            renderer = gameObject.GetComponentInChildren<Renderer>();

        if(renderer != null)
            renderer.material.color = new Color(2f, 0f, 0f, 0.7f);
    }

    public void UnMarkObject(GameObject gameObject)
    {
        Renderer renderer;
        if (!gameObject.TryGetComponent(out renderer))
            renderer = gameObject.GetComponentInChildren<Renderer>();

        if (renderer != null)
            renderer.material.color = originalColor;
    }


    public void ReplaceData(Vector3 oldKey, Vector3 newKey, GameObject newValue)
    {
        foreach (var group in Data)
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

    public bool ContainsObjectAtPosition(Vector3 position, int[] layers)
    {
        foreach (var group in Data.Values)
        {
            if (group.ContainsKey(position))
            {
                var obj = group[position];
                foreach (var layer in layers)
                {
                    if (obj.layer == layer)
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
        foreach (var group in Data.Values)
        {
            if (group.ContainsKey(position))
                group.Remove(position);
        }
    }

    public void RemoveGroupFromData(int id)
    {
        if (Data.ContainsKey(id))
        {
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

    public GameObject Paint(ItemData item, Vector3 position)
    {
        GameObject newInstance = TileBase.Instantiate(item.Prefab, position, Quaternion.identity, GridLayout.transform);
        InsertToData(item, newInstance, position);
        return newInstance;
    }

    public void ReplaceItem(ItemData newItem, Vector3 position)
    {
        GameObject newInstance = TileBase.Instantiate(newItem.Prefab, position, Quaternion.identity, GridLayout.transform);
        Destroy(Data[newItem.Id][position]);
        Data[newItem.Id][position] = newInstance;
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

        _editorActions = new EditorInputActions();
        _keyPressed = _editorActions.Player.KeyPressed;
        _keyPressed.started += ctx => OnKeyPress();
        _keyPressed.canceled += ctx => OnKeyRelease();

        _mousePressed = _editorActions.Player.MousePressed;
        _mousePressed.started += ctx => OnMousePress();
        _mousePressed.canceled += ctx => OnMouseRelease();

        _defaultAction = new MoveAction();
        _actualAction = _defaultAction;

        Selected = new Dictionary<Vector3, (GameObject, bool)>();
        Data = new Dictionary<int, Dictionary<Vector3, GameObject>>();
    }

    private void Update()
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
            _actualAction.OnMouseDown(GetPressedMouseButton());
        }
    }

    private void OnMouseRelease()
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
    }

    private void OnKeyPress()
    {
        _isActionAllowed = true;
        _actualAction.OnKeyDown(Key.LeftShift);
    }

    private void OnKeyRelease()
    {
        _isActionAllowed = false;
        _actualAction.OnKeyUp();
    }
    #endregion
}
