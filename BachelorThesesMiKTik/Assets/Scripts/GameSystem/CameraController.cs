using Assets.Scripts.GameEditor;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour, IObjectController
{
    public bool IsEnabled {  get; private set; }
    public Transform FollowTransform {get; private set;}

    private GameManager manager;
    private Vector3 startPosition;
    private int followTransformId;

    public void Initialize(Transform transform)
    {
        FollowTransform = transform;
    }

    public void Play() {}

    public void Pause() {}

    public void Enter()
    { 
        SetFollowTransform();
        IsEnabled = true;
        startPosition = transform.position;
    }

    public void Exit()
    {
        IsEnabled = false;
        transform.position = startPosition;
    }

    private void Start()
    {
        manager = GameManager.Instance; 
        if(manager != null)
            manager.Camera = this;
    }

    public bool SetFollowTransform()
    {
        if (manager != null)
        {
            if (manager.ActivePlayers.Count != 0)
            {
                followTransformId = manager.ActivePlayers.Keys.First();
                FollowTransform = manager.ActivePlayers[followTransformId].transform;
                return true;
            }
        }

        IsEnabled = false;
        return false;
    }

    private bool CheckFollowTransform()
    {
        if (manager == null)
            return false;

        if (manager.ActivePlayers == null) 
            return false;

        if(manager.ActivePlayers.ContainsKey(followTransformId))
            return true;

        return SetFollowTransform();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (IsEnabled)
        {
            if (!CheckFollowTransform())
                return;

            if (FollowTransform == null)
                return;

            transform.position = new Vector3(FollowTransform.position.x, FollowTransform.position.y, transform.position.z);
        }
    }
}
