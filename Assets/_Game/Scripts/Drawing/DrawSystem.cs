using TriInkTrack.Core;
using TriInkTrack.Ink;
using System;
using System.Collections.Generic;
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
        [SerializeField] private InkInventory inkInventory;
        [SerializeField] private InkLinePool inkLinePool;

        [Header("Drawing")]
        [SerializeField] private bool allowDrawInReadyState = true;
        [SerializeField] private float minPointDist = 0.15f;
        [SerializeField] private int minPointsToKeep = 3;
        [SerializeField] private int maxPointsPerLine = 60;
        [SerializeField] private int maxActiveLines = 30;
        [SerializeField] private bool removeOldestLineWhenLimitReached = true;
        [SerializeField] private float lineLifetimeSeconds = 7f;
        [SerializeField] private float lineFadeDuration = 0.5f;

        [Header("Play Area")]
        [SerializeField] private bool restrictDrawingToPlayArea = true;
        [SerializeField] private Collider2D playAreaCollider;
        [SerializeField] private float playAreaPadding = 0.05f;

        private bool canDraw;
        private bool isDrawing;
        private int activePointerId = -1;
        private InkLine activeLine;
        private Vector3 lastPoint;
        private readonly List<InkLine> activeLines = new List<InkLine>(32);
        private readonly List<RaycastResult> uiRaycastResults = new List<RaycastResult>(8);
        private bool hasComputedPlayAreaBounds;
        private Bounds computedPlayAreaBounds;

        private void Awake()
        {
            if (gameplayCamera == null)
            {
                gameplayCamera = Camera.main;
            }

            if (inkInventory == null)
            {
                inkInventory = InkInventory.Instance;
            }

            if (inkInventory == null)
            {
                inkInventory = FindFirstObjectByType<InkInventory>();
                if (inkInventory == null)
                {
                    GameObject inventoryObject = new GameObject("InkInventory");
                    inkInventory = inventoryObject.AddComponent<InkInventory>();
                }
            }

            ApplyConfigOverrides();
            EnsureLinePool();
            ResolvePlayAreaBoundsIfNeeded();
        }

        private void OnEnable()
        {
            if (inkLinePool != null)
            {
                inkLinePool.OnLineReturned += HandleLineReturned;
            }

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
            if (inkLinePool != null)
            {
                inkLinePool.OnLineReturned -= HandleLineReturned;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            }

            if (isDrawing)
            {
                EndDrawing();
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
                    if (IsScreenPositionOverUI(touchPos))
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
                Vector2 mousePos = Mouse.current.position.ReadValue();
                if (IsScreenPositionOverUI(mousePos))
                {
                    return;
                }

                StartDrawing(mousePos, -1);
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

            if (!TryResolveLineLimit())
            {
                return;
            }

            if (inkInventory != null && !inkInventory.HasInk())
            {
                return;
            }

            InkLine newLine = CreateLineInstance();
            if (newLine == null)
            {
                return;
            }

            Vector3 worldPoint = ScreenToWorld(screenPosition);
            if (!IsPointInPlayArea(worldPoint))
            {
                DestroyLine(newLine);
                return;
            }

            activeLine = newLine;
            activeLine.ResetLine();
            activeLine.SetMaxPoints(maxPointsPerLine);
            if (inkInventory != null)
            {
                activeLine.SetInkType(inkInventory.CurrentInkType);
            }

            if (inkInventory != null && !inkInventory.HasInk())
            {
                DestroyLine(activeLine);
                ResetDrawingState();
                return;
            }

            activeLine.AddPoint(worldPoint);

            RegisterLine(activeLine);
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
            if (!IsPointInPlayArea(worldPoint))
            {
                return;
            }

            float sqrDistance = (worldPoint - lastPoint).sqrMagnitude;
            float minDistSqr = minPointDist * minPointDist;
            if (sqrDistance < minDistSqr)
            {
                return;
            }

            if (inkInventory != null && !inkInventory.HasInk())
            {
                EndDrawing();
                return;
            }

            if (activeLine.AddPoint(worldPoint))
            {
                if (inkInventory != null)
                {
                    inkInventory.ConsumeInk(1);
                }

                lastPoint = worldPoint;

                if (activeLine.IsAtMaxPoints)
                {
                    EndDrawing();
                }
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
                DestroyLine(activeLine);
            }
            else
            {
                activeLine.LockLine();
                StartLineLifetime(activeLine);
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
            EnsureLinePool();
            if (inkLinePool != null)
            {
                return inkLinePool.Get();
            }

            Transform parent = lineRoot != null ? lineRoot : transform;
            GameObject runtimeLine = new GameObject("InkLine_Runtime");
            runtimeLine.transform.SetParent(parent, false);
            runtimeLine.AddComponent<LineRenderer>();
            runtimeLine.AddComponent<EdgeCollider2D>();
            InkLine line = runtimeLine.AddComponent<InkLine>();
            runtimeLine.AddComponent<InkLineLifetime>();
            return line;
        }

        private Vector3 ScreenToWorld(Vector2 screenPoint)
        {
            Vector3 world = gameplayCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 0f));
            world.z = 0f;
            return world;
        }

        private bool IsScreenPositionOverUI(Vector2 screenPos)
        {
            if (EventSystem.current == null)
            {
                return false;
            }

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = screenPos
            };

            uiRaycastResults.Clear();
            EventSystem.current.RaycastAll(pointerData, uiRaycastResults);
            return uiRaycastResults.Count > 0;
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
            maxPointsPerLine = Mathf.Max(2, GameManager.Instance.Config.MaxPointsPerLine);
            maxActiveLines = Mathf.Max(1, GameManager.Instance.Config.MaxActiveLines);
            lineLifetimeSeconds = Mathf.Max(0.5f, GameManager.Instance.Config.InkLifetime);
            lineFadeDuration = Mathf.Max(0.05f, GameManager.Instance.Config.FadeDuration);
        }

        private void RegisterLine(InkLine line)
        {
            if (line == null || activeLines.Contains(line))
            {
                return;
            }

            activeLines.Add(line);
        }

        private void DestroyLine(InkLine line)
        {
            if (line == null)
            {
                return;
            }

            activeLines.Remove(line);
            if (inkLinePool != null)
            {
                inkLinePool.Return(line);
                return;
            }

            Destroy(line.gameObject);
        }

        private bool TryResolveLineLimit()
        {
            CleanupMissingLines();

            if (activeLines.Count < maxActiveLines)
            {
                return true;
            }

            if (!removeOldestLineWhenLimitReached)
            {
                return false;
            }

            while (activeLines.Count >= maxActiveLines)
            {
                InkLine oldest = activeLines[0];
                activeLines.RemoveAt(0);
                if (oldest != null)
                {
                    DestroyLine(oldest);
                    return true;
                }
            }

            return activeLines.Count < maxActiveLines;
        }

        private void CleanupMissingLines()
        {
            for (int i = activeLines.Count - 1; i >= 0; i--)
            {
                InkLine line = activeLines[i];
                if (line == null || !line.gameObject.activeInHierarchy)
                {
                    activeLines.RemoveAt(i);
                }
            }
        }

        private void StartLineLifetime(InkLine line)
        {
            if (line == null)
            {
                return;
            }

            InkLineLifetime lifetime = line.GetComponent<InkLineLifetime>();
            if (lifetime == null)
            {
                lifetime = line.gameObject.AddComponent<InkLineLifetime>();
            }

            lifetime.BeginLifetime(lineLifetimeSeconds, lineFadeDuration, inkLinePool);
        }

        private void HandleLineReturned(InkLine line)
        {
            activeLines.Remove(line);
        }

        private void EnsureLinePool()
        {
            if (inkLinePool == null)
            {
                inkLinePool = GetComponent<InkLinePool>();
            }

            if (inkLinePool == null)
            {
                inkLinePool = gameObject.AddComponent<InkLinePool>();
            }

            Transform root = lineRoot != null ? lineRoot : transform;
            inkLinePool.Configure(inkLinePrefab, root);
        }

        private bool IsPointInPlayArea(Vector3 worldPoint)
        {
            if (!restrictDrawingToPlayArea)
            {
                return true;
            }

            if (playAreaCollider != null)
            {
                return playAreaCollider.OverlapPoint(worldPoint);
            }

            if (hasComputedPlayAreaBounds)
            {
                float minX = computedPlayAreaBounds.min.x + playAreaPadding;
                float maxX = computedPlayAreaBounds.max.x - playAreaPadding;
                float minY = computedPlayAreaBounds.min.y + playAreaPadding;
                float maxY = computedPlayAreaBounds.max.y - playAreaPadding;

                return worldPoint.x >= minX &&
                       worldPoint.x <= maxX &&
                       worldPoint.y >= minY &&
                       worldPoint.y <= maxY;
            }

            return true;
        }

        private void ResolvePlayAreaBoundsIfNeeded()
        {
            hasComputedPlayAreaBounds = false;

            if (playAreaCollider != null)
            {
                return;
            }

            Collider2D[] colliders = FindObjectsByType<Collider2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            Bounds bounds = default;
            bool foundAny = false;

            for (int i = 0; i < colliders.Length; i++)
            {
                Collider2D col = colliders[i];
                if (col == null || !col.enabled || col.isTrigger)
                {
                    continue;
                }

                string name = col.gameObject.name;
                if (name.IndexOf("wall", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                if (!foundAny)
                {
                    bounds = col.bounds;
                    foundAny = true;
                }
                else
                {
                    bounds.Encapsulate(col.bounds);
                }
            }

            if (foundAny)
            {
                computedPlayAreaBounds = bounds;
                hasComputedPlayAreaBounds = true;
            }
        }

        public void RebuildPlayAreaBounds()
        {
            ResolvePlayAreaBoundsIfNeeded();
        }
    }
}
