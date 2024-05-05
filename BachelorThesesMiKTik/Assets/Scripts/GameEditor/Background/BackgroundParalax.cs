using UnityEngine;

namespace Assets.Scripts.GameEditor.Background
{
    /// <summary>
    /// Reference: This code was obtained with background assets from Unity asset Store
    /// </summary>
    public class BackgroundParalax : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer image;

        public float speed = 0;
        float pos = 0;

        private void Update()
        {
            if (speed != 0 && image.sprite != null)
            {
                pos += speed;

                if (pos > 1.0F)
                    pos -= 1.0F;

                image.sprite.textureRect.Set(pos, 0, 1, 1);
            }
        }
    }
}
