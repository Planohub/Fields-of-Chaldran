# First presentation transition

## Story constraint

Graphics and sound intentionally improve as part of the story. The quarantine tutorial remains primitive. Leaving the personal quarantine unlocks the first richer presentation in the User Directory; it is not an escape from the Sorruin Engine itself. Visual tiers are separate from divine advancement. This implementation adds a small proof of the first transformation, not the full opening chapter or a new plot branch.

The initial profiles use the earlier 8-bit-style tutorial to 16-bit-style overland plan. These are art/audio styles within one Unity game. Starting with a more restricted 4-bit-style palette remains an art direction decision; the profiles expose the settings without changing combat.

## Play the transition

Fetch and update `codex/quarantine-trial-recovered`. Open **Fields of Chaldran > Open Quarantine Trial** and press Play. The weapon, sentinel, token, and gate sequence is unchanged. Interact with the exit terminal after releasing the barrier. Movement locks for the short release transition; music fades down, then a separate overland scene appears with richer visuals and a new arrangement of the same musical motif.

Your current Essence, Resonance, and weapon carry into the clearing. Resonance resumes its normal regeneration there; the transition does not heal you or spend extra resources. World scale, movement speed, interaction radius, and collision sizes remain the same across tiers. Burst and melee cooldowns reset when the new scene loads.

The overland area is a small explorable clearing. There is no new combat encounter or Oracle dialogue in it yet. A boundary marker distinguishes quarantine release from simulation escape. A restoration spring reuses the existing resource restoration mechanic.

| Control | Behavior |
| --- | --- |
| E at the waystone near the entrance | Save overland position, resources, weapon, and presentation tier |
| C | Resume the last valid overland checkpoint, including from the tutorial or pause |
| F5 | Start a fresh quarantine tutorial; keeps the previously saved checkpoint until a new escape replaces it |
| Enter after defeat | Retry; the overland scene restores its checkpoint |
| Escape | Pause/resume in either area |

The transition creates an automatic checkpoint at the overland entrance. Waystone saves replace it. Stopping Play Mode or closing the game does not erase the checkpoint. **Open Overland Preview** is a development shortcut: it loads a valid checkpoint if one exists, otherwise it creates an unlocked preview with full resources. The normal player route still passes through the tutorial or resumes a previously unlocked checkpoint.

## Tier profiles

| Profile | Sprite cell | World buffer | Presentation |
| --- | --- | --- | --- |
| QuarantinePresentation | 16x16 pixels | 256x144 | Original simple atlas, basic cues, square-wave melody |
| OverlandPresentation | 32x32 pixels | 512x288 | New grass/tree/stone/hero artwork, a short walking cycle, richer cues, melody with harmony |

Both profiles live in `Assets/Chaldran/Data`. Each owns its atlas layout, world buffer size, background/accent colors, walking frame indices, six action cues, looping music, and music volume. Atlas rectangles are resolved from those settings. Camera grid snapping uses the tier's pixels per world unit; a higher-resolution sprite still occupies the same world size.

The HUD stays readable at both stages rather than deliberately degrading text accessibility. The music and art are development assets made in the existing deterministic pixel/synth pipeline; they establish a visible and audible difference for testing. Run `python Tools/generate_presentation_assets.py` to regenerate the new assets. The original TrialAtlas and effects are not overwritten.

Future tiers can supply additional profiles and their own artwork/audio. A later move to a different rendering technique or perspective will need a matching renderer adapter; profile data alone does not implement a photorealistic finale.

## Checkpoint boundary and failure behavior

This is a version-1 **post-tutorial checkpoint**, not a complete RPG save system. It persists only the current overland tier/weapon/resources/position. Saving in the middle of the tutorial, inventory, domain selection, upgrades, world permissions beyond the trial, and multiple save slots are future work.

The prototype writes `chaldran-prototype-checkpoint-v1.json` inside Unity's persistent data directory. Writes use a temporary file and retain a `.bak` of the previous checkpoint. A failed write preserves the previous save. Invalid or unsupported checkpoints are rejected; the prototype does not automatically restore the backup. C shows a clear message when there is no valid checkpoint. If the automatic transition save fails, the current scene transfer still uses an in-memory snapshot and a warning is logged.

No save is written from an invalid/dead player state. Resource restoration clamps to current balance caps. Positions are restricted to the clearing's interior. A continued overland run restores completed tutorial permissions without recreating the sentinel or its consumed token.

## Validation

Eleven standalone NUnit tests pass for combat/resources/permission order, checkpoint continuity, invalid versions/state, changed resource caps, completed permissions, file round trips/backups, and failed writes. All 30 C# sources pass syntax parsing. Source asset verification checks both scenes, both profile references, both atlases, all 14 audio clips, GUID coverage, original input identities, and new controls.

These checks do not run Unity APIs. Unity compilation, profile import, audio listening, Play Mode, and standalone build testing are still required.

### Unity acceptance checks

1. Import with Unity 6000.5.6f1 and confirm no red Console errors. Run the EditMode test suites.
2. Play the tutorial, including blocked melee, burst, restoration, death/retry, and pause. Verify the lower-resolution presentation remains readable.
3. Before exiting, note current resources. Activate the terminal and check that movement/attacks/pause/replay inputs are blocked during the transition. Confirm the overland sprite scale, sharper detail, walking cycle, music, and action cues change.
4. Verify the weapon and resources carry across, allowing for normal Resonance regeneration. Confirm no sentinel, permission barrier, or duplicate token appears in the clearing.
5. Move to the waystone and save with E. Move away and spend Resonance with Q. Press C and verify your saved position/resources return. Repeat after stopping and restarting Play Mode, then after closing/reopening the standalone game.
6. Press F5, verify a fresh primitive tutorial starts, and press C to confirm the saved overland tier still resumes. Finishing a new tutorial should replace the old entrance checkpoint.
7. Pause/resume and test aiming in both areas at 720p, 1080p, narrow, and ultrawide sizes.
8. Build for Windows. Both trial and overland scenes must be enabled; the trial remains first. Test the transition and persistent continue outside the Editor.

After this proof passes, extend the canonical opening, checkpoints, art direction, and later tier transitions in small stages.
