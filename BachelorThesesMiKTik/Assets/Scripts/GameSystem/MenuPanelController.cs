using Assets.Scripts.GameEditor.PopUp;
using Assets.Scripts.GameEditor;
using UnityEngine.SceneManagement;
using UnityEngine;
using Assets.Scripts.JumpSystem;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace Assets.Scripts.GameSystem
{
    class MenuPanelController : MonoBehaviour
    {
        [SerializeField] public AudioMixerGroup MixerGroup;
        [SerializeField] public GameObject ConfirmationPrefab;
        [SerializeField] public LoadDataHandler LoadDataHandler;
        [SerializeField] public Canvas MenuCanvas;
        [SerializeField] public GameObject Slider;

        private Slider slider;

        /// <summary>
        /// Displayes Exit confirmation prefab and connects handler for exist confirmation.
        /// </summary>
        public void OnExitClick()
        {
            var instance = Instantiate(ConfirmationPrefab, MenuCanvas.transform);
            var controller = instance.GetComponent<ExitConfirmationPopUp>();
            controller.ShowMessage("Exit confrimation", "Are you sure, that you want to leave?");
            controller.OnExit += ResultHandler;
        }

        /// <summary>
        /// Invokes resume of played game.
        /// </summary>
        public void OnResumeClick()
        {
            var instance = GameManager.Instance;
            if (instance != null)
                instance.StartGame();

            MenuCanvas.gameObject.SetActive(false);
        }

        /// <summary>
        /// Invokes restart of played game.
        /// </summary>
        public async void OnRestartClick()
        {
            if(Loader.Data != null)
                await LoadDataHandler.Load(Loader.Data);
            MenuCanvas.gameObject.SetActive(false);
        }

        /// <summary>
        /// Changes overall volume of game.
        /// </summary>
        /// <param name="volume"></param>
        public void OnVolumeChange(float volume)
        {
            MixerGroup.audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
        }

        private void Awake()
        {
            slider = Slider.GetComponent<Slider>();
            slider.onValueChanged.AddListener(OnVolumeChange);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                var instance = GameManager.Instance;
                if (instance != null)
                    instance.PauseGame();

                MenuCanvas.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Loads Main Menu Scene.
        /// </summary>
        /// <param name="result"></param>
        private void ResultHandler(bool result)
        {
            if (result)
            {
                SceneManager.LoadScene("MenuScene");
            }
        }
    }
}
