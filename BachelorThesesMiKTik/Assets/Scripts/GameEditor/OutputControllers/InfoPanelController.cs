using Assets.Scripts.GameEditor;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;

public class InfoPanelController : Singleton<InfoPanelController>
{
    public delegate void MessageAction();
    public static event MessageAction OnMessageAdded;
    public string Message { get; set; }

    private TextMeshProUGUI textView;
    private Queue<string> messages;
    private Task performingTask;
    private bool isPerforming;

    // Start is called before the first frame update
    private void Start()
    {
        textView = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        messages = new Queue<string>();
    }

    private void Update()
    {
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

    private async Task UpdateText(string infoText)
    {
        textView.text = infoText;
        await Task.Delay(5000);
        await ClearText();
    }

    public void AddListener(MessageAction eventHandler)
    {
        OnMessageAdded += eventHandler;
    }

    public void ShowMessage(string message)
    {
        messages.Enqueue(message);
        Message = message;
    }

    public async Task ClearText()
    {
        textView.text = string.Empty;
        await Task.Delay(1000);
    }
}
