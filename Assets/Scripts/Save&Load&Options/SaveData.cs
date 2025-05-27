using UnityEngine;

namespace Save_Load_Options
{
    [System.Serializable]
    public class SaveData
    {
        public string slotName;
        public int chapterIndex;
        public string sceneName;
        public Vector2 playerPosition;
    }
}