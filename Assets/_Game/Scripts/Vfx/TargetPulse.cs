using UnityEngine;

namespace TriInkTrack.Level
{
    public class TargetPulse : MonoBehaviour
    {
        [SerializeField] private float minScale = 0.88f;
        [SerializeField] private float maxScale = 1.12f;
        [SerializeField] private float speed = 1.5f;

        private void Update()
        {
            float t = Mathf.PingPong(Time.time * speed, 1f);
            float scale = Mathf.Lerp(minScale, maxScale, t);
            transform.localScale = Vector3.one * scale;
        }
    }
}
