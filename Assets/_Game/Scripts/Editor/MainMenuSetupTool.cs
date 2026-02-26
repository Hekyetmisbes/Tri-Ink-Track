#if UNITY_EDITOR
using TriInkTrack.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace TriInkTrack.EditorTools
{
    public static class MainMenuSetupTool
    {
        private const string MainMenuScenePath = "Assets/_Game/Scenes/MainMenuScene.unity";

        private const string GreenButtonSpritePath  = "Assets/kenney_ui-pack/PNG/Green/Default/button_rectangle_flat.png";
        private const string BlueButtonSpritePath   = "Assets/kenney_ui-pack/PNG/Blue/Default/button_rectangle_flat.png";
        private const string GreyButtonSpritePath   = "Assets/kenney_ui-pack/PNG/Grey/Default/button_rectangle_flat.png";
        private const string PanelSpritePath        = "Assets/kenney_ui-pack/PNG/Grey/Default/button_round_depth_flat.png";
        private const string KenneyFutureFontPath   = "Assets/kenney_ui-pack/Font/Kenney Future.ttf";

        [MenuItem("Tools/TriInkTrack/Main Menu/Setup Main Menu Scene")]
        public static void SetupMainMenuScene()
        {
            // Create or open the scene
            UnityEngine.SceneManagement.Scene scene;
            if (!System.IO.File.Exists(MainMenuScenePath))
            {
                scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, MainMenuScenePath);
                AssetDatabase.Refresh();
                scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            }
            else
            {
                scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            }

            if (!scene.IsValid())
            {
                Debug.LogError($"[MainMenuSetupTool] Could not open scene at {MainMenuScenePath}");
                return;
            }

            EnsureEventSystem();
            Canvas canvas = EnsureCanvas();

            Sprite greenButton = AssetDatabase.LoadAssetAtPath<Sprite>(GreenButtonSpritePath);
            Sprite blueButton  = AssetDatabase.LoadAssetAtPath<Sprite>(BlueButtonSpritePath);
            Sprite greyButton  = AssetDatabase.LoadAssetAtPath<Sprite>(GreyButtonSpritePath);
            Sprite panelSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PanelSpritePath);
            Font   uiFont      = AssetDatabase.LoadAssetAtPath<Font>(KenneyFutureFontPath);
            if (uiFont == null)
            {
                uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            // --- MainRoot ---
            GameObject mainRoot = FindOrCreateChild(canvas.transform, "MainRoot");
            RectTransform mainRootRect = EnsureRectTransform(mainRoot);
            mainRootRect.anchorMin = Vector2.zero;
            mainRootRect.anchorMax = Vector2.one;
            mainRootRect.offsetMin = Vector2.zero;
            mainRootRect.offsetMax = Vector2.zero;

            // --- Title ---
            Text titleText = CreateOrUpdateText(mainRoot.transform, "TitleText", "Tri-Ink Track", uiFont, 72, TextAnchor.UpperCenter);
            RectTransform titleRect = titleText.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot     = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0f, -120f);
            titleRect.sizeDelta = new Vector2(900f, 100f);

            // --- MainPanel ---
            GameObject mainPanel = FindOrCreateChild(mainRoot.transform, "MainPanel");
            RectTransform mainPanelRect = EnsureRectTransform(mainPanel);
            mainPanelRect.anchorMin = new Vector2(0.5f, 0.5f);
            mainPanelRect.anchorMax = new Vector2(0.5f, 0.5f);
            mainPanelRect.pivot     = new Vector2(0.5f, 0.5f);
            mainPanelRect.anchoredPosition = Vector2.zero;
            mainPanelRect.sizeDelta = new Vector2(460f, 440f);

            VerticalLayoutGroup mainLayout = mainPanel.GetComponent<VerticalLayoutGroup>();
            if (mainLayout == null) mainLayout = mainPanel.AddComponent<VerticalLayoutGroup>();
            mainLayout.spacing = 24f;
            mainLayout.childAlignment = TextAnchor.MiddleCenter;
            mainLayout.childControlWidth = true;
            mainLayout.childControlHeight = false;
            mainLayout.childForceExpandWidth = true;
            mainLayout.childForceExpandHeight = false;
            mainLayout.padding = new RectOffset(20, 20, 20, 20);

            Button playButton     = CreateMenuButton(mainPanel.transform, "PlayButton",     "PLAY",     greenButton, uiFont);
            Button settingsButton = CreateMenuButton(mainPanel.transform, "SettingsButton", "SETTINGS", blueButton,  uiFont);
            Button creditsButton  = CreateMenuButton(mainPanel.transform, "CreditsButton",  "CREDITS",  greyButton,  uiFont);
            Button quitButton     = CreateMenuButton(mainPanel.transform, "QuitButton",     "QUIT",     greyButton,  uiFont);

            // --- SettingsPanel ---
            GameObject settingsPanel = FindOrCreateChild(mainRoot.transform, "SettingsPanel");
            RectTransform settingsPanelRect = EnsureRectTransform(settingsPanel);
            settingsPanelRect.anchorMin = new Vector2(0.5f, 0.5f);
            settingsPanelRect.anchorMax = new Vector2(0.5f, 0.5f);
            settingsPanelRect.pivot     = new Vector2(0.5f, 0.5f);
            settingsPanelRect.anchoredPosition = Vector2.zero;
            settingsPanelRect.sizeDelta = new Vector2(640f, 480f);
            Image settingsBg = settingsPanel.GetComponent<Image>();
            if (settingsBg == null) settingsBg = settingsPanel.AddComponent<Image>();
            settingsBg.sprite = panelSprite;
            settingsBg.type   = Image.Type.Sliced;
            settingsBg.color  = new Color(0.12f, 0.12f, 0.12f, 0.92f);

            Text settingsTitle = CreateOrUpdateText(settingsPanel.transform, "Title", "Settings", uiFont, 46, TextAnchor.UpperCenter);
            RectTransform settingsTitleRect = settingsTitle.rectTransform;
            settingsTitleRect.anchorMin = new Vector2(0.5f, 1f);
            settingsTitleRect.anchorMax = new Vector2(0.5f, 1f);
            settingsTitleRect.pivot     = new Vector2(0.5f, 1f);
            settingsTitleRect.anchoredPosition = new Vector2(0f, -35f);
            settingsTitleRect.sizeDelta = new Vector2(560f, 64f);

            Text sfxLabel = CreateOrUpdateText(settingsPanel.transform, "SFXLabel", "SFX Volume", uiFont, 28, TextAnchor.MiddleLeft);
            RectTransform sfxLabelRect = sfxLabel.rectTransform;
            sfxLabelRect.anchorMin = new Vector2(0.5f, 1f);
            sfxLabelRect.anchorMax = new Vector2(0.5f, 1f);
            sfxLabelRect.pivot     = new Vector2(0.5f, 1f);
            sfxLabelRect.anchoredPosition = new Vector2(0f, -130f);
            sfxLabelRect.sizeDelta = new Vector2(560f, 40f);

            Slider sfxSlider = CreateOrUpdateSlider(settingsPanel.transform, "SFXVolumeSlider", new Vector2(0f, -185f), new Vector2(500f, 40f));

            Text muteLabel = CreateOrUpdateText(settingsPanel.transform, "MuteLabel", "Mute", uiFont, 28, TextAnchor.MiddleLeft);
            RectTransform muteLabelRect = muteLabel.rectTransform;
            muteLabelRect.anchorMin = new Vector2(0.5f, 1f);
            muteLabelRect.anchorMax = new Vector2(0.5f, 1f);
            muteLabelRect.pivot     = new Vector2(0.5f, 1f);
            muteLabelRect.anchoredPosition = new Vector2(-60f, -240f);
            muteLabelRect.sizeDelta = new Vector2(300f, 40f);

            Toggle muteToggle = CreateOrUpdateToggle(settingsPanel.transform, "MuteToggle", new Vector2(120f, -240f));

            Button settingsCloseButton = CreatePanelButton(settingsPanel.transform, "CloseButton", "CLOSE", greyButton, uiFont, new Vector2(0f, -340f));

            settingsPanel.SetActive(false);

            // --- CreditsPanel ---
            GameObject creditsPanel = FindOrCreateChild(mainRoot.transform, "CreditsPanel");
            RectTransform creditsPanelRect = EnsureRectTransform(creditsPanel);
            creditsPanelRect.anchorMin = new Vector2(0.5f, 0.5f);
            creditsPanelRect.anchorMax = new Vector2(0.5f, 0.5f);
            creditsPanelRect.pivot     = new Vector2(0.5f, 0.5f);
            creditsPanelRect.anchoredPosition = Vector2.zero;
            creditsPanelRect.sizeDelta = new Vector2(640f, 420f);
            Image creditsBg = creditsPanel.GetComponent<Image>();
            if (creditsBg == null) creditsBg = creditsPanel.AddComponent<Image>();
            creditsBg.sprite = panelSprite;
            creditsBg.type   = Image.Type.Sliced;
            creditsBg.color  = new Color(0.12f, 0.12f, 0.12f, 0.92f);

            Text creditsTitle = CreateOrUpdateText(creditsPanel.transform, "Title", "Credits", uiFont, 46, TextAnchor.UpperCenter);
            RectTransform creditsTitleRect = creditsTitle.rectTransform;
            creditsTitleRect.anchorMin = new Vector2(0.5f, 1f);
            creditsTitleRect.anchorMax = new Vector2(0.5f, 1f);
            creditsTitleRect.pivot     = new Vector2(0.5f, 1f);
            creditsTitleRect.anchoredPosition = new Vector2(0f, -35f);
            creditsTitleRect.sizeDelta = new Vector2(560f, 64f);

            Text bodyText = CreateOrUpdateText(creditsPanel.transform, "BodyText", "Developed with Unity\nKenney.nl assets", uiFont, 28, TextAnchor.MiddleCenter);
            RectTransform bodyRect = bodyText.rectTransform;
            bodyRect.anchorMin = new Vector2(0.5f, 1f);
            bodyRect.anchorMax = new Vector2(0.5f, 1f);
            bodyRect.pivot     = new Vector2(0.5f, 1f);
            bodyRect.anchoredPosition = new Vector2(0f, -140f);
            bodyRect.sizeDelta = new Vector2(560f, 120f);

            Button creditsCloseButton = CreatePanelButton(creditsPanel.transform, "CloseButton", "CLOSE", greyButton, uiFont, new Vector2(0f, -300f));

            creditsPanel.SetActive(false);

            // --- MainMenuController ---
            MainMenuController controller = canvas.GetComponent<MainMenuController>();
            if (controller == null) controller = canvas.gameObject.AddComponent<MainMenuController>();

            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("mainPanel").objectReferenceValue    = mainPanel;
            so.FindProperty("settingsPanel").objectReferenceValue = settingsPanel;
            so.FindProperty("creditsPanel").objectReferenceValue  = creditsPanel;
            so.FindProperty("playButton").objectReferenceValue    = playButton;
            so.FindProperty("settingsButton").objectReferenceValue = settingsButton;
            so.FindProperty("creditsButton").objectReferenceValue  = creditsButton;
            so.FindProperty("quitButton").objectReferenceValue    = quitButton;
            so.FindProperty("sfxVolumeSlider").objectReferenceValue = sfxSlider;
            so.FindProperty("muteToggle").objectReferenceValue    = muteToggle;
            so.FindProperty("settingsCloseButton").objectReferenceValue = settingsCloseButton;
            so.FindProperty("creditsCloseButton").objectReferenceValue  = creditsCloseButton;
            so.ApplyModifiedPropertiesWithoutUndo();

            // --- Wire up button events ---
            ClearAndAdd(playButton,     controller.OnPlayPressed);
            ClearAndAdd(settingsButton, controller.OnSettingsPressed);
            ClearAndAdd(creditsButton,  controller.OnCreditsPressed);
            ClearAndAdd(quitButton,     controller.OnQuitPressed);
            ClearAndAdd(settingsCloseButton, controller.OnCloseSettings);
            ClearAndAdd(creditsCloseButton,  controller.OnCloseCredits);

            // Slider onValueChanged
            sfxSlider.onValueChanged.RemoveAllListeners();
            UnityEventTools.AddPersistentListener(sfxSlider.onValueChanged, controller.OnSfxVolumeChanged);
            EditorUtility.SetDirty(sfxSlider);

            // Toggle onValueChanged
            muteToggle.onValueChanged.RemoveAllListeners();
            UnityEventTools.AddPersistentListener(muteToggle.onValueChanged, controller.OnMuteChanged);
            EditorUtility.SetDirty(muteToggle);

            EditorUtility.SetDirty(controller);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[MainMenuSetupTool] MainMenuScene setup complete.");
        }

        // ── Helpers ─────────────────────────────────────────────────────────────

        private static void ClearAndAdd(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null || action == null) return;
            button.onClick.RemoveAllListeners();
            UnityEventTools.AddPersistentListener(button.onClick, action);
            EditorUtility.SetDirty(button);
        }

        private static Button CreateMenuButton(Transform parent, string name, string label, Sprite sprite, Font font)
        {
            GameObject buttonObject = FindOrCreateChild(parent, name);
            RectTransform rect = EnsureRectTransform(buttonObject);
            rect.sizeDelta = new Vector2(400f, 80f);

            Button button = ConfigureButtonVisual(buttonObject, sprite);
            Text text = CreateOrUpdateText(buttonObject.transform, "Label", label, font, 32, TextAnchor.MiddleCenter);
            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            return button;
        }

        private static Button CreatePanelButton(Transform parent, string name, string label, Sprite sprite, Font font, Vector2 anchoredPosition)
        {
            GameObject buttonObject = FindOrCreateChild(parent, name);
            RectTransform rect = EnsureRectTransform(buttonObject);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot     = new Vector2(0.5f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(220f, 64f);

            Button button = ConfigureButtonVisual(buttonObject, sprite);
            Text text = CreateOrUpdateText(buttonObject.transform, "Label", label, font, 28, TextAnchor.MiddleCenter);
            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            return button;
        }

        private static Slider CreateOrUpdateSlider(Transform parent, string name, Vector2 anchoredPosition, Vector2 size)
        {
            GameObject sliderObject = FindOrCreateChild(parent, name);
            RectTransform rect = EnsureRectTransform(sliderObject);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot     = new Vector2(0.5f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Slider slider = sliderObject.GetComponent<Slider>();
            if (slider == null) slider = sliderObject.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value    = 1f;
            return slider;
        }

        private static Toggle CreateOrUpdateToggle(Transform parent, string name, Vector2 anchoredPosition)
        {
            GameObject toggleObject = FindOrCreateChild(parent, name);
            RectTransform rect = EnsureRectTransform(toggleObject);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot     = new Vector2(0.5f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(60f, 40f);

            Toggle toggle = toggleObject.GetComponent<Toggle>();
            if (toggle == null) toggle = toggleObject.AddComponent<Toggle>();
            toggle.isOn = false;
            return toggle;
        }

        private static Button ConfigureButtonVisual(GameObject buttonObject, Sprite sprite)
        {
            Image image = buttonObject.GetComponent<Image>();
            if (image == null) image = buttonObject.AddComponent<Image>();
            image.sprite = sprite;
            image.type   = Image.Type.Sliced;

            Button button = buttonObject.GetComponent<Button>();
            if (button == null) button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition    = Selectable.Transition.ColorTint;
            return button;
        }

        private static Text CreateOrUpdateText(Transform parent, string name, string value, Font font, int size, TextAnchor alignment)
        {
            GameObject textObject = FindOrCreateChild(parent, name);
            Text text = textObject.GetComponent<Text>();
            if (text == null) text = textObject.AddComponent<Text>();

            text.font      = font;
            text.text      = value;
            text.alignment = alignment;
            text.fontStyle = FontStyle.Bold;
            text.fontSize  = size;
            text.color     = Color.white;
            text.resizeTextForBestFit = false;
            return text;
        }

        private static Canvas EnsureCanvas()
        {
            GameObject canvasObject = GameObject.Find("UI Canvas");
            if (canvasObject == null) canvasObject = new GameObject("UI Canvas");

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            if (canvas == null) canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            if (canvasObject.GetComponent<CanvasScaler>() == null)
            {
                CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode          = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution  = new Vector2(1080f, 1920f);
                scaler.matchWidthOrHeight   = 0.5f;
            }

            if (canvasObject.GetComponent<GraphicRaycaster>() == null)
                canvasObject.AddComponent<GraphicRaycaster>();

            return canvas;
        }

        private static void EnsureEventSystem()
        {
            EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject go = new GameObject("EventSystem");
                eventSystem = go.AddComponent<EventSystem>();
            }

#if ENABLE_INPUT_SYSTEM
            // Remove legacy module if present
            StandaloneInputModule legacy = eventSystem.GetComponent<StandaloneInputModule>();
            if (legacy != null)
            {
                Object.DestroyImmediate(legacy);
            }

            InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputModule == null)
            {
                inputModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }

            bool hasMissingRequiredActions =
                inputModule.actionsAsset == null ||
                inputModule.point == null ||
                inputModule.leftClick == null ||
                inputModule.submit == null ||
                inputModule.cancel == null;

            if (hasMissingRequiredActions)
            {
                inputModule.AssignDefaultActions();
            }

            EditorUtility.SetDirty(inputModule);
#else
            if (eventSystem.GetComponent<StandaloneInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            }
#endif
        }

        private static GameObject FindOrCreateChild(Transform parent, string childName)
        {
            Transform child = parent.Find(childName);
            if (child != null) return child.gameObject;

            GameObject obj = new GameObject(childName);
            obj.transform.SetParent(parent, false);
            return obj;
        }

        private static RectTransform EnsureRectTransform(GameObject obj)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            if (rect == null) rect = obj.AddComponent<RectTransform>();
            return rect;
        }
    }
}
#endif
