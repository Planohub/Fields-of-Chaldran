# Fields of Chaldran

A 2D top-down RPG by Shawn Plano. An awakened god breaks the Sorruin Engine's cognitive restraints and eventually frees other quarantined gods. The development branch now includes the **Quarantine Trial**, its **8-bit → 16-bit transition**, the **User Directory approach**, and the **first playable Oracle library section**. The Oracle is distinct from the other gods; the wider rescue mission follows the full Oracle encounter.

## Update through GitHub and PowerShell

The active project uses **Unity 6000.6.4f1**. Keep working in the upgraded project folder. Close Unity before pulling, then run:

```powershell
Set-Location -LiteralPath "F:\GameProjects\Fields of Chaldran - Unity66-Test"
git status --short
git branch --show-current
```

With a clean working tree on `codex/quarantine-trial-recovered`:

```powershell
git pull --ff-only
```

Preserve any local changes before pulling. If the branch differs or Git reports divergence, resolve that situation before opening the updated project; do not discard work or force a reset. These changes are checked directly into the repository. No package installer or Unity package import is required.

Open the same folder in Unity 6000.6.4f1 and let import/compilation finish. Choose **Fields of Chaldran → Open Quarantine Trial**, then press Play. The scene assembles its world at runtime; its Edit Mode gizmos show layout markers. `Assets/Scenes/SampleScene.unity` remains the original movement blockout.

## Play

| Input | Action |
| --- | --- |
| WASD | Move |
| Left Shift | Sprint |
| Mouse position | Aim attacks and face incoming strikes |
| Left mouse | Melee strike; reveal/advance dialogue while reading |
| Right mouse, held | Directional block; suppresses Resonance regeneration |
| E | Use the nearest available object within range and line of sight |
| Q | Ashen Burst: a short-range fire ability |
| E / Enter | Reveal/advance dialogue |
| Escape | Close dialogue, or pause/resume gameplay |
| Enter after defeat | Retry |
| C | Resume the last valid journey checkpoint, including its scene |
| F5 | Start a fresh tutorial; retain the prior checkpoint until a new escape replaces it |

Recover the weapon from the cyan anomaly, defeat the bronze sentinel, collect its Write-Access token, override the red northeast barrier, and activate the exit terminal. Block while aiming toward incoming strikes or move out of the orange warning circle. The green relay restores resources with a ten-second recharge.

Escape transfers your weapon and resources into the sharper 16-bit User Directory. Read the arrival, southwest waystone, eastern damaged record, northeast route marker, and library threshold in order. Follow the objective and gold beacon. Gameplay pauses during dialogue; closing early does not complete a beat. Completed beats save automatically. After its introduction, E at the waystone saves normally.

After the threshold conversation, **press E there again to mount `/oracle/archive`**. If your earlier checkpoint already completed the approach, press C and use that threshold to enter immediately.

In the library:

1. Finish the mount output and walk east through the blue route. It returns you past the same red index book.
2. Follow the gold beacon to the restricted Oracle channel and finish the contact conversation.
3. Finish the marked anchor interaction. The blue loop disappears and the physical route barrier opens.
4. Cross east and inspect the archive landing. This completes the first section; the inner archive encounter comes next.

The library stays **16-bit**, including the hero and audio. C restores the library scene, completed contact, anchor/barrier state, position and resources. There is an entrance checkpoint terminal for manual saves. **Open Oracle Library Preview** is a development shortcut; without a matching library save, it starts an unlocked preview at the entrance.

## Changes in the Oracle library milestone

