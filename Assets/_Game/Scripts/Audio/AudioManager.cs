using UnityEngine;

namespace TriInkTrack.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource uiSource;

        [Header("Drawing SFX")]
        [SerializeField] private AudioClip drawStartClip;
        [SerializeField] private AudioClip drawEndClip;

        [Header("Ink SFX")]
        [SerializeField] private AudioClip inkSwitchClip;
        [SerializeField] private AudioClip bouncyHitClip;

        [Header("Game State SFX")]
        [SerializeField] private AudioClip winClip;
        [SerializeField] private AudioClip failClip;
        [SerializeField] private AudioClip levelStartClip;

        [Header("UI SFX")]
        [SerializeField] private AudioClip buttonClickClip;

        [Header("Volume")]
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;

        private const string VolumeKey = "TriInkTrack_SfxVolume";
        private const string MuteKey   = "TriInkTrack_Muted";
        private bool isMuted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            sfxVolume = PlayerPrefs.GetFloat(VolumeKey, sfxVolume);
            isMuted   = PlayerPrefs.GetInt(MuteKey, 0) == 1;
            EnsureAudioSources();
        }

        public void PlayDrawStart()   => PlayOnSFX(drawStartClip);
        public void PlayDrawEnd()     => PlayOnSFX(drawEndClip);
        public void PlayInkSwitch()   => PlayOnSFX(inkSwitchClip);
        public void PlayBouncyHit()   => PlayOnSFX(bouncyHitClip);
        public void PlayWin()         => PlayOnSFX(winClip);
        public void PlayFail()        => PlayOnSFX(failClip);
        public void PlayLevelStart()  => PlayOnSFX(levelStartClip);
        public void PlayButtonClick() => PlayOnUI(buttonClickClip);

        public void PlaySFX(AudioClip clip) => PlayOnSFX(clip);

        private void PlayOnSFX(AudioClip clip)
        {
            if (isMuted || clip == null || sfxSource == null)
            {
                return;
            }

            sfxSource.PlayOneShot(clip, sfxVolume);
        }

        private void PlayOnUI(AudioClip clip)
        {
            if (isMuted || clip == null || uiSource == null)
            {
                return;
            }

            uiSource.PlayOneShot(clip, sfxVolume);
        }

        public void SetVolume(float v)
        {
            sfxVolume = Mathf.Clamp01(v);
            PlayerPrefs.SetFloat(VolumeKey, sfxVolume);
        }

        public void SetMute(bool m)
        {
            isMuted = m;
            PlayerPrefs.SetInt(MuteKey, m ? 1 : 0);
        }

        public float Volume => sfxVolume;
        public bool  IsMuted => isMuted;

        private void EnsureAudioSources()
        {
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.spatialBlend = 0f;
            }

            if (uiSource == null)
            {
                uiSource = gameObject.AddComponent<AudioSource>();
                uiSource.playOnAwake = false;
                uiSource.spatialBlend = 0f;
            }
        }
    }
}
