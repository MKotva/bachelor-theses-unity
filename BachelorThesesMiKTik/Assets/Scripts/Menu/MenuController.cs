using Assets.Core.GameEditor.Serializers;
using Assets.Scripts.JumpSystem;
using SimpleFileBrowser;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Menu
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] string DefaultPath;
        [SerializeField] TMP_Text OutputConsole;

        public void LoadInterpret()
        {
            LoadGame();
        }

        public void LoadEditor()
        {
            SceneManager.LoadScene("GameEditorScene");
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        /// <summary>
        /// Creates new file browser window on default path ./Maps.
        /// </summary>
        public void LoadGame()
        {
            FileBrowser.SetFilters(true, new FileBrowser.Filter("Files", ".json", ".txt"));
            FileBrowser.SetDefaultFilter(".json");
            FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
            FileBrowser.AddQuickLink("Users", "C:\\Users", null);
            FileBrowser.ShowLoadDialog((paths) => { OnSucces(paths[0]); }, OnFail, FileBrowser.PickMode.Files, false, DefaultPath, null, "Select Map", "Select");
        }

        /// <summary>
        /// Loads game data from the file on selected path and sets it as actual
        /// game status. (Loads the map)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void SetInterpret(string path)
        {
            if (JSONSerializer.Deserialize(path, out var gameData))
            {
                Loader.Path = path;
                Loader.Data = gameData;
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                OutputManager.Instance.ShowMessage("Unable to load selected file! Data might be corrupted.");
            }
        }

        /// <summary>
        /// This method handles the File Browser select click. Recieves path to file
        /// from file browser and then call LoadMap with this path.
        /// </summary>
        /// <param name="path"></param>
        private void OnSucces(string path)
        {
            SetInterpret(path);
        }

        /// <summary>
        /// This method handles FileBrowser fail to select a path.
        /// </summary>
        private void OnFail() {}
    }
}
