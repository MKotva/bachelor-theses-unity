using Assets.Scenes.GameEditor.Core.DTOS;
using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadDataHandler : MonoBehaviour
{

    public MapCanvasController MapController;

    public void OnLoad()
    {
        MapController.OnDisable();
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Files", ".json", ".txt"));
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        FileBrowser.ShowLoadDialog((paths) => { OnSucces(paths[0]); }, OnFail, FileBrowser.PickMode.Files, false, null, null, "Select Map", "Select");
    }

    private void OnSucces(string path)
    {
        MapController.OnEnable();
        MapController.LoadMap(path);
    }

    private void OnFail()
    {
        MapController.OnEnable();
    }
}
