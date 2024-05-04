using UnityEngine;

public class PopUpController : MonoBehaviour
{
    public delegate void ExitHandler();
    public event ExitHandler onExit;

    protected virtual void Awake()
    {
        EditorCanvas.Instance.OnDisable();
    }

    public virtual void OnExitClick()
    {
        EditorCanvas.Instance.OnEnable();

        if(onExit != null)
            onExit.Invoke();
        Destroy(gameObject);
    }

    public virtual void OnShowHelp()
    {

    }
}
