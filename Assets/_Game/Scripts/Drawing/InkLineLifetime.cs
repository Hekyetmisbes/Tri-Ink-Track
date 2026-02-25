using UnityEngine;

namespace TriInkTrack.Drawing
{
    [RequireComponent(typeof(InkLine))]
    public class InkLineLifetime : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InkLine inkLine;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private EdgeCollider2D edgeCollider;

        [Header("Timing")]
        [SerializeField] private float lifeSeconds = 7f;
        [SerializeField] private float fadeDuration = 0.5f;

        private float spawnTime;
        private float fadeStartTime;
        private bool isRunning;
        private bool isFading;
        private InkLinePool ownerPool;

        public float SpawnTime => spawnTime;

        private void Awake()
        {
            EnsureReferences();
        }

        private void Update()
        {
            if (!isRunning)
            {
                return;
            }

            float now = Time.time;
            if (!isFading)
            {
                if (now - spawnTime >= lifeSeconds)
                {
                    isFading = true;
                    fadeStartTime = now;
                }

                return;
            }

            float t = (now - fadeStartTime) / fadeDuration;
            if (t >= 1f)
            {
                CompleteLifetime();
                return;
            }

            SetAlpha(1f - t);
        }

        public void BeginLifetime(float life, float fade, InkLinePool pool)
        {
            EnsureReferences();

            lifeSeconds = Mathf.Max(0.01f, life);
            fadeDuration = Mathf.Max(0.01f, fade);
            ownerPool = pool;
            spawnTime = Time.time;
            isRunning = true;
            isFading = false;
            fadeStartTime = 0f;

            SetAlpha(1f);
        }

        public void StopLifetime()
        {
            isRunning = false;
            isFading = false;
            ownerPool = null;
        }

        public void PrepareForReuse()
        {
            StopLifetime();
            EnsureReferences();
            SetAlpha(1f);
        }

        private void CompleteLifetime()
        {
            SetAlpha(0f);
            if (edgeCollider != null)
            {
                edgeCollider.enabled = false;
            }

            isRunning = false;
            isFading = false;

            if (ownerPool != null)
            {
                ownerPool.Return(inkLine);
                return;
            }

            gameObject.SetActive(false);
        }

        private void SetAlpha(float alpha)
        {
            if (lineRenderer == null)
            {
                return;
            }

            alpha = Mathf.Clamp01(alpha);

            Color start = lineRenderer.startColor;
            Color end = lineRenderer.endColor;
            start.a = alpha;
            end.a = alpha;
            lineRenderer.startColor = start;
            lineRenderer.endColor = end;
        }

        private void EnsureReferences()
        {
            if (inkLine == null)
            {
                inkLine = GetComponent<InkLine>();
            }

            if (lineRenderer == null && inkLine != null)
            {
                lineRenderer = inkLine.CachedLineRenderer;
            }

            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
            }

            if (edgeCollider == null && inkLine != null)
            {
                edgeCollider = inkLine.CachedEdgeCollider;
            }

            if (edgeCollider == null)
            {
                edgeCollider = GetComponent<EdgeCollider2D>();
            }
        }
    }
}
