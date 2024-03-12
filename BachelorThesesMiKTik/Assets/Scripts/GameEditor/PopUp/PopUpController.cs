using UnityEngine;

public class PopUpController : MonoBehaviour
{
    public delegate void ExitHandler();
    public event ExitHandler exitHandler;

    public virtual void OnExitClick()
    {
        if(exitHandler != null)
            exitHandler.Invoke();
        Destroy(gameObject);
    }

    public virtual void OnShowHelp()
    {

    }
}
