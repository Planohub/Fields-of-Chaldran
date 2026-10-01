using NUnit.Framework;

namespace Chaldran.Tests
{
    public sealed class CombatFrameworkTests
    {
        [Test]
        public void DamageUsesArmorThenCriticalThenBlock()
        {
            Assert.That(CombatMath.Damage(40, 100, 0), Is.EqualTo(20).Within(0.001f));
            Assert.That(CombatMath.Damage(40, 100, 0.5f), Is.EqualTo(26.6667f).Within(0.001f));
            Assert.That(CombatMath.Damage(40, 100, 0, true, 0.3f), Is.EqualTo(9).Within(0.001f));
        }

        [Test]
        public void OverrideHasBoundedDiminishingReturns()
        {
            Assert.That(CombatMath.CritChance(100), Is.GreaterThan(CombatMath.CritChance(10)));
            Assert.That(CombatMath.CritChance(float.MaxValue), Is.LessThanOrEqualTo(0.35f));
            Assert.That(CombatMath.Penetration(float.MaxValue), Is.LessThanOrEqualTo(0.5f));
            Assert.That(CombatMath.CritChance(float.NaN), Is.EqualTo(0.05f));
        }

        [Test]
        public void InvalidDamageCannotCorruptHealth()
        {
            PlayerVitals stats = new PlayerVitals(100, 60, 10, 15);
            stats.ApplyDamage(float.NaN);
            stats.ApplyDamage(float.PositiveInfinity);
            stats.ApplyDamage(-50);
            Assert.That(stats.Essence, Is.EqualTo(100));
            Assert.That(CombatMath.Damage(float.NaN, 20, 0), Is.EqualTo(0));
        }

        [Test]
        public void ResourcesRemainBoundedAndDeadPlayersCannotSpendOrRegenerate()
        {
            PlayerVitals stats = new PlayerVitals(100, 60, 10, 15);
            Assert.That(stats.TrySpendResonance(20), Is.True);
            Assert.That(stats.Resonance, Is.EqualTo(40));
            Assert.That(stats.TrySpendResonance(50), Is.False);
            Assert.That(stats.TrySpendResonance(-1), Is.False);
            Assert.That(stats.TrySpendResonance(float.NaN), Is.False);
            stats.Regenerate(5, 1000);
            Assert.That(stats.Resonance, Is.EqualTo(60));
            stats.TrySpendResonance(20);
            stats.ApplyDamage(1000);
            stats.Regenerate(5, 1000);
            Assert.That(stats.Essence, Is.Zero);
            Assert.That(stats.Resonance, Is.EqualTo(40));
            Assert.That(stats.TrySpendResonance(1), Is.False);
            stats.Restore();
            Assert.That(stats.IsAlive, Is.True);
            Assert.That(stats.Essence, Is.EqualTo(100));
        }

        [Test]
        public void PermissionSequenceCannotSkipTokenOrBarrier()
        {
            PrototypeProgress progress = new PrototypeProgress();
            Assert.That(progress.DefeatWarden(), Is.False);
            Assert.That(progress.OpenGate(), Is.False);
            Assert.That(progress.Finish(), Is.False);
            Assert.That(progress.CollectWriteAccess(), Is.False);
            Assert.That(progress.RecoverWeapon(), Is.True);
            Assert.That(progress.DefeatWarden(), Is.True);
            Assert.That(progress.DefeatWarden(), Is.False, "A defeated sentinel cannot drop a second token.");
            Assert.That(progress.CollectWriteAccess(), Is.True);
            Assert.That(progress.OpenGate(), Is.True);
            Assert.That(progress.HasWriteAccess, Is.False, "Opening this prototype gate consumes its token.");
            Assert.That(progress.OpenGate(), Is.False);
            Assert.That(progress.Finish(), Is.True);
            Assert.That(progress.Finish(), Is.False);
        }
    }
}
