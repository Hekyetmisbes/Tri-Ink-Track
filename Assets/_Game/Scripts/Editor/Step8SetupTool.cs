#if UNITY_EDITOR
using TriInkTrack.Core;
using TriInkTrack.Ink;
using TriInkTrack.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace TriInkTrack.EditorTools
{
    public static class Step8SetupTool
    {
        private const string GameScenePath = "Assets/_Game/Scenes/GameScene.unity";

        private const string BlueButtonSpritePath = "Assets/kenney_ui-pack/PNG/Blue/Default/button_rectangle_flat.png";
        private const string YellowButtonSpritePath = "Assets/kenney_ui-pack/PNG/Yellow/Default/button_rectangle_flat.png";
        private const string GreenButtonSpritePath = "Assets/kenney_ui-pack/PNG/Green/Default/button_rectangle_flat.png";
        private const string GreyButtonSpritePath = "Assets/kenney_ui-pack/PNG/Grey/Default/button_rectangle_flat.png";
        private const string PanelSpritePath = "Assets/kenney_ui-pack/PNG/Grey/Default/button_round_depth_flat.png";
        private const string StarSpritePath = "Assets/kenney_rolling-ball-assets/PNG/Default/star.png";
        private const string KenneyFutureFontPath = "Assets/kenney_ui-pack/Font/Kenney Future.ttf";

        [MenuItem("Tools/TriInkTrack/Step 8/Build UI HUD Setup")]
        public static void BuildStep8()
        {
            Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError($"[Step8SetupTool] Could not open scene at {GameScenePath}");
                return;
            }

            EnsureEventSystem();
            Canvas canvas = EnsureCanvas();
            GameManager gameManager = EnsureGameManager();
            InkInventory inkInventory = EnsureInkInventory();

            Sprite blueButton = AssetDatabase.LoadAssetAtPath<Sprite>(BlueButtonSpritePath);
            Sprite yellowButton = AssetDatabase.LoadAssetAtPath<Sprite>(YellowButtonSpritePath);
            Sprite greenButton = AssetDatabase.LoadAssetAtPath<Sprite>(GreenButtonSpritePath);
            Sprite greyButton = AssetDatabase.LoadAssetAtPath<Sprite>(GreyButtonSpritePath);
            Sprite panelSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PanelSpritePath);
            Sprite starSprite = AssetDatabase.LoadAssetAtPath<Sprite>(StarSpritePath);
            Font uiFont = AssetDatabase.LoadAssetAtPath<Font>(KenneyFutureFontPath);
            if (uiFont == null)
            {
                uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            GameObject hudRoot = FindOrCreateChild(canvas.transform, "HUDRoot");
            RectTransform hudRootRect = EnsureRectTransform(hudRoot);
            hudRootRect.anchorMin = Vector2.zero;
            hudRootRect.anchorMax = Vector2.one;
            hudRootRect.offsetMin = Vector2.zero;
            hudRootRect.offsetMax = Vector2.zero;

            // Reuse existing Step 5 elements when available.
            GameObject inkBar = FindOrCreateChild(canvas.transform, "InkSelectionBar");
            GameObject inkMeter = FindOrCreateChild(canvas.transform, "InkMeter");
            inkBar.transform.SetParent(hudRoot.transform, false);
            inkMeter.transform.SetParent(hudRoot.transform, false);

            RectTransform inkBarRect = EnsureRectTransform(inkBar);
            inkBarRect.anchorMin = new Vector2(0.5f, 0f);
            inkBarRect.anchorMax = new Vector2(0.5f, 0f);
            inkBarRect.pivot = new Vector2(0.5f, 0f);
            inkBarRect.anchoredPosition = new Vector2(0f, 36f);
            inkBarRect.sizeDelta = new Vector2(700f, 110f);

            HorizontalLayoutGroup barLayout = inkBar.GetComponent<HorizontalLayoutGroup>();
            if (barLayout == null)
            {
                barLayout = inkBar.AddComponent<HorizontalLayoutGroup>();
            }
            barLayout.spacing = 20f;
            barLayout.childAlignment = TextAnchor.MiddleCenter;
            barLayout.childControlHeight = false;
            barLayout.childControlWidth = false;
            barLayout.childForceExpandHeight = false;
            barLayout.childForceExpandWidth = false;

            Button iceButton = CreateOrUpdateLabeledButton(inkBar.transform, "IceButton", "ICE", blueButton, uiFont);
            Button stickyButton = CreateOrUpdateLabeledButton(inkBar.transform, "StickyButton", "STICKY", yellowButton, uiFont);
            Button bouncyButton = CreateOrUpdateLabeledButton(inkBar.transform, "BouncyButton", "BOUNCY", greenButton, uiFont);

            RectTransform inkMeterRect = EnsureRectTransform(inkMeter);
            inkMeterRect.anchorMin = new Vector2(0.5f, 1f);
            inkMeterRect.anchorMax = new Vector2(0.5f, 1f);
            inkMeterRect.pivot = new Vector2(0.5f, 1f);
            inkMeterRect.anchoredPosition = new Vector2(0f, -24f);
            inkMeterRect.sizeDelta = new Vector2(560f, 30f);

            Image inkMeterImage = inkMeter.GetComponent<Image>();
            if (inkMeterImage == null)
            {
                inkMeterImage = inkMeter.AddComponent<Image>();
            }
            inkMeterImage.sprite = blueButton;
            inkMeterImage.type = Image.Type.Filled;
            inkMeterImage.fillMethod = Image.FillMethod.Horizontal;
            inkMeterImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            inkMeterImage.fillAmount = 1f;
            inkMeterImage.color = new Color(0.2f, 0.8f, 1f, 1f);

            Slider inkSlider = inkMeter.GetComponent<Slider>();
            if (inkSlider == null)
            {
                inkSlider = inkMeter.AddComponent<Slider>();
            }
            inkSlider.minValue = 0f;
            inkSlider.maxValue = 1f;
            inkSlider.value = 1f;
            inkSlider.targetGraphic = inkMeterImage;
            inkSlider.fillRect = inkMeterImage.rectTransform;

            Text inkValueText = CreateOrUpdateText(inkMeter.transform, "InkValueText", "INK 100 / 100", uiFont, 20, TextAnchor.MiddleCenter);
            RectTransform inkValueRect = inkValueText.rectTransform;
            inkValueRect.anchorMin = Vector2.zero;
            inkValueRect.anchorMax = Vector2.one;
            inkValueRect.offsetMin = Vector2.zero;
            inkValueRect.offsetMax = Vector2.zero;

            Button retryButton = CreateTopCornerButton(canvas.transform, "RetryButton", "RETRY", new Vector2(-36f, -90f), greyButton, uiFont);
            Button pauseButton = CreateTopCornerButton(canvas.transform, "PauseButton", "PAUSE", new Vector2(36f, -90f), greyButton, uiFont, leftCorner: true);

            GameObject winPanel = CreateResultPanel(canvas.transform, "WinPanel", "Level Complete!", panelSprite, uiFont);
            GameObject failPanel = CreateResultPanel(canvas.transform, "FailPanel", "Failed!", panelSprite, uiFont);
            GameObject gameCompletePanel = CreateResultPanel(canvas.transform, "GameCompletePanel", "Game Complete!", panelSprite, uiFont);
            gameCompletePanel.SetActive(false);

            CreateStarRow(winPanel.transform, starSprite);

            Button nextButton = CreatePanelButton(winPanel.transform, "NextButton", "NEXT", greenButton, uiFont, new Vector2(0f, -120f));
            Button winRetryButton = CreatePanelButton(winPanel.transform, "WinRetryButton", "RETRY", blueButton, uiFont, new Vector2(0f, -195f));
            Button failRetryButton = CreatePanelButton(failPanel.transform, "FailRetryButton", "RETRY", blueButton, uiFont, new Vector2(0f, -120f));
            Button gameCompleteRetryButton = CreatePanelButton(gameCompletePanel.transform, "GameCompleteRetryButton", "RETRY", blueButton, uiFont, new Vector2(0f, -120f));

            GameObject pausePanel = CreateResultPanel(canvas.transform, "PausePanel", "Paused", panelSprite, uiFont);
            Button resumeButton = CreatePanelButton(pausePanel.transform, "ResumeButton", "RESUME", greenButton, uiFont, new Vector2(0f, -120f));
            pausePanel.SetActive(false);

            // Step 5 component becomes obsolete once HUD controller exists.
            InkSelectionUI legacyInkUi = inkBar.GetComponent<InkSelectionUI>();
            if (legacyInkUi != null)
            {
                Object.DestroyImmediate(legacyInkUi, true);
            }

            UIHudController hud = canvas.GetComponent<UIHudController>();
            if (hud == null)
            {
                hud = canvas.gameObject.AddComponent<UIHudController>();
            }

            SerializedObject hudSerialized = new SerializedObject(hud);
            hudSerialized.FindProperty("gameManager").objectReferenceValue = gameManager;
            hudSerialized.FindProperty("inkInventory").objectReferenceValue = inkInventory;

            hudSerialized.FindProperty("hudRoot").objectReferenceValue = hudRoot;
            hudSerialized.FindProperty("winPanel").objectReferenceValue = winPanel;
            hudSerialized.FindProperty("failPanel").objectReferenceValue = failPanel;
            hudSerialized.FindProperty("gameCompletePanel").objectReferenceValue = gameCompletePanel;
            hudSerialized.FindProperty("pausePanel").objectReferenceValue = pausePanel;

            hudSerialized.FindProperty("iceButton").objectReferenceValue = iceButton;
            hudSerialized.FindProperty("stickyButton").objectReferenceValue = stickyButton;
            hudSerialized.FindProperty("bouncyButton").objectReferenceValue = bouncyButton;
            hudSerialized.FindProperty("inkSlider").objectReferenceValue = inkSlider;
            hudSerialized.FindProperty("inkFillImage").objectReferenceValue = inkMeterImage;
            hudSerialized.FindProperty("inkValueText").objectReferenceValue = inkValueText;

            hudSerialized.FindProperty("retryButton").objectReferenceValue = retryButton;
            hudSerialized.FindProperty("pauseButton").objectReferenceValue = pauseButton;
            hudSerialized.FindProperty("nextButton").objectReferenceValue = nextButton;
            hudSerialized.FindProperty("failRetryButton").objectReferenceValue = failRetryButton;
            hudSerialized.FindProperty("winRetryButton").objectReferenceValue = winRetryButton;
            hudSerialized.FindProperty("gameCompleteRetryButton").objectReferenceValue = gameCompleteRetryButton;
            hudSerialized.FindProperty("resumeButton").objectReferenceValue = resumeButton;
            hudSerialized.ApplyModifiedPropertiesWithoutUndo();

            ClearButtonClickEvents(iceButton);
            ClearButtonClickEvents(stickyButton);
            ClearButtonClickEvents(bouncyButton);
            ClearButtonClickEvents(retryButton);
            ClearButtonClickEvents(pauseButton);
            ClearButtonClickEvents(nextButton);
            ClearButtonClickEvents(failRetryButton);
            ClearButtonClickEvents(winRetryButton);
            ClearButtonClickEvents(gameCompleteRetryButton);
            ClearButtonClickEvents(resumeButton);
            ConfigurePersistentButtonCallbacks(
                hud,
                iceButton,
                stickyButton,
                bouncyButton,
                retryButton,
                pauseButton,
                nextButton,
                failRetryButton,
                winRetryButton,
                gameCompleteRetryButton,
                resumeButton);

            EditorUtility.SetDirty(hud);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Step8SetupTool] Step 8 UI/HUD setup completed.");
        }

        private static void ClearButtonClickEvents(Button button)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            EditorUtility.SetDirty(button);
        }

        private static void ConfigurePersistentButtonCallbacks(
            UIHudController hud,
            Button iceButton,
            Button stickyButton,
            Button bouncyButton,
            Button retryButton,
            Button pauseButton,
            Button nextButton,
            Button failRetryButton,
            Button winRetryButton,
            Button gameCompleteRetryButton,
            Button resumeButton)
        {
            if (hud == null)
            {
                return;
            }

            AddPersistent(iceButton, hud.SelectIce);
            AddPersistent(stickyButton, hud.SelectSticky);
            AddPersistent(bouncyButton, hud.SelectBouncy);
            AddPersistent(retryButton, hud.OnRetryPressed);
            AddPersistent(pauseButton, hud.OnPausePressed);
            AddPersistent(nextButton, hud.OnNextPressed);
            AddPersistent(failRetryButton, hud.OnRetryPressed);
            AddPersistent(winRetryButton, hud.OnRetryPressed);
            AddPersistent(gameCompleteRetryButton, hud.OnRetryPressed);
            AddPersistent(resumeButton, hud.OnResumePressed);
        }

        private static void AddPersistent(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null || action == null)
            {
                return;
            }

            UnityEventTools.AddPersistentListener(button.onClick, action);
            EditorUtility.SetDirty(button);
        }

        private static GameObject CreateResultPanel(Transform parent, string panelName, string title, Sprite panelSprite, Font font)
        {
            GameObject panel = FindOrCreateChild(parent, panelName);
            RectTransform panelRect = EnsureRectTransform(panel);
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(640f, 360f);
            panelRect.anchoredPosition = Vector2.zero;

            Image panelImage = panel.GetComponent<Image>();
            if (panelImage == null)
            {
                panelImage = panel.AddComponent<Image>();
            }
            panelImage.sprite = panelSprite;
            panelImage.type = Image.Type.Sliced;
            panelImage.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);

            Text titleText = CreateOrUpdateText(panel.transform, "Title", title, font, 46, TextAnchor.UpperCenter);
            RectTransform titleRect = titleText.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0f, -35f);
            titleRect.sizeDelta = new Vector2(580f, 72f);

            panel.SetActive(false);
            return panel;
        }

        private static void CreateStarRow(Transform parent, Sprite starSprite)
        {
            GameObject row = FindOrCreateChild(parent, "StarRow");
            RectTransform rowRect = EnsureRectTransform(row);
            rowRect.anchorMin = new Vector2(0.5f, 0.5f);
            rowRect.anchorMax = new Vector2(0.5f, 0.5f);
            rowRect.pivot = new Vector2(0.5f, 0.5f);
            rowRect.anchoredPosition = new Vector2(0f, -25f);
            rowRect.sizeDelta = new Vector2(320f, 80f);

            HorizontalLayoutGroup layout = row.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = row.AddComponent<HorizontalLayoutGroup>();
            }
            layout.spacing = 16f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            for (int i = 0; i < 3; i++)
            {
                string starName = "Star_" + (i + 1);
                GameObject starObject = FindOrCreateChild(row.transform, starName);
                RectTransform starRect = EnsureRectTransform(starObject);
                starRect.sizeDelta = new Vector2(64f, 64f);

                Image starImage = starObject.GetComponent<Image>();
                if (starImage == null)
                {
                    starImage = starObject.AddComponent<Image>();
                }
                starImage.sprite = starSprite;
                starImage.color = Color.white;
            }
        }

        private static Button CreateTopCornerButton(Transform parent, string name, string label, Vector2 anchoredPosition, Sprite sprite, Font font, bool leftCorner = false)
        {
            GameObject buttonObject = FindOrCreateChild(parent, name);
            RectTransform rect = EnsureRectTransform(buttonObject);
            rect.anchorMin = leftCorner ? new Vector2(0f, 1f) : new Vector2(1f, 1f);
            rect.anchorMax = leftCorner ? new Vector2(0f, 1f) : new Vector2(1f, 1f);
            rect.pivot = leftCorner ? new Vector2(0f, 1f) : new Vector2(1f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(190f, 64f);

            Button button = ConfigureButtonVisual(buttonObject, sprite);
            Text text = CreateOrUpdateText(buttonObject.transform, "Label", label, font, 28, TextAnchor.MiddleCenter);
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
            rect.pivot = new Vector2(0.5f, 1f);
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

        private static Button CreateOrUpdateLabeledButton(Transform parent, string name, string label, Sprite sprite, Font font)
        {
            GameObject buttonObject = FindOrCreateChild(parent, name);
            RectTransform rect = EnsureRectTransform(buttonObject);
            rect.sizeDelta = new Vector2(180f, 72f);

            Button button = ConfigureButtonVisual(buttonObject, sprite);
            Text text = CreateOrUpdateText(buttonObject.transform, "Label", label, font, 24, TextAnchor.MiddleCenter);
            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            return button;
        }

        private static Button ConfigureButtonVisual(GameObject buttonObject, Sprite sprite)
        {
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
            return button;
        }

        private static Text CreateOrUpdateText(Transform parent, string name, string value, Font font, int size, TextAnchor alignment)
        {
            GameObject textObject = FindOrCreateChild(parent, name);
            Text text = textObject.GetComponent<Text>();
            if (text == null)
            {
                text = textObject.AddComponent<Text>();
            }

            text.font = font;
            text.text = value;
            text.alignment = alignment;
            text.fontStyle = FontStyle.Bold;
            text.fontSize = size;
            text.color = Color.white;
            text.resizeTextForBestFit = false;
            return text;
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
                scaler.referenceResolution = new Vector2(1080f, 1920f);
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
            EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();
            if (eventSystem != null)
            {
#if ENABLE_INPUT_SYSTEM
                InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
                if (inputModule != null)
                {
                    bool hasMissingRequiredActions =
                        inputModule.actionsAsset == null ||
                        inputModule.point == null ||
                        inputModule.leftClick == null ||
                        inputModule.submit == null ||
                        inputModule.cancel == null;

                    if (hasMissingRequiredActions)
                    {
                        inputModule.AssignDefaultActions();
                        EditorUtility.SetDirty(inputModule);
                    }
                }
#endif
                return;
            }

            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        private static GameManager EnsureGameManager()
        {
            GameManager manager = Object.FindFirstObjectByType<GameManager>();
            if (manager != null)
            {
                return manager;
            }

            GameObject managerObject = GameObject.Find("GameManager");
            if (managerObject == null)
            {
                managerObject = new GameObject("GameManager");
            }

            manager = managerObject.GetComponent<GameManager>();
            if (manager == null)
            {
                manager = managerObject.AddComponent<GameManager>();
            }

            return manager;
        }

        private static InkInventory EnsureInkInventory()
        {
            InkInventory inventory = Object.FindFirstObjectByType<InkInventory>();
            if (inventory != null)
            {
                return inventory;
            }

            GameObject inventoryObject = GameObject.Find("InkInventory");
            if (inventoryObject == null)
            {
                inventoryObject = new GameObject("InkInventory");
            }

            inventory = inventoryObject.GetComponent<InkInventory>();
            if (inventory == null)
            {
                inventory = inventoryObject.AddComponent<InkInventory>();
            }

            return inventory;
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
    }
}
#endif
