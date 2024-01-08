using Assets.Core.GameEditor.Attributes;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.Enums;
using Assets.Core.SimpleCompiler.Exceptions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentObjects
{
    public class Background : EnviromentObject
    {
        public BackgroundController BackgroundController { get; set; }
        
        private List<SourceDTO> newBackground;
        private List<AnimationFrameDTO> animationData;

        public Background() 
        {
            BackgroundController = BackgroundController.Instance;
        }
        public override void SetInstance(GameObject instance) { }


        [CodeEditorAttribute("Creates new background concept. You have to create new concept so" +
            "you can append new image/animation layers. For applying concept use SetBackground().")]
        public void CreateBackgroundConcept()
        {
            newBackground = new List<SourceDTO>();
        }

        [CodeEditorAttribute("Creates new animation concept. You have to create new concept so" +
            "you can append new animation frames. For appending animation concept to background layers" +
            " use AppendAnimationLayer().")]
        public void CreateNewAnimationConcept()
        {
            animationData = new List<AnimationFrameDTO>();
        }

        [CodeEditorAttribute("Appends new animation frame to animation concept, created by CreateNewAnimationConcept()" +
            "Num displayTime sets for how long should be frame displayed." + 
            "String url is taken as path to image(.jpg//png). If you want to download image, just use the URL " +
            "ending with .jpg/.png. If you want to use image on your disk, it must be stored in " +
            "./Assest/StreamingAssets folder. In this case, set url as name of image with type (name(.jpg/.png))", "(num displayTime, string url)")]
        public void AppendAnimationFrame(float displayTime, string url)
        {
            if (animationData == null)
                throw new RuntimeException("Exception in method \"AppendAnimationFrame\"! You must create new animation by calling \"CreateNewAnimationConcept\"");
            animationData.Add(new AnimationFrameDTO(displayTime, url));
        }

        [CodeEditorAttribute("Appends new animation layer to background concept created by CreateBackgroundConcept()")]
        public void AppendAnimationLayer()
        {
            if (animationData == null)
                throw new RuntimeException("Exception in method \"AppendAnimationLayer\"! You must create new animation by calling \"CreateNewAnimationConcept\"");

            newBackground.Add((SourceDTO) new AnimationSourceDTO(SourceType.Animation, animationData));
        }

        [CodeEditorAttribute("Sets animation concept to existing background layer on index (num layer). " +
            "Indexes goes from back to front", "(num layer)")]
        public void SetAnimationLayer(int layer)
        {
            if (animationData == null)
                throw new RuntimeException("Exception in method \"SetAnimationLayer\"! You must create new animation by calling \"CreateNewAnimationConcept\"");
            if (animationData.Count == 0)
                throw new RuntimeException("Exception in method \"SetAnimationLayer\"! You must add some animation frames by calling \"AppendAnimationLayer\"");

            if (BackgroundController.BackgroundLayers.Count <= layer || layer < 0)
                throw new RuntimeException($"Ivalid layer id {layer}! Index out of range.");

            BackgroundController.SetLayer((SourceDTO) new AnimationSourceDTO(SourceType.Animation, animationData), layer);
        }

        [CodeEditorAttribute("Appends image layer to bacground concept, created by CreateBackgroundConcept()." +
            "String url is taken as path to image(.jpg//png). If you want to download image, just use the URL " +
            "ending with .jpg/.png. If you want to use image on your disk, it must be stored in " +
            "./Assest/StreamingAssets folder. In this case, set url as name of image with type (name(.jpg/.png))", "(string url)")]
        public void AppendLayerImage(string url)
        {
            if (newBackground == null)
                throw new RuntimeException("Exception in method \"AppendLayerImage\"! You must create new background by calling \"CreateBackgroundConcept\"");
            
            newBackground.Add(new SourceDTO(SourceType.Image, url));
        }

        [CodeEditorAttribute("Sets image to existing background layer on index (num layer). " +
            "Indexes goes from back to front", "(string url, num layer)")]
        public void SetImageLayer(string url, int layer)
        {
            if (BackgroundController.BackgroundLayers.Count <= layer || layer < 0)
                throw new RuntimeException($"Ivalid layer id {layer}! Index out of range.");

            BackgroundController.SetLayer(new SourceDTO(SourceType.Image, url), layer);
        }

        [CodeEditorAttribute("Sets created background concept as new background with default scale (1920x1080).")]
        public void SetBackground()
        {
            SetBackground(1920, 1080);
        }

        [CodeEditorAttribute("Sets created background concept as new background with give scale (num X, num Y).", "(num xSize, num ySize)")]
        public void SetBackground(float xSize, float ySize)
        {
            if (newBackground == null)
                throw new RuntimeException("Exception in method \"SetBackGround\"! You must create new background by calling \"CreateBackgroundConcept\"");
            if (newBackground.Count == 0)
                throw new RuntimeException("Exception in method \"SetBackGround\"! You must add some frames by calling \"AppendLayerImage\" or \"AppendAnimationLayer\"");

            var task = BackgroundController.SetBackground(newBackground, xSize, ySize);
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

        [CodeEditorAttribute("Appends animation concept (created via CreateNewAnimationConcept())" +
    " layer to actual background with default scaling 1920x1080.")]
        public void AppendAnimationLayerToActual()
        {
            AppendAnimationLayerToActual(1920, 1080);
        }

        [CodeEditorAttribute("Appends animation concept (created via CreateNewAnimationConcept())" +
            " layer to actual background with given scaling xSize, ySize.", "( num xSize, num ySize)")]
        public void AppendAnimationLayerToActual(float xSize, float ySize)
        {
            if (animationData == null)
                throw new RuntimeException("Exception in method \"SetAnimationLayer\"! You must create new animation by calling \"CreateNewAnimationConcept\"");
            if (animationData.Count == 0)
                throw new RuntimeException("Exception in method \"SetAnimationLayer\"! You must add some animation frames by calling \"AppendAnimationLayer\"");

            BackgroundController.AppendLayer(new AnimationSourceDTO(SourceType.Animation, animationData), xSize, ySize);
        }

        [CodeEditorAttribute("Appends image layer to actual background with default scaling 1920x1080" +
           "String url is taken as path to image(.jpg//png). If you want to download image, just use the URL " +
           "ending with .jpg/.png. If you want to use image on your disk, it must be stored in " +
           "./Assest/StreamingAssets folder. In this case, set url as name of image with type (name(.jpg/.png))", "(string url)")]
        public void AppendImageLayerToActual(string source)
        {
            AppendImageLayerToActual(source, 1920, 1080);
        }

        [CodeEditorAttribute("Appends image layer to actual background with given scaling xSize, ySize" +
            "String url is taken as path to image(.jpg//png). If you want to download image, just use the URL " +
            "ending with .jpg/.png. If you want to use image on your disk, it must be stored in " +
            "./Assest/StreamingAssets folder. In this case, set url as name of image with type (name(.jpg/.png))", "(string url, num xSize, num ySize)")]
        public void AppendImageLayerToActual(string source, float xSize, float ySize)
        {
            if (animationData == null)
                throw new RuntimeException("Exception in method \"SetAnimationLayer\"! You must create new animation by calling \"CreateNewAnimationConcept\"");
            if (animationData.Count == 0)
                throw new RuntimeException("Exception in method \"SetAnimationLayer\"! You must add some animation frames by calling \"AppendAnimationLayer\"");

            BackgroundController.AppendLayer(new SourceDTO(SourceType.Image, source), xSize, ySize);
        }

        [CodeEditorAttribute("Gets actual background layer count.", "returns num")]
        public int GetBackgroundLayerCount()
        {
            return BackgroundController.BackgroundLayers.Count;
        }
    }
}
