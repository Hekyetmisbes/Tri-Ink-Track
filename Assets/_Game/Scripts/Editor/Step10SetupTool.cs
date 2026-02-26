#if UNITY_EDITOR
using TriInkTrack.Audio;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TriInkTrack.EditorTools
{
    public static class Step10SetupTool
    {
        private const string GameScenePath = "Assets/_Game/Scenes/GameScene.unity";

        private const string DrawStartClipPath   = "Assets/kenney_interface-sounds/Audio/scratch_003.ogg";
        private const string DrawEndClipPath      = "Assets/kenney_interface-sounds/Audio/drop_002.ogg";
        private const string InkSwitchClipPath    = "Assets/kenney_interface-sounds/Audio/switch_003.ogg";
        private const string BouncyHitClipPath    = "Assets/kenney_interface-sounds/Audio/glass_002.ogg";
        private const string WinClipPath          = "Assets/kenney_interface-sounds/Audio/confirmation_002.ogg";
        private const string FailClipPath         = "Assets/kenney_interface-sounds/Audio/error_004.ogg";
        private const string LevelStartClipPath   = "Assets/kenney_interface-sounds/Audio/maximize_003.ogg";
        private const string ButtonClickClipPath  = "Assets/kenney_interface-sounds/Audio/click_003.ogg";

        [MenuItem("Tools/TriInkTrack/Step 10/Setup Audio Manager")]
        public static void SetupAudio()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError($"[Step10SetupTool] Could not open scene at {GameScenePath}");
                return;
            }

            AudioManager audioManager = EnsureAudioManager();

            AudioClip drawStart  = LoadClip(DrawStartClipPath);
            AudioClip drawEnd    = LoadClip(DrawEndClipPath);
            AudioClip inkSwitch  = LoadClip(InkSwitchClipPath);
            AudioClip bouncyHit  = LoadClip(BouncyHitClipPath);
            AudioClip win        = LoadClip(WinClipPath);
            AudioClip fail       = LoadClip(FailClipPath);
            AudioClip levelStart = LoadClip(LevelStartClipPath);
            AudioClip btnClick   = LoadClip(ButtonClickClipPath);

            SerializedObject serialized = new SerializedObject(audioManager);
            SetClipProperty(serialized, "drawStartClip",  drawStart);
            SetClipProperty(serialized, "drawEndClip",    drawEnd);
            SetClipProperty(serialized, "inkSwitchClip",  inkSwitch);
            SetClipProperty(serialized, "bouncyHitClip",  bouncyHit);
            SetClipProperty(serialized, "winClip",        win);
            SetClipProperty(serialized, "failClip",       fail);
            SetClipProperty(serialized, "levelStartClip", levelStart);
            SetClipProperty(serialized, "buttonClickClip", btnClick);
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(audioManager);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Step10SetupTool] AudioManager set up successfully. All SFX clips assigned.");
            LogMissingClips(drawStart, drawEnd, inkSwitch, bouncyHit, win, fail, levelStart, btnClick);
        }

        private static AudioManager EnsureAudioManager()
        {
            AudioManager existing = Object.FindFirstObjectByType<AudioManager>();
            if (existing != null)
            {
                return existing;
            }

            GameObject audioObject = GameObject.Find("AudioManager");
            if (audioObject == null)
            {
                audioObject = new GameObject("AudioManager");
            }

            AudioManager manager = audioObject.GetComponent<AudioManager>();
            if (manager == null)
            {
                manager = audioObject.AddComponent<AudioManager>();
            }

            return manager;
        }

        private static AudioClip LoadClip(string path)
        {
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip == null)
            {
                Debug.LogWarning($"[Step10SetupTool] Audio clip not found at: {path}");
            }
            return clip;
        }

        private static void SetClipProperty(SerializedObject serialized, string propertyName, AudioClip clip)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property != null)
            {
                property.objectReferenceValue = clip;
            }
        }

        private static void LogMissingClips(params AudioClip[] clips)
        {
            int missing = 0;
            foreach (AudioClip clip in clips)
            {
                if (clip == null)
                {
                    missing++;
                }
            }

            if (missing > 0)
            {
                Debug.LogWarning($"[Step10SetupTool] {missing} clip(s) could not be found. Check the kenney_interface-sounds asset folder.");
            }
        }
    }
}
#endif
