using Assets.Core.GameEditor.Attributes;
using Assets.Core.GameEditor.CodeEditor.EnviromentObjects;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Scripts.GameEditor.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentHandlers
{
    public class SoundManager : EnviromentObject
    {
        AudioManager soundManager;

        public override bool SetInstance(GameObject instance)
        {
            soundManager = AudioManager.Instance;
            if (soundManager == null)
            {
                return false;
            }
            return true;
        }

        [CodeEditorAttribute("This method will play all objects with audio clips of given name.", "(string nameOfAnimation)")]
        public void Play(string name)
        {
            if (!soundManager.OnPlay(new List<string> { name }))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will play all audio clips setted to objects.")]
        public void PlayAll()
        {
            if (!soundManager.OnPlay(new List<string>()))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will pause all objects with audio clips of given name.", "(string nameOfAnimation)")]
        public void Pause(string name)
        {
            if (!soundManager.OnPause(new List<string> { name }))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will pause all audio clips setted to objects.")]
        public void PauseAll()
        {
            if (!soundManager.OnPause(new List<string>()))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will reusume all objects with audio clips of given name.", "(string nameOfAnimation)")]
        public void Resume(string name)
        {
            if (!soundManager.OnResume(new List<string> { name }))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will resume all audio clips setted to objects.")]
        public void ResumeAll()
        {
            if (!soundManager.OnResume(new List<string>()))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will restart all objects with audio clips of given name.", "(string nameOfAnimation)")]
        public void Restart(string name)
        {
            if (!soundManager.OnRestart(new List<string> { name }))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will restart all audio clips setted to objects.")]
        public void RestartAll()
        {
            if (!soundManager.OnRestart(new List<string>()))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will stop all objects with audio clips of given name.", "(string nameOfAnimation)")]
        public void Stop(string name)
        {
            if (!soundManager.OnStop(new List<string> { name }))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will stop all audio clips setted to objects.")]
        public void StopAll()
        {
            if (!soundManager.OnStop(new List<string>()))
                throw new RuntimeException();
        }
    }
}
