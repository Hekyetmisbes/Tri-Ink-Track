using TriInkTrack.Audio;
using TriInkTrack.Ball;
using TriInkTrack.Drawing;
using TriInkTrack.Ink;
using TriInkTrack.Level;
using TriInkTrack.UI;
using UnityEngine;

namespace TriInkTrack.Core
{
    public class LevelManager : MonoBehaviour
    {
        private const string LastUnlockedLevelKey = "LastUnlockedLevel";

        [Header("References")]
        [SerializeField] private LevelRoot levelRoot;
        [SerializeField] private BallController ballController;
        [SerializeField] private InkInventory inkInventory;
        [SerializeField] private UIHudController hudController;
        [SerializeField] private DrawSystem drawSystem;

        [Header("Levels")]
        [SerializeField] private LevelDefinition[] levelDefinitions;
        [SerializeField] private GameObject[] levelPrefabs;

        [Header("Behavior")]
        [SerializeField] private bool autoLoadOnStart = true;
        [SerializeField] private bool startFromSavedProgress = true;
        [SerializeField] private int startLevelIndex = 0;
        [SerializeField] private bool loopToFirstLevelAfterLast;
        [SerializeField] private bool showGameCompletePanelOnLast = true;

        private GameObject currentLevelInstance;
        private int currentLevelIndex = -1;
        private LevelDefinition currentDefinition;

        public int CurrentLevelIndex => currentLevelIndex;
        public int LevelCount => Mathf.Max(levelDefinitions != null ? levelDefinitions.Length : 0, levelPrefabs != null ? levelPrefabs.Length : 0);
        public LevelDefinition CurrentDefinition => currentDefinition;

        private void Awake()
        {
            ResolveReferences();
        }

        private void Start()
        {
            if (!autoLoadOnStart || LevelCount <= 0)
            {
                return;
            }

            int initialLevel = startFromSavedProgress
                ? Mathf.Clamp(PlayerPrefs.GetInt(LastUnlockedLevelKey, startLevelIndex), 0, Mathf.Max(0, LevelCount - 1))
                : Mathf.Clamp(startLevelIndex, 0, Mathf.Max(0, LevelCount - 1));

            if (DevLevelSelection.TryConsumeSelectedLevelIndex(out int selectedLevel))
            {
                initialLevel = Mathf.Clamp(selectedLevel, 0, Mathf.Max(0, LevelCount - 1));
            }

            LoadLevel(initialLevel);
        }

        public void LoadLevel(int levelIndex)
        {
            LoadLevelInternal(levelIndex, updateUnlockProgress: false);
        }

        public void RestartLevel()
        {
            int index = currentLevelIndex >= 0 ? currentLevelIndex : Mathf.Clamp(startLevelIndex, 0, Mathf.Max(0, LevelCount - 1));
            LoadLevelInternal(index, updateUnlockProgress: false);
        }

        public bool NextLevel()
        {
            if (LevelCount <= 0)
            {
                return false;
            }

            int nextIndex = currentLevelIndex + 1;
            if (nextIndex < LevelCount)
            {
                LoadLevelInternal(nextIndex, updateUnlockProgress: true);
                return true;
            }

            if (loopToFirstLevelAfterLast)
            {
                LoadLevelInternal(0, updateUnlockProgress: false);
                return true;
            }

            if (showGameCompletePanelOnLast && hudController != null)
            {
                hudController.SetGameCompleteVisible(true);
            }

            return false;
        }

        private void LoadLevelInternal(int levelIndex, bool updateUnlockProgress)
        {
            if (LevelCount <= 0)
            {
                return;
            }

            ResolveReferences();

            int clampedIndex = Mathf.Clamp(levelIndex, 0, LevelCount - 1);
            GameObject prefab = ResolveLevelPrefab(clampedIndex, out LevelDefinition definition);
            if (prefab == null)
            {
                Debug.LogError($"[LevelManager] Missing level prefab at index {clampedIndex}.");
                return;
            }

            if (currentLevelInstance != null)
            {
                Destroy(currentLevelInstance);
                currentLevelInstance = null;
            }

            Transform parent = levelRoot != null ? levelRoot.transform : null;
            currentLevelInstance = Instantiate(prefab, parent);
            currentLevelInstance.name = prefab.name;

            currentLevelIndex = clampedIndex;
            currentDefinition = definition;

            ApplyDefinitionRules(definition);
            PlaceBallAtLevelSpawn();
            RebuildDrawBounds();

            if (hudController != null)
            {
                hudController.SetGameCompleteVisible(false);
            }

            AudioManager.Instance?.PlayLevelStart();

            if (updateUnlockProgress)
            {
                int unlocked = PlayerPrefs.GetInt(LastUnlockedLevelKey, 0);
                if (clampedIndex > unlocked)
                {
                    PlayerPrefs.SetInt(LastUnlockedLevelKey, clampedIndex);
                    PlayerPrefs.Save();
                }
            }
        }

        private GameObject ResolveLevelPrefab(int levelIndex, out LevelDefinition definition)
        {
            definition = null;
            if (levelDefinitions != null && levelIndex < levelDefinitions.Length)
            {
                definition = levelDefinitions[levelIndex];
                if (definition != null && definition.LevelPrefab != null)
                {
                    return definition.LevelPrefab;
                }
            }

            if (levelPrefabs != null && levelIndex < levelPrefabs.Length)
            {
                return levelPrefabs[levelIndex];
            }

            return null;
        }

        private void ApplyDefinitionRules(LevelDefinition definition)
        {
            if (inkInventory == null)
            {
                return;
            }

            bool allowIce = true;
            bool allowSticky = true;
            bool allowBouncy = true;
            int inkBudget = inkInventory.TotalInkPoints;

            if (definition != null)
            {
                allowIce = definition.AllowIce;
                allowSticky = definition.AllowSticky;
                allowBouncy = definition.AllowBouncy;
                inkBudget = definition.InkBudget;
            }

            inkInventory.ConfigureLevelInk(inkBudget, allowIce, allowSticky, allowBouncy);

            if (hudController != null)
            {
                hudController.SetInkAvailability(allowIce, allowSticky, allowBouncy);
            }
        }

        private void PlaceBallAtLevelSpawn()
        {
            if (ballController == null || currentLevelInstance == null)
            {
                return;
            }

            BallSpawnPoint spawnPoint = currentLevelInstance.GetComponentInChildren<BallSpawnPoint>();
            if (spawnPoint != null)
            {
                ballController.SetSpawnData(spawnPoint.transform.position, spawnPoint.InitialDirection, resetBall: true);
                return;
            }

            ballController.RefreshSpawnPointFromScene(resetBall: true);
        }

        private void RebuildDrawBounds()
        {
            if (drawSystem != null)
            {
                drawSystem.RebuildPlayAreaBounds();
            }
        }

        private void ResolveReferences()
        {
            if (levelRoot == null)
            {
                levelRoot = FindFirstObjectByType<LevelRoot>();
            }

            if (ballController == null)
            {
                ballController = FindFirstObjectByType<BallController>();
            }

            if (inkInventory == null)
            {
                inkInventory = InkInventory.Instance != null
                    ? InkInventory.Instance
                    : FindFirstObjectByType<InkInventory>();
            }

            if (hudController == null)
            {
                hudController = FindFirstObjectByType<UIHudController>();
            }

            if (drawSystem == null)
            {
                drawSystem = FindFirstObjectByType<DrawSystem>();
            }
        }
    }
}
