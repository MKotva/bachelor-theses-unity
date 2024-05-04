using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.OutputControllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.Audio
{
    public class AudioLoaderController : PopUpController
    {
        [SerializeField] Slider Priority;
        [SerializeField] Slider StereoPan;
        [SerializeField] Slider Pitch;
        [SerializeField] Slider Volume;

        [SerializeField] Toggle LoopToggle;
        [SerializeField] TMP_InputField URLField;
        [SerializeField] TMP_InputField NameField;
        [SerializeField] OutputController OutputConsole;

        [SerializeField] AudioSource AudioSource;

        public delegate void SaveHandler(SourceReference source);
        public event SaveHandler OnSave;

        private AudioSourceDTO actualAudioSource;

        public void Initialize(AudioSourceDTO source)
        {
            Priority.value = source.Priority;
            StereoPan.value = source.StereoPan;
            Pitch.value = source.Pitch;
            Volume.value = source.Volume;
            LoopToggle.isOn = source.ShouldLoop;
            URLField.text = source.URL;
            NameField.text = source.Name;
        }

        public async void OnPlayPress()
        {
            var audioDTO = CreateAudioDTO();
            if (audioDTO == null)
                return;

            actualAudioSource = audioDTO;
            if (!( await AudioLoader.SetAudioClip(AudioSource, actualAudioSource) ))
            {
                OutputConsole.ShowMessage("Invalid URL");
            }
            AudioSource.Play();
        }

        public void OnStopPress()
        {
            if (AudioSource.isPlaying)
                AudioSource.Stop();
        }

        public async void OnSavePress()
        {
            if (actualAudioSource == null)
            {
                if (!CheckName(NameField.text))
                {
                    return;
                }

                var audioDTO = CreateAudioDTO();
                if (audioDTO == null)
                    return;


                actualAudioSource = audioDTO;
            }

            var instance = AudioManager.Instance;
            if (instance != null)
            {
                if (await instance.AddAudioClip(actualAudioSource))
                    InvokeListeners();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            AudioSource = GetComponent<AudioSource>();
        }

        private AudioSourceDTO CreateAudioDTO()
        {
            return new AudioSourceDTO(NameField.text, URLField.text)
            {
                Priority = (int) Priority.value,
                StereoPan = StereoPan.value,
                Pitch = Pitch.value,
                Volume = Volume.value,
                ShouldLoop = LoopToggle.isOn
            };
        }

        private bool CheckName(string name)
        {
            if (name == "")
            {
                OutputConsole.ShowMessage("Empty name!");
                return false;
            }

            if (AudioManager.Instance.ContainsName(name))
            {
                OutputConsole.ShowMessage("Name is already used!");
                return false;
            }
            return true;
        }

        private void InvokeListeners() 
        {
            if (OnSave != null)
                OnSave.Invoke(new SourceReference(actualAudioSource.Name, SourceType.Sound));
        }
    }
}
