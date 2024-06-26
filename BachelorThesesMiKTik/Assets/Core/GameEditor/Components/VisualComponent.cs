﻿using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor.Managers;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.Components
{
    [Serializable]
    public class VisualComponent : CustomComponent
    {
        public SourceReference Data;

        public VisualComponent()
        {
            Data = null;
        }

        public VisualComponent(SourceReference data)
        {
            ComponentName = "Image/Animation";
            Data = data;
        }

        public override Task Initialize()
        {
            return Task.CompletedTask;
        }

        public override void Set(ItemData item)
        {
            if (Data.Type == SourceType.Image)
            {
                var manager = SpriteManager.Instance;
                if (manager == null)
                    return;

                if (manager.Sprites.ContainsKey(Data.Name))
                {
                    item.ShownImage = manager.Sprites[Data.Name];
                }
            }
            else
            {
                var manager = AnimationsManager.Instance;
                if (manager != null)
                {
                    item.ShownImage = manager.GetAnimationPreview(Data.Name);
                }
            }
        }

        public override void SetInstance(ItemData item, GameObject instance)
        {
            if (Data.Type == SourceType.Image)
            {
                var manager = SpriteManager.Instance;
                if (manager != null)
                {
                    manager.SetSprite(instance, Data);
                }

                if (manager.Sprites.ContainsKey(Data.Name))
                {
                    item.ShownImage = manager.Sprites[Data.Name];
                }
            }
            else
            {
                var manager = AnimationsManager.Instance;
                if (manager != null)
                {
                    manager.SetAnimation(instance, Data, true, false);
                }
            }
        }
    }
}