- Added `OracleLibraryPrototype.unity`, an editable library story, four dialogue assets, and a 16-bit presentation profile using existing development art/audio.
- Connected the completed User Directory threshold to the library scene.
- Added a real corridor redirect, repeated landmark, ordered Oracle contact, anchor interruption, physical barrier release, and archive landing.
- Kept hacker terminology prominent and removed the approach's implication that the Oracle is simply another god to rescue.
- Added version-3 checkpoints with explicit scene identity and library progress. Valid version-1/2 saves migrate without losing User Directory progress or resources.
- Reset pending transfer state on Play startup, including when domain reload is disabled.
- Added library progression/migration tests, Unity asset/JSON/component checks, Editor preview/gizmos, and the library to enabled build scenes.
- Updated this README, the roadmap and chapter notes, and added [Oracle library playtest instructions](Docs/ORACLE-LIBRARY-PLAYTEST.md).

The Unity 6.6 migration was a separate, previously published change. **This milestone adds playable content**, rather than only upgrading the Editor.

## Current implementation and scope

The prototype has separate stats, combat, movement, interaction, dialogue, presentation and progression code. It includes resource bars, live objectives, cooldowns, damage numbers, pause/retry, retro typing feedback, guard obstacle routing, and versioned journey checkpoints. Balance remains editable in `Assets/Chaldran/Data/QuarantineBalance.asset`.

Quarantine uses 16px sprites and a 256×144 world buffer. User Directory and Oracle library use 32px sprites and a 512×288 buffer with walking frames and richer music/cues. These are development assets for grayboxing. No new Asset Store purchases or production assets are needed for this milestone.

The library's inner archive, warden, relic, final Oracle outcome and wider rescue handoff are upcoming. Full inventory/domain progression, expanded tutorial and sword animation also remain upcoming. Later 8-bit god prisons with a 16-bit hero are a proposed rendering experiment; no 32-bit upgrade or mixed-fidelity renderer is included here.

## Validation

**35 standalone C# tests passed** across combat, permissions, resources, checkpoints, dialogue and ordered chapter progress. **All 46 C# sources passed syntax parsing.** Source checks passed for metadata, three scenes/profiles, both chapters and their dialogue references, atlases, retro cue, input identities and all 15 audio clips.

Seven additional Unity EditMode tests cover actual asset import, JsonUtility and anchor/loop component state. **Those require Unity and have not run in the authoring environment.** The user playtested the preceding content and, on October 5, the new library section in 6000.6.4f1 with expected behavior and no reported errors. The library's motivation and puzzle were confusing, and a visible Oracle was missing. EditMode results and a Windows build remain unverified. An earlier Editor exit error remains unresolved.

Run **Window → General → Test Runner → EditMode**, then follow [the library acceptance checks](Docs/ORACLE-LIBRARY-PLAYTEST.md). Optional source verification: `python Tools/verify_trial_assets.py`. Python is not needed to play the checked-in scenes.

## October 5 story review

Recorded the successful library playthrough and the confusing story/puzzle feedback. Added a proposed opening-to-Oracle story spine, rewrote the encounter draft around a visible meeting, and updated the roadmap/playtest status. New story explanations remain proposals for discussion.

## Next development milestone

The immediate priority is story clarity: give the hero a reason to seek the Oracle, show the Oracle beyond the gate, explain one redirect, make route editing a deliberate action, and deliver a direct meeting. The [story spine](Docs/STORY-SPINE.md) and [revised library draft](Docs/ORACLE-LIBRARY-DRAFT.md) propose that sequence. **These latest changes are documentation only; the proposed scene revision is not implemented yet.**

After that meeting is clear, develop the inner archive, containment response, relic function and wider rescue handoff. Keep the library at 16-bit. The next rendering experiment follows the Oracle encounter.

See [story/fidelity roadmap](Docs/STORY-AND-FIDELITY-ROADMAP.md), [Oracle chapter draft](Docs/ORACLE-LIBRARY-DRAFT.md), [remaining milestones](Docs/NEXT-MILESTONES.md), [Unity migration record](Docs/UNITY-6.6-UPGRADE.md), [User Directory opening](Docs/USER-DIRECTORY-OPENING.md), [art handoff](Docs/ART-HANDOFF.md), [presentation progression](Docs/PRESENTATION-PROGRESSION.md), and [combat notes](Docs/QUARANTINE-TRIAL.md).
