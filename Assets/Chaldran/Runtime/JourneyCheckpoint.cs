using System;

namespace Chaldran
{
    public enum PresentationTier { Quarantine = 0, Overland = 1 }

    // A small versioned post-tutorial checkpoint. Full game saves will extend this schema.
    [Serializable]
    public sealed class JourneyCheckpoint
    {
        public const int CurrentVersion = 2;
        public int version;
        public PresentationTier tier;
        public bool hasWeapon;
        public float essence, resonance, x, y;
        public string storyId;
        public int storyStep;

        private bool ValidPlayer => tier == PresentationTier.Overland
            && hasWeapon && Finite(essence) && essence > 0 && Finite(resonance) && resonance >= 0
            && Finite(x) && Math.Abs(x) <= 10.5f && Finite(y) && Math.Abs(y) <= 6.5f;

        public bool IsValid => version == CurrentVersion && ValidPlayer
            && storyId == StoryProgress.SequenceId && StoryProgress.IsValidStep(storyStep);

        private static bool Finite(float value) { return !float.IsNaN(value) && !float.IsInfinity(value); }

        public static JourneyCheckpoint Capture(PlayerVitals values, float x, float y, StoryProgress story = null)
        {
            return new JourneyCheckpoint { version = CurrentVersion, tier = PresentationTier.Overland,
                hasWeapon = true, essence = values.Essence, resonance = values.Resonance, x = x, y = y,
                storyId = StoryProgress.SequenceId, storyStep = story?.CompletedSteps ?? 0 };
        }

        public static JourneyCheckpoint Upgrade(JourneyCheckpoint stored)
        {
            if (stored == null) return null;
            if (stored.IsValid) return stored;
            if (stored.version != 1 || !stored.ValidPlayer || stored.storyStep != 0
                || !string.IsNullOrEmpty(stored.storyId)) return null;
            return new JourneyCheckpoint { version = CurrentVersion, tier = stored.tier,
                hasWeapon = stored.hasWeapon, essence = stored.essence, resonance = stored.resonance,
                x = stored.x, y = stored.y, storyId = StoryProgress.SequenceId, storyStep = 0 };
        }
    }
}
