using System.Collections.Generic;
using UnityEngine;

namespace Chaldran
{
    public sealed class PlayerInteractor : MonoBehaviour
    {
        public PrototypeInteractable Target { get; private set; }
        private PrototypeRun run;
        private readonly List<Collider2D> hits = new List<Collider2D>(12);

        public void Initialize(PrototypeRun owner) { run = owner; }

        private void Update() { RefreshTarget(); }

        private void RefreshTarget()
        {
            Target = null;
            if (run == null || !run.IsActive) return;
            float closest = run.Balance.interactionRadius * run.Balance.interactionRadius;
            hits.Clear();
            Physics2D.OverlapCircle(transform.position, run.Balance.interactionRadius,
                WorldQuery.Filter(WorldQuery.InteractableLayer, true), hits);
            foreach (Collider2D hit in hits)
            {
                PrototypeInteractable candidate = hit.GetComponentInParent<PrototypeInteractable>();
                if (candidate == null || !candidate.IsAvailable) continue;
                float distance = ((Vector2)candidate.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (distance > closest || !WorldQuery.HasLineOfSight(transform.position, candidate.transform.position, candidate)) continue;
                closest = distance;
                Target = candidate;
            }
        }

        public void TryInteract()
        {
            // Recheck on the actual key press, rather than using last frame's cached target.
            RefreshTarget();
            if (Target != null) Target.Interact();
        }
    }
}
