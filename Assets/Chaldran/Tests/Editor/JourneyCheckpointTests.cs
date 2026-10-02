using System;
using System.IO;
using NUnit.Framework;

namespace Chaldran.Tests
{
    public sealed class JourneyCheckpointTests
    {
        [Test]
        public void CheckpointPreservesResourcesAndPresentationUnlock()
        {
            PlayerVitals values = new PlayerVitals(100, 60, 10, 15);
            values.ApplyDamage(25);
            values.TrySpendResonance(20);
            JourneyCheckpoint checkpoint = JourneyCheckpoint.Capture(values, -9, -5);
            Assert.That(checkpoint.IsValid, Is.True);
            Assert.That(checkpoint.tier, Is.EqualTo(PresentationTier.Overland));
            Assert.That(checkpoint.essence, Is.EqualTo(75));
            Assert.That(checkpoint.resonance, Is.EqualTo(40));
            PlayerVitals restored = new PlayerVitals(100, 60, 10, 15);
            restored.LoadResources(checkpoint.essence, checkpoint.resonance);
            Assert.That(restored.Essence, Is.EqualTo(75));
            Assert.That(restored.Resonance, Is.EqualTo(40));
        }

        [Test]
        public void UnsupportedOrInvalidCheckpointsAreRejected()
        {
            JourneyCheckpoint checkpoint = JourneyCheckpoint.Capture(new PlayerVitals(100,60,10,15),0,0);
            checkpoint.version = 2;
            Assert.That(checkpoint.IsValid, Is.False);
            checkpoint.version = 1;
            checkpoint.tier = PresentationTier.Quarantine;
            Assert.That(checkpoint.IsValid, Is.False);
            checkpoint.tier = PresentationTier.Overland;
            checkpoint.hasWeapon = false;
            Assert.That(checkpoint.IsValid, Is.False);
            checkpoint.hasWeapon = true;
            checkpoint.x = float.NaN;
            Assert.That(checkpoint.IsValid, Is.False);
            checkpoint.x = 100;
            Assert.That(checkpoint.IsValid, Is.False);
            checkpoint.x = 0;
            checkpoint.essence = 0;
            Assert.That(checkpoint.IsValid, Is.False);
        }

        [Test]
        public void RestoredResourcesRespectNewBalanceCaps()
        {
            PlayerVitals values = new PlayerVitals(80,40,10,15);
            values.LoadResources(100,60);
            Assert.That(values.Essence, Is.EqualTo(80));
            Assert.That(values.Resonance, Is.EqualTo(40));
        }

        [Test]
        public void CompletedTutorialDoesNotRecreateTokenPermissions()
        {
            PrototypeProgress progress = new PrototypeProgress();
            progress.RestoreCompletedTutorial();
            Assert.That(progress.HasWeapon, Is.True);
            Assert.That(progress.Complete, Is.True);
            Assert.That(progress.HasWriteAccess, Is.False);
            Assert.That(progress.DefeatWarden(), Is.False);
            Assert.That(progress.CollectWriteAccess(), Is.False);
        }

        [Test]
        public void CheckpointFileRoundTripsAndRetainsPreviousVersion()
        {
            string directory = Path.Combine(Path.GetTempPath(), "chaldran-test-" + Guid.NewGuid().ToString("N"));
            string path = Path.Combine(directory, "checkpoint.json");
            try
            {
                Assert.That(CheckpointFile.TryRead(path), Is.Null);
                Assert.That(CheckpointFile.TryWrite(path, "first"), Is.True);
                Assert.That(CheckpointFile.TryRead(path), Is.EqualTo("first"));
                Assert.That(CheckpointFile.TryWrite(path, "second"), Is.True);
                Assert.That(CheckpointFile.TryRead(path), Is.EqualTo("second"));
                Assert.That(CheckpointFile.TryRead(path + ".bak"), Is.EqualTo("first"));
                Assert.That(File.Exists(path + ".tmp"), Is.False);
            }
            finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
        }

        [Test]
        public void FailedCheckpointWriteKeepsExistingSave()
        {
            string directory = Path.Combine(Path.GetTempPath(), "chaldran-test-" + Guid.NewGuid().ToString("N"));
            string path = Path.Combine(directory, "checkpoint.json");
            try
            {
                Assert.That(CheckpointFile.TryWrite(path, "existing checkpoint"), Is.True);
                Directory.CreateDirectory(path + ".tmp");
                Assert.That(CheckpointFile.TryWrite(path, "replacement"), Is.False);
                Assert.That(CheckpointFile.TryRead(path), Is.EqualTo("existing checkpoint"));
            }
            finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
        }
    }
}
