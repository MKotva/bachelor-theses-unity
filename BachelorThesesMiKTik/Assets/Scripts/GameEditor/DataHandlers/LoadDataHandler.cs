using Assets.Core.GameEditor.Serializers;
using SimpleFileBrowser;
using System.Threading.Tasks;
using UnityEngine;

public class LoadDataHandler : MonoBehaviour
{
    [SerializeField] string DefaultPath;

    private MapCanvas map;

    public void OnLoad()
    {
        map.OnDisable();
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Files", ".json", ".txt"));
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        FileBrowser.ShowLoadDialog((paths) => { OnSucces(paths[0]); }, OnFail, FileBrowser.PickMode.Files, false, DefaultPath, null, "Select Map", "Select");
    }

    public async Task LoadMap(string path)
    {
        if (JSONSerializer.Deserialize(path, out var gameData))
        {
            await GameDataSerializer.SetGame(gameData);
        }
    }

    #region PRIVATE
    private void Awake()
    {
        map = MapCanvas.Instance;
    }

    private void OnSucces(string path)
    {
        map.OnEnable();
        var task = LoadMap(path);
    }

    private void OnFail()
    {
        map.OnEnable();
    }
    #endregion
}
