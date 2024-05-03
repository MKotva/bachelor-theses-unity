using Assets.Core.GameEditor;
using Assets.Core.GameEditor.Components.Colliders;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.Colliders
{
    public class PolygonCollider : ColliderController
    {
        [SerializeField] GameObject PositionPanelPrefab;
        [SerializeField] GameObject ContentView;

        private List<PositionSoucePanelController> sourcePanels;

        public void OnAdd()
        {
            AddPanel();
        }

        public override ColliderComponent GetComponent()
        {
            var lines = GetLines();
            //if (!CheckIfDoNotIntersect(lines))
            //{
            //    ErrorOutputManager.Instance.ShowMessage("You cant have collider with crossing lines!", "ObjectCreate");
            //    return null;
            //}

            var points = new List<Vector2>();
            foreach (var line in lines) 
            {
                points.Add(line.Item1);
            }

            return new PolygonColliderComponent(points, counterScale);
        }

        public override void SetComponent(ColliderComponent data)
        {
            if(data is PolygonColliderComponent)
            {
                if (sourcePanels == null)
                    Awake();

                ClearLines();
                foreach(var point in ((PolygonColliderComponent)data).Points)
                {
                    var panelController = AddPanel();
                    panelController.SetPoint(point);
                }

                ChangePreview();
            }
        }

        protected override void Awake()
        {
            if (sourcePanels == null)
            {
                base.Awake();
                sourcePanels = new List<PositionSoucePanelController>();
                for (int i = 0; i < 3; i++)
                {
                    AddPanel();
                }
            }
        }

        private PositionSoucePanelController AddPanel()
        {
            var sourcePanel = Instantiate(PositionPanelPrefab, ContentView.transform);
            sourcePanel.GetComponent<SourcePanelController>().onDestroyClick += OnExit;
            
            var panelController = sourcePanel.GetComponent<PositionSoucePanelController>();
            panelController.OnEdit += ChangePreview;

            sourcePanels.Add(panelController);
            return panelController;
        }

        private void ChangePreview()
        {
            var lines = GetLines();
            DrawLines(lines);
        }

        private bool CheckIfDoNotIntersect(List<(Vector2, Vector2)> lines)
        {
            for(int i = 0; i < lines.Count; i++)
            {
                var line1 = lines[i];
                for(int j = i + 1; j < lines.Count; j++)
                {
                    var line2 = lines[j];
                    if (MathHelper.CheckLineIntersection(line1.Item1, line1.Item2, line2.Item1, line2.Item2))
                        return false;
                }
            }
            return true;
        }

        private List<(Vector2, Vector2)> GetLines()
        {
            var previous = Vector2.zero;
            var lines = new List<(Vector2, Vector2)>();
            for (int i = 0; i < sourcePanels.Count; i++)
            {
                if (i != 0)
                {
                    lines.Add((previous, sourcePanels[i].GetPoint()));
                }
                previous = sourcePanels[i].GetPoint();
            }

            lines.Add((previous, sourcePanels[0].GetPoint()));
            return lines;
        }

        private void ClearLines()
        {
            foreach (var line in sourcePanels)
            {
                Destroy(line.gameObject);
            }
            sourcePanels.Clear();
        }

        private void OnExit(int id)
        {
            for (int i = 0; i < sourcePanels.Count; i++)
            {
                if (sourcePanels[i].gameObject.GetInstanceID() == id)
                {
                    if (sourcePanels.Count == 3)
                    {
                        OutputManager.Instance.ShowMessage("You can not have polynom with less than three vertices!");
                    }
                    else
                    {
                        Destroy(sourcePanels[i].gameObject);
                        sourcePanels.RemoveAt(i);
                    }
                    return;
                }
            }
        }
    }
}
