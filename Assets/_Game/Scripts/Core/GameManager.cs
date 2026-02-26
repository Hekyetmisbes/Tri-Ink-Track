using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        [SerializeField] private bool reloadSceneOnRetry = true;
        [SerializeField] private LevelManager levelManager;

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

            if (levelManager == null)
            {
                levelManager = FindFirstObjectByType<LevelManager>();
            }
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
            Time.timeScale = 1f;

            if (levelManager != null)
            {
                levelManager.RestartLevel();
                SetState(GameState.Ready);
                StartGame();
                return;
            }

            if (reloadSceneOnRetry)
            {
                Scene activeScene = SceneManager.GetActiveScene();
                if (activeScene.buildIndex >= 0)
                {
                    SceneManager.LoadScene(activeScene.buildIndex);
                }
                else
                {
                    SceneManager.LoadScene(activeScene.name);
                }
                return;
            }

            SetState(GameState.Ready);
            StartGame();
        }

        public void NextLevel()
        {
            if (levelManager != null)
            {
                if (levelManager.NextLevel())
                {
                    SetState(GameState.Ready);
                    StartGame();
                }
                return;
            }

            SetState(GameState.Ready);
            StartGame();
        }
    }
}
