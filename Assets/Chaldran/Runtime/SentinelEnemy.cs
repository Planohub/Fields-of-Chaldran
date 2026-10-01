using UnityEngine;

namespace Chaldran
{
    public sealed class SentinelEnemy : MonoBehaviour
    {
        public float Essence { get; private set; }
        public float MaxEssence => run.Balance.sentinelEssence;
        public float Armor => run.Balance.sentinelArmor;
        public bool IsAlive => Essence > 0f;
        private enum Phase { Dormant, Hunting, Windup, Recovery, Defeated }
        private Phase phase;
        private PrototypeRun run;
        private Rigidbody2D body;
        private SpriteRenderer sprite;
        private GameObject warning;
        private Vector2 movement, strikeOrigin;
        private float phaseEnds, hitTintEnds;

        public void Initialize(PrototypeRun owner)
        {
            run = owner;
            Essence = owner.Balance.sentinelEssence;
            body = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
            warning = run.Visuals.Make("Sentinel attack warning", transform.position, 10,
                Vector2.one * run.Balance.sentinelAttackRange * 2f, 50);
            warning.GetComponent<SpriteRenderer>().color = new Color(1f, 0.38f, 0.2f, 0.65f);
            warning.SetActive(false);
        }

        private void Update()
        {
            movement = Vector2.zero;
            if (run == null || !IsAlive || !run.IsActive) return;
            Vector2 delta = (Vector2)run.Stats.transform.position - (Vector2)transform.position;
            sprite.color = phase == Phase.Windup ? new Color(1f, 0.62f, 0.3f)
                : Time.time < hitTintEnds ? new Color(0.6f, 1f, 1f) : Color.white;

            if (phase == Phase.Dormant)
            {
                if (!run.Progress.HasWeapon || delta.magnitude > run.Balance.sentinelAggroRange) return;
                phase = Phase.Hunting;
                run.ShowMessage("Sentinel online. Avoid the orange strike zone or block toward the sentinel.", 5f);
            }
            if (phase == Phase.Hunting)
            {
                if (!WorldQuery.HasLineOfSight(transform.position, run.Stats.transform.position)) return;
                if (delta.magnitude <= run.Balance.sentinelAttackRange)
                {
                    phase = Phase.Windup;
                    phaseEnds = Time.time + run.Balance.sentinelWindup;
                    strikeOrigin = transform.position;
                    warning.transform.position = strikeOrigin;
                    warning.SetActive(true);
                }
                else movement = delta.normalized * run.Balance.sentinelSpeed;
            }
            else if (phase == Phase.Windup && Time.time >= phaseEnds)
            {
                warning.SetActive(false);
                if (Vector2.Distance(strikeOrigin, run.Stats.transform.position) <= run.Balance.sentinelAttackRange
                    && WorldQuery.HasLineOfSight(strikeOrigin, run.Stats.transform.position))
                    run.Combat.ReceiveStrike(run.Balance.sentinelDamage, strikeOrigin);
                phase = Phase.Recovery;
                phaseEnds = Time.time + run.Balance.sentinelRecovery;
            }
            else if (phase == Phase.Recovery && Time.time >= phaseEnds) phase = Phase.Hunting;
        }

        private void FixedUpdate()
        {
            if (body != null) body.linearVelocity = run != null && run.IsActive && IsAlive ? movement : Vector2.zero;
        }

        public void ReceiveDamage(float damage, bool critical)
        {
            if (!IsAlive || !run.IsActive) return;
            float applied = Mathf.Min(Essence, CombatMath.NonNegative(damage));
            Essence -= applied;
            hitTintEnds = Time.time + 0.13f;
            run.Hud.DamageNumber(transform.position, applied, critical ? "CRIT " : "",
                critical ? new Color(1f, 0.8f, 0.4f) : new Color(0.65f, 1f, 0.92f));
            if (IsAlive) return;
            phase = Phase.Defeated;
            body.linearVelocity = Vector2.zero;
            body.simulated = false;
            GetComponent<Collider2D>().enabled = false;
            warning.SetActive(false);
            sprite.color = new Color(0.3f, 0.33f, 0.4f, 0.5f);
            run.EnemyDefeated(transform.position);
        }
    }
}
