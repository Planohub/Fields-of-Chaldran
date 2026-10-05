using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Chaldran.Tests
{
    // These tests require the Unity Editor. Standalone core validation excludes them.
    public sealed class StoryAssetTests
    {
        [Test]
        public void AnchorCompletionRemovesPhysicalBlockerAndLoopAndRestoresThatState()
        {
            GameObject owner = new GameObject("Library state test");
            GameObject barrier = new GameObject("Barrier test", typeof(SpriteRenderer), typeof(BoxCollider2D));
            GameObject portal = new GameObject("Portal test", typeof(SpriteRenderer), typeof(BoxCollider2D));
            try
            {
                PrototypeRun run = owner.AddComponent<PrototypeRun>();
                run.Library.Restore(3);
                LibraryRouteGate gate = barrier.AddComponent<LibraryRouteGate>();
                LibraryLoopPortal loop = portal.AddComponent<LibraryLoopPortal>();
                gate.Initialize(run, barrier.GetComponent<Collider2D>());
                loop.Initialize(run, new Vector2(-8.5f, 0));
                Assert.That(barrier.GetComponent<Collider2D>().enabled, Is.True);
                Assert.That(portal.GetComponent<Collider2D>().enabled, Is.True);
                run.Library.TryComplete(LibraryBeat.AnchorInterrupted);
                run.NotifyChanged();
                Assert.That(barrier.GetComponent<Collider2D>().enabled, Is.False);
                Assert.That(portal.GetComponent<Collider2D>().enabled, Is.False);
                Assert.That(portal.GetComponent<SpriteRenderer>().enabled, Is.False);
                Object.DestroyImmediate(gate);
                barrier.GetComponent<Collider2D>().enabled = true;
                barrier.AddComponent<LibraryRouteGate>().Initialize(run, barrier.GetComponent<Collider2D>());
                Assert.That(barrier.GetComponent<Collider2D>().enabled, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(portal);
                Object.DestroyImmediate(barrier);
                Object.DestroyImmediate(owner);
            }
        }

        [Test]
        public void LibraryAssetsImportAtSixteenBitWithEditableDialogue()
        {
            LibraryStoryDefinition story = AssetDatabase.LoadAssetAtPath<LibraryStoryDefinition>(
                "Assets/Chaldran/Data/Story/OracleLibraryOpening.asset");
            PresentationProfile profile = AssetDatabase.LoadAssetAtPath<PresentationProfile>(
                "Assets/Chaldran/Data/LibraryPresentation.asset");
            Assert.That(story, Is.Not.Null); Assert.That(story.IsValid, Is.True);
            Assert.That(profile, Is.Not.Null); Assert.That(profile.IsValid, Is.True);
            Assert.That(profile.tier, Is.EqualTo(PresentationTier.Overland));
            Assert.That(profile.cellPixels, Is.EqualTo(32));
            Assert.That(profile.bufferWidth, Is.EqualTo(512));
            Assert.That(story.DialogueFor(LibraryBeat.LoopObserved), Is.Null);
        }

        [Test]
        public void UnityJsonPreservesLibraryLocationAndVersionTwoMigration()
        {
            for (int step = 0; step <= LibraryProgress.StepCount; step++)
            {
                LibraryProgress library = new LibraryProgress(); library.Restore(step);
                JourneyCheckpoint saved = JourneyCheckpoint.CaptureLibrary(new PlayerVitals(100,60,10,15), -8, 0, library);
                JourneyCheckpoint restored = JourneyCheckpoint.Upgrade(JsonUtility.FromJson<JourneyCheckpoint>(JsonUtility.ToJson(saved)));
                Assert.That(restored.IsValid, Is.True);
                Assert.That(restored.location, Is.EqualTo(JourneyLocation.OracleLibrary));
                Assert.That(restored.libraryStep, Is.EqualTo(step));
                Assert.That(JourneyStore.SceneFor(restored), Is.EqualTo(JourneyStore.LibraryScene));
            }
            const string json = "{\"version\":2,\"tier\":1,\"hasWeapon\":true,\"essence\":73,\"resonance\":24,\"x\":7,\"y\":6,\"storyId\":\"user-directory-opening\",\"storyStep\":5}";
            JourneyCheckpoint migrated = JourneyCheckpoint.Upgrade(JsonUtility.FromJson<JourneyCheckpoint>(json));
            Assert.That(migrated.IsValid, Is.True); Assert.That(migrated.storyStep, Is.EqualTo(5));
            Assert.That(JourneyStore.SceneFor(migrated), Is.EqualTo(JourneyStore.OverlandScene));
        }

        [Test]
        public void RetroProfileImportsWithFastTypingAndIntroductoryDialogue()
        {
            PresentationProfile profile = AssetDatabase.LoadAssetAtPath<PresentationProfile>(
                "Assets/Chaldran/Data/QuarantinePresentation.asset");
            Assert.That(profile, Is.Not.Null);
            Assert.That(profile.retroDialogue, Is.True);
            Assert.That(profile.charactersPerSecond, Is.EqualTo(60));
            Assert.That(profile.typingSound, Is.Not.Null);
            Assert.That(profile.introduction, Is.Not.Null);
            Assert.That(profile.introduction.IsValid, Is.True);
        }

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
