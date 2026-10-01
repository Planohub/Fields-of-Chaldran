using UnityEngine;

namespace Chaldran
{
    public sealed class TransientSpriteEffect : MonoBehaviour
    {
        private SpriteRenderer sprite;
        private float age, duration;
        private Color tint;
        public void Initialize(float lifetime)
        {
            duration = Mathf.Max(0.01f, lifetime);
            sprite = GetComponent<SpriteRenderer>();
            tint = sprite.color;
        }

        private void Update()
        {
            age += Time.deltaTime;
            if (age >= duration) { Destroy(gameObject); return; }
            sprite.color = new Color(tint.r, tint.g, tint.b, tint.a * (1f - age / duration));
        }
    }
}
