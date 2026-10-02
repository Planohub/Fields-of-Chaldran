using System;
using System.IO;
using UnityEngine;

namespace Chaldran
{
    public static class JourneyStore
    {
        public const string TrialScene = "Assets/Chaldran/Scenes/QuarantinePrototype.unity";
        public const string OverlandScene = "Assets/Chaldran/Scenes/OverlandPrototype.unity";
        public static JourneyCheckpoint Pending { get; set; }
        private static string SavePath => Path.Combine(Application.persistentDataPath, "chaldran-prototype-checkpoint-v1.json");

        public static bool TryLoad(out JourneyCheckpoint checkpoint)
        {
            checkpoint = null;
            string json = CheckpointFile.TryRead(SavePath);
            if (json == null) return false;
            try { checkpoint = JsonUtility.FromJson<JourneyCheckpoint>(json); }
            catch (ArgumentException) { return false; }
            return checkpoint != null && checkpoint.IsValid;
        }

        public static bool Save(JourneyCheckpoint checkpoint)
        {
            return checkpoint != null && checkpoint.IsValid
                && CheckpointFile.TryWrite(SavePath, JsonUtility.ToJson(checkpoint, true));
        }

        public static JourneyCheckpoint TakePending()
        {
            JourneyCheckpoint result = Pending;
            Pending = null;
            return result != null && result.IsValid ? result : null;
        }
    }
}
