using System.IO;
using UnityEngine;

namespace Save_Load_Options
{
    public static class SaveSystem
    {
        private static string GetPathForSlot(int slot)
        {
            return Application.persistentDataPath + $"/save_slot_{slot}.json";
        }

        public static void SaveGame(SaveData data, int slot)
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(GetPathForSlot(slot), json);
            Debug.Log($"Game Saved to Slot {slot}");
        }

        public static SaveData LoadGame(int slot)
        {
            string path = GetPathForSlot(slot);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<SaveData>(json);
            }
            Debug.LogWarning($"No save file in Slot {slot}");
            return null;
        }

        public static bool SaveExists(int slot)
        {
            return File.Exists(GetPathForSlot(slot));
        }

        public static void DeleteSave(int slot)
        {
            string path = GetPathForSlot(slot);
            if (File.Exists(path)) File.Delete(path);
        }
    }
}