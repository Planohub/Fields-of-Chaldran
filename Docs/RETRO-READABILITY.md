# Retro dialogue and readability pass

The user confirmed that the trial, tier transition, and User Directory story worked in Unity. Their next direction is a more animated avatar, a longer purposeful prison/tutorial, and NES-style reading in the primitive tier. They also reported missing scattered HUD information and long gaps between guard attacks.

## Implemented now

- Quarantine has a short two-page introduction explaining the sentinel's Write-Access role and the weapon anomaly. It introduces the text presentation without extending the trial with extra mandatory reading.
- The quarantine dialogue box sits across the top, with a light rectangular frame, dark interior, larger bold text, and speaker/page labels. It reveals 60 characters per second with a quiet square-wave typing cue. Reveal speed, retro layout, typing clip/volume, and introductory dialogue are editable in the presentation profile.
- Left-click, E, or Enter first reveals an unfinished page immediately. A separate press advances a finished page. Skip/cancel stops the typing source; sound is limited to 25 ticks per second and letters/digits, with no backlog of sounds after a frame hitch. The overland profile keeps instant, silent text and its lower box.
- The complete text is laid out before revealing visible characters, so words keep their positions. Text uses the existing font with retro spacing/weight; a production pixel font and final artwork remain part of the art pass.
- The persistent objective and important notices are larger and centered. An interaction bubble follows the current nearby object within the displayed world, with a dark backing and horizontal edge clamping. Narrower layouts stack the objective beneath the status row, wrap the control legend, and move the ability panel above it.
- The guard's existing 0.8-second warning and 0.8-second recovery stay intact. Previously, blocked line of sight stopped pursuit completely. Now it uses a bounded cardinal route around room obstacles, with collider clearance and periodic replanning; damage also wakes a dormant guard. This is room-scale prototype navigation rather than a general dungeon navigation system.

The current guard encounters and quest order stay as the small proof of concept. This pass does not add the planned prison puzzles or replace the avatar with production animation.

## Unity playtest

Update `codex/quarantine-trial-recovered`, open the trial, and press F5 if necessary to start fresh. The introduction should appear across the top. Verify that it types quickly, produces a comfortable sound, and keeps gameplay paused. Try click/Enter/E while typing: the first press must reveal only the current page. Verify a later press advances, Escape closes, and no sword attack occurs from a dialogue click. Check the last page cannot award or dismiss twice.

Read the objective in the center, approach the anomaly, and verify its interaction bubble follows the object and stays inside the screen. Check objective changes/notices without scanning the corners. Repeat in overland, including saved story progression, rereading, cancel, C, and F5.

Stand near the sentinel to check the warning/recovery cadence. Move behind both pillars: the guard should route around them instead of stopping on loss of sight. Confirm it does not walk through walls or the sealed barrier, pause freezes it, and warning/damage still obey range and line of sight. Also check whether kiting still feels overly passive; tune speed/telegraph/recovery after observing this version.

Test 720p, 1080p, narrow/4:3, and ultrawide windows. Check dialogue wrapping, prompt edges, HUD overlap, pointer aim, and UI scale. Run all EditMode tests and repeat outside the Editor in a Windows build.

Twenty-eight standalone core tests cover the existing combat/checkpoint/story behavior plus reveal timing, immediate reveal versus page advance, cancellation after skip, and bounded routes around obstacles/blocked corners/sealed barriers. All 41 C# sources pass syntax parsing. Asset checks cover the introduction, profile references, typing cue, previous story assets, and 15 WAVs. Four additional Unity asset/serialization tests are supplied; those, actual rendering/audio, physics pursuit, and build tests have not run in the authoring environment.

API references: [TMP visible characters](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/api/TMPro.TMP_Text.maxVisibleCharacters.html), [ForceMeshUpdate](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/api/TMPro.TMP_Text.ForceMeshUpdate.html), and [Physics2D.CircleCast](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/physics2d/circlecast).
