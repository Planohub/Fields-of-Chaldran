using UnityEngine;

namespace Chaldran
{
    [CreateAssetMenu(menuName = "Fields of Chaldran/Prototype Balance")]
    public sealed class PrototypeBalance : ScriptableObject
    {
        [Header("Player resources")]
        [Min(1f)] public float maxEssence = 100f;
        [Min(1f)] public float maxResonance = 60f;
        [Min(0f)] public float overrideValue = 10f;
        [Min(0f)] public float playerArmor = 15f;
        [Min(0f)] public float resonanceRegeneration = 5f;
        [Header("Movement and interaction")]
        [Min(0.1f)] public float moveSpeed = 5f;
        [Min(1f)] public float sprintMultiplier = 1.5f;
        [Min(0.1f)] public float interactionRadius = 1.75f;
        [Header("Melee and blocking")]
        [Min(1f)] public float meleeDamage = 22f;
        [Min(0.1f)] public float meleeCooldown = 0.38f;
        [Min(0.1f)] public float meleeRange = 1.55f;
        [Range(1f, 180f)] public float meleeHalfAngle = 70f;
        [Range(0f, 1f)] public float blockedDamageMultiplier = 0.3f;
        [Min(0f)] public float blockCost = 8f;
        [Header("Ashen Crown trial ability")]
        [Min(1f)] public float burstDamage = 35f;
        [Min(0.1f)] public float burstRadius = 2.6f;
        [Min(0f)] public float burstCost = 20f;
        [Min(0.1f)] public float burstCooldown = 4f;
        [Header("Sentinel")]
        [Min(1f)] public float sentinelEssence = 150f;
        [Min(0f)] public float sentinelArmor = 20f;
        [Min(0.1f)] public float sentinelSpeed = 2.2f;
        [Min(0.1f)] public float sentinelAggroRange = 8f;
        [Min(0.1f)] public float sentinelAttackRange = 1.5f;
        [Min(0.1f)] public float sentinelWindup = 0.8f;
        [Min(0.1f)] public float sentinelRecovery = 0.8f;
        [Min(1f)] public float sentinelDamage = 24f;
    }
}
