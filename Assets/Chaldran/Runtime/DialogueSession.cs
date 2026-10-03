namespace Chaldran
{
    public enum DialogueConfirm { None, Revealed, NextPage, Completed }
    // Presentation-independent paging. Closing a session never completes it.
    public sealed class DialogueSession
    {
        public bool IsOpen { get; private set; }
        public int LineIndex { get; private set; }
        public int LineCount { get; private set; }

        public bool Begin(int lineCount)
        {
            if (IsOpen || lineCount < 1 || lineCount > 64) return false;
            LineIndex = 0;
            LineCount = lineCount;
            IsOpen = true;
            return true;
        }

        public bool Advance()
        {
            if (!IsOpen) return false;
            if (++LineIndex < LineCount) return false;
            IsOpen = false;
            return true;
        }

        public void Cancel() { IsOpen = false; LineIndex = 0; }

        public DialogueConfirm ConfirmPage(DialogueReveal reveal)
        {
            if (!IsOpen || reveal == null) return DialogueConfirm.None;
            if (!reveal.Complete) { reveal.RevealAll(); return DialogueConfirm.Revealed; }
            return Advance() ? DialogueConfirm.Completed : DialogueConfirm.NextPage;
        }
    }
}
