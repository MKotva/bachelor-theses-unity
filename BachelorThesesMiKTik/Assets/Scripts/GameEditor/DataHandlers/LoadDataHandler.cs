using Assets.Core.GameEditor.Serializers;
using Assets.Scenes.GameEditor.Core.DTOS;
using SimpleFileBrowser;
using System.Threading.Tasks;
using UnityEngine;

public class LoadDataHandler : MonoBehaviour
{
    [SerializeField] string DefaultPath;
    [SerializeField] GameObject LoadingScreen;

    private EditorCanvas map;

    /// <summary>
    /// Creates new file browser window on default path ./Maps.
    /// </summary>
    public void OnLoad()
    {
        map.OnDisable();
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Files", ".json", ".txt"));
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        FileBrowser.ShowLoadDialog((paths) => { OnSucces(paths[0]); }, OnFail, FileBrowser.PickMode.Files, false, DefaultPath, null, "Select Map", "Select");
    }

    public async Task Load(GameDataDTO gameData)
    {
            LoadingScreen.SetActive(true);
            await GameDataSerializer.Deserialize(gameData);
            LoadingScreen.SetActive(false);
    }

    /// <summary>
    /// Loads game data from the file on selected path and sets it as actual
    /// game status. (Loads the map)
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async Task LoadMap(string path)
    {
        if (JSONSerializer.Deserialize(path, out var gameData))
        { 
            await GameDataSerializer.Deserialize(gameData);
        }
    }

    #region PRIVATE
    private void Start()
    {
        map = EditorCanvas.Instance;
    }

    /// <summary>
    /// This method handles the File Browser select click. Recieves path to file
    /// from file browser and then call LoadMap with this path.
    /// </summary>
    /// <param name="path"></param>
    private async void OnSucces(string path)
    {
        LoadingScreen.SetActive(true);
        await LoadMap(path);
        LoadingScreen.SetActive(false);
        
        map.OnEnable();
    }

    /// <summary>
    /// This method handles FileBrowser fail to select a path.
    /// </summary>
    private void OnFail()
    {
        map.OnEnable();
    }
    #endregion
}
