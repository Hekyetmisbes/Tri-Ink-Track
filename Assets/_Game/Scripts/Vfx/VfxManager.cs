using TriInkTrack.Ink;
using UnityEngine;

namespace TriInkTrack.Vfx
{
    public enum VfxType { Win, Fail, InkHit }

    public class VfxManager : MonoBehaviour
    {
        public static VfxManager Instance { get; private set; }

        [SerializeField] private ParticleSystem winEffect;
        [SerializeField] private ParticleSystem failEffect;
        [SerializeField] private ParticleSystem inkHitIce;
        [SerializeField] private ParticleSystem inkHitSticky;
        [SerializeField] private ParticleSystem inkHitBouncy;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            EnsureParticleSystems();
        }

        public void PlayWin()
        {
            if (winEffect == null) return;
            if (winEffect.isPlaying) winEffect.Stop();
            winEffect.Play();
        }

        public void PlayFail()
        {
            if (failEffect == null) return;
            if (failEffect.isPlaying) failEffect.Stop();
            failEffect.Play();
        }

        public void PlayInkHit(InkType type, Vector3 position)
        {
            ParticleSystem ps = GetInkHitSystem(type);
            if (ps == null) return;
            ps.transform.position = position;
            if (ps.isPlaying) ps.Stop();
            ps.Play();
        }

        private ParticleSystem GetInkHitSystem(InkType type)
        {
            switch (type)
            {
                case InkType.Sticky: return inkHitSticky;
                case InkType.Bouncy: return inkHitBouncy;
                case InkType.Ice:
                default: return inkHitIce;
            }
        }

        private void EnsureParticleSystems()
        {
            if (winEffect == null)
                winEffect = CreateParticleSystem("WinEffect", new Color(1f, 0.92f, 0.016f), 25, 0.3f);
            if (failEffect == null)
                failEffect = CreateParticleSystem("FailEffect", new Color(1f, 0.2f, 0.2f), 15, 0.25f);
            if (inkHitIce == null)
                inkHitIce = CreateParticleSystem("InkHitIce", new Color(0f, 0.749f, 1f), 8, 0.15f);
            if (inkHitSticky == null)
                inkHitSticky = CreateParticleSystem("InkHitSticky", new Color(1f, 0.549f, 0f), 8, 0.15f);
            if (inkHitBouncy == null)
                inkHitBouncy = CreateParticleSystem("InkHitBouncy", new Color(0.196f, 0.804f, 0.196f), 8, 0.15f);
        }

        private ParticleSystem CreateParticleSystem(string psName, Color color, int burstCount, float startSize)
        {
            GameObject go = new GameObject(psName);
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;

            ParticleSystem ps = go.AddComponent<ParticleSystem>();

            ParticleSystem.MainModule main = ps.main;
            main.startColor = color;
            main.startSize = startSize;
            main.startLifetime = 0.5f;
            main.startSpeed = 2f;
            main.maxParticles = burstCount * 2;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            ParticleSystem.EmissionModule emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)burstCount) });

            ps.Stop();
            return ps;
        }
    }
}
