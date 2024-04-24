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
        [SerializeField] public LoaderController LoaderController;
        [SerializeField] public Canvas MenuCanvas;
        [SerializeField] public GameObject Slider;

        private Slider slider;
        public void OnExitClick()
        {
            var instance = Instantiate(ConfirmationPrefab, GameManager.Instance.PopUpCanvas.transform);
            var controller = instance.GetComponent<ExitConfirmationPopUp>();
            controller.ShowMessage("Exit confrimation", "Are you sure, that you want to leave?");
            controller.OnExit += ResultHandler;
        }

        public void OnResumeClick()
        {
            var instance = GameManager.Instance;
            if (instance != null)
                instance.StartGame();

            MenuCanvas.gameObject.SetActive(false);
        }

        public async void OnRestartClick()
        {
            await LoaderController.Load();
            MenuCanvas.gameObject.SetActive(false);
        }

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

        private void ResultHandler(bool result)
        {
            if (result)
            {
                SceneManager.LoadScene("MenuScene");
            }
        }
    }
}
