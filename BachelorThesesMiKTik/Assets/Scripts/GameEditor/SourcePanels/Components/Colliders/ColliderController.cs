using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.PopUp.CodeEditor;
using Assets.Core.GameEditor.Components.Colliders;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.Colliders
{
    public class ColliderController : MonoBehaviour
    {
        [SerializeField] GameObject LinePrefab;
        [SerializeField] GameObject PointPrefab;
        [SerializeField] GameObject CirclePrefab;
        [SerializeField] GameObject PreviewPanel;
        [SerializeField] internal Image PreviewImage;

        private List<GameObject> instances;
        internal Vector2 counterScale;

        public virtual ColliderComponent GetComponent() { return null; }
        public virtual void SetComponent(ColliderComponent component) { }

        public void ChangePreview(SourceReference source)
        {
            if (source == null)
            {
                PreviewImage.sprite = null;
                return;
            }

            if (source.Type == SourceType.Image)
                PreviewImage.sprite = SpriteManager.Instance.GetSprite(source.Name);
            else if (source.Type == SourceType.Animation)
                PreviewImage.sprite = AnimationsManager.Instance.GetAnimationPreview(source.Name);
            else
                return;

            CorrectSize(source);

            var transform = PreviewImage.GetComponent<RectTransform>();
            transform.sizeDelta = new Vector2(source.XSize, source.YSize);
            ScalePreview();
        }


        public void DrawLines(List<(Vector2, Vector2)> points)
        {
            ClearInstances();

            var scale = PreviewPanel.transform.localScale;
            foreach (var point in points)
            {
                if (point.Item1 != point.Item2)
                {
                    instances.Add(MakePoint(point.Item1));
                    instances.Add(MakePoint(point.Item2));
                    instances.Add(MakeLine(point.Item1, point.Item2, scale));
                }
                else
                {
                    instances.Add(MakePoint(point.Item1));
                }
            }

            ScalePreview();
        }

        public void DrawCircle(Vector2 center, float radius)
        {
            ClearInstances();
            var newCircle = Instantiate(CirclePrefab, PreviewPanel.GetComponent<RectTransform>());
            var circleTransform = newCircle.GetComponent<RectTransform>();
            circleTransform.localPosition = new Vector2(center.x, center.y);
            circleTransform.sizeDelta = new Vector2(radius * 2, radius * 2);

            instances.Add(newCircle);
            ScalePreview();
        }

        protected virtual void Awake()
        {
            instances = new List<GameObject>() { };

            var previewManager = PreviewManager.Instance;
            if (previewManager != null)
                previewManager.onPreviewChange += ChangePreview;
        }

        #region PRIVATE
        private void OnEnable()
        {
            var previewManager = PreviewManager.Instance;
            if (previewManager != null)
            {
                var preview = previewManager.GetPreview();
                if(preview != null)
                    ChangePreview(preview);
            }
        }

        //Reference: https://forum.unity.com/threads/any-good-way-to-draw-lines-between-ui-elements.317902/
        private GameObject MakeLine(Vector2 aPoint, Vector2 bPoint, Vector2 previewScale)
        {
            var newLine = Instantiate(LinePrefab, PreviewPanel.GetComponent<RectTransform>());
            RectTransform rect = newLine.GetComponent<RectTransform>();
            rect.localScale = Vector3.one;

            rect.localPosition = ( aPoint + bPoint ) / 2;
            Vector3 dif = aPoint - bPoint;
            rect.sizeDelta = new Vector3(dif.magnitude, 5);
            rect.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
            return newLine;
        }

        private GameObject MakePoint(Vector2 position)
        {
            var scale = PreviewImage.transform.localScale;
            var point = Instantiate(PointPrefab, PreviewPanel.GetComponent<RectTransform>());
            point.transform.localPosition = new Vector2(position.x * scale.x, position.y * scale.y);
            return point;
        }

        private void ScalePreview()
        {
            var previewTransform = PreviewPanel.GetComponent<RectTransform>();
            var previewSize = previewTransform.sizeDelta;
            
            var maxDiff = 0f;
            for(int i = 0; i < instances.Count; i++) 
            {
                GetMaxDifference(instances[i], previewSize, maxDiff, out maxDiff);
            }

            GetMaxDifference(PreviewImage.gameObject, previewSize, maxDiff, out maxDiff);

            if (maxDiff > 0f)
            {
                var halfPreviewSize = previewSize / 2;
                var newScale = ( halfPreviewSize / new Vector2((halfPreviewSize.x) + (maxDiff), (halfPreviewSize.y) + (maxDiff)));
                PreviewPanel.transform.localScale = new Vector3(newScale.x, newScale.y, 1);
            }
        }

        private void GetMaxDifference(GameObject instance, Vector2 previewSize, float defaultMax, out float maxDiff)
        {
            maxDiff = defaultMax;

            var center = instance.transform.localPosition;
            var transform = instance.GetComponent<RectTransform>();
            var scaledSizes = transform.localScale * transform.sizeDelta;
            var x = scaledSizes.x / 2;
            var y = scaledSizes.y / 2;

            SetDifference(( x + center.x ), previewSize.x / 2, maxDiff, out maxDiff);
            SetDifference(Mathf.Abs(x - center.x), previewSize.x / 2, maxDiff, out maxDiff);
            SetDifference(( y + center.y ), previewSize.y / 2, maxDiff, out maxDiff);
            SetDifference(Mathf.Abs(y - center.y), previewSize.y / 2, maxDiff, out maxDiff);
        }

        private void SetDifference(float newValue, float oldValue, float actualMax, out float max)
        {
            max = actualMax;
            var diff = newValue - oldValue;
            if (diff > max)
                max = diff;
        }

        private void CorrectSize(SourceReference reference)
        {
            if (PreviewImage.sprite != null)
            {
                var pixelsPerUnit = PreviewImage.sprite.pixelsPerUnit;
                var xScale = PreviewImage.sprite.rect.width / reference.XSize;
                var yScale = PreviewImage.sprite.rect.height / reference.YSize;
                counterScale = new Vector2(xScale / pixelsPerUnit, yScale / pixelsPerUnit);
            }
            else
            {
                counterScale = new Vector2(1 / 100, 1 / 100);
            }
        }

        private void ClearInstances()
        {
            PreviewPanel.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            foreach (var instance in instances)
                Destroy(instance);
            instances.Clear();
        }
        #endregion
    }
}
