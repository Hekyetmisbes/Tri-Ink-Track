#if UNITY_EDITOR
using TriInkTrack.Drawing;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TriInkTrack.EditorTools
{
    public static class Step3SetupTool
    {
        private const string GameScenePath = "Assets/_Game/Scenes/GameScene.unity";
        private const string InkLinePrefabPath = "Assets/_Game/Prefabs/InkLine.prefab";
        private const string AutoSetupSessionKey = "TriInkTrack.Step3.AutoSetupDone";

        [InitializeOnLoadMethod]
        private static void TryAutoSetupWhenMissing()
        {
            if (SessionState.GetBool(AutoSetupSessionKey, false))
            {
                return;
            }

            SessionState.SetBool(AutoSetupSessionKey, true);
            if (AssetDatabase.LoadAssetAtPath<GameObject>(InkLinePrefabPath) != null)
            {
                return;
            }

            BuildStep3();
        }

        [MenuItem("Tools/TriInkTrack/Step 3/Build DrawSystem Setup")]
        public static void BuildStep3()
        {
            CreateOrUpdateInkLinePrefab();
            ConfigureGameScene();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Step3SetupTool] Step 3 draw system setup completed.");
        }

        private static void CreateOrUpdateInkLinePrefab()
        {
            GameObject root = new GameObject("InkLine");
            LineRenderer lineRenderer = root.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.numCapVertices = 4;
            lineRenderer.numCornerVertices = 2;
            lineRenderer.alignment = LineAlignment.View;
            lineRenderer.positionCount = 0;

            Shader shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                lineRenderer.sharedMaterial = new Material(shader);
            }

            EdgeCollider2D edgeCollider = root.AddComponent<EdgeCollider2D>();
            edgeCollider.points = new Vector2[0];

            root.AddComponent<InkLine>();
            PrefabUtility.SaveAsPrefabAsset(root, InkLinePrefabPath);
            Object.DestroyImmediate(root);
        }

        private static void ConfigureGameScene()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError($"[Step3SetupTool] Could not open scene at: {GameScenePath}");
                return;
            }

            DrawSystem drawSystem = Object.FindFirstObjectByType<DrawSystem>();
            if (drawSystem == null)
            {
                GameObject drawSystemObject = new GameObject("DrawSystem");
                drawSystem = drawSystemObject.AddComponent<DrawSystem>();
            }

            GameObject lineRoot = GameObject.Find("InkLines");
            if (lineRoot == null)
            {
                lineRoot = new GameObject("InkLines");
            }

            InkLine prefab = AssetDatabase.LoadAssetAtPath<InkLine>(InkLinePrefabPath);
            if (prefab == null)
            {
                Debug.LogError($"[Step3SetupTool] Missing prefab at: {InkLinePrefabPath}");
                return;
            }

            SerializedObject serialized = new SerializedObject(drawSystem);
            SerializedProperty gameplayCamera = serialized.FindProperty("gameplayCamera");
            SerializedProperty inkLinePrefab = serialized.FindProperty("inkLinePrefab");
            SerializedProperty lineRootProp = serialized.FindProperty("lineRoot");

            if (gameplayCamera != null && gameplayCamera.objectReferenceValue == null)
            {
                Camera camera = Camera.main;
                if (camera != null)
                {
                    gameplayCamera.objectReferenceValue = camera;
                }
            }

            if (inkLinePrefab != null)
            {
                inkLinePrefab.objectReferenceValue = prefab;
            }

            if (lineRootProp != null)
            {
                lineRootProp.objectReferenceValue = lineRoot.transform;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }
}
#endif
