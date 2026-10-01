using UnityEngine;

namespace Chaldran
{
    public sealed class PrototypeCamera2D : MonoBehaviour
    {
        public Transform Target { get; set; }
        private Vector2 smoothPosition;

        private void Start() { smoothPosition = transform.position; }

        private void LateUpdate()
        {
            if (Target == null) return;
            Vector2 desired = new Vector2(Mathf.Clamp(Target.position.x, -0.5f, 0.5f), Mathf.Clamp(Target.position.y, -1.75f, 1.75f));
            smoothPosition = Vector2.Lerp(smoothPosition, desired, 1f - Mathf.Exp(-8f * Time.deltaTime));
            transform.position = new Vector3(Mathf.Round(smoothPosition.x * 16f) / 16f,
                Mathf.Round(smoothPosition.y * 16f) / 16f, -10f);
        }
    }
}
