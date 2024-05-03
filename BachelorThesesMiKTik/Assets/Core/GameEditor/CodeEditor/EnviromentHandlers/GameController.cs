using Assets.Core.GameEditor.Attributes;
using Assets.Core.GameEditor.CodeEditor.EnviromentObjects;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Scripts.GameEditor;
using System.IO;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentHandlers
{
    public class GameController : EnviromentObject
    {
        private GameManager gameManager;

        public override bool SetInstance(GameObject instance)
        {
            gameManager = GameManager.Instance;
            if(gameManager == null) 
            {
                return false;            
            }

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

        [CodeEditorAttribute("")]
        public void LoadGame(string path) 
        {
            if(!File.Exists(path)) 
                throw new RuntimeException($"\"Exception in method \\\"LoadGame\\\"! Invalid path to file!");

            var task = LoadDataHandler.LoadMap(path);
            GameManager.Instance.RestartGame();
        }
    }
}
