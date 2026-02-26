using UnityEngine;

namespace TriInkTrack.Level
{
    public class BoundarySystem : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private float padding = 1.5f;
        [SerializeField] private float triggerThickness = 2f;
        [SerializeField] private bool rebuildOnAwake = true;

        private const string BoundaryTag = "Boundary";

        private void Awake()
        {
            if (!rebuildOnAwake)
            {
                return;
            }

            RebuildBoundaries();
        }

        public void RebuildBoundaries()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            if (targetCamera == null || !targetCamera.orthographic)
            {
                return;
            }

            ClearExistingBoundaryChildren();

            float halfHeight = targetCamera.orthographicSize + padding;
            float halfWidth = (targetCamera.orthographicSize * targetCamera.aspect) + padding;

            CreateBoundaryChild("Boundary_Top",
                new Vector2(0f, halfHeight + (triggerThickness * 0.5f)),
                new Vector2((halfWidth * 2f) + (triggerThickness * 2f), triggerThickness));

            CreateBoundaryChild("Boundary_Bottom",
                new Vector2(0f, -halfHeight - (triggerThickness * 0.5f)),
                new Vector2((halfWidth * 2f) + (triggerThickness * 2f), triggerThickness));

            CreateBoundaryChild("Boundary_Left",
                new Vector2(-halfWidth - (triggerThickness * 0.5f), 0f),
                new Vector2(triggerThickness, (halfHeight * 2f) + (triggerThickness * 2f)));

            CreateBoundaryChild("Boundary_Right",
                new Vector2(halfWidth + (triggerThickness * 0.5f), 0f),
                new Vector2(triggerThickness, (halfHeight * 2f) + (triggerThickness * 2f)));
        }

        private void CreateBoundaryChild(string name, Vector2 localPosition, Vector2 size)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(transform, false);
            child.transform.localPosition = localPosition;

            BoxCollider2D col = child.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = size;

            child.AddComponent<BoundaryTrigger>();
            TrySetTag(child, BoundaryTag);
        }

        private void ClearExistingBoundaryChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (child.name.StartsWith("Boundary_"))
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private static void TrySetTag(GameObject target, string tag)
        {
            try
            {
                target.tag = tag;
            }
            catch (UnityException)
            {
                // Tag does not exist yet.
            }
        }
    }
}
