using UnityEngine;

public class PopUpController : MonoBehaviour
{
    public delegate void ExitHandler();
    public event ExitHandler onExit;

    private void Start()
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
