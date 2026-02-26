#if UNITY_EDITOR
using TriInkTrack.Level;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TriInkTrack.EditorTools
{
    public static class Step7SetupTool
    {
        private const string GameScenePath = "Assets/_Game/Scenes/GameScene.unity";
        private const string HazardPrefabPath = "Assets/_Game/Prefabs/Hazard.prefab";
        private const string HazardSpritePath = "Assets/kenney_rolling-ball-assets/PNG/Default/block_large.png";

        [MenuItem("Tools/TriInkTrack/Step 7/Build Fail Conditions Setup")]
        public static void BuildStep7()
        {
            EnsureTagExists("Hazard");
            EnsureTagExists("Boundary");

            CreateOrUpdateHazardPrefab();
            ConfigureGameScene();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Step7SetupTool] Step 7 fail conditions setup completed.");
        }

        private static void CreateOrUpdateHazardPrefab()
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(HazardSpritePath);

            GameObject root = new GameObject("Hazard");
            TrySetTag(root, "Hazard");

            SpriteRenderer spriteRenderer = root.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = new Color(0.85f, 0.2f, 0.2f, 1f);

            BoxCollider2D boxCollider = root.AddComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;

            root.AddComponent<HazardZone>();

            PrefabUtility.SaveAsPrefabAsset(root, HazardPrefabPath);
            Object.DestroyImmediate(root);
        }

        private static void ConfigureGameScene()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError($"[Step7SetupTool] Could not open scene at {GameScenePath}");
                return;
            }

            EnsureBoundarySystem();
            EnsureHazardSample();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void EnsureBoundarySystem()
        {
            GameObject boundaryRoot = GameObject.Find("BoundarySystem");
            if (boundaryRoot == null)
            {
                boundaryRoot = new GameObject("BoundarySystem");
            }

            BoundarySystem boundarySystem = boundaryRoot.GetComponent<BoundarySystem>();
            if (boundarySystem == null)
            {
                boundarySystem = boundaryRoot.AddComponent<BoundarySystem>();
            }

            SerializedObject serialized = new SerializedObject(boundarySystem);
            SerializedProperty cameraProperty = serialized.FindProperty("targetCamera");
            if (cameraProperty != null && cameraProperty.objectReferenceValue == null)
            {
                Camera camera = Camera.main;
                if (camera != null)
                {
                    cameraProperty.objectReferenceValue = camera;
                }
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            boundarySystem.RebuildBoundaries();
        }

        private static void EnsureHazardSample()
        {
            GameObject levelRoot = GameObject.Find("LevelRoot");
            if (levelRoot == null)
            {
                levelRoot = new GameObject("LevelRoot");
            }

            Transform existing = levelRoot.transform.Find("Hazard_Sample");
            if (existing != null)
            {
                return;
            }

            GameObject hazardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(HazardPrefabPath);
            if (hazardPrefab == null)
            {
                return;
            }

            GameObject hazard = PrefabUtility.InstantiatePrefab(hazardPrefab) as GameObject;
            if (hazard == null)
            {
                return;
            }

            hazard.name = "Hazard_Sample";
            hazard.transform.SetParent(levelRoot.transform);
            hazard.transform.position = new Vector3(0f, -2.75f, 0f);
            hazard.transform.localScale = new Vector3(1.5f, 0.6f, 1f);
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

        private static void TrySetTag(GameObject target, string tag)
        {
            try
            {
                target.tag = tag;
            }
            catch (UnityException)
            {
                // Tag is created before this method; this is a fallback guard.
            }
        }
    }
}
#endif
