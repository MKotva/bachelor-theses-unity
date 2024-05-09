using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.GameEditor.Audio
{
    public class AudioController : MonoBehaviour, IObjectController
    {
        public string Name { get; set; }
        public AudioSourceDTO AudioSourceDTO { get; set; }
        public bool IsManualyPaused { get; set; }

        public bool WasCreatedFromCode = false;

        private AudioSource audioSource;
        private AudioSourceDTO snapshot;

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

            Name = name;
        }

        public void EditAudio(AudioSourceDTO sourceEdit)
        {
            if (audioSource == null)
                InitAudioSource();

            AudioSourceDTO = sourceEdit;
            audioSource.pitch = sourceEdit.Pitch;
            audioSource.panStereo = sourceEdit.StereoPan;
            audioSource.priority = sourceEdit.Priority;
            audioSource.volume = sourceEdit.Volume;
            audioSource.loop = sourceEdit.ShouldLoop;
        }

        public void Play() 
        {
            if(!IsManualyPaused)
                ResetClip();
        }

        public void Pause()
        {
            if (audioSource == null)
                return;

            audioSource.Pause();
            IsManualyPaused = false;
        }

        public void Resume()
        {
            if (audioSource == null)
                return;

            audioSource.UnPause();
            IsManualyPaused = false;
        }

        public void ResetClip()
        {
            if (audioSource == null)
                return;

            audioSource.Play();
            IsManualyPaused = false;
        }

        public void RemoveClip()
        {
            audioSource.Stop();
            audioSource.clip = null;
            AudioSourceDTO = null;
            IsManualyPaused = false;
        }

        public void StopClip()
        {
            if (audioSource == null)
                return;

            audioSource.Stop();
            IsManualyPaused = false;
        }

        public void StopAfterFinishingLoop()
        {
            if (audioSource == null)
                return;

            audioSource.loop = false;
        }

        public bool HasFinished()
        {
            if (audioSource == null)
                return true;

            return !audioSource.isPlaying;
        }
        public void Enter() 
        {
            snapshot = AudioSourceDTO;
        }

        public void Exit()
        {
            IsManualyPaused = false;

            if (WasCreatedFromCode)
            {
                RemoveController();
                audioSource.clip = null;
                AudioSourceDTO = null;
            }
            Restore();
        }

        #region PRIVATE
        private void Start()
        {
            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(AudioController), this);
            }

            if (audioSource == null)
                InitAudioSource();
        }

        private void OnDestroy()
        {
            RemoveController();
        }

        private void RemoveController()
        {
            var instance = AudioManager.Instance;
            if (instance != null && AudioSourceDTO != null)
                instance.RemoveActiveController(AudioSourceDTO.Name, this);
        }

        private void Restore()
        {
            var instance = AudioManager.Instance;
            if (snapshot != null && instance != null)
            {

                if (AudioSourceDTO != null)
                    if (snapshot.Name == AudioSourceDTO.Name)
                        return;

                RemoveController();
                if (instance.ContainsName(snapshot.Name))
                {
                    instance.SetAudioClip(this, new SourceReference(snapshot.Name, SourceType.Sound), true);
                    StopClip();
                }
            }
        }

        private void InitAudioSource()
        {
            if (!TryGetComponent(out audioSource))
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.hideFlags = HideFlags.HideInInspector;

                var instance = AudioManager.Instance;
                if (instance != null && instance.MixerGroup)
                    audioSource.outputAudioMixerGroup = instance.MixerGroup;
            }
        }
        #endregion
    }
}
