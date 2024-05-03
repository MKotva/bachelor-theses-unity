using Assets.Scripts.GameEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Common
{
    public class RestartGamePanelController : MonoBehaviour
    {
        private GameManager gameManager;

        /// <summary>
        /// Handles restart button click by invoking GameManager Restart method.
        /// </summary>
        public void OnRestart()
        {
            if(gameManager != null)
            {
                gameManager.RestartGame();
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// Handles exit button click by invoking GameManager Exit method.
        /// </summary>
        public void OnExit()
        {
            if (gameManager != null)
            {
                if (gameManager.IsInPlayMode)
                {
                    gameManager.ExitPlayMode();
                }
                else
                {
                    gameManager.ExitGame();
                    SceneManager.LoadScene("MenuScene");
                }
            }
            Destroy(gameObject);
        }

        private void Awake()
        {
            gameManager = GameManager.Instance;
        }
    }
}
