using UnityEngine;
using UnityEngine.InputSystem;

namespace Chaldran
{
    public sealed class PlayerInputRouter : MonoBehaviour
    {
        private InputActionAsset actions;
        private InputAction move, sprint, interact, attack, block, ability, pause, retry, aim, resumeCheckpoint, newTrial;
        private PrototypeRun run;

        public void Initialize(PrototypeRun owner, InputActionAsset source)
        {
            run = owner;
            // Each player owns a private action asset; disabling it cannot disable other consumers.
            actions = Instantiate(source);
            move = actions.FindAction("Player/Move", true);
            sprint = actions.FindAction("Player/Sprint", true);
            interact = actions.FindAction("Player/Interact", true);
            attack = actions.FindAction("Player/Attack", true);
            block = actions.FindAction("Player/Block", true);
            ability = actions.FindAction("Player/Ability", true);
            pause = actions.FindAction("Player/Pause", true);
            retry = actions.FindAction("Player/Retry", true);
            aim = actions.FindAction("Player/Aim", true);
            resumeCheckpoint = actions.FindAction("Player/Continue", true);
            newTrial = actions.FindAction("Player/NewTrial", true);
            if (isActiveAndEnabled) actions.Enable();
        }

        private void OnEnable() { if (Application.isPlaying) actions?.Enable(); }
        private void OnDisable()
        {
            actions?.Disable();
            if (run != null && run.Motor != null) run.Motor.MoveInput = Vector2.zero;
        }

        private void OnDestroy() { if (actions != null) Destroy(actions); }

        private void Update()
        {
            if (actions == null || run == null) return;
            if (run.IsTransitioning) return;
            if (run.Dialogue.IsOpen)
            {
                if (pause.WasPressedThisFrame()) run.Dialogue.Cancel();
                else if (interact.WasPressedThisFrame() || retry.WasPressedThisFrame()) run.Dialogue.Advance();
                return;
            }
            if (newTrial.WasPressedThisFrame()) { run.NewTrial(); return; }
            if (resumeCheckpoint.WasPressedThisFrame()) { run.ContinueJourney(); return; }
            if (pause.WasPressedThisFrame()) run.TogglePause();
            if (retry.WasPressedThisFrame() && (!run.Stats.Vitals.IsAlive || (!run.IsOverland && run.Progress.Complete)))
            {
                run.Restart();
                return;
            }
            if (!run.IsActive)
            {
                run.Motor.MoveInput = Vector2.zero;
                run.Combat.IsBlocking = false;
                run.Stats.RegenerationSuppressed = false;
                return;
            }

            run.Motor.MoveInput = move.ReadValue<Vector2>();
            run.Motor.Sprinting = sprint.IsPressed();
            // The world is letterboxed, so aim must use the actual world image rectangle.
            Vector2 direction = run.Hud.PointerToWorld(aim.ReadValue<Vector2>()) - (Vector2)transform.position;
            if (direction.sqrMagnitude > 0.01f) run.Motor.Facing = direction.normalized;
            run.Combat.IsBlocking = block.IsPressed() && run.Progress.HasWeapon;
            run.Stats.RegenerationSuppressed = run.Combat.IsBlocking;
            if (interact.WasPressedThisFrame())
            {
                if (run.IsOverland && run.Story.CompletedSteps == 0) run.BeginStoryBeat(DirectoryBeat.Arrival);
                else run.Interactor.TryInteract();
                if (!run.IsActive) return;
            }
            if (attack.WasPressedThisFrame()) run.Combat.Attack();
            if (ability.WasPressedThisFrame()) run.Combat.Burst();
        }
    }
}
