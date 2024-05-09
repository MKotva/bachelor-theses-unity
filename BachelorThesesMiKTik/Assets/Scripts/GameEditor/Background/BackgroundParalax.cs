using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.Background
{
    /// <summary>
    /// Reference: This code was obtained with background assets from Unity asset Store
    /// </summary>
    public class BackgroundParalax : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer image;
        [SerializeField] private RawImage rawImage;

        public float speed = 0.005f;
        float pos = 0;

        private void Start () 
        {   
            //image.sprite.      
        }

        //private void Update()
        //{
        //    if (speed != 0 && image.sprite != null)
        //    {
        //        pos += speed;

        //        if (pos > 1.0F)
        //            pos -= 1.0F;

        //        rawImage.uvRect = new Rect(pos, 0, 1, 1);
        //    }
        //}
    }
}
