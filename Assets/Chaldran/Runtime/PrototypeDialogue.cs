using System;
using UnityEngine;

namespace Chaldran
{
    public sealed class PrototypeDialogue : MonoBehaviour
    {
        public DialogueSession Session { get; } = new DialogueSession();
        public DialogueDefinition Current { get; private set; }
        public bool IsOpen => Session.IsOpen;
        private PrototypeRun run;
        private Action completed;

        public void Initialize(PrototypeRun owner) { run = owner; }

        public bool Begin(DialogueDefinition definition, Action onCompleted)
        {
            if (run == null || !run.IsActive || definition == null || !definition.IsValid
                || !Session.Begin(definition.lines.Length)) return false;
            Current = definition;
            completed = onCompleted;
            run.Motor.Stop();
            run.Combat.IsBlocking = false;
            run.Stats.RegenerationSuppressed = false;
            Time.timeScale = 0f;
            return true;
        }

        public void Advance()
        {
            if (!Session.Advance()) return;
            Action callback = completed;
            completed = null;
            Current = null;
            Time.timeScale = 1f;
            callback?.Invoke();
        }

        public void Cancel()
        {
            if (!Session.IsOpen) return;
            Session.Cancel();
            completed = null;
            Current = null;
            Time.timeScale = 1f;
        }
    }
}
