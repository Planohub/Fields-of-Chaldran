using System.Collections.Generic;
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
        private float nextRoute;
        private Vector2 routeTarget;
        private List<GridPoint> route;
        private const float NavigationRadius = 0.48f;

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
                if (delta.magnitude <= run.Balance.sentinelAttackRange
                    && WorldQuery.HasLineOfSight(transform.position, run.Stats.transform.position))
                {
                    phase = Phase.Windup;
                    phaseEnds = Time.time + run.Balance.sentinelWindup;
                    strikeOrigin = transform.position;
                    warning.transform.position = strikeOrigin;
                    warning.SetActive(true);
                }
                else movement = Pursue(delta);
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

        private Vector2 Pursue(Vector2 delta)
        {
            Vector2 position = transform.position;
            float distance = Mathf.Max(0f, delta.magnitude - run.Balance.sentinelAttackRange * 0.75f);
            if (WorldQuery.HasLineOfSight(position, run.Stats.transform.position)
                && Physics2D.CircleCast(position, NavigationRadius, delta.normalized, distance, WorldQuery.SolidMask).collider == null)
            {
                route = null;
                return delta.normalized * run.Balance.sentinelSpeed;
            }
            Vector2 target = run.Stats.transform.position;
            if (Time.time >= nextRoute && (route == null || route.Count == 0 || Vector2.Distance(routeTarget, target) > 0.75f))
            {
                nextRoute = Time.time + 0.35f;
                routeTarget = target;
                int sx = Mathf.RoundToInt(position.x), sy = Mathf.RoundToInt(position.y);
                int centerX = sx, centerY = sy;
                float closest = float.PositiveInfinity;
                bool found = false;
                for (int x = centerX - 1; x <= centerX + 1; x++)
                for (int y = centerY - 1; y <= centerY + 1; y++)
                {
                    Vector2 node = new Vector2(x, y), step = node - position;
                    if (x < -11 || x > 11 || y < -7 || y > 7 || step.sqrMagnitude >= closest
                        || !Walkable(x, y) || Physics2D.CircleCast(position, NavigationRadius, step.normalized,
                            step.magnitude, WorldQuery.SolidMask).collider != null) continue;
                    closest = step.sqrMagnitude;
                    sx = x; sy = y; found = true;
                }
                route = found ? SentinelRoute.Find(sx, sy, -11, 11, -7, 7,
                    Walkable,
                    (x, y) => Vector2.Distance(new Vector2(x, y), target) <= run.Balance.sentinelAttackRange * 0.9f
                        && WorldQuery.HasLineOfSight(new Vector2(x, y), target)) : null;
                if (route != null) route.Insert(0, new GridPoint(sx, sy));
            }
            while (route != null && route.Count > 0)
            {
                Vector2 waypoint = new Vector2(route[0].X, route[0].Y);
                Vector2 direction = waypoint - position;
                if (direction.magnitude < 0.15f) { route.RemoveAt(0); continue; }
                if (Physics2D.CircleCast(position, NavigationRadius, direction.normalized, direction.magnitude, WorldQuery.SolidMask).collider != null)
                    return Vector2.zero;
                return direction.normalized * run.Balance.sentinelSpeed;
            }
            return Vector2.zero;
        }

        private static bool Walkable(int x, int y)
        {
            return Physics2D.OverlapCircle(new Vector2(x, y), NavigationRadius, WorldQuery.SolidMask) == null;
        }

        private void FixedUpdate()
        {
            if (body != null) body.linearVelocity = run != null && run.IsActive && IsAlive ? movement : Vector2.zero;
        }

        public void ReceiveDamage(float damage, bool critical)
        {
            if (!IsAlive || !run.IsActive) return;
            float applied = Mathf.Min(Essence, CombatMath.NonNegative(damage));
            if (applied > 0f && phase == Phase.Dormant) phase = Phase.Hunting;
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
