using System.Collections.Generic;
using UnityEngine;

namespace TriInkTrack.Drawing
{
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(EdgeCollider2D))]
    public class InkLine : MonoBehaviour
    {
        private static readonly Vector2[] EmptyPoints = System.Array.Empty<Vector2>();

        [Header("References")]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private EdgeCollider2D edgeCollider;

        [Header("Visual")]
        [SerializeField] private float lineWidth = 0.1f;
        [SerializeField] private Color lineColor = new Color(0f, 0.75f, 1f, 1f);
        [SerializeField] private int roundCapVertices = 8;
        [SerializeField] private int roundCornerVertices = 6;

        [Header("Limits")]
        [SerializeField] private int maxPoints = 60;

        private readonly List<Vector3> worldPoints = new List<Vector3>(64);
        private readonly List<Vector2> localPoints = new List<Vector2>(64);
        private bool isLocked;

        public int PointCount => worldPoints.Count;
        public int MaxPoints => maxPoints;
        public bool IsAtMaxPoints => worldPoints.Count >= maxPoints;

        public void SetMaxPoints(int value)
        {
            maxPoints = Mathf.Max(2, value);
        }

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
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            lineRenderer.positionCount = 0;
            ClearCollider();
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
                ClearCollider();
                return;
            }

            edgeCollider.enabled = true;
            localPoints.Clear();
            int count = worldPoints.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 local3 = transform.InverseTransformPoint(worldPoints[i]);
                localPoints.Add(new Vector2(local3.x, local3.y));
            }

            edgeCollider.SetPoints(localPoints);
        }

        private void ClearCollider()
        {
            edgeCollider.enabled = false;
            edgeCollider.points = EmptyPoints;
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
            lineRenderer.numCapVertices = roundCapVertices;
            lineRenderer.numCornerVertices = roundCornerVertices;
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
