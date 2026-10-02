using UnityEngine;

namespace Chaldran
{
    public sealed class AvatarPresentation : MonoBehaviour
    {
        private PrototypeRun run;
        private SpriteRenderer sprite;
        public void Initialize(PrototypeRun owner) { run = owner; sprite = GetComponent<SpriteRenderer>(); }
        private void LateUpdate()
        {
            if (run == null || sprite == null) return;
            int[] frames = run.Presentation.walkingFrames;
            bool moving = run.IsActive && run.Motor.MoveInput.sqrMagnitude > 0.01f;
            int index = moving && frames != null && frames.Length > 0
                ? frames[(int)(Time.time * 6f) % frames.Length] : 2;
            if (index < 0 || index >= run.Presentation.columns * run.Presentation.rows) index = 2;
            sprite.sprite = run.Visuals.GetSprite(index);
            sprite.flipX = run.Motor.Facing.x < -0.1f;
        }
    }
}
