using Assets.Scenes.GameEditor.Core.DTOS;
using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadDataHandler : MonoBehaviour
{
    [SerializeField] string DefaultPath;

    private Editor editor;
    private void Awake()
    {
        editor = Editor.Instance;
    }

    public void OnLoad()
    {
        editor.OnDisable();
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Files", ".json", ".txt"));
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        FileBrowser.ShowLoadDialog((paths) => { OnSucces(paths[0]); }, OnFail, FileBrowser.PickMode.Files, false, DefaultPath, null, "Select Map", "Select");
    }

    private void OnSucces(string path)
    {
        editor.OnEnable();
        editor.LoadMap(path);
    }

    private void OnFail()
    {
        editor.OnEnable();
    }
}
