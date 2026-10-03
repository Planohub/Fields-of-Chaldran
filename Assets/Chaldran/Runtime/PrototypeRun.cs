using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chaldran
{
    public sealed class PrototypeRun : MonoBehaviour
    {
        public PrototypeBalance Balance { get; private set; }
        public PresentationProfile Presentation { get; private set; }
        public DirectoryStoryDefinition StoryDefinition { get; private set; }
        public StoryProgress Story { get; } = new StoryProgress();
        public PrototypeDialogue Dialogue { get; set; }
        public bool IsOverland { get; private set; }
        public bool IsTransitioning { get; private set; }
        public string Objective => IsOverland ? StoryDefinition.Objective(Story)
            : Progress.Objective;
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
        public bool IsActive => Stats != null && Stats.Vitals != null && Stats.Vitals.IsAlive
            && !IsPaused && !IsTransitioning && !(Dialogue != null && Dialogue.IsOpen) && (IsOverland || !Progress.Complete);
        public string Message { get; private set; }
        public float MessageUntil { get; private set; }
        public event Action Changed;
        private bool arrivalPrompted;

        public void Initialize(PrototypeBalance balance, PrototypeVisuals visuals, PresentationProfile presentation,
            bool overland, DirectoryStoryDefinition storyDefinition)
        {
            Presentation = presentation;
            IsOverland = overland;
            StoryDefinition = storyDefinition;
            Balance = balance;
            Visuals = visuals;
            Time.timeScale = 1f;
        }

        private IEnumerator Start()
        {
            if (!IsOverland || Story.CompletedSteps != 0) yield break;
            yield return new WaitForSecondsRealtime(0.85f);
            if (!arrivalPrompted && IsActive && Story.CompletedSteps == 0) BeginStoryBeat(DirectoryBeat.Arrival);
        }

        public void BeginStoryBeat(DirectoryBeat beat)
        {
            if (!IsOverland || !IsActive) return;
            int index = (int)beat;
            if (index < 0 || index >= StoryProgress.StepCount) return;
            if (Story.CompletedSteps == 0) { beat = DirectoryBeat.Arrival; index = 0; }
            if (index > Story.CompletedSteps)
            {
                ShowMessage(Objective, 5f);
                return;
            }
            DirectoryBeat completedBeat = beat;
            bool began = Dialogue.Begin(StoryDefinition.steps[index].dialogue, () =>
            {
                if (!Story.TryComplete(completedBeat)) return;
                NotifyChanged();
                bool saved = SaveCurrentCheckpoint();
                Audio.Play(Story.Complete ? TrialSound.Complete : TrialSound.Pickup);
                ShowMessage(saved ? "Story progress saved. " + Objective
                    : "Story advanced, but saving failed. Use the waystone to try again.", 6f);
            });
            if (began && completedBeat == DirectoryBeat.Arrival) arrivalPrompted = true;
        }

        private bool SaveCurrentCheckpoint()
        {
            Vector2 position = Stats.transform.position;
            return JourneyStore.Save(JourneyCheckpoint.Capture(Stats.Vitals,
                Mathf.Clamp(position.x, -10.5f, 10.5f), Mathf.Clamp(position.y, -6.5f, 6.5f), Story));
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
            if (Stats == null || !Stats.Vitals.IsAlive || IsTransitioning || Dialogue.IsOpen || (!IsOverland && Progress.Complete)) return;
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

        public void BeginTransition()
        {
            if (IsOverland || !Progress.Complete || IsTransitioning) return;
            IsTransitioning = true;
            Motor.Stop();
            Combat.IsBlocking = false;
            StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            float remaining = 1.5f;
            while (remaining > 0f)
            {
                Audio.FadeMusic(remaining / 1.5f);
                remaining -= Time.unscaledDeltaTime;
                yield return null;
            }
            JourneyCheckpoint checkpoint = JourneyCheckpoint.Capture(Stats.Vitals, -9f, -5f);
            JourneyStore.Pending = checkpoint;
            if (!JourneyStore.Save(checkpoint)) Debug.LogWarning("The overland checkpoint could not be saved. The transition will still preserve this run in memory.");
            Time.timeScale = 1f;
            AudioListener.pause = false;
            SceneManager.LoadScene(JourneyStore.OverlandScene);
        }

        public void SaveCheckpoint()
        {
            if (!IsOverland || !IsActive) return;
            bool saved = SaveCurrentCheckpoint();
            if (saved) Audio.Play(TrialSound.Pickup);
            ShowMessage(saved ? "Waystone checkpoint saved. Press C to return here."
                : "Checkpoint could not be saved. Your current run is still active.");
        }

        public void ContinueJourney()
        {
            if (IsTransitioning || Dialogue.IsOpen) return;
            if (!JourneyStore.TryLoad(out JourneyCheckpoint checkpoint))
            {
                ShowMessage("No valid overland checkpoint. Complete the quarantine tutorial first.");
                return;
            }
            JourneyStore.Pending = checkpoint;
            Time.timeScale = 1f;
            AudioListener.pause = false;
            SceneManager.LoadScene(JourneyStore.OverlandScene);
        }

        public void NewTrial()
        {
            if (IsTransitioning || Dialogue.IsOpen) return;
            JourneyStore.Pending = null;
            Time.timeScale = 1f;
            AudioListener.pause = false;
            SceneManager.LoadScene(JourneyStore.TrialScene);
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            Visuals?.Dispose();
        }
    }
}
