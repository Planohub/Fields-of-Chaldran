using UnityEngine;

namespace Chaldran
{
    public sealed class LibraryRouteGate : MonoBehaviour
    {
        private PrototypeRun run;
        private Collider2D blocker;
        private SpriteRenderer sprite;
        public void Initialize(PrototypeRun owner, Collider2D collision)
        {
            run = owner; blocker = collision; sprite = GetComponent<SpriteRenderer>();
            run.Changed += Refresh;
            Refresh();
        }
        private void Refresh()
        {
            bool open = run.Library.AnchorInterrupted;
            blocker.enabled = !open;
            sprite.color = open ? new Color(0.35f, 1f, 0.8f, 0.18f) : new Color(1f, 0.4f, 0.5f);
        }
        private void OnDestroy() { if (run != null) run.Changed -= Refresh; }
    }
}
