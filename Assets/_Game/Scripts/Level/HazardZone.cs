using UnityEngine;

namespace TriInkTrack.Level
{
    [RequireComponent(typeof(Collider2D))]
    public class HazardZone : MonoBehaviour
    {
        private void Awake()
        {
            Collider2D col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }
    }
}
