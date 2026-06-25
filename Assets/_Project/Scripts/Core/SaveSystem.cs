using System.IO;
using UnityEngine;

namespace BeachVolley.Core
{
    /// <summary>
    /// Reads/writes SaveData as a JSON file in Application.persistentDataPath — the OS-managed
    /// per-user location that survives app restarts and updates. Unlike MatchSession /
    /// TournamentSession (session state, reset every Play), this is durable on disk.
    /// </summary>
    public static class SaveSystem
    {
        private static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");
        private static SaveData cached;

        /// <summary>Load the save (from disk on first call, then cached). Never returns null.</summary>
        public static SaveData Load()
        {
            if (cached != null) return cached;

            try
            {
                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);
                    cached = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
                }
                else
                {
                    cached = new SaveData();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveSystem] Load failed, starting fresh: {e.Message}");
                cached = new SaveData();
            }

            return cached;
        }

        /// <summary>Persist the save to disk.</summary>
        public static void Save(SaveData data)
        {
            cached = data;
            try
            {
                string json = JsonUtility.ToJson(data, prettyPrint: true);
                File.WriteAllText(FilePath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveSystem] Save failed: {e.Message}");
            }
        }
    }
}
