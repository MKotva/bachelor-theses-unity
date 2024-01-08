using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS;
using Assets.Scripts.GameEditor.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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
        public static async Task SetAnimation(GameObject ob, List<AnimationFrameDTO> data, float xSize = 0, float ySize = 0, bool shouldLoop = true)
        {
            if(ob.TryGetComponent<CustomAnimationController>(out var controller)) 
            {
                var animation = await LoadAnimation(data);
                controller.SetCustomAnimation(animation, shouldLoop);

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
                Scale(ob.GetComponent<SpriteRenderer>(), xSize, ySize);
            }
        }


        /// <summary>
        /// Loads animation frames(Sprites) via SpriteLoader from a given url, stored in AnimationFrameDTO.
        /// Each frame has its own display time.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>New custom animation object, which is just DTO for CustomAnimationController</returns>
        public static async Task<CustomAnimation> LoadAnimation(List<AnimationFrameDTO> data)
        {
            var spriteTasks = new List<Task<Sprite>>();
            foreach (var frame in data) 
            {
                spriteTasks.Add(SpriteLoader.LoadSprite(frame.URL));
            }
            await Task.WhenAll(spriteTasks);


            var frames = new List<CustomAnimationFrame>();
            for(int i = 0; i < spriteTasks.Count; i++)
            {
                if (spriteTasks[i].Result != null)
                {
                    frames.Add(new CustomAnimationFrame(data[i].DisplayTime, spriteTasks[i].Result));
                }
                else
                {
                    return null;
                }        
            }
            return new CustomAnimation(frames);
        }


        /// <summary>
        /// Set sprite renderer to a given size;
        /// </summary>
        /// <param name="spriteRenderer"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        private static void Scale(SpriteRenderer spriteRenderer, float xSize, float ySize)
        {
            var rect = spriteRenderer.sprite.rect;

            var xScale = 1 / ( rect.width / xSize );
            var yScale = 1 / ( rect.height / ySize );

            spriteRenderer.transform.localScale = new Vector3(xScale, yScale, 1);
        }
    }
}
