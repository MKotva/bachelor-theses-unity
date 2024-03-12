using Assets.Core.GameEditor.AnimationControllers;
using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS;
using Assets.Scripts.GameEditor.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Core.GameEditor.Animation
{
    public static class AnimationLoader
    {
        /// <summary>
        /// Loads CustomAnimation and scales sprite renderer to a given size or to the size of object. 
        /// Than the method assignes CustomAnimation to a CustomAnimationController if is present.
        /// </summary>
        /// <param name="ob">Object to be set.</param>
        /// <param name="data">Animation frames data.</param>
        /// <param name="xSize">X scale size</param>
        /// <param name="ySize">Y scale size</param>
        /// <param name="shouldLoop">Determines if animation should run just once or repeat</param>
        /// <returns></returns>
        public static async Task SetAnimation(GameObject ob, AnimationSourceDTO data, bool shouldLoop = true, bool animateOnAwake = false, float xSize = 0, float ySize = 0)
        {
            if (ob.TryGetComponent<AnimationsController>(out var controller))
            {
                var animation = await LoadAnimation(data);
                if (xSize == 0 || ySize == 0)
                {
                    if (ob.TryGetComponent(out RectTransform rect))
                    {
                        xSize = rect.rect.width;
                        ySize = rect.rect.height;
                    }
                    else
                    {
                        InfoPanelController.Instance.ShowMessage("Unable to set sprite to an object! Scale size is equal to Zero");
                        return;
                    }
                }

                controller.SetCustomAnimation(animation, shouldLoop, animateOnAwake, xSize, ySize);
            }
        }


        /// <summary>
        /// Loads animation frames(Sprites) via SpriteLoader from a given url, stored in AnimationFrameDTO.
        /// Each frame has its own display time.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>New custom animation object, which is just DTO for CustomAnimationController</returns>
        public static async Task<CustomAnimation> LoadAnimation(AnimationSourceDTO data)
        {
            var spriteTasks = new List<Task<Sprite>>();
            foreach (var frame in data.AnimationData)
            {
                spriteTasks.Add(SpriteLoader.LoadSprite(frame.URL));
            }
            await Task.WhenAll(spriteTasks);


            var frames = new List<CustomAnimationFrame>();
            for (int i = 0; i < spriteTasks.Count; i++)
            {
                if (spriteTasks[i].Result != null)
                {
                    frames.Add(new CustomAnimationFrame(data.AnimationData[i].DisplayTime, spriteTasks[i].Result));
                }
                else
                {
                    return null;
                }
            }
            return new CustomAnimation(frames);
        }
    }
}
