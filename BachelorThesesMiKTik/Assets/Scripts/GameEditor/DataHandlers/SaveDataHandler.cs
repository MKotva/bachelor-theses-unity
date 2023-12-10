using Assets.Scenes.GameEditor.Core.DTOS;
using Mono.Cecil.Cil;
using SimpleFileBrowser;
using System.IO;
using UnityEngine;

public class SaveDataHandler : MonoBehaviour
{ 
    [SerializeField] public string DefaultPath;
    [SerializeField] public string DefaultSaveAsPath;

    private Editor editor;

    public void Awake()
    {
        editor = Editor.Instance;
    }

    public void OnSaveClick()
    {
        var filepath = GetUniqueFilePath(DefaultPath);
        editor.SaveMap(filepath);
    }

    public void OnSaveAsClick()
    {
        SaveToPath();
    }
    private void SaveToPath()
    {
        editor.OnDisable();
        FileBrowser.ShowSaveDialog((paths) => { OnSucces(paths[0]);}, OnFail, FileBrowser.PickMode.Files, false, DefaultSaveAsPath, "Map.json", "Save As", "Save");
    }

    private void OnSucces(string path)
    {
       editor.SaveMap(path);
       editor.OnEnable();
    }
    private void OnFail()
    {
       editor.OnEnable();
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
