using UnityEngine;

namespace TriInkTrack.Level
{
    public class BallSpawnPoint : MonoBehaviour
    {
        [SerializeField] private Vector2 initialDirection = Vector2.right;

        public Vector2 InitialDirection => initialDirection;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.2f);

            Vector3 direction = ((Vector3)initialDirection.normalized) * 0.8f;
            Gizmos.DrawLine(transform.position, transform.position + direction);
        }
    }
}
