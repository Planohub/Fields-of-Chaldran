# Quarantine Trial — implementation and verification

## Scope

This short development scene adds an actual objective sequence and combat to the project's existing movement foundation. It is a mechanics trial, not the final tutorial or a replacement for the game's AGDD. Existing SampleScene layout and asset GUIDs are preserved.

The avatar begins without a weapon. Interacting with the anomaly enables melee and the Ashen Crown trial ability. A sentinel drops a token when defeated. The token must be collected, the barrier must be opened, and the exit must be activated in that order. There is no persistent save yet; Enter starts a fresh run after death or completion.

## Prototype combat rules

These are initial tunable values, not previously approved final balance:

| Parameter | Initial value |
| --- | --- |
| Essence / Resonance | 100 / 60 |
| Player armor / Override | 15 / 10 |
| Resonance regeneration | 5 per second; stopped while holding block |
| Melee | 22 raw damage; 0.38s cooldown; 1.55-unit range; 70° half-angle |
| Block | 8 Resonance per successful frontal block; receive 30% of mitigated damage |
| Ashen Burst | 35 raw damage; 2.6-unit radius; 20 Resonance; 4s cooldown |
| Sentinel | 150 Essence; 20 armor; 24 raw strike damage |
| Sentinel warning / recovery | 0.8s / 0.8s |
| Interaction | 1.75-unit center distance and unobstructed line of sight |
| Restoration relay | Full resources; 10s recharge |

For nonnegative Override `O`:

- Critical chance = `0.05 + 0.30 * O / (O + 100)`.
- Armor penetration = `0.50 * O / (O + 100)`.
- Effective armor = `armor * (1 - penetration)`.
- Damage = `rawDamage * 100 / (100 + effectiveArmor)`, multiplied by `1.5` for a critical hit and by the block multiplier when applicable.

Essence increases the health pool; armor is a separate mitigation value. This prevents the same durability bonus being applied twice. Resource and damage input validation prevents negative/NaN/infinite values from corrupting current values. The normal balance values remain editable in the ScriptableObject.

The sentinel is a small state machine: dormant, hunting, warning, recovery, defeated. It stops to warn before striking a fixed circular area. It does not navigate around obstacles when line of sight is blocked; pathfinding is future work. Player attacks and interactions also check walls.

## Unity checks before merging

1. Import with 6000.5.6f1. Confirm no compiler errors, missing scripts, missing font, or magenta sprites.
2. Open QuarantinePrototype and run. Confirm HUD and all room objects render, and the six cues play at comfortable levels.
3. Confirm movement speed is consistent on diagonals, sprint works, and boundaries/pillars stop the avatar.
4. Press E away from objects: nothing should fire. Approach the anomaly: its prompt appears and E changes the objective. Check that walls prevent interaction through them.
5. Verify melee hits within range in the aiming direction, cooldown prevents unlimited clicks, and walls block hits.
6. Verify the sentinel's warning gives a clear opportunity to move. A frontal block costs Resonance and reduces damage; rear attacks still hurt. Empty Resonance cannot pay for a block. Release block to regenerate.
7. Cast Q: it spends 20 Resonance, affects nearby visible targets, and cannot repeat until its cooldown ends.
8. Let the sentinel kill the avatar. Inputs stop affecting gameplay; Enter restarts with full resources and fresh objective state.
9. Defeat the sentinel. Confirm the token appears once, E collects it, the barrier consumes it and releases collision, and using the exit shows completion. The sequence must not complete early.
10. Pause during pursuit and during a warning. Movement, attacks, audio, and cooldowns should stop; Escape resumes them. Retry should still work after a completed run.
11. Test 1280×720, 1920×1080, a narrower window, and an ultrawide window. Verify the world preserves aspect ratio and mouse aim maps to the displayed world correctly.
12. Run the EditMode tests and make a Windows standalone build. QuarantinePrototype is the first enabled build scene; SampleScene remains second. Test the complete build folder outside the Editor, including retry.

## Validation already completed

The October 1 recovery copied the earlier session's uncommitted implementation into a separate checkout. The recovered implementation was reviewed and the checks below were rerun. Progression now rejects sentinel defeat before weapon recovery and duplicate defeat notifications; resource regeneration stops while paused or after completion. Retry reloads the active scene by its full path.

- Shipped core test source compiled with Roslyn in C# 9 mode and ran against NUnit 3.14.0: 5 passed, 0 failed.
- C# syntax parsing: 24 files, no parse errors.
- Metadata uniqueness and coverage; trial scene GUIDs and scene IDs; build scene entry; preserved original input IDs and valid added bindings; correct layers; atlas size; six unclipped mono 44.1 kHz WAV files.
- Diff whitespace checks and visual inspection of the generated pixel atlas.

These checks do not establish Unity API compilation or runtime behavior. The authoring environment has no Unity Editor. Record the PC playtest and standalone build results before accepting the branch.

## Asset pipeline

The atlas contains sixteen 16×16 frames in an 8×2 grid. The runtime creates Sprite objects using those exact rectangles and 16 pixels per world unit. The camera renders 24×13.5 world units into 384×216 pixels; the HUD uses a separate scaled canvas. Rendering is point-filtered, and the camera snaps to the world pixel grid. Scene runtime objects are regenerated for each run; editable parameters live in the balance asset and input action asset.

Art and six small synthesized effects were created specifically for this prototype. The existing Liberation Sans SDF font is reused with its existing license files. The existing URP package's default 2D unlit material is referenced explicitly so it is available to builds. Blender/Audacity production sources can replace the temporary assets once the perspective, palettes, animation directions, and world scale are locked.

## After the trial passes

1. Tune combat, then add a small set of meaningful equipment/relic definitions and player animation states.
2. Add versioned saves and checkpoints for resources, domain, scene, presentation tier, and world permissions.
3. Build the actual quarantine escape and its presentation transition using persistent progression state.
4. Implement the Broken Oracle library's loop anchors, permission rules, relic upgrade, and boss encounter.
5. Expand the validated art/audio pipeline to the remaining domains and visual tiers.
