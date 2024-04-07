using Assets.Scripts.GameEditor;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ErrorOutputManager : Singleton<ErrorOutputManager>
{
    //[SerializeField] TextMeshProUGUI TextView;

    public delegate void ShowMessageAction(string message);
    public delegate void DisposeMessageAction();
    public string Message { get; set; }

    private Queue<(string, string)> messages;
    private Task performingTask;
    private bool isPerforming;

    private Dictionary<string, Dictionary<string, ShowMessageAction>> onShowListeners;
    private Dictionary<string, Dictionary<string, ShowMessageAction>> onAddMessageListeners;
    private Dictionary<string, DisposeMessageAction> onDisposeMessageListeners;

    /// <summary>
    /// Adds listener to UpdateText, if you specify authorName, then you will
    /// recieve messages only from that author.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="eventHandler"></param>
    /// <param name="authorName"></param>
    public void AddOnShowListener(string name, ShowMessageAction eventHandler, string authorName = "")
    {
        if (!onShowListeners.ContainsKey(name))
        {
            onShowListeners.Add(name, new Dictionary<string, ShowMessageAction> { { authorName, eventHandler } });
        }
        else
        {
            if (!onShowListeners[name].ContainsKey(authorName))
                onShowListeners[name].Add(authorName, eventHandler);
        }
    }

    /// <summary>
    /// Adds listener to AddMessage, if you specify authorName, then you will
    /// recieve messages only from that author.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="eventHandler"></param>
    /// <param name="authorName"></param>
    public void AddOnAddListener(string name, ShowMessageAction eventHandler, string authorName = "")
    {
        if (!onAddMessageListeners.ContainsKey(name))
            onAddMessageListeners.Add(name, new Dictionary<string, ShowMessageAction> { { authorName, eventHandler } });
        else
        {
            if (!onAddMessageListeners[name].ContainsKey(authorName))
                onAddMessageListeners[name].Add(authorName, eventHandler);
        }
    }
    public void RemoveListener(string name) 
    {
        if (name == null)
            return;

        if(onShowListeners.ContainsKey(name))
            onShowListeners.Remove(name);
        if(onAddMessageListeners.ContainsKey(name))
            onAddMessageListeners.Remove(name);
    }

    public void AddDisposeListerer(string name, DisposeMessageAction disposer)
    {
        if(!onDisposeMessageListeners.ContainsKey(name))
        {
            onDisposeMessageListeners.Add(name, disposer);
        }
    }

    public void RemoveDisposeListener(string name)
    {
        if (onDisposeMessageListeners.ContainsKey(name))
        {
            onDisposeMessageListeners.Remove(name);
        }
    }

    public void ShowMessage(string message, string authorName = "")
    {
        messages.Enqueue((authorName, message));
        InvokeListeners(onAddMessageListeners, authorName, message);
    }

    public async Task ClearText()
    {
        foreach(var disposer  in onDisposeMessageListeners.Values)
            disposer.Invoke();
        await Task.Delay(1000);
    }

    #region PRIVATE
    // Start is called before the first frame update
    protected override void Awake()
    {
        onShowListeners = new Dictionary<string, Dictionary<string, ShowMessageAction>>();
        onAddMessageListeners = new Dictionary<string, Dictionary<string, ShowMessageAction>>();
        onDisposeMessageListeners = new Dictionary<string, DisposeMessageAction>();
        messages = new Queue<(string, string)>();
        base.Awake();
    }

    /// <summary>
    /// If there is any pending messages, update method will show then in info console for 5s.
    /// </summary>
    private void Update()
    {
        if (messages == null)
            messages = new Queue<(string, string)>();

        if (messages.Count > 0 && !isPerforming)
        {
            isPerforming = true;
            var message = messages.Dequeue();
            performingTask = UpdateText(message);
        }
        else if (performingTask != null)
        {
            if (performingTask.IsCompleted)
                isPerforming = false;
        }
    }

    //TODO: Create locks !!! Can be called from paralel threads!!

    /// <summary>
    /// Displays message in info panel for 5s 
    /// </summary>
    /// <param name="infoText"></param>
    /// <returns></returns>
    private async Task UpdateText((string, string) infoText)
    {
        InvokeListeners(onShowListeners, infoText.Item1, infoText.Item2);
        await Task.Delay(10000);
        await ClearText();
    }

    private void InvokeListeners(Dictionary<string, Dictionary<string, ShowMessageAction>> listeners, string authorName, string message)
    {
        foreach(var listener in listeners) 
        {
            foreach (var listenMethod in listener.Value)
            {
                if (listenMethod.Key == authorName || listenMethod.Key == "")
                    listenMethod.Value.Invoke(message);
            }
        }
    }
    #endregion
}
