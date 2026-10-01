using UnityEngine;

namespace Chaldran
{
    public static class WorldQuery
    {
        public const int SolidLayer = 8;
        public const int InteractableLayer = 9;
        public const int ActorLayer = 10;
        public const int SolidMask = 1 << SolidLayer;

        public static bool HasLineOfSight(Vector2 from, Vector2 to, PrototypeInteractable target = null)
        {
            RaycastHit2D hit = Physics2D.Linecast(from, to, SolidMask);
            return hit.collider == null || (target != null && hit.collider.GetComponentInParent<PrototypeInteractable>() == target);
        }

        public static ContactFilter2D Filter(int layer, bool triggers)
        {
            ContactFilter2D filter = new ContactFilter2D { useTriggers = triggers };
            filter.SetLayerMask(1 << layer);
            return filter;
        }
    }
}
