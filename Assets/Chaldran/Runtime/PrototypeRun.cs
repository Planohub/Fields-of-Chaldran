using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chaldran
{
    public sealed class PrototypeRun : MonoBehaviour
    {
        public PrototypeBalance Balance { get; private set; }
        public PrototypeProgress Progress { get; } = new PrototypeProgress();
        public PrototypeVisuals Visuals { get; private set; }
        public PlayerStats Stats { get; set; }
        public PlayerMotor2D Motor { get; set; }
        public PlayerCombat Combat { get; set; }
        public PlayerInteractor Interactor { get; set; }
        public PrototypeHud Hud { get; set; }
        public SentinelEnemy Sentinel { get; set; }
        public Camera WorldCamera { get; set; }
        public PrototypeAudio Audio { get; set; }
        public bool IsPaused { get; private set; }
        public bool IsActive => Stats != null && Stats.Vitals != null && Stats.Vitals.IsAlive && !IsPaused && !Progress.Complete;
        public string Message { get; private set; }
        public float MessageUntil { get; private set; }
        public event Action Changed;

        public void Initialize(PrototypeBalance balance, PrototypeVisuals visuals)
        {
            Balance = balance;
            Visuals = visuals;
            Time.timeScale = 1f;
        }

        public void NotifyChanged() { Changed?.Invoke(); }
        public void ShowMessage(string text, float seconds = 3.5f)
        {
            Message = text;
            MessageUntil = Time.unscaledTime + seconds;
        }

        public void EnemyDefeated(Vector2 position)
        {
            if (!Progress.DefeatWarden()) return;
            GameObject token = Visuals.Make("Write-Access token", position, 6, Vector2.one * 0.8f);
            token.layer = WorldQuery.InteractableLayer;
            token.AddComponent<CircleCollider2D>().isTrigger = true;
            token.AddComponent<PrototypeInteractable>().Initialize(this, TrialInteraction.WriteAccess);
            token.AddComponent<PrototypeYSort>();
            ShowMessage("Sentinel defeated. Collect the cyan Write-Access token with E.", 5f);
            NotifyChanged();
        }

        public void TogglePause()
        {
            if (Stats == null || !Stats.Vitals.IsAlive || Progress.Complete) return;
            IsPaused = !IsPaused;
            Time.timeScale = IsPaused ? 0f : 1f;
            AudioListener.pause = IsPaused;
            NotifyChanged();
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().path);
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            Visuals?.Dispose();
        }
    }
}
