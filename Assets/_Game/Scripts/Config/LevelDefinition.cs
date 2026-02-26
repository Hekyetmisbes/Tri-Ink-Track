using TriInkTrack.Ink;
using UnityEngine;

namespace TriInkTrack.Core
{
    [CreateAssetMenu(fileName = "LevelDefinition", menuName = "TriInkTrack/Level Definition")]
    public class LevelDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string levelName = "Level";

        [Header("Content")]
        [SerializeField] private GameObject levelPrefab;

        [Header("Ink Rules")]
        [SerializeField] private bool allowIce = true;
        [SerializeField] private bool allowSticky = true;
        [SerializeField] private bool allowBouncy = true;
        [SerializeField] private int inkBudget = 100;

        [Header("Targets")]
        [SerializeField] private float targetTime = 0f;

        public string LevelName => levelName;
        public GameObject LevelPrefab => levelPrefab;
        public bool AllowIce => allowIce;
        public bool AllowSticky => allowSticky;
        public bool AllowBouncy => allowBouncy;
        public int InkBudget => inkBudget;
        public float TargetTime => targetTime;

        public bool IsInkAllowed(InkType type)
        {
            return type switch
            {
                InkType.Ice => allowIce,
                InkType.Sticky => allowSticky,
                InkType.Bouncy => allowBouncy,
                _ => true
            };
        }
    }
}
