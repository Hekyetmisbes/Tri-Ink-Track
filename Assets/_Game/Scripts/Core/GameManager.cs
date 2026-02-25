using System;
using UnityEngine;

namespace TriInkTrack.Core
{
    public enum GameState
    {
        Ready,
        Playing,
        Win,
        Fail
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameConfig config;
        [SerializeField] private bool autoStartOnSceneLoad = true;

        public GameState CurrentState { get; private set; }
        public event Action<GameState> OnGameStateChanged;

        public GameConfig Config => config;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            SetState(GameState.Ready);

            if (autoStartOnSceneLoad)
            {
                StartGame();
            }
        }

        private void SetState(GameState newState)
        {
            CurrentState = newState;
            OnGameStateChanged?.Invoke(newState);
#if UNITY_EDITOR
            Debug.Log($"[GameManager] State: {newState}");
#endif
        }

        public void StartGame()
        {
            if (CurrentState == GameState.Ready)
                SetState(GameState.Playing);
        }

        public void OnWin()
        {
            if (CurrentState == GameState.Playing)
                SetState(GameState.Win);
        }

        public void OnFail()
        {
            if (CurrentState == GameState.Playing)
                SetState(GameState.Fail);
        }

        public void Retry()
        {
            SetState(GameState.Ready);
        }

        public void NextLevel()
        {
            SetState(GameState.Ready);
        }
    }
}
