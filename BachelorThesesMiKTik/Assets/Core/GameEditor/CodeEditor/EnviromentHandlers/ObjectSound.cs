using Assets.Core.GameEditor.Attributes;
using Assets.Core.GameEditor.CodeEditor.EnviromentObjects;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Core.GameEditor.Enums;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Scripts.GameEditor.Audio;
using Assets.Scripts.GameEditor.Managers;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentHandlers
{
    public class ObjectSound : EnviromentObject
    {
        AudioController audioController;
        AudioManager audioManager;

        public override bool SetInstance(GameObject instance)
        {
            if (!instance.TryGetComponent(out audioController))
            {
                audioController = instance.AddComponent<AudioController>();
                return false;
            }

            audioManager = AudioManager.Instance;
            if (audioManager == null)
                return false;

            return true;
        }

        [CodeEditorAttribute("Finds created audio clip by given name and sets it for this object.", 
            "(string nameOfAudioClip, bool playAfterSet)")]
        public void SetAudio(string name, bool playAfterSet) 
        {
            if(!audioManager.ContainsName(name)) 
                throw new RuntimeException($"\"Exception in method \\\"SetAudio\\\"!  There is no audio clip with name: {name}");

            var audioSource = audioController.AudioSourceDTO;
            if(audioSource != null ) 
            {
                audioManager.RemoveActiveController(audioSource.Name, audioController);
            }

            var sourceReference = new SourceReference(name, SourceType.Sound);
            audioManager.SetAudioClip(audioController, sourceReference, playAfterSet);
        }

        [CodeEditorAttribute("Plays setted audio clip if it is not already running, otherwise nothing happens.")]
        public void PlayClip()
        {
            audioController.Play();
        }

        [CodeEditorAttribute("Resumes paused audio clip on this object. If clip is not paused" +
            "nothing happens.")]
        public void ResumeClip()
        {
            audioController.Resume();
        }

        [CodeEditorAttribute("Pauses the actual playing audio clip.")]
        public void PauseClip()
        {
            audioController.Pause();
        }


        [CodeEditorAttribute("Stops the actual playing audio clip -> If you hit play after calling this method," +
            " clip will start from begin.")]
        public void StopClip()
        {
            audioController.StopClip();
        }

        [CodeEditorAttribute("Breakes the audio clip loop after finishing the cycle.")]
        public void StopLooping()
        {
            audioController.StopAfterFinishingLoop();
        }

        [CodeEditorAttribute("Restarts actual playing audio clip.")]
        public void RestartAnimation()
        {
            audioController.ResetClip();
        }

        [CodeEditorAttribute("Removes actual audio clip from object and sets image to empty.")]
        public void RemoveClip()
        {
            audioController.RemoveClip();
        }
    }
}
