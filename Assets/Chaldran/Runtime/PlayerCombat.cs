using System.Collections.Generic;
using UnityEngine;

namespace Chaldran
{
    public sealed class PlayerCombat : MonoBehaviour
    {
        public bool IsBlocking { get; set; }
        public float BurstCooldownRemaining => Mathf.Max(0f, nextBurst - Time.time);
        private PrototypeRun run;
        private float nextAttack, nextBurst;
        private readonly List<Collider2D> hits = new List<Collider2D>(12);
        private readonly HashSet<SentinelEnemy> damaged = new HashSet<SentinelEnemy>();

        public void Initialize(PrototypeRun owner) { run = owner; }

        public void Attack()
        {
            if (!CanFight() || IsBlocking || Time.time < nextAttack) return;
            nextAttack = Time.time + run.Balance.meleeCooldown;
            run.Audio.Play(TrialSound.Strike);
            Vector2 facing = run.Motor.Facing;
            run.Visuals.Effect("Weapon arc", (Vector2)transform.position + facing * 0.8f, 9, 1.7f,
                Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg, new Color(0.8f, 1f, 1f), 0.18f);
            DealAreaDamage(run.Balance.meleeRange, run.Balance.meleeDamage, true);
        }

        public void Burst()
        {
            if (!CanFight() || IsBlocking || Time.time < nextBurst) return;
            if (!run.Stats.TrySpend(run.Balance.burstCost))
            {
                run.ShowMessage("Insufficient Resonance for Ashen Burst.");
                return;
            }
            nextBurst = Time.time + run.Balance.burstCooldown;
            run.Audio.Play(TrialSound.Burst);
            run.Visuals.Effect("Ashen Burst", transform.position, 10, run.Balance.burstRadius * 2f,
                0f, new Color(1f, 0.62f, 0.28f), 0.4f);
            DealAreaDamage(run.Balance.burstRadius, run.Balance.burstDamage, false);
        }

        private bool CanFight()
        {
            if (run == null || !run.IsActive) return false;
            if (run.Progress.HasWeapon) return true;
            run.ShowMessage("Recover your weapon from the cyan anomaly first.");
            return false;
        }

        private void DealAreaDamage(float radius, float baseDamage, bool directional)
        {
            hits.Clear();
            damaged.Clear();
            Physics2D.OverlapCircle(transform.position, radius, WorldQuery.Filter(WorldQuery.ActorLayer, false), hits);
            foreach (Collider2D hit in hits)
            {
                SentinelEnemy target = hit.GetComponentInParent<SentinelEnemy>();
                if (target == null || !target.IsAlive || !damaged.Add(target)) continue;
                Vector2 delta = (Vector2)target.transform.position - (Vector2)transform.position;
                if (delta.sqrMagnitude > radius * radius) continue;
                if (directional && delta.sqrMagnitude > 0.001f && Vector2.Angle(run.Motor.Facing, delta) > run.Balance.meleeHalfAngle) continue;
                if (!WorldQuery.HasLineOfSight(transform.position, target.transform.position)) continue;
                bool critical = Random.value < run.Stats.Vitals.CritChance;
                target.ReceiveDamage(CombatMath.Damage(baseDamage, target.Armor, run.Stats.Vitals.Penetration, critical), critical);
            }
        }

        public void ReceiveStrike(float rawDamage, Vector2 attackerPosition)
        {
            if (!run.IsActive) return;
            Vector2 incoming = attackerPosition - (Vector2)transform.position;
            bool facingStrike = incoming.sqrMagnitude < 0.001f || Vector2.Angle(run.Motor.Facing, incoming) <= 70f;
            bool blocked = IsBlocking && facingStrike && run.Stats.TrySpend(run.Balance.blockCost);
            if (IsBlocking && facingStrike && !blocked)
                run.ShowMessage("Guard broken: insufficient Resonance. Release block to regenerate.");
            float damage = CombatMath.Damage(rawDamage, run.Stats.Vitals.Armor, 0f,
                false, blocked ? run.Balance.blockedDamageMultiplier : 1f);
            float applied = run.Stats.TakeDamage(damage);
            run.Audio.Play(TrialSound.Hit);
            run.Hud.DamageNumber(transform.position, applied, blocked ? "BLOCK " : "", new Color(1f, 0.5f, 0.4f));
            run.Visuals.Effect("Hit response", transform.position, 13, 1.3f, 0f,
                blocked ? Color.cyan : new Color(1f, 0.5f, 0.4f), 0.15f);
        }
    }
}
