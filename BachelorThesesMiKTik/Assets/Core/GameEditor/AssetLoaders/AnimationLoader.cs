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
        public static async Task SetAnimation(GameObject ob, List<AnimationFrameDTO> data, int xSize, int ySize)
        {
            if(ob.TryGetComponent<CustomAnimationController>(out var controller)) 
            {
                var animation = await LoadAnimation(data, xSize, ySize);
                controller.SetCustomAnimation(animation); //TODO: Set scale
            }
        }

        public static async Task<CustomAnimation> LoadAnimation(List<AnimationFrameDTO> data, int xSize, int ySize)
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
    }
}
