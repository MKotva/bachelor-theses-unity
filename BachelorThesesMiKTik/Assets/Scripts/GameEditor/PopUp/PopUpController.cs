using Assets.Scripts.Common;
using Assets.Scripts.GameEditor;
using UnityEngine;

public class PopUpController : MonoBehaviour
{
    [SerializeField] GameObject InfoPanel;
    [SerializeField] string Info;

    public delegate void ExitHandler();
    public event ExitHandler onExit;


    private GameObject instance;

    protected virtual void Awake()
    {
        EditorCanvas.Instance.IsDisabled = true;
    }

    public virtual void OnExitClick()
    {
        var instance = GameManager.Instance;
        if (instance != null)
        {   
            if(instance.PopUpCanvas.transform.childCount == 2)
                EditorCanvas.Instance.IsDisabled = false;
        }

        if (onExit != null)
            onExit.Invoke();
        Destroy(gameObject);
    }

    public void OnShowHelp()
    {
        if(instance == null)
        {
            instance = CreateInfoPanelInstace();
        }
    }

    private GameObject CreateInfoPanelInstace()
    {
        if (InfoPanel == null)
            return null;

        var instance = GameManager.Instance;
        if (instance == null)
            return null;

        if (instance.PopUpCanvas == null)
            return null;

        var infoPanel = Instantiate(InfoPanel, instance.PopUpCanvas.transform).GetComponent<PopUpInfoPanel>();
        infoPanel.onExit += OnInfoExit;
        infoPanel.text.text = Info;
        return infoPanel.gameObject;
    }

    private void OnInfoExit()
    {
        instance = null;
    }
}
