using System.Collections.Generic;
using TriInkTrack.Ink;
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
        [SerializeField] private int roundCapVertices = 8;
        [SerializeField] private int roundCornerVertices = 6;
        [SerializeField] private Color iceColor = new Color(0f, 0.749f, 1f, 1f);
        [SerializeField] private Color stickyColor = new Color(1f, 0.549f, 0f, 1f);
        [SerializeField] private Color bouncyColor = new Color(0.196f, 0.804f, 0.196f, 1f);

        [Header("Limits")]
        [SerializeField] private int maxPoints = 60;

        [Header("Ink Physics")]
        [SerializeField] private InkType currentType = InkType.Ice;
        [SerializeField] private PhysicsMaterial2D iceMaterial;
        [SerializeField] private PhysicsMaterial2D stickyMaterial;
        [SerializeField] private PhysicsMaterial2D bouncyMaterial;

        private readonly List<Vector3> worldPoints = new List<Vector3>(64);
        private readonly List<Vector2> localPoints = new List<Vector2>(64);
        private bool isLocked;

        public int PointCount => worldPoints.Count;
        public int MaxPoints => maxPoints;
        public bool IsAtMaxPoints => worldPoints.Count >= maxPoints;
        public InkType CurrentType => currentType;

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
            ApplyInkVisualsAndPhysics();
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

        public void SetInkType(InkType type)
        {
            currentType = type;
            ApplyInkVisualsAndPhysics();
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

            if (lineRenderer.sharedMaterial == null)
            {
                Shader shader = Shader.Find("Sprites/Default");
                if (shader != null)
                {
                    lineRenderer.sharedMaterial = new Material(shader);
                }
            }

            ApplyInkVisualsAndPhysics();
        }

        private void ApplyInkVisualsAndPhysics()
        {
            Color color = GetColorForType(currentType);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            edgeCollider.sharedMaterial = GetMaterialForType(currentType);
        }

        private PhysicsMaterial2D GetMaterialForType(InkType type)
        {
            switch (type)
            {
                case InkType.Sticky:
                    return stickyMaterial;
                case InkType.Bouncy:
                    return bouncyMaterial;
                case InkType.Ice:
                default:
                    return iceMaterial;
            }
        }

        private Color GetColorForType(InkType type)
        {
            switch (type)
            {
                case InkType.Sticky:
                    return stickyColor;
                case InkType.Bouncy:
                    return bouncyColor;
                case InkType.Ice:
                default:
                    return iceColor;
            }
        }
    }
}
