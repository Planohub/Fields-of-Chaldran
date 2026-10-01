using UnityEngine;

namespace Chaldran
{
    public sealed class PrototypeYSort : MonoBehaviour
    {
        private SpriteRenderer sprite;
        private void Awake() { sprite = GetComponent<SpriteRenderer>(); }
        private void LateUpdate() { sprite.sortingOrder = 100 - Mathf.RoundToInt(transform.position.y * 10f); }
    }
}
