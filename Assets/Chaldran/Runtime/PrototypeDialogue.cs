using System;
using UnityEngine;

namespace Chaldran
{
    public sealed class PrototypeDialogue : MonoBehaviour
    {
        public DialogueSession Session { get; } = new DialogueSession();
        public DialogueDefinition Current { get; private set; }
        public bool IsOpen => Session.IsOpen;
        public DialogueReveal Reveal { get; } = new DialogueReveal();
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
            PreparePage();
            return true;
        }

        private void PreparePage()
        {
            int characters = run.Hud.PrepareDialoguePage(Current.lines[Session.LineIndex]);
            Reveal.Begin(characters, run.Presentation.charactersPerSecond);
            run.Audio.StopTyping();
        }

        private void Update()
        {
            if (!IsOpen) return;
            int previous = Reveal.VisibleCharacters;
            Reveal.Tick(Time.unscaledDeltaTime);
            if (Reveal.VisibleCharacters > previous && run.Hud.HasTypingCharacter(previous, Reveal.VisibleCharacters))
                run.Audio.PlayTyping();
        }

        public void Advance()
        {
            if (!IsOpen) return;
            DialogueConfirm result = Session.ConfirmPage(Reveal);
            if (result == DialogueConfirm.Revealed) { run.Audio.StopTyping(); return; }
            if (result == DialogueConfirm.NextPage) { PreparePage(); return; }
            if (result != DialogueConfirm.Completed) return;
            Action callback = completed;
            completed = null;
            Current = null;
            Time.timeScale = 1f;
            run.Audio.StopTyping();
            callback?.Invoke();
        }

        public void Cancel()
        {
            if (!Session.IsOpen) return;
            Session.Cancel();
            completed = null;
            Current = null;
            Time.timeScale = 1f;
            run.Audio.StopTyping();
        }
    }
}
