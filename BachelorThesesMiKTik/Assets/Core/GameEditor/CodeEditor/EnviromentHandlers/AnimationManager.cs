using Assets.Core.GameEditor.Attributes;
using Assets.Core.GameEditor.CodeEditor.EnviromentObjects;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Scripts.GameEditor.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentHandlers
{
    internal class AnimationManager : EnviromentObject
    {
        AnimationsManager animationsManager;
        public override bool SetInstance(GameObject instance)
        {
            animationsManager = AnimationsManager.Instance;
            if (animationsManager == null) 
            {
                return false;
            }
            return true;
        }

        [CodeEditorAttribute("This method will play all objects with animation of given name.", "(string nameOfAnimation)")]
        public void Play(string name)
        {
            if (!animationsManager.OnPlay(new List<string> { name }))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will play all animations setted to objects.")]
        public void PlayAll()
        {
            if (!animationsManager.OnPlay(new List<string>()))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will pause all objects with animation of given name.", "(string nameOfAnimation)")]
        public void Pause(string name)
        {
            if (!animationsManager.OnPause(new List<string> { name }))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will pause all animations setted to objects.")]
        public void PauseAll()
        {
            if (!animationsManager.OnPause(new List<string>()))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will reusume all objects with animation of given name.", "(string nameOfAnimation)")]
        public void Resume(string name)
        {
            if (!animationsManager.OnResume(new List<string> { name }))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will resume all animations setted to objects.")]
        public void ResumeAll()
        {
            if (!animationsManager.OnResume(new List<string>()))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will restart all objects with animation of given name.", "(string nameOfAnimation)")]
        public void Restart(string name)
        {
            if (!animationsManager.OnRestart(new List<string> { name }))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will restart all animations setted to objects.")]
        public void RestartAll()
        {
            if (!animationsManager.OnRestart(new List<string>()))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will stop all objects with animation of given name.", "(string nameOfAnimation)")]
        public void Stop(string name)
        {
            if (!animationsManager.OnStop(new List<string> { name }))
                throw new RuntimeException();
        }

        [CodeEditorAttribute("This method will stop all animations setted to objects.")]
        public void StopAll()
        {
            if (!animationsManager.OnStop(new List<string>()))
                throw new RuntimeException();
        }
    }
}
