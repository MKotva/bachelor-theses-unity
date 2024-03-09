using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Scripts.GameEditor.Managers;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.Audio
{
    public class AudioLoaderController : MonoBehaviour
    {
        [SerializeField] Slider Priority;
        [SerializeField] Slider StereoPan;
        [SerializeField] Slider Pitch;
        [SerializeField] Slider Volume;

        [SerializeField] Toggle LoopToggle;
        [SerializeField] TMP_InputField URLField;
        [SerializeField] TMP_InputField NameField;
        [SerializeField] TMP_Text OutputConsole;

        [SerializeField] AudioSource AudioSource;

        public delegate Task<bool> SaveHandler(AudioSourceDTO audioSource);
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
                OutputConsole.text = "Invalid URL";
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
                var audioDTO = CreateAudioDTO();
                if (audioDTO == null)
                    return;

                actualAudioSource = audioDTO;
            }

            //TODO Check on this. Await on event invoke?
            if (!( await OnSave.Invoke(actualAudioSource) ))
            {
                OutputConsole.text = "Saving failed, invalid URL!";
            }
        }

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
        }

        private AudioSourceDTO CreateAudioDTO()
        {
            if (!CheckName(NameField.text))
            {
                return null;
            }

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
                OutputConsole.text = "Empty name!";
                return false;
            }

            if (AudioManager.Instance.ContainsName(name))
            {
                OutputConsole.text = "Name is already used!";
                return false;
            }
            return true;
        }
    }
}
