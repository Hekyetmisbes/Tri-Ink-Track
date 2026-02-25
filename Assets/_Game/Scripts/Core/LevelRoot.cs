using UnityEngine;

namespace TriInkTrack.Core
{
    public class LevelRoot : MonoBehaviour
    {
        [SerializeField] private bool clearChildrenOnStart;

        private void Start()
        {
            if (!clearChildrenOnStart)
                return;

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
