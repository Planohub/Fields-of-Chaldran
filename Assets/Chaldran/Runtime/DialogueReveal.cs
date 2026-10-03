using System;

namespace Chaldran
{
    // Unscaled time is supplied by the controller so reading can pause gameplay.
    public sealed class DialogueReveal
    {
        public int VisibleCharacters { get; private set; }
        public int TotalCharacters { get; private set; }
        public bool Complete => VisibleCharacters >= TotalCharacters;
        private double elapsed;
        private float rate;

        public void Begin(int characters, float charactersPerSecond)
        {
            TotalCharacters = Math.Max(0, characters);
            rate = float.IsNaN(charactersPerSecond) || float.IsInfinity(charactersPerSecond)
                ? 0f : Math.Max(0f, charactersPerSecond);
            elapsed = 0;
            VisibleCharacters = rate > 0 ? 0 : TotalCharacters;
        }

        public void Tick(float unscaledSeconds)
        {
            if (Complete || unscaledSeconds <= 0 || float.IsNaN(unscaledSeconds) || float.IsInfinity(unscaledSeconds)) return;
            elapsed += unscaledSeconds;
            VisibleCharacters = (int)Math.Min(TotalCharacters, Math.Floor(elapsed * rate + 0.000001));
        }

        public void RevealAll() { VisibleCharacters = TotalCharacters; }
    }
}
