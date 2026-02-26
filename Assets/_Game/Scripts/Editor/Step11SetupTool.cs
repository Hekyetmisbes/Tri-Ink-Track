#if UNITY_EDITOR
using TriInkTrack.Audio;
using TriInkTrack.Core;
using TriInkTrack.Level;
using TriInkTrack.Vfx;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TriInkTrack.EditorTools
{
    public static class Step11SetupTool
    {
        private const string GameScenePath = "Assets/_Game/Scenes/GameScene.unity";

        [MenuItem("Tools/TriInkTrack/Step 11/Setup VFX & Polish")]
        public static void SetupVfxAndPolish()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError($"[Step11SetupTool] Could not open scene at {GameScenePath}");
                return;
            }

            EnsureVfxManager();
            AddTargetPulseToTargets();
            AddTrailRendererToBall();
            ValidateRequiredSystems();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.Refresh();

            Debug.Log("[Step11SetupTool] VFX & Polish setup complete.");
        }

        private static void EnsureVfxManager()
        {
            VfxManager existing = Object.FindFirstObjectByType<VfxManager>();
            if (existing != null)
            {
                Debug.Log("[Step11SetupTool] VfxManager already exists in scene.");
                return;
            }

            GameObject go = new GameObject("VfxManager");
            go.AddComponent<VfxManager>();
            EditorUtility.SetDirty(go);
            Debug.Log("[Step11SetupTool] Created VfxManager. Assign ParticleSystem references in Inspector for best results.");
        }

        private static void AddTargetPulseToTargets()
        {
            GameObject[] targets;
            try
            {
                targets = GameObject.FindGameObjectsWithTag("Target");
            }
            catch (UnityException)
            {
                Debug.LogWarning("[Step11SetupTool] Tag 'Target' is not defined. Add it via Edit > Project Settings > Tags and Layers, then re-run this tool.");
                return;
            }

            int added = 0;
            foreach (GameObject target in targets)
            {
                if (target.GetComponent<TargetPulse>() == null)
                {
                    target.AddComponent<TargetPulse>();
                    EditorUtility.SetDirty(target);
                    added++;
                }
            }

            if (added > 0)
                Debug.Log($"[Step11SetupTool] Added TargetPulse to {added} Target(s).");
            else
                Debug.Log("[Step11SetupTool] All Target objects already have TargetPulse (or none tagged 'Target' found).");
        }

        private static void AddTrailRendererToBall()
        {
            GameObject ball = GameObject.FindGameObjectWithTag("Ball");
            if (ball == null)
            {
                Debug.LogWarning("[Step11SetupTool] No GameObject tagged 'Ball' found.");
                return;
            }

            TrailRenderer trail = ball.GetComponent<TrailRenderer>();
            if (trail != null)
            {
                Debug.Log("[Step11SetupTool] Ball already has a TrailRenderer.");
                return;
            }

            trail = ball.AddComponent<TrailRenderer>();
            trail.time = 0.25f;
            trail.startWidth = 0.15f;
            trail.endWidth = 0f;
            trail.minVertexDistance = 0.05f;
            trail.enabled = false;

            Shader shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                trail.material = new Material(shader);
                trail.startColor = new Color(0.6f, 0.85f, 1f, 0.8f);
                trail.endColor = new Color(0.6f, 0.85f, 1f, 0f);
            }

            EditorUtility.SetDirty(ball);
            Debug.Log("[Step11SetupTool] Added TrailRenderer to Ball. Assign it to BallController's TrailRenderer field in Inspector.");
        }

        private static void ValidateRequiredSystems()
        {
            AudioManager audioManager = Object.FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                Debug.LogWarning("[Step11SetupTool] AudioManager not found in scene. Run Step 10 setup first.");
            else
                Debug.Log("[Step11SetupTool] AudioManager OK.");

            LevelManager levelManager = Object.FindFirstObjectByType<LevelManager>();
            if (levelManager == null)
                Debug.LogWarning("[Step11SetupTool] LevelManager not found in scene. Run Step 9 setup first.");
            else
                Debug.Log("[Step11SetupTool] LevelManager OK.");
        }
    }
}
#endif
