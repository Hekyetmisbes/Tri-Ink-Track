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
            if (reloadSceneOnRetry)
            {
                Time.timeScale = 1f;
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
            SetState(GameState.Ready);
            StartGame();
        }
    }
}
