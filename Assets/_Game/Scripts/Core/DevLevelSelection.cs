using UnityEngine;

namespace TriInkTrack.Core
{
    public static class DevLevelSelection
    {
        private const string SelectedLevelIndexKey = "DevSelectedLevelIndex";

        public static void SetSelectedLevelIndex(int levelIndex)
        {
            PlayerPrefs.SetInt(SelectedLevelIndexKey, Mathf.Max(0, levelIndex));
            PlayerPrefs.Save();
        }

        public static bool TryConsumeSelectedLevelIndex(out int levelIndex)
        {
            if (!PlayerPrefs.HasKey(SelectedLevelIndexKey))
            {
                levelIndex = -1;
                return false;
            }

            levelIndex = Mathf.Max(0, PlayerPrefs.GetInt(SelectedLevelIndexKey, 0));
            PlayerPrefs.DeleteKey(SelectedLevelIndexKey);
            PlayerPrefs.Save();
            return true;
        }

        public static void ClearSelectedLevelIndex()
        {
            PlayerPrefs.DeleteKey(SelectedLevelIndexKey);
            PlayerPrefs.Save();
        }
    }
}
