using System;

namespace Chaldran
{
    public enum PresentationTier { Quarantine = 0, Overland = 1 }
    public enum JourneyLocation { UserDirectory = 0, OracleLibrary = 1 }

    // A small versioned post-tutorial checkpoint. Full game saves will extend this schema.
    [Serializable]
    public sealed class JourneyCheckpoint
    {
        public const int CurrentVersion = 3;
        public int version;
        public PresentationTier tier;
        public bool hasWeapon;
        public float essence, resonance, x, y;
        public string storyId;
        public int storyStep;
        public JourneyLocation location;
        public int libraryStep;

        private bool ValidPlayer => tier == PresentationTier.Overland
            && hasWeapon && Finite(essence) && essence > 0 && Finite(resonance) && resonance >= 0
            && Finite(x) && Math.Abs(x) <= 10.5f && Finite(y) && Math.Abs(y) <= 6.5f;

        public bool IsValid => version == CurrentVersion && ValidPlayer
            && ((location == JourneyLocation.UserDirectory && storyId == StoryProgress.SequenceId
                    && StoryProgress.IsValidStep(storyStep) && libraryStep == 0)
                || (location == JourneyLocation.OracleLibrary && storyId == LibraryProgress.SequenceId
                    && storyStep == StoryProgress.StepCount && LibraryProgress.IsValidStep(libraryStep)
                    && (libraryStep >= 4 || (x <= 2.5f && Math.Abs(y) <= 1.5f))));

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
            if (!stored.ValidPlayer || stored.location != JourneyLocation.UserDirectory || stored.libraryStep != 0) return null;
            bool versionOne = stored.version == 1 && stored.storyStep == 0 && string.IsNullOrEmpty(stored.storyId);
            bool versionTwo = stored.version == 2 && stored.storyId == StoryProgress.SequenceId && StoryProgress.IsValidStep(stored.storyStep);
            if (!versionOne && !versionTwo) return null;
            return new JourneyCheckpoint { version = CurrentVersion, tier = stored.tier,
                hasWeapon = stored.hasWeapon, essence = stored.essence, resonance = stored.resonance,
                x = stored.x, y = stored.y, storyId = StoryProgress.SequenceId, storyStep = stored.storyStep };
        }

        public static JourneyCheckpoint CaptureLibrary(PlayerVitals values, float x, float y, LibraryProgress library)
        {
            return new JourneyCheckpoint { version = CurrentVersion, tier = PresentationTier.Overland,
                location = JourneyLocation.OracleLibrary, hasWeapon = true, essence = values.Essence,
                resonance = values.Resonance, x = x, y = y, storyId = LibraryProgress.SequenceId,
                storyStep = StoryProgress.StepCount, libraryStep = library.CompletedSteps };
        }
    }
}
