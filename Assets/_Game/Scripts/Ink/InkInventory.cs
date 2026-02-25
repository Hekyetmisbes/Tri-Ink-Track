using System;
using TriInkTrack.Core;
using UnityEngine;

namespace TriInkTrack.Ink
{
    public class InkInventory : MonoBehaviour
    {
        public static InkInventory Instance { get; private set; }

        [SerializeField] private int totalInkPoints = 100;
        [SerializeField] private InkType currentInkType = InkType.Ice;

        private int currentInkPoints;

        public event Action<int, int, InkType> OnInkChanged;

        public int CurrentInkPoints => currentInkPoints;
        public int TotalInkPoints => totalInkPoints;
        public InkType CurrentInkType => currentInkType;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            ApplyConfigOverrides();
            ResetInk();
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            }
        }

        public bool ConsumeInk(int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            if (currentInkPoints < amount)
            {
                return false;
            }

            currentInkPoints -= amount;
            NotifyInkChanged();
            return true;
        }

        public bool HasInk()
        {
            return currentInkPoints > 0;
        }

        public void ResetInk()
        {
            currentInkPoints = totalInkPoints;
            NotifyInkChanged();
        }

        public void SetInkType(InkType type)
        {
            if (currentInkType == type)
            {
                return;
            }

            currentInkType = type;
            NotifyInkChanged();
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (state == GameState.Ready)
            {
                ResetInk();
            }
        }

        private void ApplyConfigOverrides()
        {
            if (GameManager.Instance == null || GameManager.Instance.Config == null)
            {
                return;
            }

            totalInkPoints = Mathf.Max(1, GameManager.Instance.Config.TotalInkPoints);
        }

        private void NotifyInkChanged()
        {
            OnInkChanged?.Invoke(currentInkPoints, totalInkPoints, currentInkType);
        }
    }
}
