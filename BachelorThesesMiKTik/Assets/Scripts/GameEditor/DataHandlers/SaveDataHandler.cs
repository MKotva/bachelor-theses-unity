using Assets.Scenes.GameEditor.Core.DTOS;
using Mono.Cecil.Cil;
using SimpleFileBrowser;
using System.IO;
using UnityEngine;

public class SaveDataHandler : MonoBehaviour
{
    public MapCanvasController MapController;
    public string DefaultPath;

    public void OnSaveClick()
    {
        var filepath = GetUniqueFilePath(DefaultPath);
        MapController.SaveMap(filepath);
    }

    public void OnSaveAsClick()
    {
        SaveToPath();
    }
    private void SaveToPath()
    {
        MapController.OnDisable();
        FileBrowser.ShowSaveDialog((paths) => { OnSucces(paths[0]);}, OnFail, FileBrowser.PickMode.Files, false, "C:\\", "Map.json", "Save As", "Save");
    }

    private void OnSucces(string path)
    {
        MapController.SaveMap(path);
        MapController.OnEnable();
    }
    private void OnFail()
    {
        MapController.OnEnable();
    }

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
}
