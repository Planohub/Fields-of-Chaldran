namespace Chaldran
{
    public enum DirectoryBeat { Arrival = 0, Waystone = 1, SignalRecord = 2, RouteMarker = 3, LibraryThreshold = 4 }

    // Stable beat order for the first quest. New chapters need their own sequence ID.
    public sealed class StoryProgress
    {
        public const string SequenceId = "user-directory-opening";
        public const int StepCount = 5;
        public int CompletedSteps { get; private set; }
        public bool Complete => CompletedSteps == StepCount;
        public static bool IsValidStep(int step) { return step >= 0 && step <= StepCount; }

        public bool Restore(int completedSteps)
        {
            if (!IsValidStep(completedSteps)) return false;
            CompletedSteps = completedSteps;
            return true;
        }

        public bool TryComplete(DirectoryBeat beat)
        {
            if (Complete || (int)beat != CompletedSteps) return false;
            CompletedSteps++;
            return true;
        }
    }
}
