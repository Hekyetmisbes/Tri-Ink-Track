#if UNITY_EDITOR
using TriInkTrack.Ball;
using TriInkTrack.Core;
using TriInkTrack.Level;
using TriInkTrack.Target;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TriInkTrack.EditorTools
{
    public static class Step2SetupTool
    {
        private const string GameScenePath = "Assets/_Game/Scenes/GameScene.unity";
        private const string BallPrefabPath = "Assets/_Game/Prefabs/Ball.prefab";
        private const string TargetPrefabPath = "Assets/_Game/Prefabs/Target.prefab";
        private const string WallPrefabPath = "Assets/_Game/Prefabs/Wall.prefab";

        private const string BallSpritePath = "Assets/kenney_rolling-ball-assets/PNG/Default/ball_blue_large.png";
        private const string TargetSpritePath = "Assets/kenney_rolling-ball-assets/PNG/Default/hole_small_end.png";
        private const string WallSpritePath = "Assets/kenney_rolling-ball-assets/PNG/Default/block_large.png";

        [MenuItem("Tools/TriInkTrack/Step 2/Build Ball+Target Setup")]
        public static void BuildStep2()
        {
            EnsureTagExists("Ball");
            CreateOrUpdateBallPrefab();
            CreateOrUpdateTargetPrefab();
            CreateOrUpdateWallPrefab();
            BuildTestLevelInGameScene();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Step2SetupTool] Step 2 prefabs and test level were created/updated.");
        }

        private static void CreateOrUpdateBallPrefab()
        {
            Sprite sprite = LoadSprite(BallSpritePath);
            GameObject root = new GameObject("Ball");
            root.tag = "Ball";

            SpriteRenderer spriteRenderer = root.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;

            Rigidbody2D rb = root.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearDamping = 0.2f;
            rb.angularDamping = 0.5f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            root.AddComponent<CircleCollider2D>();
            root.AddComponent<BallController>();

            SavePrefab(root, BallPrefabPath);
        }

        private static void CreateOrUpdateTargetPrefab()
        {
            Sprite sprite = LoadSprite(TargetSpritePath);
            GameObject root = new GameObject("Target");

            SpriteRenderer spriteRenderer = root.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;

            CircleCollider2D col = root.AddComponent<CircleCollider2D>();
            col.isTrigger = true;

            root.AddComponent<TargetGoal>();

            SavePrefab(root, TargetPrefabPath);
        }

        private static void CreateOrUpdateWallPrefab()
        {
            Sprite sprite = LoadSprite(WallSpritePath);
            GameObject root = new GameObject("Wall");

            SpriteRenderer spriteRenderer = root.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            spriteRenderer.size = new Vector2(4f, 1f);

            BoxCollider2D boxCollider = root.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(4f, 1f);

            SavePrefab(root, WallPrefabPath);
        }

        private static void BuildTestLevelInGameScene()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError($"[Step2SetupTool] Could not open scene at {GameScenePath}");
                return;
            }

            GameObject levelRoot = GameObject.Find("LevelRoot");
            if (levelRoot == null)
            {
                levelRoot = new GameObject("LevelRoot");
            }

            GameObject existing = GameObject.Find("Step2_TestLevel");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject testLevel = new GameObject("Step2_TestLevel");
            testLevel.transform.SetParent(levelRoot.transform);
            testLevel.transform.localPosition = Vector3.zero;

            CreateWalls(testLevel.transform);
            CreateBall(testLevel.transform);
            CreateTarget(testLevel.transform);
            EnsureGameManagerAutostart();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void CreateWalls(Transform parent)
        {
            GameObject wallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(WallPrefabPath);
            CreateWall(parent, wallPrefab, "Wall_Top", new Vector3(0f, 4.5f, 0f), new Vector2(12f, 1f));
            CreateWall(parent, wallPrefab, "Wall_Bottom", new Vector3(0f, -4.5f, 0f), new Vector2(12f, 1f));
            CreateWall(parent, wallPrefab, "Wall_Left", new Vector3(-6f, 0f, 0f), new Vector2(1f, 9f));
            CreateWall(parent, wallPrefab, "Wall_Right", new Vector3(6f, 0f, 0f), new Vector2(1f, 9f));
        }

        private static void CreateWall(Transform parent, GameObject wallPrefab, string name, Vector3 position, Vector2 size)
        {
            GameObject wall = PrefabUtility.InstantiatePrefab(wallPrefab) as GameObject;
            wall.name = name;
            wall.transform.SetParent(parent);
            wall.transform.position = position;
            wall.transform.localScale = Vector3.one;

            SpriteRenderer renderer = wall.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.drawMode = SpriteDrawMode.Tiled;
                renderer.size = size;
            }

            BoxCollider2D boxCollider = wall.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                boxCollider.size = size;
            }
        }

        private static void CreateBall(Transform parent)
        {
            GameObject ballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BallPrefabPath);
            GameObject ball = PrefabUtility.InstantiatePrefab(ballPrefab) as GameObject;
            ball.transform.SetParent(parent);
            ball.transform.position = new Vector3(-4f, 0f, 0f);
            ball.transform.localScale = Vector3.one;
            ball.tag = "Ball";

            GameObject spawnPoint = new GameObject("BallSpawnPoint");
            spawnPoint.transform.SetParent(parent);
            spawnPoint.transform.position = new Vector3(-4f, 0f, 0f);
            BallSpawnPoint marker = spawnPoint.AddComponent<BallSpawnPoint>();
            SerializedObject markerSerialized = new SerializedObject(marker);
            markerSerialized.FindProperty("initialDirection").vector2Value = Vector2.right;
            markerSerialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateTarget(Transform parent)
        {
            GameObject targetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(TargetPrefabPath);
            GameObject target = PrefabUtility.InstantiatePrefab(targetPrefab) as GameObject;
            target.transform.SetParent(parent);
            target.transform.position = new Vector3(4f, 0f, 0f);
            target.transform.localScale = Vector3.one;
        }

        private static void EnsureGameManagerAutostart()
        {
            GameManager manager = Object.FindFirstObjectByType<GameManager>();
            if (manager == null)
            {
                return;
            }

            SerializedObject serialized = new SerializedObject(manager);
            SerializedProperty autoStart = serialized.FindProperty("autoStartOnSceneLoad");
            if (autoStart != null)
            {
                autoStart.boolValue = true;
                serialized.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static Sprite LoadSprite(string path)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
            {
                Debug.LogError($"[Step2SetupTool] Missing sprite at: {path}");
            }
            return sprite;
        }

        private static void SavePrefab(GameObject root, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static void EnsureTagExists(string tagName)
        {
            foreach (string existingTag in InternalEditorUtility.tags)
            {
                if (existingTag == tagName)
                {
                    return;
                }
            }

            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            int newIndex = tagsProp.arraySize;
            tagsProp.InsertArrayElementAtIndex(newIndex);
            tagsProp.GetArrayElementAtIndex(newIndex).stringValue = tagName;
            tagManager.ApplyModifiedProperties();
        }
    }
}
#endif
