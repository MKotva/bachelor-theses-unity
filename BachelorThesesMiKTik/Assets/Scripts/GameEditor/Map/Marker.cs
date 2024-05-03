using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.GameEditor.Map
{
    public class Marker : MonoBehaviour
    {
        [SerializeField] public GameObject MarkerPrefab;
        [SerializeField] public GameObject MarkerDotPrefab;
        [SerializeField] private Transform Parent;

        public Color originalColor = Color.white;

        public GameObject CreateMarkAtPosition(Vector2 position)
        {
            return TileBase.Instantiate(MarkerPrefab, position, Quaternion.identity, Parent);
        }

        public List<GameObject> CreateMarkAtPosition(List<Vector2> positions)
        {
            var markers = new List<GameObject>();
            foreach (Vector3 position in positions)
                markers.Add(CreateMarkAtPosition(position));

            return markers;
        }

        public GameObject CreateMarkAtPosition(GameObject markerPrefab, Vector2 position)
        {
            return TileBase.Instantiate(markerPrefab, position, Quaternion.identity, Parent);
        }

        public List<GameObject> CreateMarkAtPosition(GameObject markerPrefab, List<Vector2> positions)
        {
            var markers = new List<GameObject>();
            foreach (Vector2 position in positions)
                markers.Add(CreateMarkAtPosition(markerPrefab, position));

            return markers;
        }

        public GameObject CreateMarkAtPosition(GameObject markerPrefab, Vector2 position, Color color)
        {
            var marker = TileBase.Instantiate(markerPrefab, position, Quaternion.identity, Parent);
            marker.GetComponent<Renderer>().material.color = color;
            return marker;
        }

        public void DestroyMark(GameObject marker)
        {
            Destroy(marker);
        }

        public void MarkObject(GameObject gameObject)
        {
            Renderer renderer;
            if (!gameObject.TryGetComponent(out renderer))
                renderer = gameObject.GetComponentInChildren<Renderer>();

            if (renderer != null)
                renderer.material.color = new Color(2f, 0f, 0f, 0.7f);
        }

        public void UnMarkObject(GameObject gameObject)
        {
            Renderer renderer;
            if (!gameObject.TryGetComponent(out renderer))
                renderer = gameObject.GetComponentInChildren<Renderer>();

            if (renderer != null)
                renderer.material.color = originalColor;
        }
    }
}
