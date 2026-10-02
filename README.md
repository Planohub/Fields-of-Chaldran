# Fields of Chaldran

A 2D top-down RPG by Shawn Plano. An awakened god breaks the Sorruin Engine's cognitive restraints and frees other quarantined gods. The development branch includes a **Quarantine Trial** and its first story-driven visual/audio transition into an overland **User Directory** clearing. The tutorial deliberately retains simple presentation.

## Open the new trial

1. Fetch the repository and switch to `codex/quarantine-trial-recovered`. Commit or stash local changes before switching branches.
2. Open the project with **Unity 6000.5.6f1** and let package resolution and script compilation finish.
3. Choose **Fields of Chaldran → Open Quarantine Trial**, or open `Assets/Chaldran/Scenes/QuarantinePrototype.unity` directly.
4. Press Play. The trial assembles its room, avatar, sentinel, interactables, and HUD at startup. Before Play, scene gizmos show its layout markers.

The existing `Assets/Scenes/SampleScene.unity` remains a movement blockout. Its attack/interact messages remain placeholders. Test the new gameplay in **QuarantinePrototype**.

## Play

| Input | Action |
| --- | --- |
| WASD | Move |
| Left Shift | Sprint |
| Mouse position | Aim attacks and face incoming strikes |
| Left mouse | Melee strike |
| Right mouse, held | Directional block; suppresses Resonance regeneration |
| E | Use the closest available object within range and line of sight |
| Q | Ashen Burst: a short-range fire ability |
| Escape | Pause / resume |
| Enter | Retry after defeat |
| C | Resume the last valid overland checkpoint |
| F5 | Start a fresh tutorial |

Approach the cyan anomaly near the starting position and recover your weapon. Fight the bronze sentinel; its orange circle warns of an incoming strike. Move out of the circle or block while aiming toward the sentinel. The green relay on the west side restores resources, with a ten-second recharge.

Defeat the sentinel, collect its Write-Access token with E, override the red northeast barrier, then use the exit terminal. The token is consumed when opening the gate. The terminal now transfers you to the richer overland tier with your weapon and current resources, and saves an entrance checkpoint. E at the overland waystone saves another checkpoint; C resumes it and F5 replays the tutorial.

## Current implementation

- Separate runtime stats, combat calculations, input routing, movement, interaction, presentation, and trial progression.
- Essence and Resonance bars, a live objective, sentinel health, cooldown feedback, damage numbers, pause, defeat/retry, and completion.
- Editable presentation profiles: 16px sprites/256×144 world in quarantine; 32px sprites/512×288 world, a walking cycle, and richer music/cues in overland.
- Automatic post-tutorial checkpoint plus a waystone save, with a versioned resource/position/tier snapshot.
- Editable tuning in `Assets/Chaldran/Data/QuarantineBalance.asset`.
- Preserved original input action IDs; additional bindings support the trial.
- Corrected named input callback cleanup in the original `PlayerController`.

This is a development trial. The Ashen Crown is assigned for testing; full domain selection, progression, inventory, full RPG persistence, Oracle dialogue, spatial loops, and later presentation tiers are subsequent work. The trial art and sounds establish feedback and readability while final assets are developed.

## Validation

Eleven tests across the combat and checkpoint suites passed as standalone C# tests using NUnit 3.14.0. All 30 C# files passed a syntax parse. Metadata, both scenes/profiles/atlases, input bindings, and all 14 audio clips passed source checks. The user reported the earlier trial working in Unity; the new transition still needs a Unity playtest.

**Unity compilation, Play Mode, profiling, audio playback, and a Windows build have not been run in the authoring environment.** Open **Window → General → Test Runner** to run the EditMode tests in Unity, then follow [the transition playtest checks](Docs/PRESENTATION-PROGRESSION.md). The branch should remain under review until those checks pass.

Optional source asset verification: `python Tools/verify_trial_assets.py`. Reproduce the trial atlas and SFX with `python Tools/generate_trial_assets.py` (Python 3 and Pillow). Use `python Tools/generate_presentation_assets.py` for the new overland assets and musical tiers. Unity users do not need Python to play either checked-in scene.

## Next development milestone

Validate the first transition and checkpoint resume in Unity, then extend the canonical opening before building the Broken Oracle library mechanics. See [presentation progression](Docs/PRESENTATION-PROGRESSION.md) and [combat/production notes](Docs/QUARANTINE-TRIAL.md).
