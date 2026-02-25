using TriInkTrack.Core;
using TriInkTrack.Level;
using UnityEngine;

namespace TriInkTrack.Ball
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BallController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float targetSpeed = 5f;
        [SerializeField] private float minSpeedThreshold = 0.1f;
        [SerializeField] private float maxSpeed = 8f;
        [SerializeField] private Vector2 initialDirection = Vector2.right;

        private Rigidbody2D rb;
        private Vector3 spawnPosition;
        private Vector2 spawnDirection;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            CacheSpawnData();
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            }
        }

        private void Start()
        {
            ApplyConfigOverrides();
            ResetBall();
        }

        private void FixedUpdate()
        {
            if (!CanMove())
            {
                return;
            }

            Vector2 velocity = rb.linearVelocity;
            float speed = velocity.magnitude;

            if (speed < minSpeedThreshold)
            {
                Vector2 fallback = spawnDirection.sqrMagnitude > 0f ? spawnDirection : initialDirection;
                rb.linearVelocity = fallback.normalized * minSpeedThreshold;
                return;
            }

            if (speed > maxSpeed)
            {
                Vector2 normalized = velocity / speed;
                rb.linearVelocity = normalized * maxSpeed;
            }
        }

        public void ResetBall()
        {
            transform.position = spawnPosition;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            Vector2 fallback = spawnDirection.sqrMagnitude > 0f ? spawnDirection : initialDirection;
            rb.linearVelocity = fallback.normalized * targetSpeed;
        }

        private bool CanMove()
        {
            if (GameManager.Instance == null)
            {
                return true;
            }

            GameState state = GameManager.Instance.CurrentState;
            return state == GameState.Ready || state == GameState.Playing;
        }

        private void CacheSpawnData()
        {
            spawnPosition = transform.position;
            spawnDirection = initialDirection.normalized;

            BallSpawnPoint marker = FindFirstObjectByType<BallSpawnPoint>();
            if (marker == null)
            {
                return;
            }

            spawnPosition = marker.transform.position;
            spawnDirection = marker.InitialDirection.normalized;
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (state == GameState.Fail || state == GameState.Win)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                return;
            }

            if (state == GameState.Ready)
            {
                ResetBall();
            }
        }

        private void ApplyConfigOverrides()
        {
            if (GameManager.Instance == null || GameManager.Instance.Config == null)
            {
                return;
            }

            targetSpeed = GameManager.Instance.Config.BallSpeed;
            if (maxSpeed < targetSpeed)
            {
                maxSpeed = targetSpeed;
            }
        }
    }
}
