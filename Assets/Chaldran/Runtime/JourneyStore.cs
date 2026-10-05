using System;
using System.IO;
using UnityEngine;

namespace Chaldran
{
    public static class JourneyStore
    {
        public const string TrialScene = "Assets/Chaldran/Scenes/QuarantinePrototype.unity";
        public const string OverlandScene = "Assets/Chaldran/Scenes/OverlandPrototype.unity";
        public const string LibraryScene = "Assets/Chaldran/Scenes/OracleLibraryPrototype.unity";
        public static JourneyCheckpoint Pending { get; set; }
        private static string SavePath => Path.Combine(Application.persistentDataPath, "chaldran-prototype-checkpoint-v3.json");
        private static string VersionTwoPath => Path.Combine(Application.persistentDataPath, "chaldran-prototype-checkpoint-v2.json");
        private static string LegacyPath => Path.Combine(Application.persistentDataPath, "chaldran-prototype-checkpoint-v1.json");

        public static bool TryLoad(out JourneyCheckpoint checkpoint)
        {
            checkpoint = null;
            // A corrupt current save must not silently roll back to the old tutorial-only save.
            string json = CheckpointFile.TryRead(File.Exists(SavePath) ? SavePath
                : File.Exists(VersionTwoPath) ? VersionTwoPath : LegacyPath);
            if (json == null) return false;
            try { checkpoint = JsonUtility.FromJson<JourneyCheckpoint>(json); }
            catch (ArgumentException) { return false; }
            checkpoint = JourneyCheckpoint.Upgrade(checkpoint);
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
            return JourneyCheckpoint.Upgrade(result);
        }

        public static string SceneFor(JourneyCheckpoint checkpoint)
        {
            return checkpoint.location == JourneyLocation.OracleLibrary ? LibraryScene : OverlandScene;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetPending() { Pending = null; }
    }
}
