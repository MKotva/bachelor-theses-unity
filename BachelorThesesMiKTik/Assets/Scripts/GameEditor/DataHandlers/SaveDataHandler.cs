using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.Serializers;
using Assets.Scenes.GameEditor.Core.DTOS;
using Assets.Scripts.GameEditor.ItemView;
using SimpleFileBrowser;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;

public class SaveDataHandler : MonoBehaviour
{ 
    [SerializeField] public string DefaultPath;
    [SerializeField] public string DefaultSaveAsPath;

    private MapCanvas map;

    public void Awake()
    {
        map = MapCanvas.Instance;
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
        JSONSerializer.Serialize(GameDataSerializer.GetGameDTO(), path);
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

    private void OnSucces(string path)
    {
       SaveMap(path);
       map.OnEnable();
    }
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
