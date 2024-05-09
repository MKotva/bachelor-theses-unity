using Assets.Core.GameEditor.Attributes;
using Assets.Core.GameEditor.CodeEditor.EnviromentObjects;
using Assets.Core.GameEditor.Serializers;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Scenes.GameEditor.Core.DTOS;
using Assets.Scripts.GameEditor;
using Assets.Scripts.GameEditor.Managers;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentHandlers
{
    public class GameController : EnviromentObject
    {
        private GameManager gameManager;
        private EditorCanvas canvasManager;
        private PrototypeManager itemManager;
        private string defaultPath;

        public override bool SetInstance(GameObject instance)
        {
            gameManager = GameManager.Instance;
            if (gameManager == null)
            {
                return false;
            }

            canvasManager = EditorCanvas.Instance;
            itemManager = PrototypeManager.Instance;
            defaultPath = "C:\\Users\\mkotv\\Documents\\bachelor-theses-unity\\BachelorThesesMiKTik\\Maps\\";

            return true;
        }

        [CodeEditorAttribute("Shows panel with anouncement of finishing game. Player can choose to restart game or quit.")]
        public void ShowGameWin()
        {
            gameManager.ShowGameSucces();
        }

        [CodeEditorAttribute("Shows panel with anouncement of failing game. Player can choose to restart game or quit.")]
        public void ShowGameFail()
        {
            gameManager.ShowGameFail();
        }

        [CodeEditorAttribute("Load new game from Maps forlder with provided absolute path" +
        "Example of path: \"C:\\\\Users\\Jumpking\\Map1.json\"", "(string absolutePath)")]
        public void LoadGame(string path)
        { 
            if (gameManager != null)
            {
                if (!File.Exists(path))
                    throw new RuntimeException($"\"Exception in method \\\"LoadGame\\\"! Invalid path to file!");

                var task = LoadLevelFromGame(path);
            }
        }

        [CodeEditorAttribute("Load new game from Maps forlder with provided relative path" +
            "Example of path: \"Jumpking\\Map1.json\"", "(string relativePath)")]
        public void LoadGameFromMaps(string relativePath)
        {
            if (gameManager != null)
            {
                var path = defaultPath + relativePath;
                if (!File.Exists(path))
                    throw new RuntimeException($"\"Exception in method \\\"LoadGame\\\"! Invalid path to file!");

                var task = LoadLevelFromGame(path);
            }
        }

        [CodeEditorAttribute("This method will check for any active object of given name in game scene"
            + "returns true, if exists", "(string prototypeName)")]
        public bool ContainsActiveObject(string name)
        {
            if(canvasManager != null && itemManager != null)
            {
                int id = 0;
                if (!itemManager.TryFindIdByName(name, out id))
                {
                    return false;
                }

                if(!canvasManager.Data.ContainsKey(id))
                {
                    return false;
                }

                foreach (var ob in canvasManager.Data[id].Values)
                {
                    if (ob.activeSelf == true)
                        return true;
                }
            }
            return false;
        }

        [CodeEditorAttribute("This method will check for selected amount of active object of given name in game scene"
            + "returns true, if exists. Otherwise false", "(string prototypeName, num amount)")]

        public bool ContainsNumberOfAtiveObjects(string name, float num)
        {
            int count = (int)num;
            if (canvasManager != null && itemManager != null)
            {
                int id = 0;
                if (!itemManager.TryFindIdByName(name, out id))
                {
                    return false;
                }

                if (!canvasManager.Data.ContainsKey(id))
                {
                    return false;
                }

                var founded = 0;
                foreach (var ob in canvasManager.Data[id].Values)
                {
                    if (ob.activeSelf == true)
                    {
                        if (founded < count)
                        {
                            founded++;
                        }
                        else if (founded == count)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private async Task<bool> LoadLevelFromGame(string path)
        {
            GameDataDTO gameDataDTO = null;
            if (!JSONSerializer.Deserialize(path, out gameDataDTO))
            {
                throw new RuntimeException($"\"Exception in method \\\"LoadGame\\\"! Invalid JSON file!");
            }

            var playmode = gameManager.IsInPlayMode;
            if (playmode)
            {
                gameManager.DisablePlayMode();
            }
            gameManager.Clear();

            await GameDataSerializer.Deserialize(gameDataDTO);
            if (playmode)
            {
                gameManager.DisplayPlayMode();
                gameManager.IsInPlayMode = true;
            }
            else
            {
                gameManager.EnterGame();
                gameManager.StartGame();
            }
            gameManager.LoadedIngame = true;
            return true;
        }
    }
}
