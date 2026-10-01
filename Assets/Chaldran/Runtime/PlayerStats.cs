using System;
using UnityEngine;

namespace Chaldran
{
    public sealed class PlayerStats : MonoBehaviour
    {
        public PlayerVitals Vitals { get; private set; }
        public bool RegenerationSuppressed { get; set; }
        public event Action Changed;
        public event Action Died;
        private float regeneration;
        private PrototypeRun run;

        public void Initialize(PrototypeBalance balance, PrototypeRun owner)
        {
            run = owner;
            Vitals = new PlayerVitals(balance.maxEssence, balance.maxResonance, balance.overrideValue, balance.playerArmor);
            regeneration = balance.resonanceRegeneration;
            Changed?.Invoke();
        }

        private void Update()
        {
            if (run == null || !run.IsActive || Vitals == null || RegenerationSuppressed || Vitals.Resonance >= Vitals.MaxResonance) return;
            Vitals.Regenerate(regeneration, Time.deltaTime);
            Changed?.Invoke();
        }

        public bool TrySpend(float cost)
        {
            if (Vitals == null || !Vitals.TrySpendResonance(cost)) return false;
            Changed?.Invoke();
            return true;
        }

        public float TakeDamage(float amount)
        {
            if (Vitals == null || !Vitals.IsAlive) return 0f;
            float applied = Vitals.ApplyDamage(amount);
            Changed?.Invoke();
            if (!Vitals.IsAlive) Died?.Invoke();
            return applied;
        }

        public void Restore()
        {
            Vitals?.Restore();
            Changed?.Invoke();
        }
    }
}
