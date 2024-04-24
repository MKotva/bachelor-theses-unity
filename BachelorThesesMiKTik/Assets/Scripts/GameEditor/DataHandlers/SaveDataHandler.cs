using Assets.Core.GameEditor.Serializers;
using SimpleFileBrowser;
using System.IO;
using UnityEngine;

public class SaveDataHandler : MonoBehaviour
{ 
    [SerializeField] public string DefaultPath;
    [SerializeField] public string DefaultSaveAsPath;

    private EditorCanvas map;

    public void Start()
    {
        map = EditorCanvas.Instance;
    }

    /// <summary>
    /// Handles save button click. Finds new unique path in DefaultPath(./Maps) and then
    /// saves map into JSON.
    /// </summary>
    public void OnSaveClick()
    {
        var filepath = GetUniqueFilePath(DefaultPath);
        SaveMap(filepath);
    }

    /// <summary>
    /// Creates "Save file browser window" (from unity package) which handles whole save procces.
    /// </summary>
    public void OnSaveAsClick()
    {
        ShowDialogWindow();
    }

    /// <summary>
    /// Gets GameDTO object which represents actual game status and calls JSON serialization.
    /// </summary>
    /// <param name="path"></param>
    public void SaveMap(string path)
    {
        JSONSerializer.Serialize(GameDataSerializer.Serialize(), path);
    }

    #region PRIVATE
    /// <summary>
    /// Creates "Save dialog window" and sets OnSucces(If selected path is valid) and OnFail(otherwise).
    /// </summary>
    private void ShowDialogWindow()
    {
        map.OnDisable();
        FileBrowser.ShowSaveDialog((paths) => { OnSucces(paths[0]);}, OnFail, FileBrowser.PickMode.Files, false, DefaultSaveAsPath, "Map.json", "Save As", "Save");
    }

    /// <summary>
    /// This method handles the File Browser save click. Recieves path to file
    /// from file browser and then call SaveMap with this path.
    /// </summary>
    /// <param name="path"></param>
    private void OnSucces(string path)
    {
       SaveMap(path);
       map.OnEnable();
    }

    /// <summary>
    /// This method handles FileBrowser fail to select a path.
    /// </summary>
    private void OnFail()
    {
       map.OnEnable();
    }

    /// <summary>
    /// Extracts directory and file name, and adjusts name to be unique.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private string GetUniqueFilePath(string path)
    {
        string directoryPath = Path.GetDirectoryName(path);
        string extension = Path.GetExtension(path);
        string name = Path.GetFileNameWithoutExtension(path);

        for (int i = 1; ; ++i)
        {
            if (!File.Exists(path))
                return path;

            path = Path.Combine(directoryPath, name + "-" + i + extension);
        }
    }

    #endregion
}
