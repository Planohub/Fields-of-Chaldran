using NUnit.Framework;

namespace Chaldran.Tests
{
    public sealed class LibraryFrameworkTests
    {
        [Test]
        public void LibraryRequiresLoopAndOracleBeforeInterruptingAnchor()
        {
            LibraryProgress state = new LibraryProgress();
            Assert.That(state.TryComplete(LibraryBeat.AnchorInterrupted), Is.False);
            Assert.That(state.TryComplete(LibraryBeat.Arrival), Is.True);
            Assert.That(state.TryComplete(LibraryBeat.OracleContact), Is.False);
            Assert.That(state.TryComplete(LibraryBeat.LoopObserved), Is.True);
            Assert.That(state.TryComplete(LibraryBeat.LoopObserved), Is.False);
            Assert.That(state.TryComplete(LibraryBeat.OracleContact), Is.True);
            Assert.That(state.AnchorInterrupted, Is.False);
            Assert.That(state.TryComplete(LibraryBeat.RouteReached), Is.False);
            Assert.That(state.TryComplete(LibraryBeat.AnchorInterrupted), Is.True);
            Assert.That(state.AnchorInterrupted, Is.True);
            Assert.That(state.TryComplete(LibraryBeat.AnchorInterrupted), Is.False);
            Assert.That(state.TryComplete(LibraryBeat.RouteReached), Is.True);
            Assert.That(state.Complete, Is.True);
            Assert.That(state.TryComplete(LibraryBeat.RouteReached), Is.False);
        }

        [Test]
        public void ClosingAnchorDialogueLeavesRouteClosed()
        {
            LibraryProgress state = new LibraryProgress(); state.Restore(3);
            DialogueSession dialogue = new DialogueSession(); dialogue.Begin(3);
            if (dialogue.Advance()) state.TryComplete(LibraryBeat.AnchorInterrupted);
            dialogue.Cancel();
            if (dialogue.Advance()) state.TryComplete(LibraryBeat.AnchorInterrupted);
            Assert.That(state.AnchorInterrupted, Is.False);
            dialogue.Begin(3);
            dialogue.Advance(); dialogue.Advance();
            if (dialogue.Advance()) state.TryComplete(LibraryBeat.AnchorInterrupted);
            Assert.That(state.AnchorInterrupted, Is.True);
        }

        [Test]
        public void LibraryRestorationRejectsInvalidStepsWithoutChangingWorldState()
        {
            LibraryProgress state = new LibraryProgress(); state.Restore(4);
            Assert.That(state.Restore(-1), Is.False);
            Assert.That(state.Restore(6), Is.False);
            Assert.That(state.TryComplete((LibraryBeat)99), Is.False);
            Assert.That(state.AnchorInterrupted, Is.True);
            Assert.That(state.CompletedSteps, Is.EqualTo(4));
        }

        [Test]
        public void VersionTwoMigrationPreservesEveryDirectoryBeatAndDoesNotInventLibraryProgress()
        {
            for (int step = 0; step <= StoryProgress.StepCount; step++)
            {
                JourneyCheckpoint old = new JourneyCheckpoint { version = 2, tier = PresentationTier.Overland,
                    hasWeapon = true, essence = 72, resonance = 23, x = 7, y = 6,
                    storyId = StoryProgress.SequenceId, storyStep = step };
                JourneyCheckpoint migrated = JourneyCheckpoint.Upgrade(old);
                Assert.That(migrated.IsValid, Is.True);
                Assert.That(migrated.storyStep, Is.EqualTo(step));
                Assert.That(migrated.libraryStep, Is.Zero);
                Assert.That(migrated.location, Is.EqualTo(JourneyLocation.UserDirectory));
                Assert.That(migrated.essence, Is.EqualTo(72));
                Assert.That(migrated.resonance, Is.EqualTo(23));
                Assert.That(migrated.x, Is.EqualTo(7));
                Assert.That(old.version, Is.EqualTo(2));
            }
        }

        [Test]
        public void LibraryCheckpointsPreserveChapterResourcesAndReleasedAnchor()
        {
            PlayerVitals values = new PlayerVitals(100,60,10,15);
            values.ApplyDamage(25); values.TrySpendResonance(20);
            for (int step = 0; step <= LibraryProgress.StepCount; step++)
            {
                LibraryProgress library = new LibraryProgress(); library.Restore(step);
                JourneyCheckpoint stored = JourneyCheckpoint.CaptureLibrary(values, step < 4 ? -8 : 6, 0, library);
                Assert.That(stored.IsValid, Is.True);
                Assert.That(stored.tier, Is.EqualTo(PresentationTier.Overland));
                Assert.That(stored.location, Is.EqualTo(JourneyLocation.OracleLibrary));
                Assert.That(stored.storyStep, Is.EqualTo(StoryProgress.StepCount));
                Assert.That(stored.essence, Is.EqualTo(75));
                Assert.That(stored.resonance, Is.EqualTo(40));
                LibraryProgress restored = new LibraryProgress(); restored.Restore(JourneyCheckpoint.Upgrade(stored).libraryStep);
                Assert.That(restored.AnchorInterrupted, Is.EqualTo(step >= 4));
                Assert.That(restored.Complete, Is.EqualTo(step == LibraryProgress.StepCount));
            }
        }

        [Test]
        public void MixedChapterStatesAndPositionsBeyondClosedAnchorAreRejected()
        {
            PlayerVitals values = new PlayerVitals(100,60,10,15);
            JourneyCheckpoint c = JourneyCheckpoint.CaptureLibrary(values, -9, 0, new LibraryProgress());
            c.x = 6; Assert.That(c.IsValid, Is.False);
            c.x = -9; c.y = 4; Assert.That(c.IsValid, Is.False);
            c.y = 0; c.storyStep = 0; Assert.That(c.IsValid, Is.False);
            c.storyStep = StoryProgress.StepCount; c.storyId = StoryProgress.SequenceId;
            Assert.That(JourneyCheckpoint.Upgrade(c), Is.Null);
            c.storyId = LibraryProgress.SequenceId; c.location = (JourneyLocation)99;
            Assert.That(c.IsValid, Is.False);
            c = JourneyCheckpoint.Capture(values, 0, 0); c.libraryStep = 1;
            Assert.That(c.IsValid, Is.False);
            c.version = 2; Assert.That(JourneyCheckpoint.Upgrade(c), Is.Null);
        }

        [Test]
        public void LibraryCheckpointCannotSaveADeadAvatar()
        {
            PlayerVitals values = new PlayerVitals(100,60,10,15); values.ApplyDamage(100);
            Assert.That(JourneyCheckpoint.CaptureLibrary(values, -9, 0, new LibraryProgress()).IsValid, Is.False);
        }
    }
}
