using UnityEngine;

public class PopUpController : MonoBehaviour
{
    public delegate void ExitHandler();
    public event ExitHandler onExit;

    private void Start()
    {
        MapCanvas.Instance.OnDisable();
    }

    public virtual void OnExitClick()
    {
        MapCanvas.Instance.OnEnable();

        if(onExit != null)
            onExit.Invoke();
        Destroy(gameObject);
    }

    public virtual void OnShowHelp()
    {

    }
}
