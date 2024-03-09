using Assets.Scripts.GameEditor;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class InfoPanelController : Singleton<InfoPanelController>
{
    [SerializeField] TextMeshProUGUI TextView;

    public delegate void MessageAction(string message);
    public string Message { get; set; }

    private Queue<(string, string)> messages;
    private Task performingTask;
    private bool isPerforming;

    private Dictionary<string, Dictionary<string, MessageAction>> onShowListeners;
    private Dictionary<string, Dictionary<string, MessageAction>> onAddMessageListeners;


    // Start is called before the first frame update
    protected override void Awake()
    {
        onShowListeners = new Dictionary<string, Dictionary<string, MessageAction>>();
        onAddMessageListeners = new Dictionary<string, Dictionary<string, MessageAction>>();
        messages = new Queue<(string, string)>();
        base.Awake();
    }

    /// <summary>
    /// If there is any pending messages, update method will show then in info console for 5s.
    /// </summary>
    private void Update()
    {
        if(messages == null)
            messages = new Queue<(string, string)>();

        if(messages.Count > 0 && !isPerforming) 
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
        TextView.text = infoText.Item2;
        await Task.Delay(5000);
        await ClearText();
    }

    /// <summary>
    /// Adds listener to UpdateText, if you specify authorName, then you will
    /// recieve messages only from that author.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="eventHandler"></param>
    /// <param name="authorName"></param>
    public void AddOnShowListener(string name, MessageAction eventHandler, string authorName = "")
    {
        if(!onShowListeners.ContainsKey(name))
            onShowListeners.Add(name, new Dictionary<string, MessageAction> {{authorName, eventHandler }});
        else
            onShowListeners[name].Add(authorName, eventHandler);
    }

    /// <summary>
    /// Adds listener to AddMessage, if you specify authorName, then you will
    /// recieve messages only from that author.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="eventHandler"></param>
    /// <param name="authorName"></param>
    public void AddOnAddListener(string name, MessageAction eventHandler, string authorName = "")
    {
        if (!onAddMessageListeners.ContainsKey(name))
            onAddMessageListeners.Add(name, new Dictionary<string, MessageAction> { { authorName, eventHandler } });
        else
        {
            if (!onAddMessageListeners[name].ContainsKey(authorName))
                onAddMessageListeners[name].Add(authorName, eventHandler);
        }
    }

    public void RemoveListener(string name) 
    {
        if(onShowListeners.ContainsKey(name))
            onShowListeners.Remove(name);
        if(onAddMessageListeners.ContainsKey(name))
            onAddMessageListeners.Remove(name);
    }

    public void ShowMessage(string message, string authorName = "")
    {
        messages.Enqueue((authorName, message));
        InvokeListeners(onAddMessageListeners, authorName, message);
    }

    public async Task ClearText()
    {
        TextView.text = string.Empty;
        await Task.Delay(1000);
    }

    private void InvokeListeners(Dictionary<string, Dictionary<string, MessageAction>> listeners, string authorName, string message)
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
}
