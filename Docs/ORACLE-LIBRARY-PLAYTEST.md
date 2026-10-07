# Oracle library opening: delivery and playtest

## October 5 user result

Shawn reports that this section behaved as expected and produced no errors. He found its purpose and puzzle confusing, and the absence of a visible Oracle weakened the payoff. This confirms the reported playthrough, not every individual acceptance check below; EditMode and Windows build results remain unreported.

The next task is the story revision in [STORY-SPINE.md](STORY-SPINE.md) and [ORACLE-LIBRARY-DRAFT.md](ORACLE-LIBRARY-DRAFT.md). Those proposals are documentation only. The current playable sequence is described below.

## Delivered section

The User Directory threshold now mounts a separate 16-bit library scene. A blue corridor route returns the avatar to the entrance, passing the same red index book. The first repeat unlocks contact with the Oracle. Completing that conversation identifies the anchor. Completing the anchor interaction disables the redirect and physically removes the barrier. The player can then cross to the archive landing.

This section establishes a restricted communication channel. It does not settle the Oracle's origin or classification, free the Oracle, award a relic, spawn a warden, or start the broader god rescue mission. Those belong to the inner archive milestone. `AUDIT QUEUED` is narrative setup here, not an implemented enemy spawn. The final objective explicitly identifies the end of the current section.

Existing graybox artwork, walking frames, effects and music are reused. The world and hero remain at 16-bit; there is no upgrade to 32-bit and no mixed-fidelity rendering yet.

## Pull and launch

Close Unity and use PowerShell in the active upgraded project:

```powershell
Set-Location -LiteralPath "F:\GameProjects\Fields of Chaldran - Unity66-Test"
git status --short
git branch --show-current
```

If the working tree is clean, switch to the canonical branch and update:

```powershell
git fetch origin --prune
git switch master
git pull --ff-only origin master
```

Preserve local changes first. If the command cannot fast-forward, inspect the divergence before proceeding. No installer, package export/import, folder replacement or repeat Editor migration is needed. Open the project with **6000.6.4f1**.

The normal route is **Open Quarantine Trial → Play → C** to resume your prior approach, or complete a new tutorial. Finish the Directory story if necessary. Use the threshold at (7, 6) once to finish its conversation, then E again to enter the library. A completed older Directory checkpoint can enter directly.

**Fields of Chaldran → Open Oracle Library Preview** opens the new scene directly. A matching library checkpoint restores; otherwise this creates a full-resource preview at the entrance. Normal access through the threshold still requires the completed Directory approach.

## Sequence and state

| Completed steps | Current action | Saved route state |
| --- | --- | --- |
| 0 | Read mount output; E reopens it if cancelled | Loop and barrier active |
| 1 | Walk east through the blue route at (1.5, 0) | First redirect still required |
| 2 | Probe the Oracle channel at (-5, 0) | Loop active; channel available |
| 3 | Interrupt the anchor at (-1, 0) | Barrier stays closed until the final page |
| 4 | Cross east and inspect the landing at (6, 0) | Loop disabled; barrier open |
| 5 | First section complete | Contact and anchor retained; inner archive upcoming |

The manual checkpoint terminal is at (-9, -0.9); the loop returns to (-8.5, 0). Gold beacons follow the active action. Later interactions report the current objective rather than skipping it. Earlier interactions can be reread without duplicate progress. The resolved blue route disappears; the released barrier becomes a faint cyan trace and stops blocking movement.

## Unity acceptance checks

1. Import in **6000.6.4f1**, confirm no red Console errors, and run all EditMode suites in Test Runner. The seven tests in `StoryAssetTests` include library asset import, JSON scene continuity, and component blocker/loop release. They have not run in the authoring environment.
2. Resume an existing version-2 Directory checkpoint. Verify the prior beat count, weapon and resources remain. Complete the approach if needed. Confirm the finished threshold still has a beacon and an E mount prompt.
3. Enter the library. Check the 16-bit hero/world, mount output, resources, audio and objective. Cancel the arrival dialogue, then reopen with E. It must not award progress until the final page is dismissed.
4. Walk east through the blue route. Confirm an actual return past the same red book and a new Oracle contact objective. Repeat the route: there should be no additional progress award or skipped conversation.
5. Try the anchor before finishing Oracle contact. Confirm access is denied with the current objective. Complete contact, then open the anchor interaction and cancel it before the final page. The blue redirect and red physical barrier must remain active.
6. Finish the anchor interaction. Confirm the blue route disappears, the barrier fades to cyan and no longer blocks passage, and the landing receives the beacon. Walk through and finish its dialogue. The first-section completion message must appear.
7. After contact, anchor completion and landing completion, move away and press C. Each time, confirm the library scene, last saved position/resources, objective, contact and physical route state restore. The released route must stay released. Completed arrival/contact must not autoplay.
8. Stop/restart Play, then close/reopen Unity and resume with C. Repeat with domain reload disabled if that is your normal setup. A previous in-memory transfer must not leak into a new play session.
9. Save at the entrance terminal after opening the anchor, then C. Confirm the open route is retained even though the player saved west of it. Reread completed interactions; they must not roll back state or duplicate completion.
10. During dialogue, test held movement/block, attack, Q, C and F5. Gameplay remains paused; E/Enter/click reveals or advances; Escape cancels. Check wrapping, prompts, aiming and camera framing at 720p, 1080p, narrow and ultrawide sizes.
11. Press F5, then C to verify the prior library checkpoint still resumes. Completing that fresh tutorial intentionally replaces the single checkpoint with a new User Directory entrance and resets the chapter route. Back up the checkpoint first if you want to keep the completed library state.
12. Make a Windows development build. Confirm QuarantinePrototype, OverlandPrototype and OracleLibraryPrototype are enabled, with the trial first. Test scene transfers and continued library progress outside the Editor. Check for errors and record whether the earlier exit dialog recurs; its cause remains unresolved.

## Checkpoint compatibility

Version 3 writes `chaldran-prototype-checkpoint-v3.json` under Unity's persistent data directory. It records explicit location, resources, weapon, position, Directory completion and library step. Library anchor/barrier state is derived from completed steps, so cancellation cannot release it.

If v3 is absent, loading tries v2, then v1 if v2 is also absent. Valid v2 saves preserve every Directory beat; v1 starts at Directory arrival. Older files remain untouched. A present corrupt/newer save is rejected instead of silently falling back to older progress. Safe writes retain the prior v3 file as `.bak`; automatic backup recovery is not implemented. Failed saves retain this session's state and show a notice. The schema still represents one post-tutorial checkpoint, not full RPG persistence.

## Source validation

35 standalone C# tests passed, including seven new library order/cancellation/migration/state checks. All 46 C# sources passed syntax parsing. Asset verification passed for three scenes/profiles, both chapters/dialogue references, metadata, atlases, existing input identities, retro cue and 15 audio clips. These checks do not establish Unity compilation, rendered layout, physics behavior, audio playback or Windows build success.

Next: revise the purpose, route interaction and visible Oracle meeting before extending the inner archive. Decide the warden/relic/Oracle outcome before the wider mission handoff.
