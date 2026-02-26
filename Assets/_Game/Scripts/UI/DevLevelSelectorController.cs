using System.Collections.Generic;
using TriInkTrack.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace TriInkTrack.UI
{
    public class DevLevelSelectorController : MonoBehaviour
    {
        [Header("Scene Flow")]
        [SerializeField] private string gameSceneName = "GameScene";

        [Header("Source")]
        [SerializeField] private LevelDefinition[] levelDefinitions;

        [Header("Visual")]
        [SerializeField] private Vector2 referenceResolution = new Vector2(1080f, 1920f);
        [SerializeField] private string titleText = "DEV LEVEL SELECTOR";

        private readonly List<Button> levelButtons = new List<Button>();

        private void Awake()
        {
            EnsureEventSystem();
            BuildUi();
        }

        private void BuildUi()
        {
            Canvas canvas = CreateCanvas();
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            RectTransform root = CreateFullRect("Root", canvas.transform);
            Image background = root.gameObject.AddComponent<Image>();
            background.color = new Color(0.08f, 0.1f, 0.14f, 1f);

            RectTransform panel = CreateRect("Panel", root, new Vector2(60f, 120f), new Vector2(-60f, -80f));
            Image panelImage = panel.gameObject.AddComponent<Image>();
            panelImage.color = new Color(0.12f, 0.15f, 0.22f, 0.94f);

            Text title = CreateText("Title", panel, titleText, font, 56, TextAnchor.MiddleCenter);
            title.rectTransform.anchorMin = new Vector2(0f, 1f);
            title.rectTransform.anchorMax = new Vector2(1f, 1f);
            title.rectTransform.offsetMin = new Vector2(24f, -120f);
            title.rectTransform.offsetMax = new Vector2(-24f, -24f);

            Text subtitle = CreateText("Subtitle", panel, "Tum levelleri test icin dogrudan ac", font, 28, TextAnchor.MiddleCenter);
            subtitle.color = new Color(0.8f, 0.86f, 0.95f, 0.9f);
            subtitle.rectTransform.anchorMin = new Vector2(0f, 1f);
            subtitle.rectTransform.anchorMax = new Vector2(1f, 1f);
            subtitle.rectTransform.offsetMin = new Vector2(24f, -170f);
            subtitle.rectTransform.offsetMax = new Vector2(-24f, -124f);

            RectTransform listRoot = CreateRect("ListRoot", panel, new Vector2(24f, 24f), new Vector2(-24f, -210f));
            GridLayoutGroup grid = listRoot.gameObject.AddComponent<GridLayoutGroup>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            grid.cellSize = new Vector2(440f, 120f);
            grid.spacing = new Vector2(20f, 20f);
            grid.childAlignment = TextAnchor.UpperCenter;

            levelButtons.Clear();
            int levelCount = levelDefinitions != null ? levelDefinitions.Length : 0;
            for (int i = 0; i < levelCount; i++)
            {
                int index = i;
                string label = BuildLevelLabel(levelDefinitions[i], i);
                Button button = CreateButton($"LevelButton_{i + 1:00}", listRoot, label, font);
                button.onClick.AddListener(() => OnLevelSelected(index));
                levelButtons.Add(button);
            }

            if (levelCount <= 0)
            {
                Text emptyText = CreateText("EmptyText", listRoot, "LevelDefinition bulunamadi.", font, 30, TextAnchor.MiddleCenter);
                emptyText.color = new Color(1f, 0.6f, 0.55f, 1f);
                emptyText.rectTransform.anchorMin = Vector2.zero;
                emptyText.rectTransform.anchorMax = Vector2.one;
                emptyText.rectTransform.offsetMin = new Vector2(10f, 10f);
                emptyText.rectTransform.offsetMax = new Vector2(-10f, -10f);
            }

            Button continueButton = CreateButton("ContinueButton", panel, "Kayitli ilerleme ile ac", font);
            continueButton.GetComponent<Image>().color = new Color(0.35f, 0.42f, 0.68f, 1f);
            RectTransform continueRect = continueButton.GetComponent<RectTransform>();
            continueRect.anchorMin = new Vector2(0.5f, 0f);
            continueRect.anchorMax = new Vector2(0.5f, 0f);
            continueRect.pivot = new Vector2(0.5f, 0f);
            continueRect.anchoredPosition = new Vector2(0f, 28f);
            continueRect.sizeDelta = new Vector2(520f, 84f);
            continueButton.onClick.AddListener(OnContinueWithSavedProgress);
        }

        private void OnLevelSelected(int levelIndex)
        {
            DevLevelSelection.SetSelectedLevelIndex(levelIndex);
            SceneManager.LoadScene(gameSceneName);
        }

        private void OnContinueWithSavedProgress()
        {
            DevLevelSelection.ClearSelectedLevelIndex();
            SceneManager.LoadScene(gameSceneName);
        }

        private string BuildLevelLabel(LevelDefinition definition, int index)
        {
            if (definition == null || string.IsNullOrWhiteSpace(definition.LevelName))
            {
                return $"LEVEL {index + 1:00}";
            }

            return definition.LevelName.ToUpperInvariant();
        }

        private Canvas CreateCanvas()
        {
            GameObject canvasObject = new GameObject("DevLevelSelectorCanvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static RectTransform CreateFullRect(string name, Transform parent)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        private static RectTransform CreateRect(string name, Transform parent, Vector2 offsetMin, Vector2 offsetMax)
        {
            RectTransform rect = CreateFullRect(name, parent);
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return rect;
        }

        private static Text CreateText(string name, Transform parent, string value, Font font, int fontSize, TextAnchor anchor)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            Text text = textObject.AddComponent<Text>();
            text.font = font;
            text.text = value;
            text.fontSize = fontSize;
            text.alignment = anchor;
            text.fontStyle = FontStyle.Bold;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            textObject.AddComponent<RectTransform>();
            return text;
        }

        private static Button CreateButton(string name, Transform parent, string label, Font font)
        {
            GameObject buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.18f, 0.54f, 0.58f, 1f);

            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;

            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(440f, 120f);

            Text text = CreateText("Label", buttonObject.transform, label, font, 28, TextAnchor.MiddleCenter);
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(24f, 8f);
            textRect.offsetMax = new Vector2(-24f, -8f);

            return button;
        }

        private void EnsureEventSystem()
        {
            EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
            if (eventSystem != null)
            {
#if ENABLE_INPUT_SYSTEM
                StandaloneInputModule legacyModule = eventSystem.GetComponent<StandaloneInputModule>();
                if (legacyModule != null)
                {
                    Destroy(legacyModule);
                }

                InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
                if (inputModule == null)
                {
                    inputModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                }

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
                    }
                }
#endif
                return;
            }

            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
            InputSystemUIInputModule createdInputModule = eventSystemObject.AddComponent<InputSystemUIInputModule>();
            if (createdInputModule != null)
            {
                createdInputModule.AssignDefaultActions();
            }
#else
            eventSystemObject.AddComponent<StandaloneInputModule>();
#endif
        }
    }
}
