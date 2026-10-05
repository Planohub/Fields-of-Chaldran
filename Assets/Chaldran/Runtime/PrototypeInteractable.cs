using UnityEngine;

namespace Chaldran
{
    public enum TrialInteraction { Anomaly, RestorationRelay, WriteAccess, Barrier, Exit, Checkpoint, Landmark, SignalRecord, LibraryThreshold,
        OracleChannel, LoopAnchor, ArchiveLanding }

    public sealed class PrototypeInteractable : MonoBehaviour
    {
        public TrialInteraction Kind { get; private set; }
        private PrototypeRun run;
        private Collider2D barrierCollider;
        private bool used;
        private float nextRestore;
        public bool IsAvailable => !used && run != null && run.IsActive;

        public string Prompt
        {
            get
            {
                switch (Kind)
                {
                    case TrialInteraction.Checkpoint: return run.IsLibrary ? "[E] Commit archive checkpoint" : run.Story.CompletedSteps <= 1
                        ? "[E] Read the directory waystone" : "[E] Save at the directory waystone";
                    case TrialInteraction.Landmark: return "[E] Read the library route marker";
                    case TrialInteraction.SignalRecord: return "[E] Read the damaged record";
                    case TrialInteraction.LibraryThreshold: return run.Story.Complete ? "[E] Mount /oracle/archive"
                        : "[E] Examine the compressed library threshold";
                    case TrialInteraction.OracleChannel: return "[E] Probe the restricted Oracle channel";
                    case TrialInteraction.LoopAnchor: return run.Library.AnchorInterrupted ? "[E] Inspect interrupted anchor"
                        : "[E] Interrupt the loop anchor binding";
                    case TrialInteraction.ArchiveLanding: return "[E] Inspect the resolved archive route";
                    case TrialInteraction.Anomaly: return "[E] Recover the anomalous weapon";
                    case TrialInteraction.RestorationRelay:
                        return Time.time < nextRestore ? "Restoration relay recharging" : "[E] Restore Essence and Resonance";
                    case TrialInteraction.WriteAccess: return "[E] Collect Write-Access token";
                    case TrialInteraction.Barrier: return run.Progress.HasWriteAccess ? "[E] Override Read-Only barrier" : "READ-ONLY: Write-Access token required";
                    default: return "[E] Activate exit terminal";
                }
            }
        }

        public void Initialize(PrototypeRun owner, TrialInteraction kind, Collider2D blocker = null)
        {
            run = owner;
            Kind = kind;
            barrierCollider = blocker;
        }

        public void Interact()
        {
            if (!IsAvailable) return;
            switch (Kind)
            {
                case TrialInteraction.Anomaly:
                    if (!run.Progress.RecoverWeapon()) return;
                    used = true;
                    GetComponent<SpriteRenderer>().color = new Color(0.35f, 0.55f, 0.6f);
                    run.Audio.Play(TrialSound.Pickup);
                    run.ShowMessage("The anomaly resolves into a weapon. Your divine signal is returning.", 5f);
                    break;
                case TrialInteraction.RestorationRelay:
                    if (Time.time < nextRestore) return;
                    nextRestore = Time.time + 10f;
                    run.Stats.Restore();
                    run.Audio.Play(TrialSound.Pickup);
                    run.ShowMessage("Essence and Resonance restored. Relay recharging for 10 seconds.");
                    break;
                case TrialInteraction.WriteAccess:
                    if (!run.Progress.CollectWriteAccess()) return;
                    used = true;
                    gameObject.SetActive(false);
                    run.Audio.Play(TrialSound.Pickup);
                    run.ShowMessage("Write-Access acquired. The northeast barrier can now be overridden.");
                    break;
                case TrialInteraction.Barrier:
                    if (!run.Progress.OpenGate())
                    {
                        run.ShowMessage("Permission denied. Defeat the sentinel and collect its token.");
                        return;
                    }
                    used = true;
                    if (barrierCollider != null) barrierCollider.enabled = false;
                    run.Audio.Play(TrialSound.Unlock);
                    GetComponent<SpriteRenderer>().color = new Color(0.3f, 1f, 0.75f, 0.18f);
                    run.ShowMessage("Permission accepted. Read-Only barrier released.");
                    break;
                case TrialInteraction.Exit:
                    if (!run.Progress.Finish()) return;
                    used = true;
                    run.Audio.Play(TrialSound.Complete);
                    run.BeginTransition();
                    break;
                case TrialInteraction.Checkpoint:
                    if (run.IsLibrary) run.SaveCheckpoint();
                    else if (run.Story.CompletedSteps <= 1) run.BeginStoryBeat(DirectoryBeat.Waystone);
                    else run.SaveCheckpoint();
                    break;
                case TrialInteraction.Landmark:
                    run.BeginStoryBeat(DirectoryBeat.RouteMarker);
                    break;
                case TrialInteraction.SignalRecord:
                    run.BeginStoryBeat(DirectoryBeat.SignalRecord);
                    break;
                case TrialInteraction.LibraryThreshold:
                    if (run.Story.Complete) run.EnterLibrary();
                    else run.BeginStoryBeat(DirectoryBeat.LibraryThreshold);
                    break;
                case TrialInteraction.OracleChannel:
                    run.BeginLibraryBeat(LibraryBeat.OracleContact);
                    break;
                case TrialInteraction.LoopAnchor:
                    run.BeginLibraryBeat(LibraryBeat.AnchorInterrupted);
                    break;
                case TrialInteraction.ArchiveLanding:
                    run.BeginLibraryBeat(LibraryBeat.RouteReached);
                    break;
            }
            run.NotifyChanged();
        }
    }
}
