using System;

namespace Chaldran
{
    public enum PresentationTier { Quarantine = 0, Overland = 1 }

    // A small versioned post-tutorial checkpoint. Full game saves will extend this schema.
    [Serializable]
    public sealed class JourneyCheckpoint
    {
        public const int CurrentVersion = 1;
        public int version;
        public PresentationTier tier;
        public bool hasWeapon;
        public float essence, resonance, x, y;

        public bool IsValid => version == CurrentVersion && tier == PresentationTier.Overland
            && hasWeapon && Finite(essence) && essence > 0 && Finite(resonance) && resonance >= 0
            && Finite(x) && Math.Abs(x) <= 10.5f && Finite(y) && Math.Abs(y) <= 6.5f;

        private static bool Finite(float value) { return !float.IsNaN(value) && !float.IsInfinity(value); }

        public static JourneyCheckpoint Capture(PlayerVitals values, float x, float y)
        {
            return new JourneyCheckpoint { version = CurrentVersion, tier = PresentationTier.Overland,
                hasWeapon = true, essence = values.Essence, resonance = values.Resonance, x = x, y = y };
        }
    }
}
