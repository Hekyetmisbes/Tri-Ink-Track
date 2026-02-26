using TriInkTrack.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TriInkTrack.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject creditsPanel;

        [Header("Main Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;

        [Header("Settings")]
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle muteToggle;
        [SerializeField] private Button settingsCloseButton;

        [Header("Credits")]
        [SerializeField] private Button creditsCloseButton;

        [SerializeField] private string gameSceneName = "GameScene";

        private void Awake()
        {
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = AudioManager.Instance != null
                    ? AudioManager.Instance.Volume
                    : 1f;
            }

            if (muteToggle != null)
            {
                muteToggle.isOn = AudioManager.Instance != null && AudioManager.Instance.IsMuted;
            }
        }

        public void OnPlayPressed()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void OnSettingsPressed()
        {
            mainPanel?.SetActive(false);
            settingsPanel?.SetActive(true);
        }

        public void OnCreditsPressed()
        {
            mainPanel?.SetActive(false);
            creditsPanel?.SetActive(true);
        }

        public void OnQuitPressed()
        {
            Application.Quit();
        }

        public void OnSfxVolumeChanged(float value)
        {
            AudioManager.Instance?.SetVolume(value);
        }

        public void OnMuteChanged(bool muted)
        {
            AudioManager.Instance?.SetMute(muted);
        }

        public void OnCloseSettings()
        {
            settingsPanel?.SetActive(false);
            mainPanel?.SetActive(true);
        }

        public void OnCloseCredits()
        {
            creditsPanel?.SetActive(false);
            mainPanel?.SetActive(true);
        }
    }
}
