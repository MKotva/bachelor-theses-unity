using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Scripts.GameEditor.Entiti;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.GameEditor.Audio
{
    public class AudioController : MonoBehaviour, IObjectController
    {
        [SerializeField] AudioMixerGroup MixerGroup;

        public AudioSourceDTO AudioSourceDTO { get; set; }
        private AudioSource audioSource;

        private void Start()
        {
            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(AudioController), this);
            }

            if (!TryGetComponent(out audioSource))
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

        public void SetAudioClip(string name, bool shouldPlay = true)
        {
            var instance = AudioManager.Instance;
            if (instance == null)
                return;

            if (!instance.ContainsName(name))
                return;

            EditAudio(instance.AudioData[name]);
            audioSource.clip = instance.AudioClips[name];

            if(shouldPlay)
                audioSource.Play();
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

        public void RemoveClip()
        {
            audioSource.Stop();
            audioSource.clip = null;
            AudioSourceDTO = null;
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

        public void Play()
        {
        }
        public void Enter()
        {
         
        }

        public void Exit()
        {
         
        }
    }
}
