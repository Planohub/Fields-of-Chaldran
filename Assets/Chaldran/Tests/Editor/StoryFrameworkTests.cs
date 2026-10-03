using NUnit.Framework;

namespace Chaldran.Tests
{
    public sealed class StoryFrameworkTests
    {
        [Test]
        public void StoryBeatsCannotBeSkippedOrAwardedTwice()
        {
            StoryProgress story = new StoryProgress();
            Assert.That(story.TryComplete(DirectoryBeat.LibraryThreshold), Is.False);
            Assert.That(story.TryComplete((DirectoryBeat)(-1)), Is.False);
            for (int step = 0; step < StoryProgress.StepCount; step++)
            {
                Assert.That(story.TryComplete((DirectoryBeat)step), Is.True);
                Assert.That(story.TryComplete((DirectoryBeat)step), Is.False);
            }
            Assert.That(story.Complete, Is.True);
            Assert.That(story.CompletedSteps, Is.EqualTo(StoryProgress.StepCount));
            Assert.That(story.TryComplete((DirectoryBeat)StoryProgress.StepCount), Is.False);
        }

        [Test]
        public void RestoringProgressRejectsOutOfRangeValuesWithoutChangingState()
        {
            StoryProgress story = new StoryProgress();
            Assert.That(story.Restore(3), Is.True);
            Assert.That(story.Restore(-1), Is.False);
            Assert.That(story.Restore(StoryProgress.StepCount + 1), Is.False);
            Assert.That(story.CompletedSteps, Is.EqualTo(3));
            Assert.That(story.TryComplete(DirectoryBeat.RouteMarker), Is.True);
            Assert.That(story.Complete, Is.False);
        }

        [Test]
        public void DialogueCompletesOnlyAfterItsLastPageAndOnlyOnce()
        {
            DialogueSession session = new DialogueSession();
            Assert.That(session.Begin(3), Is.True);
            Assert.That(session.LineIndex, Is.Zero);
            Assert.That(session.Advance(), Is.False);
            Assert.That(session.Advance(), Is.False);
            Assert.That(session.IsOpen, Is.True);
            Assert.That(session.LineIndex, Is.EqualTo(2));
            Assert.That(session.Advance(), Is.True);
            Assert.That(session.IsOpen, Is.False);
            Assert.That(session.Advance(), Is.False);
        }

        [Test]
        public void CancellingDialogueDoesNotCompleteTheStoryBeat()
        {
            StoryProgress story = new StoryProgress();
            DialogueSession session = new DialogueSession();
            session.Begin(2);
            if (session.Advance()) story.TryComplete(DirectoryBeat.Arrival);
            session.Cancel();
            if (session.Advance()) story.TryComplete(DirectoryBeat.Arrival);
            Assert.That(story.CompletedSteps, Is.Zero);
            Assert.That(session.Begin(2), Is.True);
            Assert.That(session.LineIndex, Is.Zero);
            session.Advance();
            if (session.Advance()) story.TryComplete(DirectoryBeat.Arrival);
            Assert.That(story.CompletedSteps, Is.EqualTo(1));
        }

        [Test]
        public void DialogueRejectsEmptyOversizedAndNestedSessions()
        {
            DialogueSession session = new DialogueSession();
            Assert.That(session.Begin(0), Is.False);
            Assert.That(session.Begin(65), Is.False);
            Assert.That(session.Begin(2), Is.True);
            Assert.That(session.Begin(1), Is.False);
            Assert.That(session.LineCount, Is.EqualTo(2));
        }

        [Test]
        public void CheckpointCarriesCompletedConversationsAndRouteProgress()
        {
            for (int step = 0; step <= StoryProgress.StepCount; step++)
            {
                StoryProgress story = new StoryProgress();
                story.Restore(step);
                JourneyCheckpoint checkpoint = JourneyCheckpoint.Capture(new PlayerVitals(100, 60, 10, 15), 7, 6, story);
                Assert.That(checkpoint.IsValid, Is.True);
                Assert.That(checkpoint.storyId, Is.EqualTo(StoryProgress.SequenceId));
                StoryProgress restored = new StoryProgress();
                Assert.That(restored.Restore(checkpoint.storyStep), Is.True);
                Assert.That(restored.CompletedSteps, Is.EqualTo(step));
            }
        }

        [Test]
        public void LegacyCheckpointUpgradesWithoutErasingWeaponOrResources()
        {
            JourneyCheckpoint legacy = new JourneyCheckpoint { version = 1, tier = PresentationTier.Overland,
                hasWeapon = true, essence = 73, resonance = 24, x = -6, y = -4 };
            JourneyCheckpoint current = JourneyCheckpoint.Upgrade(legacy);
            Assert.That(current, Is.Not.Null);
            Assert.That(current.IsValid, Is.True);
            Assert.That(current.essence, Is.EqualTo(73));
            Assert.That(current.resonance, Is.EqualTo(24));
            Assert.That(current.x, Is.EqualTo(-6));
            Assert.That(current.y, Is.EqualTo(-4));
            Assert.That(current.storyStep, Is.Zero);
            Assert.That(legacy.version, Is.EqualTo(1));
            legacy.essence = 0;
            Assert.That(JourneyCheckpoint.Upgrade(legacy), Is.Null);
        }

        [Test]
        public void UnsupportedStoryIdsVersionsAndStepsAreRejected()
        {
            JourneyCheckpoint checkpoint = JourneyCheckpoint.Capture(new PlayerVitals(100, 60, 10, 15), 0, 0);
            checkpoint.storyId = "another-chapter";
            Assert.That(JourneyCheckpoint.Upgrade(checkpoint), Is.Null);
            checkpoint.storyId = StoryProgress.SequenceId;
            checkpoint.storyStep = -1;
            Assert.That(checkpoint.IsValid, Is.False);
            checkpoint.storyStep = StoryProgress.StepCount + 1;
            Assert.That(checkpoint.IsValid, Is.False);
            checkpoint.storyStep = 0;
            checkpoint.version = JourneyCheckpoint.CurrentVersion + 1;
            Assert.That(JourneyCheckpoint.Upgrade(checkpoint), Is.Null);
        }
    }
}
