using UnityEngine;

namespace TriInkTrack.Core
{
    public class DrawSystem : MonoBehaviour
    {
        [SerializeField] private Camera gameplayCamera;
        [SerializeField] private bool allowDrawInReadyState = true;

        private bool canDraw;

        private void Awake()
        {
            if (gameplayCamera == null)
            {
                gameplayCamera = Camera.main;
            }
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
                HandleGameStateChanged(GameManager.Instance.CurrentState);
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            }
        }

        public bool CanDraw => canDraw;
        public Camera GameplayCamera => gameplayCamera;

        private void HandleGameStateChanged(GameState state)
        {
            canDraw = state == GameState.Playing || (allowDrawInReadyState && state == GameState.Ready);
        }
    }
}
