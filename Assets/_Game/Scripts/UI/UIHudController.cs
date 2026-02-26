using TriInkTrack.Audio;
using TriInkTrack.Core;
using TriInkTrack.Ink;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace TriInkTrack.UI
{
    public class UIHudController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private InkInventory inkInventory;

        [Header("Roots")]
        [SerializeField] private GameObject hudRoot;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject failPanel;
        [SerializeField] private GameObject gameCompletePanel;
        [SerializeField] private GameObject pausePanel;

        [Header("Ink")]
        [SerializeField] private Button iceButton;
        [SerializeField] private Button stickyButton;
        [SerializeField] private Button bouncyButton;
        [SerializeField] private Slider inkSlider;
        [SerializeField] private Image inkFillImage;
        [SerializeField] private Text inkValueText;

        [Header("Actions")]
        [SerializeField] private Button retryButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button failRetryButton;
        [SerializeField] private Button winRetryButton;
        [SerializeField] private Button gameCompleteRetryButton;
        [SerializeField] private Button resumeButton;

        [Header("Selection Visual")]
        [SerializeField] private float selectedScale = 1.1f;
        [SerializeField] private float unselectedScale = 1f;
        [SerializeField] private float unselectedAlpha = 0.45f;

        [Header("Behavior")]
        [SerializeField] private bool disableHudInputOnResultState = true;
        [SerializeField] private bool showRetryButtonInAllStates = true;
        [SerializeField] private bool enablePause = true;

        private CanvasGroup iceCanvasGroup;
        private CanvasGroup stickyCanvasGroup;
        private CanvasGroup bouncyCanvasGroup;
        private bool isPaused;
        private bool allowIce = true;
        private bool allowSticky = true;
        private bool allowBouncy = true;

        private void Awake()
        {
            EnsureEventSystemInputBindings();
            ResolveReferences();
            CacheInkButtonCanvasGroups();
        }

        private void OnEnable()
        {
            BindButtons();
            SubscribeEvents();
            RefreshState();
        }

        private void OnDisable()
        {
            UnbindButtons();
            UnsubscribeEvents();
            ResumeGameIfPaused();
        }

        public void SelectIce()
        {
            if (inkInventory != null)
            {
                inkInventory.SetInkType(InkType.Ice);
            }
        }

        public void SelectSticky()
        {
            if (inkInventory != null)
            {
                inkInventory.SetInkType(InkType.Sticky);
            }
        }

        public void SelectBouncy()
        {
            if (inkInventory != null)
            {
                inkInventory.SetInkType(InkType.Bouncy);
            }
        }

        public void OnRetryPressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            ResumeGameIfPaused();
            SetPanelActive(gameCompletePanel, false);

            if (gameManager != null)
            {
                gameManager.Retry();
            }
        }

        public void OnNextPressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            ResumeGameIfPaused();
            SetPanelActive(gameCompletePanel, false);

            if (gameManager != null)
            {
                gameManager.NextLevel();
            }
        }

        public void OnPausePressed()
        {
            if (!enablePause)
            {
                return;
            }

            AudioManager.Instance?.PlayButtonClick();
            bool nextPauseState = !isPaused;
            SetPause(nextPauseState);
        }

        public void OnResumePressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            ResumeGameIfPaused();
        }

        public void SetGameCompleteVisible(bool visible)
        {
            SetPanelActive(gameCompletePanel, visible);
            if (visible)
            {
                SetPanelActive(winPanel, false);
                SetPanelActive(failPanel, false);
                SetHudInkInteractable(false);
            }
            else
            {
                RefreshState();
            }
        }

        public void SetInkAvailability(bool iceAllowed, bool stickyAllowed, bool bouncyAllowed)
        {
            allowIce = iceAllowed;
            allowSticky = stickyAllowed;
            allowBouncy = bouncyAllowed;

            if (!allowIce && !allowSticky && !allowBouncy)
            {
                allowIce = true;
            }

            SetButtonVisible(iceButton, true);
            SetButtonVisible(stickyButton, true);
            SetButtonVisible(bouncyButton, true);

            if (inkInventory != null && !inkInventory.IsInkAllowed(inkInventory.CurrentInkType))
            {
                if (allowIce)
                {
                    inkInventory.SetInkType(InkType.Ice);
                }
                else if (allowSticky)
                {
                    inkInventory.SetInkType(InkType.Sticky);
                }
                else
                {
                    inkInventory.SetInkType(InkType.Bouncy);
                }
            }

            RefreshState();
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (state == GameState.Win || state == GameState.Fail)
            {
                ResumeGameIfPaused();
            }
            else
            {
                SetPanelActive(gameCompletePanel, false);
            }

            bool showResultPanels = state == GameState.Win || state == GameState.Fail;
            SetPanelActive(hudRoot, !showResultPanels);
            SetPanelActive(winPanel, state == GameState.Win);
            SetPanelActive(failPanel, state == GameState.Fail);

            if (showRetryButtonInAllStates && retryButton != null)
            {
                retryButton.gameObject.SetActive(true);
            }

            if (disableHudInputOnResultState)
            {
                SetHudInkInteractable(!showResultPanels);
            }

            if (pauseButton != null)
            {
                pauseButton.gameObject.SetActive(enablePause && !showResultPanels);
            }
        }

        private void HandleInkChanged(int currentInk, int maxInk, InkType selectedInkType)
        {
            float normalized = maxInk > 0 ? (float)currentInk / maxInk : 0f;

            if (inkSlider != null)
            {
                inkSlider.minValue = 0f;
                inkSlider.maxValue = 1f;
                inkSlider.value = normalized;
            }

            if (inkFillImage != null)
            {
                inkFillImage.fillAmount = normalized;
            }

            if (inkValueText != null)
            {
                inkValueText.text = "INK " + currentInk + " / " + maxInk;
            }

            UpdateInkSelectionVisual(selectedInkType);
        }

        private void UpdateInkSelectionVisual(InkType selectedInkType)
        {
            SetInkButtonVisual(iceButton, iceCanvasGroup, selectedInkType == InkType.Ice);
            SetInkButtonVisual(stickyButton, stickyCanvasGroup, selectedInkType == InkType.Sticky);
            SetInkButtonVisual(bouncyButton, bouncyCanvasGroup, selectedInkType == InkType.Bouncy);
        }

        private void SetInkButtonVisual(Button button, CanvasGroup canvasGroup, bool selected)
        {
            if (button == null)
            {
                return;
            }

            float scale = selected ? selectedScale : unselectedScale;
            button.transform.localScale = new Vector3(scale, scale, 1f);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = selected ? 1f : unselectedAlpha;
            }
        }

        private void SetHudInkInteractable(bool interactable)
        {
            if (iceButton != null)
            {
                iceButton.interactable = interactable && allowIce;
            }

            if (stickyButton != null)
            {
                stickyButton.interactable = interactable && allowSticky;
            }

            if (bouncyButton != null)
            {
                bouncyButton.interactable = interactable && allowBouncy;
            }
        }

        private void SetPause(bool pause)
        {
            isPaused = pause;
            Time.timeScale = isPaused ? 0f : 1f;
            SetPanelActive(pausePanel, isPaused);
        }

        private void ResumeGameIfPaused()
        {
            if (!isPaused)
            {
                return;
            }

            SetPause(false);
        }

        private void RefreshState()
        {
            if (gameManager != null)
            {
                HandleGameStateChanged(gameManager.CurrentState);
            }

            if (inkInventory != null)
            {
                HandleInkChanged(
                    inkInventory.CurrentInkPoints,
                    inkInventory.TotalInkPoints,
                    inkInventory.CurrentInkType);
            }
        }

        private void ResolveReferences()
        {
            if (gameManager == null)
            {
                gameManager = GameManager.Instance;
                if (gameManager == null)
                {
                    gameManager = FindFirstObjectByType<GameManager>();
                }
            }

            if (inkInventory == null)
            {
                inkInventory = InkInventory.Instance;
                if (inkInventory == null)
                {
                    inkInventory = FindFirstObjectByType<InkInventory>();
                }
            }
        }

        private void SubscribeEvents()
        {
            if (gameManager != null)
            {
                gameManager.OnGameStateChanged += HandleGameStateChanged;
            }

            if (inkInventory != null)
            {
                inkInventory.OnInkChanged += HandleInkChanged;
            }
        }

        private void UnsubscribeEvents()
        {
            if (gameManager != null)
            {
                gameManager.OnGameStateChanged -= HandleGameStateChanged;
            }

            if (inkInventory != null)
            {
                inkInventory.OnInkChanged -= HandleInkChanged;
            }
        }

        private void BindButtons()
        {
            AddButtonListener(iceButton, SelectIce);
            AddButtonListener(stickyButton, SelectSticky);
            AddButtonListener(bouncyButton, SelectBouncy);
            AddButtonListener(retryButton, OnRetryPressed);
            AddButtonListener(pauseButton, OnPausePressed);
            AddButtonListener(nextButton, OnNextPressed);
            AddButtonListener(failRetryButton, OnRetryPressed);
            AddButtonListener(winRetryButton, OnRetryPressed);
            AddButtonListener(gameCompleteRetryButton, OnRetryPressed);
            AddButtonListener(resumeButton, OnResumePressed);
        }

        private void UnbindButtons()
        {
            RemoveButtonListener(iceButton, SelectIce);
            RemoveButtonListener(stickyButton, SelectSticky);
            RemoveButtonListener(bouncyButton, SelectBouncy);
            RemoveButtonListener(retryButton, OnRetryPressed);
            RemoveButtonListener(pauseButton, OnPausePressed);
            RemoveButtonListener(nextButton, OnNextPressed);
            RemoveButtonListener(failRetryButton, OnRetryPressed);
            RemoveButtonListener(winRetryButton, OnRetryPressed);
            RemoveButtonListener(gameCompleteRetryButton, OnRetryPressed);
            RemoveButtonListener(resumeButton, OnResumePressed);
        }

        private void CacheInkButtonCanvasGroups()
        {
            iceCanvasGroup = EnsureCanvasGroup(iceButton);
            stickyCanvasGroup = EnsureCanvasGroup(stickyButton);
            bouncyCanvasGroup = EnsureCanvasGroup(bouncyButton);
        }

        private static CanvasGroup EnsureCanvasGroup(Button button)
        {
            if (button == null)
            {
                return null;
            }

            CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = button.gameObject.AddComponent<CanvasGroup>();
            }

            return canvasGroup;
        }

        private static void AddButtonListener(Button button, UnityEngine.Events.UnityAction callback)
        {
            if (button != null && callback != null)
            {
                if (button.onClick.GetPersistentEventCount() > 0)
                {
                    return;
                }

                button.onClick.AddListener(callback);
            }
        }

        private static void RemoveButtonListener(Button button, UnityEngine.Events.UnityAction callback)
        {
            if (button != null && callback != null)
            {
                button.onClick.RemoveListener(callback);
            }
        }

        private static void SetPanelActive(GameObject panel, bool visible)
        {
            if (panel != null && panel.activeSelf != visible)
            {
                panel.SetActive(visible);
            }
        }

        private static void SetButtonVisible(Button button, bool visible)
        {
            if (button == null)
            {
                return;
            }

            if (button.gameObject.activeSelf != visible)
            {
                button.gameObject.SetActive(visible);
            }
        }

        private static void EnsureEventSystemInputBindings()
        {
#if ENABLE_INPUT_SYSTEM
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                eventSystem = FindFirstObjectByType<EventSystem>();
            }

            if (eventSystem == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem");
                eventSystem = eventSystemObject.AddComponent<EventSystem>();
            }

            StandaloneInputModule legacyModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (legacyModule != null)
            {
                Object.Destroy(legacyModule);
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

            if (!hasMissingRequiredActions)
            {
                return;
            }

            inputModule.AssignDefaultActions();
#endif
        }
    }
}
