using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Scripts.GameEditor.Managers;
using UnityEngine;


namespace Assets.Scripts.GameEditor.ObjectInstancesController
{
    public class SpriteController : MonoBehaviour, IObjectController
    {
        public  SourceReference SourceReference { get; private set; }
        public SpriteRenderer spriteRendered;
        public bool WasCreatedFromCode = false;
        private SpriteRendererSnapshot snapshot;

        public void SetSprite(Sprite sprite, SourceReference source)
        {
            if (spriteRendered == null)
                SetRenderer();

            SourceReference = source;
            spriteRendered.sprite = sprite;
            SpriteLoader.SetScale(spriteRendered, SourceReference.XSize, SourceReference.YSize);
        }

        public void EditSprite(Sprite sprite)
        {
            if (spriteRendered == null)
                SetRenderer();

            if (SourceReference.XSize <= 0 || SourceReference.YSize <= 0)
            {
                SourceReference.XSize = 30;
                SourceReference.YSize = 30;
            }
            spriteRendered.sprite = sprite;
            SpriteLoader.SetScale(spriteRendered, SourceReference.XSize, SourceReference.YSize);
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

        public void SetImage(SourceReference source)
        {
            var instance = SpriteManager.Instance;
            if (instance != null)
            {
                if (instance.ContainsName(name))
                {
                    var sprite = instance.Sprites[name];
                    SetSprite(sprite, source);
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

            if (WasCreatedFromCode)
                RemoveController();
        }

        private void Awake()
        {
            if (spriteRendered == null)
                SetRenderer();

            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(SpriteController), this);
            }
        }

        private void OnDestroy()
        {
            RemoveController();
        }

        private void SetRenderer()
        {
            if (!TryGetComponent(out spriteRendered))
            {
                spriteRendered = gameObject.AddComponent<SpriteRenderer>();
            }
        }

        private void RemoveController()
        {
            var instance = SpriteManager.Instance;
            if (instance != null && SourceReference != null)
                instance.RemoveActiveController(SourceReference.Name, this);
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
            renderer.flipX = false;
            renderer.flipY = false;
        }
    }
}
