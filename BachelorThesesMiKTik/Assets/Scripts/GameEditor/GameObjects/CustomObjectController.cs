using Assets.Core.GameEditor.Animation;
using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor.Controllers;
using UnityEngine;

namespace Assets.Scripts.GameEditor.GameObjects.Elements
{
    public class CustomObjectController : MonoBehaviour
    {
        [SerializeField] public AnimationsController AnimationController;
        [SerializeField] public SpriteRenderer SpriteRenderer;
        [SerializeField] public Rigidbody2D Rigidbody;
        [SerializeField] public BoxCollider2D BoxCollider;

        private RectTransform rectTransform;

        private void Awake()
        {
            TryGetComponent(out rectTransform);
            TryGetComponent(out AnimationController);
            TryGetComponent(out SpriteRenderer);
            TryGetComponent(out Rigidbody);
            TryGetComponent(out BoxCollider);

            EditorController.Instance.PlayModeEnter += ActivateObject;
            EditorController.Instance.PlayModePause += DeactiveObject;
            EditorController.Instance.PlayModeExit += DeactiveObject;
        }

        public void ActivateObject()
        {

        }

        public void DeactiveObject()
        {

        }

        public virtual void Destroy()
        {
            Destroy(this);
        }

        public virtual void SetSource(SourceDTO source, float xSize = 0, float ySize = 0, bool shouldLoop = true)
        {
            if (rectTransform == null)
                return;

            switch (source.Type)
            {
                case SourceType.Image:
                    var sTask = SpriteLoader.SetSprite(gameObject, source.URL, xSize, ySize);
                    break;
                case SourceType.Sound:
                    break;
                case SourceType.Animation:
                    var aTask = AnimationLoader.SetAnimation(gameObject, (AnimationSourceDTO)source, shouldLoop, true, xSize, ySize);
                    break;
                default:
                    break;
            }
        }
    }
}
