#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TriInkTrack.Core;
using TriInkTrack.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TriInkTrack.EditorTools
{
    public static class DevLevelSelectorSceneTool
    {
        private const string SelectorScenePath = "Assets/_Game/Scenes/LevelSelectorScene.unity";
        private const string GameScenePath = "Assets/_Game/Scenes/GameScene.unity";
        private const string LevelDefinitionsFolder = "Assets/_Game/ScriptableObjects/Levels";

        [MenuItem("Tools/TriInkTrack/Dev/Build Level Selector Scene")]
        public static void BuildLevelSelectorScene()
        {
            EnsureFolderExists(Path.GetDirectoryName(SelectorScenePath));

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("[DevLevelSelectorSceneTool] Could not create a new scene.");
                return;
            }

            Camera camera = EnsureCamera();
            if (camera != null)
            {
                camera.backgroundColor = new Color(0.05f, 0.07f, 0.1f, 1f);
            }

            GameObject controllerObject = new GameObject("DevLevelSelector");
            DevLevelSelectorController controller = controllerObject.AddComponent<DevLevelSelectorController>();

            LevelDefinition[] levelDefinitions = LoadLevelDefinitions();
            SerializedObject serialized = new SerializedObject(controller);
            serialized.FindProperty("gameSceneName").stringValue = Path.GetFileNameWithoutExtension(GameScenePath);
            SerializedProperty levelsProp = serialized.FindProperty("levelDefinitions");
            levelsProp.arraySize = levelDefinitions.Length;
            for (int i = 0; i < levelDefinitions.Length; i++)
            {
                levelsProp.GetArrayElementAtIndex(i).objectReferenceValue = levelDefinitions[i];
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, SelectorScenePath);
            EnsureScenesInBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[DevLevelSelectorSceneTool] Scene created: {SelectorScenePath}");
        }

        private static Camera EnsureCamera()
        {
            Camera existing = Object.FindFirstObjectByType<Camera>();
            if (existing != null)
            {
                return existing;
            }

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.nearClipPlane = -10f;
            camera.farClipPlane = 10f;
            camera.orthographicSize = 5f;
            return camera;
        }

        private static LevelDefinition[] LoadLevelDefinitions()
        {
            if (!AssetDatabase.IsValidFolder(LevelDefinitionsFolder))
            {
                return new LevelDefinition[0];
            }

            string[] guids = AssetDatabase.FindAssets("t:LevelDefinition", new[] { LevelDefinitionsFolder });
            List<LevelDefinition> definitions = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath<LevelDefinition>(path))
                .Where(def => def != null)
                .OrderBy(def => def.name)
                .ToList();

            return definitions.ToArray();
        }

        private static void EnsureScenesInBuildSettings()
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

            AddOrEnableScene(scenes, SelectorScenePath);
            AddOrEnableScene(scenes, GameScenePath);

            int selectorIndex = scenes.FindIndex(scene => scene.path == SelectorScenePath);
            if (selectorIndex > 0)
            {
                EditorBuildSettingsScene selector = scenes[selectorIndex];
                scenes.RemoveAt(selectorIndex);
                scenes.Insert(0, selector);
            }

            int gameIndex = scenes.FindIndex(scene => scene.path == GameScenePath);
            if (gameIndex >= 0 && gameIndex != 1)
            {
                EditorBuildSettingsScene game = scenes[gameIndex];
                scenes.RemoveAt(gameIndex);
                int insertIndex = Mathf.Min(1, scenes.Count);
                scenes.Insert(insertIndex, game);
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private static void AddOrEnableScene(List<EditorBuildSettingsScene> scenes, string scenePath)
        {
            int index = scenes.FindIndex(scene => scene.path == scenePath);
            if (index >= 0)
            {
                scenes[index] = new EditorBuildSettingsScene(scenePath, true);
                return;
            }

            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        }

        private static void EnsureFolderExists(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string normalized = folderPath.Replace("\\", "/");
            string[] parts = normalized.Split('/');
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
    }
}
#endif
