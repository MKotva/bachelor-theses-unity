using Assets.Core.GameEditor.Attributes;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Core.GameEditor.Enums;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Scripts.GameEditor.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentObjects
{
    public class Background : EnviromentObject
    {
        public BackgroundController BackgroundController { get; set; }
        
        private List<SourceDTO> newBackground;

        public Background() 
        {
            BackgroundController = BackgroundController.Instance;
        }
        public override void SetInstance(GameObject instance) { }

        #region NewBackgroundCreate

        [CodeEditorAttribute("Creates new background concept. You have to create new concept so" +
            "you can append new image/animation layers. For applying concept use SetBackground().")]
        public void CreateBackgroundConcept()
        {
            newBackground = new List<SourceDTO>();
        }

        [CodeEditorAttribute("Appends animation with given name (if exists) as layer to background concept, created by CreateBackgroundConcept()," +
            "with default scaling 1920x1080", "(string imageName)")]
        public void AppendAnimationLayer(string animationName)
        {
            AppendImageLayer(animationName, 1920, 1080);
        }

        [CodeEditorAttribute("Appends animation with given name (if exists) as layer to background concept, created by CreateBackgroundConcept()," +
            "with scaling X and Y.", "(string animationName, num xSize, num ySize)")]
        public void AppendAnimationLayer(string name, float xSize, float ySize)
        {
            if(newBackground == null)
                throw new RuntimeException("Exception in method \"SetBackGround\"! You must create new background by calling \"CreateBackgroundConcept\"");

            if (!AnimationsManager.Instance.ContainsName(name))
                throw new RuntimeException($"Exception in method \"AppendAnimationLayer\"! There is no animation with name: {name}");

            newBackground.Add(new SourceDTO(name, SourceType.Animation, xSize, ySize));
        }

        [CodeEditorAttribute("Appends image with given name (if exists) as layer to background concept, created by CreateBackgroundConcept()," +
            "with default scaling 1920x1080", "(string imageName)")]
        public void AppendImageLayer(string imageName)
        {
            AppendImageLayer(imageName, 1920, 1080);
        }

        [CodeEditorAttribute("Appends image with given name (if exists) as layer to background concept, created by CreateBackgroundConcept()," +
            "with given scaling X and Y.", "(string imageName, num xSize, num ySize)")]
        public void AppendImageLayer(string name, float xSize, float ySize)
        {
            if (newBackground == null)
                throw new RuntimeException("Exception in method \"SetBackGround\"! You must create new background by calling \"CreateBackgroundConcept\"");

            if (!SpriteManager.Instance.ContainsName(name))
                throw new RuntimeException($"Exception in method \"AppendLayerImage\"! There is no sprite with name: {name}");

            newBackground.Add(new SourceDTO(name, SourceType.Image, xSize, ySize));
        }

        [CodeEditorAttribute("Sets created background concept as new background.")]
        public void SetBackground()
        {
            if (newBackground == null)
                throw new RuntimeException("Exception in method \"SetBackGround\"! You must create new background by calling \"CreateBackgroundConcept\"");
            if (newBackground.Count == 0)
                throw new RuntimeException("Exception in method \"SetBackGround\"! You must add some frames by calling \"AppendLayerImage\" or \"AppendAnimationLayer\"");

            BackgroundController.SetBackground(newBackground);
        }
        #endregion

        #region ActualBackgroundEdit

        [CodeEditorAttribute("Appends animation with given name as layer to actual background with default scaling 1920x1080.", "(string animationName)")]
        public void AppendAnimationLayerToActual(string animationName)
        {
            AppendAnimationLayerToActual(animationName, 1920, 1080);
        }

        [CodeEditorAttribute("Appends animation with given name as layer to actual background with scaling X and Y.", "(string animationName, num xSize, num ySize)")]
        public void AppendAnimationLayerToActual(string animationName, float xSize, float ySize)
        {
            if (!AnimationsManager.Instance.ContainsName(animationName))
                throw new RuntimeException($"Exception in method \"AppendAnimationLayer\"! There is no animation with name: {animationName}");

            BackgroundController.AppendLayer(new SourceDTO(animationName, SourceType.Animation, xSize, ySize));
        }


        [CodeEditorAttribute("Sets animation with name to existing background layer on index (num layer), with scaling X and Y. " +
            "Indexes goes from back to front", "(num layer, string animationName, num xSize, num ySize)")]
        public void SetAnimationLayer(int layer, string name, float xSize, float ySize)
        {
            if (!AnimationsManager.Instance.ContainsName(name))
                throw new RuntimeException($"Exception in method \"AppendAnimationLayer\"! There is no animation with name: {name}");

            if (BackgroundController.BackgroundLayers.Count <= layer || layer < 0)
                throw new RuntimeException($"Ivalid layer id {layer}! Index out of range.");

            BackgroundController.SetLayer(new SourceDTO(name, SourceType.Animation, xSize, ySize), layer);
        }


        [CodeEditorAttribute("Appends image with given name as layer to actual background with default scaling 1920x1080.", "(string imageName)")]
        public void AppendImageLayerToActual(string name)
        {
            AppendImageLayerToActual(name, 1920, 1080);
        }

        [CodeEditorAttribute("Appends image with given name as layer to actual background with scaling X and Y.", "(string imageName, num xSize, num ySize)")]
        public void AppendImageLayerToActual(string name, float xSize, float ySize)
        {
            if (!SpriteManager.Instance.ContainsName(name))
                throw new RuntimeException($"Exception in method \"AppendLayerImage\"! There is no sprite with name: {name}");

            BackgroundController.AppendLayer(new SourceDTO(name, SourceType.Image, xSize, ySize));
        }

        [CodeEditorAttribute("Sets image with given name to existing background layer on index (num layer), with scaling X and Y. " +
            "Indexes goes from back to front", "(string imageName, num layer, num xSize, num ySize)")]
        public void SetImageLayer(string name, int layer, float xSize, float ySize)
        {
            if (!AnimationsManager.Instance.ContainsName(name))
                throw new RuntimeException($"Exception in method \"AppendAnimationLayer\"! There is no animation with name: {name}");

            if (BackgroundController.BackgroundLayers.Count <= layer || layer < 0)
                throw new RuntimeException($"Ivalid layer id {layer}! Index out of range.");

            BackgroundController.SetLayer(new SourceDTO(name, SourceType.Image, xSize, ySize), layer);
        }

        [CodeEditorAttribute("Gets actual background layer count.", "returns num")]
        public int GetBackgroundLayerCount()
        {
            return BackgroundController.BackgroundLayers.Count;
        }

        [CodeEditorAttribute("Removes actual background.")]
        public void ClearBackground()
        {
            BackgroundController.ClearBackground();
        }

        [CodeEditorAttribute("Removes background layer from actual background.", "(num layer)")]
        public void RemoveImageLayer(int layer)
        {
            if (BackgroundController.BackgroundLayers.Count <= layer || layer < 0)
                throw new RuntimeException($"Ivalid layer id {layer}! Index out of range.");

            BackgroundController.RemoveLayer(layer);
        }
        #endregion
    }
}
