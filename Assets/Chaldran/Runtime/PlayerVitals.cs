using System;

namespace Chaldran
{
    // Runtime state, separate from Unity presentation and editable balance data.
    public sealed class PlayerVitals
    {
        public float MaxEssence { get; }
        public float MaxResonance { get; }
        public float Override { get; }
        public float Armor { get; }
        public float Essence { get; private set; }
        public float Resonance { get; private set; }
        public bool IsAlive => Essence > 0f;
        public float CritChance => CombatMath.CritChance(Override);
        public float Penetration => CombatMath.Penetration(Override);

        public PlayerVitals(float essence, float resonance, float overrideValue, float armor)
        {
            MaxEssence = Math.Max(1f, CombatMath.NonNegative(essence));
            MaxResonance = Math.Max(1f, CombatMath.NonNegative(resonance));
            Override = CombatMath.NonNegative(overrideValue);
            Armor = CombatMath.NonNegative(armor);
            Restore();
        }

        public bool TrySpendResonance(float cost)
        {
            if (!IsAlive || float.IsNaN(cost) || float.IsInfinity(cost) || cost < 0f || cost > Resonance)
                return false;
            Resonance -= cost;
            return true;
        }

        public float ApplyDamage(float amount)
        {
            float applied = Math.Min(Essence, CombatMath.NonNegative(amount));
            Essence -= applied;
            return applied;
        }

        public void Regenerate(float rate, float seconds)
        {
            if (!IsAlive) return;
            double gain = (double)CombatMath.NonNegative(rate) * CombatMath.NonNegative(seconds);
            Resonance = (float)Math.Min(MaxResonance, Resonance + gain);
        }

        public void Restore()
        {
            Essence = MaxEssence;
            Resonance = MaxResonance;
        }
    }
}
