# First presentation transition

## Story constraint

For the October 4 direction, see [Story and fidelity roadmap](STORY-AND-FIDELITY-ROADMAP.md): the Oracle library stays 16-bit, the rescue mission follows that distinct encounter, and later 8-bit prisons with a 16-bit hero are a proposed rendering experiment. The current single-profile renderer does not implement mixed fidelity.

Graphics and sound intentionally improve as part of the story. The quarantine tutorial remains primitive. Leaving the personal quarantine unlocks the first richer presentation in the User Directory; it is not an escape from the Sorruin Engine itself. Visual tiers are separate from divine advancement. This implementation adds a small proof of the first transformation, not the full opening chapter or a new plot branch.

The initial profiles use the earlier 8-bit-style tutorial to 16-bit-style overland plan. These are art/audio styles within one Unity game. Starting with a more restricted 4-bit-style palette remains an art direction decision; the profiles expose the settings without changing combat.

## Play the transition

Fetch and update `codex/quarantine-trial-recovered`. Open **Fields of Chaldran > Open Quarantine Trial** and press Play. The weapon, sentinel, token, and gate sequence is unchanged. Interact with the exit terminal after releasing the barrier. Movement locks for the short release transition; music fades down, then a separate overland scene appears with richer visuals and a new arrangement of the same musical motif.

Your current Essence, Resonance, and weapon carry into the clearing. Resonance resumes its normal regeneration there; the transition does not heal you or spend extra resources. World scale, movement speed, interaction radius, and collision sizes remain the same across tiers. Burst and melee cooldowns reset when the new scene loads.

The overland area is a small explorable clearing. Its new Path to the Oracle objective connects arrival, the waystone, a damaged record, a route marker, and the library threshold. Completing the threshold conversation now enables entry into the separate 16-bit library opening; see [its playtest](ORACLE-LIBRARY-PLAYTEST.md). A restoration spring reuses the existing resource restoration mechanic. See [the opening story sequence](USER-DIRECTORY-OPENING.md) for dialogue and saved objectives.

| Control | Behavior |
| --- | --- |
| E at the waystone near the entrance | Read its introductory dialogue, then save position, resources, weapon, tier, and story progress |
| C | Resume the last valid journey checkpoint, including its saved Directory/library scene, including from the tutorial or pause |
| F5 | Start a fresh quarantine tutorial; keeps the previously saved checkpoint until a new escape replaces it |
| Enter after defeat | Retry; the overland scene restores its checkpoint |
| Escape | Pause/resume in each area |

The transition creates an automatic checkpoint at the overland entrance. Completed story beats and waystone saves replace it. Stopping Play Mode or closing the game does not erase the checkpoint. **Open Overland Preview** is a development shortcut: it loads a valid checkpoint if one exists, otherwise it creates an unlocked preview with full resources. The normal player route still passes through the tutorial or resumes a previously unlocked checkpoint.

## Tier profiles

| Profile | Sprite cell | World buffer | Presentation |
| --- | --- | --- | --- |
| QuarantinePresentation | 16x16 pixels | 256x144 | Original simple atlas, basic cues, square-wave melody |
| OverlandPresentation | 32x32 pixels | 512x288 | Grass/tree/stone/hero artwork, a short walking cycle, richer cues, melody with harmony |
| LibraryPresentation | 32x32 pixels | 512x288 | Reuses the 16-bit atlas, walking frames and audio with archive colors and graybox geometry |

The three profiles live in `Assets/Chaldran/Data`. Each owns its atlas layout, world buffer size, background/accent colors, walking frame indices, six action cues, looping music, and music volume. Atlas rectangles are resolved from those settings. Camera grid snapping uses the tier's pixels per world unit; a higher-resolution sprite still occupies the same world size.

The HUD stays readable at both stages rather than deliberately degrading text accessibility. The music and art are development assets made in the existing deterministic pixel/synth pipeline; they establish a visible and audible difference for testing. Run `python Tools/generate_presentation_assets.py` to regenerate the new assets. The original TrialAtlas and effects are not overwritten.

Future tiers can supply additional profiles and their own artwork/audio. A later move to a different rendering technique or perspective will need a matching renderer adapter; profile data alone does not implement a photorealistic finale.

## Checkpoint boundary and failure behavior

This is a version-3 **post-tutorial/story checkpoint**, not a complete RPG save system. It persists the tier/weapon/resources/position, location, Directory progress and library beat count. Library contact and anchor/barrier state derive from those ordered beats. Saving in the middle of the tutorial, inventory, domain selection, upgrades, dungeon permissions, and multiple save slots are future work.

The prototype writes `chaldran-prototype-checkpoint-v3.json` inside Unity's persistent data directory. If absent, it tries v2, then v1 only if v2 is also absent. Valid older saves migrate without deleting their original files; v2 retains its Directory progress and v1 starts at arrival. Writes use a temporary file and retain a `.bak` of the previous checkpoint. A failed write preserves the previous save. Invalid or unsupported checkpoints are rejected; the prototype does not automatically restore the backup. C shows a clear message when there is no valid checkpoint. If the automatic transition save fails, the current scene transfer still uses an in-memory snapshot and a warning is logged.

No save is written from an invalid/dead player state. Resource restoration clamps to current balance caps. Positions are restricted to the scene interior; library saves before anchor release must remain in the accessible corridor. A continued overland run restores completed tutorial permissions without recreating the sentinel or its consumed token.

## Validation

Thirty-five standalone NUnit tests pass for combat/resources/permission order, story order, dialogue completion/cancellation, checkpoint continuity/migration, invalid versions/state, changed resource caps, completed permissions, file round trips/backups, and failed writes. All 46 C# sources pass syntax parsing. Source asset verification checks three scenes/profiles, atlases, both chapters and their dialogue references, all 15 audio clips, GUID coverage, original input identities, and controls.

These checks do not run Unity APIs. The user reported the first presentation transition working in Unity. The preceding story and readability pass were also user-tested in Unity 6000.6.4f1. The new library, seven Unity asset/JSON/component tests and standalone build testing remain pending.

### Unity acceptance checks

1. Import with Unity 6000.6.4f1 and confirm no red Console errors. Run the EditMode test suites.
2. Play the tutorial, including blocked melee, burst, restoration, death/retry, and pause. Verify the lower-resolution presentation remains readable.
3. Before exiting, note current resources. Activate the terminal and check that movement/attacks/pause/replay inputs are blocked during the transition. Confirm the overland sprite scale, sharper detail, walking cycle, music, and action cues change.
4. Verify the weapon and resources carry across, allowing for normal Resonance regeneration. Confirm no sentinel, permission barrier, or duplicate token appears in the clearing.
5. Complete the waystone's introductory dialogue, then save there with E. Move away and spend Resonance with Q. Press C and verify your saved position/resources return. Repeat after stopping and restarting Play Mode, then after closing/reopening the standalone game.
6. Press F5, verify a fresh primitive tutorial starts, and press C to confirm the saved overland tier still resumes. Finishing a new tutorial should replace the old entrance checkpoint.
7. Pause/resume and test aiming in both areas at 720p, 1080p, narrow, and ultrawide sizes.
8. Build for Windows. Trial, overland and Oracle library scenes must be enabled; the trial remains first. Test the transition and persistent continue outside the Editor.

The approach and first library section are now implemented. Follow [library acceptance checks](ORACLE-LIBRARY-PLAYTEST.md) before extending the inner archive or later tier transitions.
