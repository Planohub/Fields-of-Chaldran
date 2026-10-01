using System;

namespace Chaldran
{
    // Pure calculations shared by combat and tests. Values are prototype tuning.
    public static class CombatMath
    {
        public static float NonNegative(float value)
        {
            return float.IsNaN(value) || float.IsInfinity(value) ? 0f : Math.Max(0f, value);
        }

        public static float CritChance(float overrideValue)
        {
            double value = NonNegative(overrideValue);
            return (float)(0.05 + 0.30 * value / (value + 100.0));
        }

        public static float Penetration(float overrideValue)
        {
            double value = NonNegative(overrideValue);
            return (float)(0.50 * value / (value + 100.0));
        }

        public static float Damage(float rawDamage, float armor, float penetration,
            bool critical = false, float blockMultiplier = 1f)
        {
            double effectiveArmor = NonNegative(armor) * (1.0 - Math.Min(1f, NonNegative(penetration)));
            double result = NonNegative(rawDamage) * 100.0 / (100.0 + effectiveArmor);
            if (critical) result *= 1.5;
            result *= Math.Min(1f, NonNegative(blockMultiplier));
            return (float)Math.Min(float.MaxValue, result);
        }
    }
}
