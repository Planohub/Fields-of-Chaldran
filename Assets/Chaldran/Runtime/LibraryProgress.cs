namespace Chaldran
{
    public enum LibraryBeat { Arrival = 0, LoopObserved = 1, OracleContact = 2, AnchorInterrupted = 3, RouteReached = 4 }

    // Stable, ordered world state for the first library section, independent of its art tier.
    public sealed class LibraryProgress
    {
        public const string SequenceId = "oracle-library-opening";
        public const int StepCount = 5;
        public int CompletedSteps { get; private set; }
        public bool AnchorInterrupted => CompletedSteps >= 4;
        public bool Complete => CompletedSteps == StepCount;
        public static bool IsValidStep(int step) { return step >= 0 && step <= StepCount; }

        public bool Restore(int step)
        {
            if (!IsValidStep(step)) return false;
            CompletedSteps = step;
            return true;
        }

        public bool TryComplete(LibraryBeat beat)
        {
            if (Complete || (int)beat != CompletedSteps) return false;
            CompletedSteps++;
            return true;
        }
    }
}
