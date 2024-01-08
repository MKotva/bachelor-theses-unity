using Assets.Core.GameEditor.Animation;
using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor.Controllers;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.Components
{
    [Serializable]
    public class ImageComponentDTO : ComponentDTO
    {
        public float XSize;
        public float YSize;
        public SourceDTO Data;

        public ImageComponentDTO() 
        {
            XSize = 0;
            YSize = 0;
            Data = null;
        }

        public ImageComponentDTO(float xSize, float ySize, SourceDTO data) 
        {
            ComponentName = "Image/Animation";
            XSize = xSize;
            YSize = ySize;
            Data = data;
        }

        public override async Task Set(ItemData item)
        {
            var rendered = GetOrAddComponent<SpriteRenderer>(item.Prefab);
            switch (Data.Type)
            {
                case SourceType.Image:
                    await SpriteLoader.SetSprite(item.Prefab, Data.URL, XSize, YSize); //TODO: Change values to some real stuff, this is just placeholder.
                    break;
                case SourceType.Animation:
                    await AnimationLoader.SetAnimation(item.Prefab, ( (AnimationSourceDTO) Data ).AnimationData, XSize, YSize); //TODO: Change values to some real stuff, this is just placeholder.
                    rendered.sprite = item.Prefab.GetComponent<CustomAnimationController>().GetAnimationPreview();
                    break;
                case SourceType.Video:
                    //TODO: Video loading
                    break;
            }

            item.ShownImage = rendered.sprite;
        }
    }
}
