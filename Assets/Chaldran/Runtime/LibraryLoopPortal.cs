using UnityEngine;

namespace Chaldran
{
    public sealed class LibraryLoopPortal : MonoBehaviour
    {
        private PrototypeRun run;
        private Vector2 destination;
        private float nextLoop;
        public void Initialize(PrototypeRun owner, Vector2 returnPosition)
        {
            run = owner; destination = returnPosition;
            run.Changed += Refresh;
            Refresh();
        }
        private void Refresh()
        {
            bool resolved = run.Library.AnchorInterrupted;
            GetComponent<Collider2D>().enabled = !resolved;
            GetComponent<SpriteRenderer>().enabled = !resolved;
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            if (run == null || !run.IsActive || run.Library.CompletedSteps == 0 || run.Library.AnchorInterrupted
                || Time.time < nextLoop || other.GetComponentInParent<PlayerStats>() != run.Stats) return;
            nextLoop = Time.time + 1f;
            run.Motor.Stop();
            Rigidbody2D body = run.Stats.GetComponent<Rigidbody2D>();
            body.position = destination;
            run.Stats.transform.position = destination;
            run.ObserveLibraryLoop();
        }
        private void OnDestroy() { if (run != null) run.Changed -= Refresh; }
    }
}
