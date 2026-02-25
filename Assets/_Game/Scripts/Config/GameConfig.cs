using UnityEngine;

namespace TriInkTrack.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "TriInkTrack/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Ball")]
        [SerializeField] private float ballSpeed = 5f;

        [Header("Drawing")]
        [SerializeField] private float minPointDist = 0.15f;
        [SerializeField] private int maxPointsPerLine = 60;
        [SerializeField] private int maxActiveLines = 30;
        [SerializeField] private int totalInkPoints = 100;

        [Header("Lifetime")]
        [SerializeField] private float inkLifetime = 7f;
        [SerializeField] private float fadeDuration = 0.5f;

        public float BallSpeed => ballSpeed;
        public float MinPointDist => minPointDist;
        public int MaxPointsPerLine => maxPointsPerLine;
        public int MaxActiveLines => maxActiveLines;
        public int TotalInkPoints => totalInkPoints;
        public float InkLifetime => inkLifetime;
        public float FadeDuration => fadeDuration;
    }
}
