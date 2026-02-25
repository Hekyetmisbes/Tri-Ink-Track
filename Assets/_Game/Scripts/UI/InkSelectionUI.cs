using TriInkTrack.Ink;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace TriInkTrack.UI
{
    public class InkSelectionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InkInventory inkInventory;
        [SerializeField] private Button iceButton;
        [SerializeField] private Button stickyButton;
        [SerializeField] private Button bouncyButton;
        [SerializeField] private Image inkFillImage;
        [SerializeField] private Slider inkSlider;

        [Header("Selection Highlight")]
        [SerializeField] private float selectedScale = 1.1f;
        [SerializeField] private float unselectedScale = 1f;
        [SerializeField] private float unselectedAlpha = 0.45f;

        private CanvasGroup iceCG;
        private CanvasGroup stickyCG;
        private CanvasGroup bouncyCG;
        private bool isSubscribedToInventory;

        private void Awake()
        {
            EnsureEventSystemInputBindings();
            EnsureInkInventory();
            CacheCanvasGroups();
        }

        private void OnEnable()
        {
            BindButtons();
            TrySubscribeInventory();
        }

        private void OnDisable()
        {
            UnbindButtons();
            UnsubscribeInventory();
        }

        public void SelectIce()
        {
#if UNITY_EDITOR
            Debug.Log("[InkSelectionUI] SelectIce clicked");
#endif
            if (EnsureInkInventory())
            {
                inkInventory.SetInkType(InkType.Ice);
            }
            UpdateSelectionVisual(InkType.Ice);
        }

        public void SelectSticky()
        {
#if UNITY_EDITOR
            Debug.Log("[InkSelectionUI] SelectSticky clicked");
#endif
            if (EnsureInkInventory())
            {
                inkInventory.SetInkType(InkType.Sticky);
            }
            UpdateSelectionVisual(InkType.Sticky);
        }

        public void SelectBouncy()
        {
#if UNITY_EDITOR
            Debug.Log("[InkSelectionUI] SelectBouncy clicked");
#endif
            if (EnsureInkInventory())
            {
                inkInventory.SetInkType(InkType.Bouncy);
            }
            UpdateSelectionVisual(InkType.Bouncy);
        }

        private void HandleInkChanged(int current, int total, InkType selectedType)
        {
#if UNITY_EDITOR
            Debug.Log($"[InkSelectionUI] HandleInkChanged: type={selectedType}, ink={current}/{total}");
#endif
            float normalized = total > 0 ? (float)current / total : 0f;

            if (inkFillImage != null)
            {
                inkFillImage.fillAmount = normalized;
            }

            if (inkSlider != null)
            {
                inkSlider.minValue = 0f;
                inkSlider.maxValue = 1f;
                inkSlider.value = normalized;
            }

            UpdateSelectionVisual(selectedType);
        }

        private void UpdateSelectionVisual(InkType selectedType)
        {
            SetButtonVisual(iceButton, iceCG, selectedType == InkType.Ice);
            SetButtonVisual(stickyButton, stickyCG, selectedType == InkType.Sticky);
            SetButtonVisual(bouncyButton, bouncyCG, selectedType == InkType.Bouncy);
        }

        private void SetButtonVisual(Button button, CanvasGroup cg, bool isSelected)
        {
            if (button == null)
            {
                return;
            }

            float scale = isSelected ? selectedScale : unselectedScale;
            button.transform.localScale = new Vector3(scale, scale, 1f);

            if (cg != null)
            {
                cg.alpha = isSelected ? 1f : unselectedAlpha;
            }
        }

        private void CacheCanvasGroups()
        {
            iceCG = GetOrAddCanvasGroup(iceButton);
            stickyCG = GetOrAddCanvasGroup(stickyButton);
            bouncyCG = GetOrAddCanvasGroup(bouncyButton);
        }

        private CanvasGroup GetOrAddCanvasGroup(Button button)
        {
            if (button == null)
            {
                return null;
            }

            var cg = button.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = button.gameObject.AddComponent<CanvasGroup>();
            }

            return cg;
        }

        private void BindButtons()
        {
            if (iceButton != null)
            {
                iceButton.onClick.AddListener(SelectIce);
            }

            if (stickyButton != null)
            {
                stickyButton.onClick.AddListener(SelectSticky);
            }

            if (bouncyButton != null)
            {
                bouncyButton.onClick.AddListener(SelectBouncy);
            }
        }

        private void UnbindButtons()
        {
            if (iceButton != null)
            {
                iceButton.onClick.RemoveListener(SelectIce);
            }

            if (stickyButton != null)
            {
                stickyButton.onClick.RemoveListener(SelectSticky);
            }

            if (bouncyButton != null)
            {
                bouncyButton.onClick.RemoveListener(SelectBouncy);
            }
        }

        private bool EnsureInkInventory()
        {
            if (inkInventory != null)
            {
                return true;
            }

            inkInventory = InkInventory.Instance;
            if (inkInventory == null)
            {
                inkInventory = FindFirstObjectByType<InkInventory>();
            }

            if (inkInventory != null)
            {
                CacheCanvasGroups();
                TrySubscribeInventory();
                return true;
            }

#if UNITY_EDITOR
            Debug.LogWarning("[InkSelectionUI] InkInventory bulunamadı!");
#endif
            return false;
        }

        private void TrySubscribeInventory()
        {
            if (inkInventory == null || isSubscribedToInventory)
            {
                return;
            }

            inkInventory.OnInkChanged += HandleInkChanged;
            isSubscribedToInventory = true;
#if UNITY_EDITOR
            Debug.Log("[InkSelectionUI] InkInventory'ye subscribe edildi.");
#endif
            HandleInkChanged(
                inkInventory.CurrentInkPoints,
                inkInventory.TotalInkPoints,
                inkInventory.CurrentInkType);
        }

        private void UnsubscribeInventory()
        {
            if (inkInventory == null || !isSubscribedToInventory)
            {
                return;
            }

            inkInventory.OnInkChanged -= HandleInkChanged;
            isSubscribedToInventory = false;
        }

        private void EnsureEventSystemInputBindings()
        {
#if ENABLE_INPUT_SYSTEM
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                eventSystem = FindFirstObjectByType<EventSystem>();
            }

            if (eventSystem == null)
            {
                return;
            }

            InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputModule == null)
            {
                return;
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
#if UNITY_EDITOR
            Debug.Log("[InkSelectionUI] InputSystemUIInputModule varsayılan UI action'ları ile düzeltildi.");
#endif
#endif
        }
    }
}
