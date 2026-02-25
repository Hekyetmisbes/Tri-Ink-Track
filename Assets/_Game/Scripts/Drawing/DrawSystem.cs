using TriInkTrack.Core;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace TriInkTrack.Drawing
{
    public class DrawSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera gameplayCamera;
        [SerializeField] private InkLine inkLinePrefab;
        [SerializeField] private Transform lineRoot;

        [Header("Drawing")]
        [SerializeField] private bool allowDrawInReadyState = true;
        [SerializeField] private float minPointDist = 0.15f;
        [SerializeField] private int minPointsToKeep = 2;

        private bool canDraw;
        private bool isDrawing;
        private int activePointerId = -1;
        private InkLine activeLine;
        private Vector3 lastPoint;

        private void Awake()
        {
            if (gameplayCamera == null)
            {
                gameplayCamera = Camera.main;
            }

            ApplyConfigOverrides();
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
                HandleGameStateChanged(GameManager.Instance.CurrentState);
            }
            else
            {
                canDraw = true;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            }
        }

        private void Update()
        {
            if (!canDraw)
            {
                if (isDrawing)
                {
                    EndDrawing();
                }
                return;
            }

#if ENABLE_INPUT_SYSTEM
            if (HasActiveTouchInput())
            {
                HandleTouchInput();
            }
            else
            {
                HandleMouseInput();
            }
#endif
        }

        public bool CanDraw => canDraw;

        private void HandleTouchInput()
        {
            if (Touchscreen.current == null)
            {
                return;
            }

            foreach (var touch in Touchscreen.current.touches)
            {
                int touchId = touch.touchId.ReadValue();
                Vector2 touchPos = touch.position.ReadValue();
                UnityEngine.InputSystem.TouchPhase phase = touch.phase.ReadValue();

                if (phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    if (IsPointerOverUI(touchId))
                    {
                        continue;
                    }

                    StartDrawing(touchPos, touchId);
                    continue;
                }

                if (!isDrawing || touchId != activePointerId)
                {
                    continue;
                }

                if (phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                    phase == UnityEngine.InputSystem.TouchPhase.Stationary)
                {
                    ContinueDrawing(touchPos);
                }
                else if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                         phase == UnityEngine.InputSystem.TouchPhase.Canceled)
                {
                    EndDrawing();
                }
            }
        }

        private void HandleMouseInput()
        {
            if (Mouse.current == null)
            {
                return;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (IsPointerOverUI(-1))
                {
                    return;
                }

                StartDrawing(Mouse.current.position.ReadValue(), -1);
            }

            if (isDrawing && Mouse.current.leftButton.isPressed)
            {
                ContinueDrawing(Mouse.current.position.ReadValue());
            }

            if (isDrawing && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                EndDrawing();
            }
        }

        private bool HasActiveTouchInput()
        {
            if (Touchscreen.current == null)
            {
                return false;
            }

            foreach (var touch in Touchscreen.current.touches)
            {
                UnityEngine.InputSystem.TouchPhase phase = touch.phase.ReadValue();
                if (phase != UnityEngine.InputSystem.TouchPhase.None)
                {
                    return true;
                }
            }

            return false;
        }

        private void StartDrawing(Vector2 screenPosition, int pointerId)
        {
            if (isDrawing)
            {
                EndDrawing();
            }

            InkLine newLine = CreateLineInstance();
            if (newLine == null)
            {
                return;
            }

            Vector3 worldPoint = ScreenToWorld(screenPosition);
            activeLine = newLine;
            activeLine.ResetLine();
            activeLine.AddPoint(worldPoint);
            lastPoint = worldPoint;
            activePointerId = pointerId;
            isDrawing = true;
        }

        private void ContinueDrawing(Vector2 screenPosition)
        {
            if (!isDrawing || activeLine == null)
            {
                return;
            }

            Vector3 worldPoint = ScreenToWorld(screenPosition);
            float sqrDistance = (worldPoint - lastPoint).sqrMagnitude;
            float minDistSqr = minPointDist * minPointDist;
            if (sqrDistance < minDistSqr)
            {
                return;
            }

            if (activeLine.AddPoint(worldPoint))
            {
                lastPoint = worldPoint;
            }
            else
            {
                EndDrawing();
            }
        }

        private void EndDrawing()
        {
            if (activeLine == null)
            {
                ResetDrawingState();
                return;
            }

            if (activeLine.PointCount < minPointsToKeep)
            {
                Destroy(activeLine.gameObject);
            }
            else
            {
                activeLine.LockLine();
            }

            ResetDrawingState();
        }

        private void ResetDrawingState()
        {
            isDrawing = false;
            activePointerId = -1;
            activeLine = null;
        }

        private InkLine CreateLineInstance()
        {
            if (inkLinePrefab != null)
            {
                Transform parent = lineRoot != null ? lineRoot : transform;
                return Instantiate(inkLinePrefab, parent);
            }

            GameObject runtimeLine = new GameObject("InkLine_Runtime");
            runtimeLine.transform.SetParent(lineRoot != null ? lineRoot : transform, false);
            runtimeLine.AddComponent<LineRenderer>();
            runtimeLine.AddComponent<EdgeCollider2D>();
            return runtimeLine.AddComponent<InkLine>();
        }

        private Vector3 ScreenToWorld(Vector2 screenPoint)
        {
            Vector3 world = gameplayCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 0f));
            world.z = 0f;
            return world;
        }

        private bool IsPointerOverUI(int pointerId)
        {
            if (EventSystem.current == null)
            {
                return false;
            }

            if (pointerId >= 0)
            {
                return EventSystem.current.IsPointerOverGameObject(pointerId);
            }

            return EventSystem.current.IsPointerOverGameObject();
        }

        private void HandleGameStateChanged(GameState state)
        {
            canDraw = state == GameState.Playing || (allowDrawInReadyState && state == GameState.Ready);
        }

        private void ApplyConfigOverrides()
        {
            if (GameManager.Instance == null || GameManager.Instance.Config == null)
            {
                return;
            }

            minPointDist = GameManager.Instance.Config.MinPointDist;
        }
    }
}
