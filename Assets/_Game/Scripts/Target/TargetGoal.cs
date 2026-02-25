using TriInkTrack.Core;
using UnityEngine;

namespace TriInkTrack.Target
{
    [RequireComponent(typeof(Collider2D))]
    public class TargetGoal : MonoBehaviour
    {
        [Header("Pulse")]
        [SerializeField] private bool pulseEnabled = true;
        [SerializeField] private float pulseSpeed = 2.25f;
        [SerializeField] private float pulseAmount = 0.08f;

        private Vector3 baseScale;

        private void Awake()
        {
            Collider2D targetCollider = GetComponent<Collider2D>();
            targetCollider.isTrigger = true;
            baseScale = transform.localScale;
        }

        private void Update()
        {
            if (!pulseEnabled)
            {
                return;
            }

            float t = 0.5f + 0.5f * Mathf.Sin(Time.time * pulseSpeed);
            float scaleMultiplier = 1f + (t * pulseAmount);
            transform.localScale = baseScale * scaleMultiplier;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Ball"))
            {
                return;
            }

            if (GameManager.Instance == null)
            {
                return;
            }

            GameManager.Instance.OnWin();
        }
    }
}
