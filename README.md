# Fields of Chaldran

A 2D top-down RPG by Shawn Plano. An awakened god breaks the Sorruin Engine's cognitive restraints and frees other quarantined gods. The current development branch adds a short **Quarantine Trial** for validating the gameplay foundation.

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
| Enter | Retry after defeat or trial completion |

Approach the cyan anomaly near the starting position and recover your weapon. Fight the bronze sentinel; its orange circle warns of an incoming strike. Move out of the circle or block while aiming toward the sentinel. The green relay on the west side restores resources, with a ten-second recharge.

Defeat the sentinel, collect its Write-Access token with E, override the red northeast barrier, then use the exit terminal. This trial consumes its token when opening the gate.

## Current implementation

- Separate runtime stats, combat calculations, input routing, movement, interaction, presentation, and trial progression.
- Essence and Resonance bars, a live objective, sentinel health, cooldown feedback, damage numbers, pause, defeat/retry, and completion.
- Original temporary pixel sprites, a 384×216 point-filtered world buffer, and six short original synthesized sound effects.
- Editable tuning in `Assets/Chaldran/Data/QuarantineBalance.asset`.
- Preserved original input action IDs; additional bindings support the trial.
- Corrected named input callback cleanup in the original `PlayerController`.

This is a development trial. The Ashen Crown is assigned for testing; full domain selection, progression, inventory, persistence, Oracle dialogue, spatial loops, and the presentation-tier transition are subsequent work. The trial art and sounds establish feedback and readability while final assets are developed.

## Validation

Five tests in `Assets/Chaldran/Tests/Editor/CombatFrameworkTests.cs` cover damage order, bounded Override scaling, invalid damage, resource/death behavior, and permission progression. They passed as standalone C# tests using NUnit 3.14.0. All 24 C# files passed a syntax parse, and source asset checks passed.

**Unity compilation, Play Mode, profiling, audio playback, and a Windows build have not been run in the authoring environment.** Open **Window → General → Test Runner** to run the EditMode tests in Unity, then follow [the playtest checks](Docs/QUARANTINE-TRIAL.md). The branch should remain under review until those checks pass.

Optional source asset verification: `python Tools/verify_trial_assets.py`. Reproduce the trial atlas and SFX with `python Tools/generate_trial_assets.py` (Python 3 and Pillow). Unity users do not need Python to play the checked-in scene.

## Next development milestone

Validate this trial in Unity, address observed issues, and tune combat. Then add versioned save/checkpoint state and the first visual transition, followed by the Broken Oracle library mechanics. See [production notes](Docs/QUARANTINE-TRIAL.md).
