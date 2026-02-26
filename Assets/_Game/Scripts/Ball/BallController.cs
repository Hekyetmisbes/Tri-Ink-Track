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
        [SerializeField] private bool failWhenOutOfCameraBounds = true;
        [SerializeField] private float outOfBoundsPadding = 1.5f;

        private Rigidbody2D rb;
        private Vector3 spawnPosition;
        private Vector2 spawnDirection;
        private Camera gameplayCamera;

        private const string HazardTag = "Hazard";
        private const string BoundaryTag = "Boundary";

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            gameplayCamera = Camera.main;
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
            EnsureBoundarySystemExists();
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
                rb.linearVelocity = fallback.normalized * targetSpeed;
            }
            else
            {
                Vector2 normalized = velocity / speed;
                float desiredSpeed = Mathf.Min(targetSpeed, maxSpeed);
                rb.linearVelocity = normalized * desiredSpeed;
            }

            if (failWhenOutOfCameraBounds && IsOutOfBounds())
            {
                TryFail();
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

        public void RefreshSpawnPointFromScene(bool resetBall = true)
        {
            CacheSpawnData();
            if (resetBall)
            {
                ResetBall();
            }
        }

        public void SetSpawnData(Vector3 position, Vector2 direction, bool resetBall = true)
        {
            spawnPosition = position;
            spawnDirection = direction.sqrMagnitude > 0f ? direction.normalized : initialDirection.normalized;
            if (resetBall)
            {
                ResetBall();
            }
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == null)
            {
                return;
            }

            if (other.CompareTag(HazardTag) ||
                other.CompareTag(BoundaryTag) ||
                other.GetComponent<HazardZone>() != null ||
                other.GetComponent<BoundaryTrigger>() != null)
            {
                TryFail();
            }
        }

        private bool IsOutOfBounds()
        {
            if (gameplayCamera == null)
            {
                gameplayCamera = Camera.main;
                if (gameplayCamera == null || !gameplayCamera.orthographic)
                {
                    return false;
                }
            }

            float halfHeight = gameplayCamera.orthographicSize + outOfBoundsPadding;
            float halfWidth = (gameplayCamera.orthographicSize * gameplayCamera.aspect) + outOfBoundsPadding;
            Vector3 cameraPos = gameplayCamera.transform.position;
            Vector3 pos = transform.position;

            return pos.x < cameraPos.x - halfWidth ||
                   pos.x > cameraPos.x + halfWidth ||
                   pos.y < cameraPos.y - halfHeight ||
                   pos.y > cameraPos.y + halfHeight;
        }

        private void TryFail()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            if (GameManager.Instance.CurrentState != GameState.Playing)
            {
                return;
            }

            GameManager.Instance.OnFail();
        }

        private static void EnsureBoundarySystemExists()
        {
            if (FindFirstObjectByType<BoundarySystem>() != null)
            {
                return;
            }

            GameObject boundaryObject = new GameObject("BoundarySystem");
            boundaryObject.AddComponent<BoundarySystem>();
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
