using Assets.Core.GameEditor.DTOS.Assets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.Audio
{
    public class AudioEditorController : MonoBehaviour
    {
        [SerializeField] Slider Priority;
        [SerializeField] Slider StereoPan;
        [SerializeField] Slider Pitch;
        [SerializeField] Slider Volume;
        [SerializeField] Toggle LoopToggle;

        public delegate void SaveHandler(AudioSourceDTO audioSource);
        public event SaveHandler OnSave;

        public void Initialize(AudioSourceDTO source)
        {
            Priority.value = source.Priority;
            StereoPan.value = source.StereoPan;
            Pitch.value = source.Pitch;
            Volume.value = source.Volume;
            LoopToggle.isOn = source.ShouldLoop;
        }

        public void OnSavePress()
        {
            var audioDTO = CreateAudioDTO();
            if (audioDTO == null)
                return;

            OnSave.Invoke(audioDTO);
        }

        private AudioSourceDTO CreateAudioDTO()
        {
            return new AudioSourceDTO("", "")
            {
                Priority = (int) Priority.value,
                StereoPan = StereoPan.value,
                Pitch = Pitch.value,
                Volume = Volume.value,
                ShouldLoop = LoopToggle.isOn
            };
        }
    }
}
