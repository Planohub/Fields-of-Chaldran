using UnityEngine;

namespace Chaldran
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerMotor2D : MonoBehaviour
    {
        public Vector2 Facing { get; set; } = Vector2.up;
        public Vector2 MoveInput { get; set; }
        public bool Sprinting { get; set; }
        private Rigidbody2D body;
        private PrototypeRun run;

        public void Initialize(PrototypeRun owner) { run = owner; body = GetComponent<Rigidbody2D>(); }

        public void Stop()
        {
            MoveInput = Vector2.zero;
            if (body != null) body.linearVelocity = Vector2.zero;
        }

        private void FixedUpdate()
        {
            if (body == null || run == null) return;
            if (!run.IsActive) { body.linearVelocity = Vector2.zero; return; }
            float speed = run.Balance.moveSpeed;
            if (run.Combat.IsBlocking) speed *= 0.45f;
            else if (Sprinting) speed *= run.Balance.sprintMultiplier;
            body.linearVelocity = Vector2.ClampMagnitude(MoveInput, 1f) * speed;
        }
    }
}
