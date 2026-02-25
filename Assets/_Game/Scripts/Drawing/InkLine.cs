using System.Collections.Generic;
using UnityEngine;

namespace TriInkTrack.Drawing
{
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(EdgeCollider2D))]
    public class InkLine : MonoBehaviour
    {
        private static readonly Vector2[] EmptyPoints = new Vector2[0];

        [Header("References")]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private EdgeCollider2D edgeCollider;

        [Header("Visual")]
        [SerializeField] private float lineWidth = 0.1f;
        [SerializeField] private Color lineColor = new Color(0f, 0.75f, 1f, 1f);

        [Header("Limits")]
        [SerializeField] private int maxPoints = 60;

        private readonly List<Vector3> worldPoints = new List<Vector3>(64);
        private readonly List<Vector2> localPoints = new List<Vector2>(64);
        private bool isLocked;

        public int PointCount => worldPoints.Count;
        public int MaxPoints => maxPoints;

        private void Awake()
        {
            EnsureReferences();
            ConfigureLineRenderer();
            ResetLine();
        }

        public void ResetLine()
        {
            isLocked = false;
            worldPoints.Clear();
            localPoints.Clear();

            lineRenderer.positionCount = 0;
            edgeCollider.enabled = true;
            edgeCollider.points = EmptyPoints;
        }

        public bool AddPoint(Vector3 worldPos)
        {
            if (isLocked || worldPoints.Count >= maxPoints)
            {
                return false;
            }

            worldPos.z = 0f;
            worldPoints.Add(worldPos);
            int newIndex = worldPoints.Count - 1;
            lineRenderer.positionCount = worldPoints.Count;
            lineRenderer.SetPosition(newIndex, worldPos);
            SyncCollider();
            return true;
        }

        public void LockLine()
        {
            isLocked = true;
        }

        private void SyncCollider()
        {
            if (worldPoints.Count < 2)
            {
                edgeCollider.points = EmptyPoints;
                return;
            }

            localPoints.Clear();
            int count = worldPoints.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 local3 = transform.InverseTransformPoint(worldPoints[i]);
                localPoints.Add(new Vector2(local3.x, local3.y));
            }

            edgeCollider.SetPoints(localPoints);
        }

        private void EnsureReferences()
        {
            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
            }

            if (edgeCollider == null)
            {
                edgeCollider = GetComponent<EdgeCollider2D>();
            }
        }

        private void ConfigureLineRenderer()
        {
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.positionCount = 0;
            lineRenderer.numCapVertices = 4;
            lineRenderer.numCornerVertices = 2;
            lineRenderer.alignment = LineAlignment.View;

            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;

            if (lineRenderer.sharedMaterial == null)
            {
                Shader shader = Shader.Find("Sprites/Default");
                if (shader != null)
                {
                    lineRenderer.sharedMaterial = new Material(shader);
                }
            }
        }
    }
}
