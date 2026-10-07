# User Directory opening: Path to the Oracle

## Story basis

The avatar is an awakened god who has escaped personal quarantine inside the Sorruin Engine. The medieval overland is the shared simulated operating system. Presentation improves with story progression; divine advancement is a separate system. The next established destination is the Broken Oracle's compressed library, where the first loop/anchor/contact section is now playable. The relic, mechanical warden and full Oracle outcome remain upcoming.

This milestone builds the approach to that library. The arrival thoughts, inscriptions, and damaged record are new connective dialogue drafts based on those constraints. They remain editable; they do not introduce a named guide, a different domain, or an Oracle rescue before the library encounter. The approach ends at the threshold; using it again now mounts the playable library opening.

## Play

Update `master`. Complete the tutorial, or press C to resume the checkpoint you already made. A valid version-1 checkpoint starts this new story at arrival with its existing weapon, resources, and position. **Fields of Chaldran > Open Overland Preview** also opens the clearing directly; without a checkpoint it is an unlocked development preview.

The opening dialogue starts after the arrival fade. E or Enter advances one page; Escape closes it. Movement, combat, regeneration, physics, and cooldown clocks pause while dialogue is open. Music continues. C, F5, and normal pause/retry inputs are held until the dialogue closes. The key that opens or finishes a conversation cannot also fire another interaction or attack in the same frame.

| Beat | What to do | Result after the final page |
| --- | --- | --- |
| Arrival | Read the avatar's thoughts; E reopens them if closed | Seek the southwest waystone |
| Waystone | E near the stone at (-6, -4) | Follow the eastern path toward the damaged record |
| Signal record | E near the record at (-1, -4) | Seek the northeast library route marker |
| Route marker | E near the marker at (7, 4) | Follow the path north to the threshold |
| Library threshold | E near the stone entrance at (7, 6) | Library located; E again mounts the library scene, while its Oracle channel remains restricted |

The current destination has a small gold-tinted beacon. Visiting a later object early gives the current objective without skipping a beat. Previous record/marker dialogue can be reread without awarding duplicate progress. After completion, the threshold becomes the library entry instead of repeating its conversation. Once its first reading is complete, E at the waystone saves instead of repeating its introduction.

Every newly completed beat saves position, resources, weapon, tier, sequence identity, and completed beat count. C restores the last successful save. Closing a conversation partway does not advance it or save a partial page; reopening begins at page one. F5 starts a fresh tutorial while retaining the previous checkpoint; finishing that tutorial replaces it with a fresh entrance checkpoint and resets the opening story.

## Author dialogue and objectives

`Assets/Chaldran/Data/Story/UserDirectoryOpening.asset` owns the quest title, five ordered beats, each current objective, and the completed objective. Its dialogue references point to five separate `DialogueDefinition` assets in the same folder. Each page has a speaker and text, editable in Unity's Inspector. The dialogue paging/controller are reusable by future NPCs and inspectable objects.

`DirectoryBeat` values and `StoryProgress.SequenceId` are save identities. Keep their meaning and order stable when polishing prose or changing art. A different chapter/order needs its own sequence and an explicit migration rather than reinterpreting existing saves. The runtime and source validator reject missing pages, out-of-order beats, empty objectives, unsupported story identities, and out-of-range saved steps.

## Save compatibility

Current checkpoints use version 3 in `chaldran-prototype-checkpoint-v3.json` inside Unity's persistent data directory. If absent, loading tries v2, then v1 if v2 is also absent. Valid v2 saves retain the existing Directory beat count; v1 starts at this quest's beginning. Resources and position carry forward, and old files remain untouched. The next successful save writes v3. Library checkpoints have their own location and sequence identity; C chooses the saved scene. See [library checkpoint details](ORACLE-LIBRARY-PLAYTEST.md).

A present invalid current save is rejected instead of silently restoring older progress. Future schema versions are rejected. Writes use a temporary file and keep the previous version-3 checkpoint as `.bak`; automatic backup recovery is not implemented. Failed saves leave the prior file intact and show a notice while retaining this session's progress. Save positions near the perimeter are clamped into the safe interior; resources clamp to the current balance caps on load.

The completed beat count records the mandatory conversations and discovery of the library route. It is not yet an inventory, branching dialogue history, multiple-slot system, or a full dungeon save.

## Validation and Unity acceptance

Thirty-five standalone NUnit tests cover combat, permissions, resources, ordered story progression, dialogue completion/cancellation, saved beat continuity, version-1 migration, invalid state, and file write failures. All 46 C# files pass syntax parsing. Source checks cover three scenes/profiles, atlases, both chapters and their story-to-dialogue references, GUIDs, existing input identities, and 15 audio clips.

Seven additional EditMode tests are checked in for actual Unity asset import and JsonUtility serialization/migration. They require Unity and have not run in the authoring environment. The user reported this opening story working in Unity. The subsequent retro/readability pass and Unity 6.6 migration have also been user-playtested. The new [library section](ORACLE-LIBRARY-PLAYTEST.md) needs its Unity playtest.

1. Import in Unity 6000.6.4f1; confirm no red Console errors. Run all EditMode tests in Test Runner.
2. Resume the existing checkpoint with C. Confirm the saved objective, weapon and resources remain intact; arrival opens only for a checkpoint at step zero.
3. Hold movement/block and press attack/Q/C/F5 while a dialogue is open. Verify gameplay and cooldowns stay paused, no resources are spent, and the scene does not reload. E/Enter advances once; Escape closes and resumes movement/audio normally.
4. Close the arrival or waystone dialogue before its last page. Confirm the objective stays unchanged and reopening starts at page one. Do not award completion until the final page is dismissed.
5. Visit the threshold and marker early. Confirm the objective points back to the expected object. Complete the full route, checking each objective and beacon change.
6. After each completed beat, move away and use C. Verify the objective, resources, position, and beacon restore. Repeat after stopping Play Mode and after closing/reopening a Windows build. Completed arrival dialogue must not autoplay again.
7. Reread the record/marker. Confirm they do not roll back the objective or overwrite the save. E at the already-read waystone should save normally. The completed threshold should mount the library.
8. Press F5, then C to check the prior story resumes. Complete a new tutorial and verify the opening restarts at arrival.
9. Check dialogue wrapping, objective readability, aim, and paths at 720p, 1080p, narrow, and ultrawide sizes. Build for Windows and repeat the route and continue tests outside the Editor.

The approach and subsequent readability pass have now been user-playtested. The 16-bit library loop/anchor/contact opening is now delivered. Playtest it, then extend the inner archive and distinct Oracle encounter; its exact outcome remains a story decision. See [the updated direction](STORY-AND-FIDELITY-ROADMAP.md) and [chapter draft](ORACLE-LIBRARY-DRAFT.md). Asset replacement can proceed later through the existing tier profiles; see [art handoff](ART-HANDOFF.md).
