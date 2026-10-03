using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Chaldran.Tests
{
    // These tests require the Unity Editor. Standalone core validation excludes them.
    public sealed class StoryAssetTests
    {
        [Test]
        public void CheckedInStoryHasOrderedBeatsAndCompleteDialogueReferences()
        {
            DirectoryStoryDefinition story = AssetDatabase.LoadAssetAtPath<DirectoryStoryDefinition>(
                "Assets/Chaldran/Data/Story/UserDirectoryOpening.asset");
            Assert.That(story, Is.Not.Null);
            Assert.That(story.IsValid, Is.True);
            for (int step = 0; step < StoryProgress.StepCount; step++)
            {
                StoryProgress progress = new StoryProgress();
                progress.Restore(step);
                Assert.That(story.Objective(progress), Is.EqualTo(story.steps[step].objective));
            }
            StoryProgress complete = new StoryProgress();
            complete.Restore(StoryProgress.StepCount);
            Assert.That(story.Objective(complete), Is.EqualTo(story.completedObjective));
        }

        [Test]
        public void UnityJsonRoundTripPreservesEveryQuestStage()
        {
            for (int step = 0; step <= StoryProgress.StepCount; step++)
            {
                StoryProgress story = new StoryProgress();
                story.Restore(step);
                PlayerVitals values = new PlayerVitals(100, 60, 10, 15);
                values.ApplyDamage(27);
                values.TrySpendResonance(36);
                JourneyCheckpoint checkpoint = JourneyCheckpoint.Capture(values, 7, 6, story);
                JourneyCheckpoint restored = JourneyCheckpoint.Upgrade(
                    JsonUtility.FromJson<JourneyCheckpoint>(JsonUtility.ToJson(checkpoint)));
                Assert.That(restored, Is.Not.Null);
                Assert.That(restored.IsValid, Is.True);
                Assert.That(restored.storyStep, Is.EqualTo(step));
                Assert.That(restored.essence, Is.EqualTo(73));
                Assert.That(restored.resonance, Is.EqualTo(24));
            }
        }

        [Test]
        public void UnityLegacyJsonStartsTheNewStoryAtArrival()
        {
            const string json = "{\"version\":1,\"tier\":1,\"hasWeapon\":true,\"essence\":73,\"resonance\":24,\"x\":-6,\"y\":-4}";
            JourneyCheckpoint upgraded = JourneyCheckpoint.Upgrade(JsonUtility.FromJson<JourneyCheckpoint>(json));
            Assert.That(upgraded, Is.Not.Null);
            Assert.That(upgraded.IsValid, Is.True);
            Assert.That(upgraded.storyStep, Is.Zero);
            Assert.That(upgraded.essence, Is.EqualTo(73));
        }
    }
}
