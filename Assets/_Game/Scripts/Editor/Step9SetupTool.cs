#if UNITY_EDITOR
using TriInkTrack.Ball;
using TriInkTrack.Core;
using TriInkTrack.Drawing;
using TriInkTrack.Ink;
using TriInkTrack.Level;
using TriInkTrack.Target;
using TriInkTrack.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TriInkTrack.EditorTools
{
    public static class Step9SetupTool
    {
        private const string GameScenePath = "Assets/_Game/Scenes/GameScene.unity";
        private const string LevelsFolderPath = "Assets/_Game/Prefabs/Levels";
        private const string LevelDefinitionsFolderPath = "Assets/_Game/ScriptableObjects/Levels";
        private const string WallPrefabPath = "Assets/_Game/Prefabs/Wall.prefab";
        private const string HazardPrefabPath = "Assets/_Game/Prefabs/Hazard.prefab";
        private const string TargetPrefabPath = "Assets/_Game/Prefabs/Target.prefab";
        private const string BallPrefabPath = "Assets/_Game/Prefabs/Ball.prefab";
        private const string BackgroundBluePath = "Assets/kenney_rolling-ball-assets/PNG/Default/background_blue.png";
        private const string BackgroundBrownPath = "Assets/kenney_rolling-ball-assets/PNG/Default/background_brown.png";
        private const string BackgroundGreenPath = "Assets/kenney_rolling-ball-assets/PNG/Default/background_green.png";

        private static readonly Vector2 ArenaSize = new Vector2(18f, 10f);

        private struct RectSpec
        {
            public RectSpec(float x, float y, float width, float height)
            {
                Center = new Vector2(x, y);
                Size = new Vector2(width, height);
            }

            public Vector2 Center;
            public Vector2 Size;
        }

        private sealed class LevelPlan
        {
            public string AssetName;
            public string DisplayName;
            public Vector2 SpawnPosition;
            public Vector2 SpawnDirection;
            public Vector2 TargetPosition;
            public int InkBudget;
            public bool AllowIce;
            public bool AllowSticky;
            public bool AllowBouncy;
            public RectSpec[] Walls;
            public RectSpec[] Hazards;
        }

        [MenuItem("Tools/TriInkTrack/Step 9/Build Level System and 10 Levels")]
        public static void BuildStep9()
        {
            EnsureFolderExists(LevelsFolderPath);
            EnsureFolderExists(LevelDefinitionsFolderPath);

            LevelPlan[] plans = BuildPlans();
            GameObject[] levelPrefabs = new GameObject[plans.Length];
            LevelDefinition[] definitions = new LevelDefinition[plans.Length];

            for (int i = 0; i < plans.Length; i++)
            {
                LevelPlan plan = plans[i];
                string prefabPath = $"{LevelsFolderPath}/{plan.AssetName}.prefab";
                CreateOrUpdateLevelPrefab(prefabPath, plan);

                GameObject levelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                levelPrefabs[i] = levelPrefab;

                string defPath = $"{LevelDefinitionsFolderPath}/{plan.AssetName}_Def.asset";
                definitions[i] = CreateOrUpdateLevelDefinition(defPath, plan, levelPrefab);
            }

            ConfigureGameScene(definitions, levelPrefabs);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Step9SetupTool] Step 9 level system setup completed.");
        }

        private static void CreateOrUpdateLevelPrefab(string prefabPath, LevelPlan plan)
        {
            Sprite backgroundBlue = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundBluePath);
            Sprite backgroundBrown = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundBrownPath);
            Sprite backgroundGreen = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundGreenPath);
            Sprite selectedBackground = SelectBackgroundSprite(plan.AssetName, backgroundBlue, backgroundBrown, backgroundGreen);

            GameObject wallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(WallPrefabPath);
            GameObject hazardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(HazardPrefabPath);
            GameObject targetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(TargetPrefabPath);

            GameObject root = new GameObject(plan.AssetName);

            CreateBackground(root.transform, selectedBackground);
            CreateBallSpawn(root.transform, plan.SpawnPosition, plan.SpawnDirection);
            CreateTarget(root.transform, plan.TargetPosition, targetPrefab);

            AddArenaWalls(root.transform, ArenaSize);
            AddRects(root.transform, "Wall", wallPrefab, plan.Walls, false, new Color(0.58f, 0.58f, 0.58f, 1f));
            AddRects(root.transform, "Hazard", hazardPrefab, plan.Hazards, true, new Color(0.82f, 0.2f, 0.2f, 1f));

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Object.DestroyImmediate(root);
        }

        private static LevelDefinition CreateOrUpdateLevelDefinition(string assetPath, LevelPlan plan, GameObject levelPrefab)
        {
            LevelDefinition definition = AssetDatabase.LoadAssetAtPath<LevelDefinition>(assetPath);
            if (definition == null)
            {
                definition = ScriptableObject.CreateInstance<LevelDefinition>();
                AssetDatabase.CreateAsset(definition, assetPath);
            }

            SerializedObject serialized = new SerializedObject(definition);
            serialized.FindProperty("levelName").stringValue = plan.DisplayName;
            serialized.FindProperty("levelPrefab").objectReferenceValue = levelPrefab;
            serialized.FindProperty("allowIce").boolValue = plan.AllowIce;
            serialized.FindProperty("allowSticky").boolValue = plan.AllowSticky;
            serialized.FindProperty("allowBouncy").boolValue = plan.AllowBouncy;
            serialized.FindProperty("inkBudget").intValue = plan.InkBudget;
            serialized.FindProperty("targetTime").floatValue = 0f;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(definition);
            return definition;
        }

        private static void ConfigureGameScene(LevelDefinition[] definitions, GameObject[] levelPrefabs)
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError($"[Step9SetupTool] Could not open scene at {GameScenePath}");
                return;
            }

            GameObject levelRootObject = GameObject.Find("LevelRoot");
            if (levelRootObject == null)
            {
                levelRootObject = new GameObject("LevelRoot");
            }

            LevelRoot levelRoot = levelRootObject.GetComponent<LevelRoot>();
            if (levelRoot == null)
            {
                levelRoot = levelRootObject.AddComponent<LevelRoot>();
            }

            ClearChildren(levelRootObject.transform);

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            if (gameManager == null)
            {
                GameObject gameManagerObject = new GameObject("GameManager");
                gameManager = gameManagerObject.AddComponent<GameManager>();
            }

            LevelManager levelManager = gameManager.GetComponent<LevelManager>();
            if (levelManager == null)
            {
                levelManager = gameManager.gameObject.AddComponent<LevelManager>();
            }

            BallController ballController = EnsureBallExists();
            InkInventory inkInventory = InkInventory.Instance != null ? InkInventory.Instance : Object.FindFirstObjectByType<InkInventory>();
            DrawSystem drawSystem = Object.FindFirstObjectByType<DrawSystem>();
            UIHudController hudController = Object.FindFirstObjectByType<UIHudController>();

            SerializedObject levelSerialized = new SerializedObject(levelManager);
            levelSerialized.FindProperty("levelRoot").objectReferenceValue = levelRoot;
            levelSerialized.FindProperty("ballController").objectReferenceValue = ballController;
            levelSerialized.FindProperty("inkInventory").objectReferenceValue = inkInventory;
            levelSerialized.FindProperty("drawSystem").objectReferenceValue = drawSystem;
            levelSerialized.FindProperty("hudController").objectReferenceValue = hudController;
            levelSerialized.FindProperty("autoLoadOnStart").boolValue = true;
            levelSerialized.FindProperty("startFromSavedProgress").boolValue = true;
            levelSerialized.FindProperty("startLevelIndex").intValue = 0;
            levelSerialized.FindProperty("loopToFirstLevelAfterLast").boolValue = false;
            levelSerialized.FindProperty("showGameCompletePanelOnLast").boolValue = true;

            SerializedProperty defsProp = levelSerialized.FindProperty("levelDefinitions");
            defsProp.arraySize = definitions.Length;
            for (int i = 0; i < definitions.Length; i++)
            {
                defsProp.GetArrayElementAtIndex(i).objectReferenceValue = definitions[i];
            }

            SerializedProperty prefabsProp = levelSerialized.FindProperty("levelPrefabs");
            prefabsProp.arraySize = levelPrefabs.Length;
            for (int i = 0; i < levelPrefabs.Length; i++)
            {
                prefabsProp.GetArrayElementAtIndex(i).objectReferenceValue = levelPrefabs[i];
            }

            levelSerialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(levelManager);

            SerializedObject gameSerialized = new SerializedObject(gameManager);
            gameSerialized.FindProperty("levelManager").objectReferenceValue = levelManager;
            gameSerialized.FindProperty("reloadSceneOnRetry").boolValue = false;
            gameSerialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(gameManager);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static BallController EnsureBallExists()
        {
            BallController controller = Object.FindFirstObjectByType<BallController>();
            if (controller != null)
            {
                return controller;
            }

            GameObject ballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BallPrefabPath);
            GameObject ballObject = null;
            if (ballPrefab != null)
            {
                ballObject = PrefabUtility.InstantiatePrefab(ballPrefab) as GameObject;
            }

            if (ballObject == null)
            {
                ballObject = new GameObject("Ball");
                ballObject.AddComponent<SpriteRenderer>();
                ballObject.AddComponent<CircleCollider2D>();
                ballObject.AddComponent<Rigidbody2D>();
                controller = ballObject.AddComponent<BallController>();
            }
            else
            {
                controller = ballObject.GetComponent<BallController>();
                if (controller == null)
                {
                    controller = ballObject.AddComponent<BallController>();
                }
            }

            ballObject.name = "Ball";
            if (ballObject.transform.parent != null)
            {
                ballObject.transform.SetParent(null);
            }

            return controller;
        }

        private static void CreateBackground(Transform parent, Sprite sprite)
        {
            GameObject background = new GameObject("Background");
            background.transform.SetParent(parent, false);
            background.transform.localPosition = new Vector3(0f, 0f, 4f);

            SpriteRenderer renderer = background.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = -20;
            renderer.sprite = sprite;
            renderer.drawMode = SpriteDrawMode.Sliced;
            renderer.size = new Vector2(20f, 12f);
            renderer.color = new Color(1f, 1f, 1f, 0.5f);
        }

        private static void CreateBallSpawn(Transform parent, Vector2 position, Vector2 direction)
        {
            GameObject spawn = new GameObject("BallSpawnPoint");
            spawn.transform.SetParent(parent, false);
            spawn.transform.localPosition = position;

            BallSpawnPoint marker = spawn.AddComponent<BallSpawnPoint>();
            SerializedObject markerSerialized = new SerializedObject(marker);
            markerSerialized.FindProperty("initialDirection").vector2Value = direction.normalized;
            markerSerialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateTarget(Transform parent, Vector2 position, GameObject targetPrefab)
        {
            GameObject targetObject = null;
            if (targetPrefab != null)
            {
                targetObject = PrefabUtility.InstantiatePrefab(targetPrefab) as GameObject;
            }

            if (targetObject == null)
            {
                targetObject = new GameObject("Target");
                targetObject.AddComponent<SpriteRenderer>();
                targetObject.AddComponent<CircleCollider2D>().isTrigger = true;
                targetObject.AddComponent<TargetGoal>();
            }

            targetObject.name = "Target";
            targetObject.transform.SetParent(parent, false);
            targetObject.transform.localPosition = position;
        }

        private static void AddArenaWalls(Transform parent, Vector2 arenaSize)
        {
            float halfW = arenaSize.x * 0.5f;
            float halfH = arenaSize.y * 0.5f;

            RectSpec[] bounds =
            {
                new RectSpec(0f, halfH, arenaSize.x, 0.8f),
                new RectSpec(0f, -halfH, arenaSize.x, 0.8f),
                new RectSpec(-halfW, 0f, 0.8f, arenaSize.y),
                new RectSpec(halfW, 0f, 0.8f, arenaSize.y)
            };

            AddRects(parent, "ArenaWall", AssetDatabase.LoadAssetAtPath<GameObject>(WallPrefabPath), bounds, false, new Color(0.52f, 0.52f, 0.52f, 1f));
        }

        private static void AddRects(Transform parent, string baseName, GameObject prefab, RectSpec[] rects, bool trigger, Color fallbackColor)
        {
            if (rects == null)
            {
                return;
            }

            for (int i = 0; i < rects.Length; i++)
            {
                RectSpec rect = rects[i];
                GameObject instance = null;
                if (prefab != null)
                {
                    instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                }

                if (instance == null)
                {
                    instance = new GameObject(baseName);
                    instance.AddComponent<SpriteRenderer>();
                    instance.AddComponent<BoxCollider2D>();
                }

                instance.name = $"{baseName}_{i + 1:D2}";
                instance.transform.SetParent(parent, false);
                instance.transform.localPosition = rect.Center;
                instance.transform.localScale = new Vector3(rect.Size.x, rect.Size.y, 1f);

                SpriteRenderer renderer = instance.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.color = fallbackColor;
                    renderer.drawMode = SpriteDrawMode.Sliced;
                    renderer.size = Vector2.one;
                }

                BoxCollider2D boxCollider = instance.GetComponent<BoxCollider2D>();
                if (boxCollider != null)
                {
                    boxCollider.isTrigger = trigger;
                    boxCollider.size = Vector2.one; // scale × 1 = gerçek boyut
                }

                if (trigger && instance.GetComponent<HazardZone>() == null)
                {
                    instance.AddComponent<HazardZone>();
                }
            }
        }

        private static Sprite SelectBackgroundSprite(string assetName, Sprite blue, Sprite brown, Sprite green)
        {
            if (assetName.EndsWith("03") || assetName.EndsWith("06") || assetName.EndsWith("09"))
            {
                return green != null ? green : blue;
            }

            if (assetName.EndsWith("02") || assetName.EndsWith("05") || assetName.EndsWith("08"))
            {
                return brown != null ? brown : blue;
            }

            return blue;
        }

        private static void ClearChildren(Transform root)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(root.GetChild(i).gameObject);
            }
        }

        private static void EnsureFolderExists(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string[] parts = folderPath.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }
                current = next;
            }
        }

        private static LevelPlan[] BuildPlans()
        {
            return new[]
            {
                // Level 01 — Giriş: Sticky ile düz çizgi çekmeyi öğren
                new LevelPlan
                {
                    AssetName = "Level_01",
                    DisplayName = "Level 01 - Giris",
                    SpawnPosition = new Vector2(-7f, 0f),
                    SpawnDirection = Vector2.right,
                    TargetPosition = new Vector2(7f, 0f),
                    InkBudget = 80,
                    AllowIce = false,
                    AllowSticky = true,
                    AllowBouncy = false,
                    Walls = new RectSpec[0],
                    Hazards = new RectSpec[0]
                },
                // Level 02 — İlk Sektirme: Bouncy ile yatay duvarı aş
                new LevelPlan
                {
                    AssetName = "Level_02",
                    DisplayName = "Level 02 - Ilk Sektirme",
                    SpawnPosition = new Vector2(-7f, 3f),
                    SpawnDirection = Vector2.right,
                    TargetPosition = new Vector2(7f, -3f),
                    InkBudget = 70,
                    AllowIce = false,
                    AllowSticky = false,
                    AllowBouncy = true,
                    Walls = new[]
                    {
                        new RectSpec(0f, 0.8f, 10f, 0.8f)
                    },
                    Hazards = new RectSpec[0]
                },
                // Level 03 — Kaygan Yol: Ice ile dikey bariyer etrafından geç
                new LevelPlan
                {
                    AssetName = "Level_03",
                    DisplayName = "Level 03 - Kaygan Yol",
                    SpawnPosition = new Vector2(-7f, -3f),
                    SpawnDirection = Vector2.right,
                    TargetPosition = new Vector2(7f, 3f),
                    InkBudget = 80,
                    AllowIce = true,
                    AllowSticky = false,
                    AllowBouncy = false,
                    Walls = new[]
                    {
                        new RectSpec(-1f, 0f, 0.8f, 5f)
                    },
                    Hazards = new[]
                    {
                        new RectSpec(3f, 2.5f, 2f, 0.8f)
                    }
                },
                // Level 04 — İki Bariyer: Ice + Sticky ile çapraz zigzag
                new LevelPlan
                {
                    AssetName = "Level_04",
                    DisplayName = "Level 04 - Iki Bariyer",
                    SpawnPosition = new Vector2(-7f, 0f),
                    SpawnDirection = Vector2.right,
                    TargetPosition = new Vector2(7f, 3f),
                    InkBudget = 80,
                    AllowIce = true,
                    AllowSticky = true,
                    AllowBouncy = false,
                    Walls = new[]
                    {
                        new RectSpec(-2f, -1.5f, 0.8f, 5f),
                        new RectSpec(3f, 1.5f, 0.8f, 5f)
                    },
                    Hazards = new[]
                    {
                        new RectSpec(0.5f, 0f, 2f, 0.8f)
                    }
                },
                // Level 05 — Labirent Başlangıcı: Sticky + Bouncy ile almaşık geçitler
                new LevelPlan
                {
                    AssetName = "Level_05",
                    DisplayName = "Level 05 - Labirent Baslangici",
                    SpawnPosition = new Vector2(-7f, 0f),
                    SpawnDirection = Vector2.right,
                    TargetPosition = new Vector2(7f, 0f),
                    InkBudget = 70,
                    AllowIce = false,
                    AllowSticky = true,
                    AllowBouncy = true,
                    Walls = new[]
                    {
                        new RectSpec(-4f, 2f, 0.8f, 5f),
                        new RectSpec(-1f, -2f, 0.8f, 5f),
                        new RectSpec(2f, 2f, 0.8f, 5f),
                        new RectSpec(5f, -2f, 0.8f, 5f)
                    },
                    Hazards = new[]
                    {
                        new RectSpec(-2.5f, 0f, 1.5f, 0.8f),
                        new RectSpec(3.5f, 0f, 1.5f, 0.8f)
                    }
                },
                // Level 06 — Hız Tuzağı: Ice + Sticky ile uzun bariyeri aş
                new LevelPlan
                {
                    AssetName = "Level_06",
                    DisplayName = "Level 06 - Hiz Tuzagi",
                    SpawnPosition = new Vector2(-7f, -3f),
                    SpawnDirection = Vector2.right,
                    TargetPosition = new Vector2(7f, 3f),
                    InkBudget = 60,
                    AllowIce = true,
                    AllowSticky = true,
                    AllowBouncy = false,
                    Walls = new[]
                    {
                        new RectSpec(0f, 1f, 12f, 0.8f)
                    },
                    Hazards = new[]
                    {
                        new RectSpec(4.5f, -0.5f, 2f, 2f)
                    }
                },
                // Level 07 — Pinball Köşeleri: Ice + Bouncy ile merkezi kare çerçeve
                new LevelPlan
                {
                    AssetName = "Level_07",
                    DisplayName = "Level 07 - Pinball Koseleri",
                    SpawnPosition = new Vector2(-7f, -3f),
                    SpawnDirection = new Vector2(1f, 1f),
                    TargetPosition = new Vector2(6f, 3f),
                    InkBudget = 70,
                    AllowIce = true,
                    AllowSticky = false,
                    AllowBouncy = true,
                    Walls = new[]
                    {
                        new RectSpec(-3f, 0f, 0.8f, 4.5f),
                        new RectSpec(3f, 0f, 0.8f, 4.5f),
                        new RectSpec(0f, 2.5f, 4.5f, 0.8f),
                        new RectSpec(0f, -2.5f, 4.5f, 0.8f)
                    },
                    Hazards = new[]
                    {
                        new RectSpec(0f, 0f, 1.2f, 1.2f)
                    }
                },
                // Level 08 — Dar Geçit: Sticky + Bouncy ile almaşık dar geçitler
                new LevelPlan
                {
                    AssetName = "Level_08",
                    DisplayName = "Level 08 - Dar Gecit",
                    SpawnPosition = new Vector2(-7f, 3.5f),
                    SpawnDirection = Vector2.right,
                    TargetPosition = new Vector2(7f, -3.5f),
                    InkBudget = 50,
                    AllowIce = false,
                    AllowSticky = true,
                    AllowBouncy = true,
                    Walls = new[]
                    {
                        new RectSpec(-3f, 1.5f, 0.8f, 5.5f),
                        new RectSpec(0f, -1.5f, 0.8f, 5.5f),
                        new RectSpec(3f, 1.5f, 0.8f, 5.5f)
                    },
                    Hazards = new[]
                    {
                        new RectSpec(-1.5f, -3.5f, 2f, 0.8f),
                        new RectSpec(1.5f, 3.5f, 2f, 0.8f)
                    }
                },
                // Level 09 — Spiral: Ice + Sticky + Bouncy ile spiral yol
                new LevelPlan
                {
                    AssetName = "Level_09",
                    DisplayName = "Level 09 - Spiral",
                    SpawnPosition = new Vector2(-7f, 3.8f),
                    SpawnDirection = Vector2.right,
                    TargetPosition = new Vector2(0f, 0f),
                    InkBudget = 60,
                    AllowIce = true,
                    AllowSticky = true,
                    AllowBouncy = true,
                    Walls = new[]
                    {
                        new RectSpec(0f, 3f, 12f, 0.8f),
                        new RectSpec(4.5f, 0f, 0.8f, 5.5f),
                        new RectSpec(0f, -3f, 9f, 0.8f),
                        new RectSpec(-3.5f, 0f, 0.8f, 5.5f),
                        new RectSpec(0f, 1.2f, 5.5f, 0.8f)
                    },
                    Hazards = new[]
                    {
                        new RectSpec(0f, -1.2f, 3f, 0.8f)
                    }
                },
                // Level 10 — Final: Tüm ink tipleri ile çoklu bariyer
                new LevelPlan
                {
                    AssetName = "Level_10",
                    DisplayName = "Level 10 - Final",
                    SpawnPosition = new Vector2(-7.5f, -3.8f),
                    SpawnDirection = Vector2.right,
                    TargetPosition = new Vector2(7.5f, 3.8f),
                    InkBudget = 80,
                    AllowIce = true,
                    AllowSticky = true,
                    AllowBouncy = true,
                    Walls = new[]
                    {
                        new RectSpec(-3.5f, -1.5f, 0.8f, 5f),
                        new RectSpec(0f, 1.5f, 0.8f, 5f),
                        new RectSpec(3.5f, -1.5f, 0.8f, 5f),
                        new RectSpec(-1.7f, 0f, 4f, 0.8f),
                        new RectSpec(1.7f, 0f, 4f, 0.8f)
                    },
                    Hazards = new[]
                    {
                        new RectSpec(-5.5f, 2.8f, 2f, 0.8f),
                        new RectSpec(-1f, -3.5f, 2f, 0.8f),
                        new RectSpec(4.8f, 0f, 2f, 0.8f)
                    }
                }
            };
        }
    }
}
#endif
