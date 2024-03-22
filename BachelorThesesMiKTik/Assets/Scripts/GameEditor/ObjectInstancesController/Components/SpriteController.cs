using Assets.Core.GameEditor.AssetLoaders;
using Assets.Scripts.GameEditor.Managers;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.GameEditor.ObjectInstancesController
{
    public class SpriteController : MonoBehaviour, IObjectController
    {
        private float x;
        private float y;
        private SpriteRenderer spriteRendered;
        private SpriteRendererSnapshot snapshot;

        public void SetSprite(Sprite sprite, float xSize = 30, float ySize = 30)
        {
            x = xSize;
            y = ySize;

            spriteRendered.sprite = sprite;
            SpriteLoader.SetScale(spriteRendered, xSize, ySize);
        }

        public void EditSprite(Sprite sprite)
        {
            if (x <= 0 || y <= 0)
            {
                x = 30;
                y = 30;
            }
            spriteRendered.sprite = sprite;
            SpriteLoader.SetScale(spriteRendered, x, y);
        }

        public void DeleteSprite()
        {
            if (spriteRendered != null)
                spriteRendered.sprite = null;
        }

        public void ChangeColor(float r, float g, float b)
        {
            if (spriteRendered != null)
                spriteRendered.color = new Color(r, g, b);
        }

        public void SetImage(string source)
        {
            SetImage(source, 0, 0);
        }

        public void SetImage(string name, float xSize, float ySize)
        {
            var instance = SpriteManager.Instance;
            if (instance != null)
            {
                if (instance.ContainsName(name))
                {
                    var sprite = instance.Sprites[name];
                    SetSprite(sprite, xSize, ySize);
                }
            }
        }

        public void Play() { }

        public void Pause() { }

        public void Enter()
        {
            snapshot = new SpriteRendererSnapshot(spriteRendered);
        }

        public void Exit()
        {
            if (snapshot != null)
                snapshot.Set(spriteRendered);
            snapshot = null;
        }

        private void Awake()
        {
            if (!TryGetComponent(out spriteRendered))
            {
                spriteRendered = gameObject.AddComponent<SpriteRenderer>();
            }

            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(SpriteController), this);

            }
        }
    }

    class SpriteRendererSnapshot
    {
        public Sprite OriginalSprite;
        public Color OriginalColor;
        public Vector3 OriginalScale;

        public SpriteRendererSnapshot(SpriteRenderer renderer)
        {
            OriginalSprite = renderer.sprite;
            OriginalColor = renderer.color;
            OriginalScale = renderer.transform.localScale;
        }

        public void Set(SpriteRenderer renderer)
        {
            renderer.sprite = OriginalSprite;
            renderer.color = OriginalColor;
            renderer.transform.localScale = OriginalScale;
        }
    }
}
