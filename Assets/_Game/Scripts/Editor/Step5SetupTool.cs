#if UNITY_EDITOR
using TriInkTrack.Drawing;
using TriInkTrack.Ink;
using TriInkTrack.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TriInkTrack.EditorTools
{
    public static class Step5SetupTool
    {
        private const string AutoSetupSessionKey = "TriInkTrack.Step5.AutoSetupDone";
        private const string GameScenePath = "Assets/_Game/Scenes/GameScene.unity";
        private const string InkLinePrefabPath = "Assets/_Game/Prefabs/InkLine.prefab";
        private const string PhysicsMaterialsFolder = "Assets/_Game/PhysicsMaterials";
        private const string IceMatPath = PhysicsMaterialsFolder + "/Ice.physicsMaterial2D";
        private const string StickyMatPath = PhysicsMaterialsFolder + "/Sticky.physicsMaterial2D";
        private const string BouncyMatPath = PhysicsMaterialsFolder + "/Bouncy.physicsMaterial2D";

        private const string IceButtonSpritePath = "Assets/kenney_ui-pack/PNG/Blue/Default/button_rectangle_flat.png";
        private const string StickyButtonSpritePath = "Assets/kenney_ui-pack/PNG/Yellow/Default/button_rectangle_flat.png";
        private const string BouncyButtonSpritePath = "Assets/kenney_ui-pack/PNG/Green/Default/button_rectangle_flat.png";

        [InitializeOnLoadMethod]
        private static void TryAutoSetupWhenMissing()
        {
            if (SessionState.GetBool(AutoSetupSessionKey, false))
            {
                return;
            }

            SessionState.SetBool(AutoSetupSessionKey, true);
            BuildStep5();
        }

        [MenuItem("Tools/TriInkTrack/Step 5/Build Ink Types Setup")]
        public static void BuildStep5()
        {
            EnsureFolderExists(PhysicsMaterialsFolder);
            PhysicsMaterial2D ice = CreateOrUpdatePhysicsMaterial(IceMatPath, 0.02f, 0.05f);
            PhysicsMaterial2D sticky = CreateOrUpdatePhysicsMaterial(StickyMatPath, 0.9f, 0f);
            PhysicsMaterial2D bouncy = CreateOrUpdatePhysicsMaterial(BouncyMatPath, 0.1f, 0.9f);

            ConfigureInkLinePrefab(ice, sticky, bouncy);
            ConfigureGameScene();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Step5SetupTool] Step 5 setup completed.");
        }

        private static void ConfigureInkLinePrefab(PhysicsMaterial2D ice, PhysicsMaterial2D sticky, PhysicsMaterial2D bouncy)
        {
            InkLine inkLine = AssetDatabase.LoadAssetAtPath<InkLine>(InkLinePrefabPath);
            if (inkLine == null)
            {
                Debug.LogError($"[Step5SetupTool] Missing InkLine prefab: {InkLinePrefabPath}");
                return;
            }

            SerializedObject serialized = new SerializedObject(inkLine);
            serialized.FindProperty("iceMaterial").objectReferenceValue = ice;
            serialized.FindProperty("stickyMaterial").objectReferenceValue = sticky;
            serialized.FindProperty("bouncyMaterial").objectReferenceValue = bouncy;
            serialized.FindProperty("currentType").enumValueIndex = (int)InkType.Ice;
            serialized.FindProperty("iceColor").colorValue = HexToColor("#00BFFF");
            serialized.FindProperty("stickyColor").colorValue = HexToColor("#FF8C00");
            serialized.FindProperty("bouncyColor").colorValue = HexToColor("#32CD32");
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(inkLine);
        }

        private static void ConfigureGameScene()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError($"[Step5SetupTool] Could not open scene at {GameScenePath}");
                return;
            }

            DrawSystem drawSystem = Object.FindFirstObjectByType<DrawSystem>();
            if (drawSystem == null)
            {
                Debug.LogError("[Step5SetupTool] DrawSystem is missing in GameScene.");
                return;
            }

            GameObject inkInventoryObject = GameObject.Find("InkInventory");
            if (inkInventoryObject == null)
            {
                inkInventoryObject = new GameObject("InkInventory");
            }

            InkInventory inkInventory = inkInventoryObject.GetComponent<InkInventory>();
            if (inkInventory == null)
            {
                inkInventory = inkInventoryObject.AddComponent<InkInventory>();
            }

            SerializedObject drawSerialized = new SerializedObject(drawSystem);
            drawSerialized.FindProperty("inkInventory").objectReferenceValue = inkInventory;
            drawSerialized.ApplyModifiedPropertiesWithoutUndo();

            Canvas canvas = EnsureCanvas();
            EnsureEventSystem();
            SetupInkSelectionUI(canvas.transform, inkInventory);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void SetupInkSelectionUI(Transform canvas, InkInventory inkInventory)
        {
            GameObject bar = FindOrCreateChild(canvas, "InkSelectionBar");
            RectTransform barRect = EnsureRectTransform(bar);
            barRect.anchorMin = new Vector2(0.5f, 0f);
            barRect.anchorMax = new Vector2(0.5f, 0f);
            barRect.pivot = new Vector2(0.5f, 0f);
            barRect.sizeDelta = new Vector2(620f, 180f);
            barRect.anchoredPosition = new Vector2(0f, 24f);

            HorizontalLayoutGroup layout = bar.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = bar.AddComponent<HorizontalLayoutGroup>();
            }
            layout.spacing = 20f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            Sprite iceSprite = AssetDatabase.LoadAssetAtPath<Sprite>(IceButtonSpritePath);
            Sprite stickySprite = AssetDatabase.LoadAssetAtPath<Sprite>(StickyButtonSpritePath);
            Sprite bouncySprite = AssetDatabase.LoadAssetAtPath<Sprite>(BouncyButtonSpritePath);

            Button iceButton = CreateOrUpdateInkButton(bar.transform, "IceButton", "ICE", iceSprite);
            Button stickyButton = CreateOrUpdateInkButton(bar.transform, "StickyButton", "STICKY", stickySprite);
            Button bouncyButton = CreateOrUpdateInkButton(bar.transform, "BouncyButton", "BOUNCY", bouncySprite);

            GameObject meter = FindOrCreateChild(canvas, "InkMeter");
            RectTransform meterRect = EnsureRectTransform(meter);
            meterRect.anchorMin = new Vector2(0.5f, 0f);
            meterRect.anchorMax = new Vector2(0.5f, 0f);
            meterRect.pivot = new Vector2(0.5f, 0f);
            meterRect.sizeDelta = new Vector2(500f, 24f);
            meterRect.anchoredPosition = new Vector2(0f, 210f);

            Image meterImage = meter.GetComponent<Image>();
            if (meterImage == null)
            {
                meterImage = meter.AddComponent<Image>();
            }
            meterImage.sprite = iceSprite;
            meterImage.type = Image.Type.Filled;
            meterImage.fillMethod = Image.FillMethod.Horizontal;
            meterImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            meterImage.fillAmount = 1f;
            meterImage.color = new Color(0.2f, 0.8f, 1f, 1f);

            InkSelectionUI selectionUI = bar.GetComponent<InkSelectionUI>();
            if (selectionUI == null)
            {
                selectionUI = bar.AddComponent<InkSelectionUI>();
            }

            SerializedObject uiSerialized = new SerializedObject(selectionUI);
            uiSerialized.FindProperty("inkInventory").objectReferenceValue = inkInventory;
            uiSerialized.FindProperty("iceButton").objectReferenceValue = iceButton;
            uiSerialized.FindProperty("stickyButton").objectReferenceValue = stickyButton;
            uiSerialized.FindProperty("bouncyButton").objectReferenceValue = bouncyButton;
            uiSerialized.FindProperty("inkFillImage").objectReferenceValue = meterImage;
            uiSerialized.ApplyModifiedPropertiesWithoutUndo();

            ConfigureButtonOnClick(iceButton, selectionUI.SelectIce);
            ConfigureButtonOnClick(stickyButton, selectionUI.SelectSticky);
            ConfigureButtonOnClick(bouncyButton, selectionUI.SelectBouncy);
        }

        private static Button CreateOrUpdateInkButton(Transform parent, string name, string label, Sprite sprite)
        {
            GameObject buttonObject = FindOrCreateChild(parent, name);
            RectTransform rect = EnsureRectTransform(buttonObject);
            rect.sizeDelta = new Vector2(180f, 72f);

            Image image = buttonObject.GetComponent<Image>();
            if (image == null)
            {
                image = buttonObject.AddComponent<Image>();
            }
            image.sprite = sprite;
            image.type = Image.Type.Sliced;

            Button button = buttonObject.GetComponent<Button>();
            if (button == null)
            {
                button = buttonObject.AddComponent<Button>();
            }
            button.targetGraphic = image;
            button.transition = Selectable.Transition.ColorTint;

            GameObject textObject = FindOrCreateChild(buttonObject.transform, "Label");
            RectTransform textRect = EnsureRectTransform(textObject);
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text text = textObject.GetComponent<Text>();
            if (text == null)
            {
                text = textObject.AddComponent<Text>();
            }
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = FontStyle.Bold;
            text.color = Color.white;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 18;
            text.resizeTextMaxSize = 28;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return button;
        }

        private static Canvas EnsureCanvas()
        {
            GameObject canvasObject = GameObject.Find("UI Canvas");
            if (canvasObject == null)
            {
                canvasObject = new GameObject("UI Canvas");
            }

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = canvasObject.AddComponent<Canvas>();
            }
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            if (canvasObject.GetComponent<CanvasScaler>() == null)
            {
                CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.matchWidthOrHeight = 0.5f;
            }

            if (canvasObject.GetComponent<GraphicRaycaster>() == null)
            {
                canvasObject.AddComponent<GraphicRaycaster>();
            }

            return canvas;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        private static PhysicsMaterial2D CreateOrUpdatePhysicsMaterial(string path, float friction, float bounciness)
        {
            PhysicsMaterial2D material = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(path);
            if (material == null)
            {
                material = new PhysicsMaterial2D();
                AssetDatabase.CreateAsset(material, path);
            }

            material.friction = friction;
            material.bounciness = bounciness;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static GameObject FindOrCreateChild(Transform parent, string childName)
        {
            Transform child = parent.Find(childName);
            if (child != null)
            {
                return child.gameObject;
            }

            GameObject obj = new GameObject(childName);
            obj.transform.SetParent(parent, false);
            return obj;
        }

        private static RectTransform EnsureRectTransform(GameObject obj)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            if (rect == null)
            {
                rect = obj.AddComponent<RectTransform>();
            }
            return rect;
        }

        private static void EnsureFolderExists(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string[] segments = folderPath.Split('/');
            string current = segments[0];
            for (int i = 1; i < segments.Length; i++)
            {
                string next = current + "/" + segments[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, segments[i]);
                }
                current = next;
            }
        }

        private static Color HexToColor(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }
            return Color.white;
        }

        private static void ConfigureButtonOnClick(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null || action == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            UnityEventTools.AddPersistentListener(button.onClick, action);
            EditorUtility.SetDirty(button);
        }
    }
}
#endif
