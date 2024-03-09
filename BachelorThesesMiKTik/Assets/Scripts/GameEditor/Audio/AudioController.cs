using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Scripts.GameEditor.Managers;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.GameEditor.Audio
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField] AudioMixerGroup MixerGroup;

        public AudioSourceDTO AudioSourceDTO { get; set; }
        private AudioSource audioSource;

        private void Start()
        {
            if(!TryGetComponent(out audioSource))
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.hideFlags = HideFlags.HideInInspector;
            }
        }

        private void Update()
        {
            if (audioSource == null)
            {
                return;
            }
        }

        public async Task<bool> SetAudioClip(AudioSourceDTO audioSourceDTO, bool shouldPlay = true)
        {
            var result = await AudioLoader.SetAudioClip(audioSource, audioSourceDTO);
            if (result)
            {
                AudioManager.Instance.AddActivePlayer(audioSourceDTO.Name, this);
                AudioSourceDTO = audioSourceDTO;

                if(shouldPlay)
                    audioSource.Play();
            }
            return result;
        }

        public void EditAudio(AudioSourceDTO sourceEdit)
        {
            AudioSourceDTO = sourceEdit;
            audioSource.pitch = sourceEdit.Pitch;
            audioSource.panStereo = sourceEdit.StereoPan;
            audioSource.priority = sourceEdit.Priority;
            audioSource.volume = sourceEdit.Volume;
            audioSource.loop = sourceEdit.ShouldLoop;
        }

        public void Pause()
        {
            audioSource.Pause();
        }

        public void Resume()
        {
            audioSource.UnPause();
        }

        public void ResetClip()
        {
            audioSource.Play();
        }

        private void OnDestroy()
        {
            var instance = AudioManager.Instance;
            if(instance != null)
                instance.RemoveActivePlayer(AudioSourceDTO.Name);
        }
    }
}
