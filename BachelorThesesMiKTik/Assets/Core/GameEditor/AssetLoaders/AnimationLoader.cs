using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS;
using Assets.Scripts.GameEditor.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

namespace Assets.Core.GameEditor.Animation
{
    public static class AnimationLoader
    {
        public static async Task SetAnimation(GameObject ob, List<AnimationFrameDTO> data, uint xSize = 0, uint ySize = 0)
        {
            if(ob.TryGetComponent<CustomAnimationController>(out var controller)) 
            {
                var animation = await LoadAnimation(data);
                controller.SetCustomAnimation(animation);

                if (xSize > 0 && ySize > 0)
                {
                    Scale(ob.GetComponent<SpriteRenderer>(), xSize, ySize);
                }
            }
        }

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

        private static void Scale(SpriteRenderer spriteRenderer, uint xSize, uint ySize)
        {
            var rect = spriteRenderer.sprite.rect;

            var xScale = 1 / ( rect.width / xSize );
            var yScale = 1 / ( rect.height / ySize );

            spriteRenderer.transform.localScale = new Vector3((float) xScale, (float) yScale, 1);
        }
    }
}
