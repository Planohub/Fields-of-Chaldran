# User Directory opening: Path to the Oracle

## Story basis

The avatar is an awakened god who has escaped personal quarantine inside the Sorruin Engine. The medieval overland is the shared simulated operating system. Presentation improves with story progression; divine advancement is a separate system. The next established destination is the Broken Oracle's compressed library, where spatial loops, anchors, permissions, a relic, and a mechanical warden belong to a later implementation.

This milestone builds the approach to that library. The arrival thoughts, inscriptions, and damaged record are new connective dialogue drafts based on those constraints. They remain editable; they do not introduce a named guide, a different domain, or an Oracle rescue before the library encounter. The playable sequence ends at the sealed threshold.

## Play

Update `codex/quarantine-trial-recovered`. Complete the tutorial, or press C to resume the checkpoint you already made. A valid version-1 checkpoint starts this new story at arrival with its existing weapon, resources, and position. **Fields of Chaldran > Open Overland Preview** also opens the clearing directly; without a checkpoint it is an unlocked development preview.

The opening dialogue starts after the arrival fade. E or Enter advances one page; Escape closes it. Movement, combat, regeneration, physics, and cooldown clocks pause while dialogue is open. Music continues. C, F5, and normal pause/retry inputs are held until the dialogue closes. The key that opens or finishes a conversation cannot also fire another interaction or attack in the same frame.

| Beat | What to do | Result after the final page |
| --- | --- | --- |
| Arrival | Read the avatar's thoughts; E reopens them if closed | Seek the southwest waystone |
| Waystone | E near the stone at (-6, -4) | Follow the eastern path toward the damaged record |
| Signal record | E near the record at (-1, -4) | Seek the northeast library route marker |
| Route marker | E near the marker at (7, 4) | Follow the path north to the threshold |
| Library threshold | E near the stone entrance at (7, 6) | Library located; the Oracle's quarantine remains sealed |

The current destination has a small gold-tinted beacon. Visiting a later object early gives the current objective without skipping a beat. Previous record/marker/threshold dialogue can be reread without awarding duplicate progress. Once its first reading is complete, E at the waystone saves instead of repeating its introduction.

Every newly completed beat saves position, resources, weapon, tier, sequence identity, and completed beat count. C restores the last successful save. Closing a conversation partway does not advance it or save a partial page; reopening begins at page one. F5 starts a fresh tutorial while retaining the previous checkpoint; finishing that tutorial replaces it with a fresh entrance checkpoint and resets the opening story.

## Author dialogue and objectives

`Assets/Chaldran/Data/Story/UserDirectoryOpening.asset` owns the quest title, five ordered beats, each current objective, and the completed objective. Its dialogue references point to five separate `DialogueDefinition` assets in the same folder. Each page has a speaker and text, editable in Unity's Inspector. The dialogue paging/controller are reusable by future NPCs and inspectable objects.

`DirectoryBeat` values and `StoryProgress.SequenceId` are save identities. Keep their meaning and order stable when polishing prose or changing art. A different chapter/order needs its own sequence and an explicit migration rather than reinterpreting existing saves. The runtime and source validator reject missing pages, out-of-order beats, empty objectives, unsupported story identities, and out-of-range saved steps.

## Save compatibility

Current checkpoints use version 2 in `chaldran-prototype-checkpoint-v2.json` inside Unity's persistent data directory. If that file does not exist, loading reads `chaldran-prototype-checkpoint-v1.json`, validates its player state, and upgrades it in memory to the beginning of this quest. The original file is retained. The first successful story or waystone save writes version 2.

A present invalid version-2 save is rejected instead of silently restoring an older version-1 state. Future schema versions are rejected. Writes use a temporary file and keep the previous version-2 checkpoint as `.bak`; automatic backup recovery is not implemented. Failed saves leave the prior file intact and show a notice while retaining this session's progress. Save positions near the perimeter are clamped into the safe interior; resources clamp to the current balance caps on load.

The completed beat count records the mandatory conversations and discovery of the library route. It is not yet an inventory, branching dialogue history, multiple-slot system, or a full dungeon save.

## Validation and Unity acceptance

Twenty-eight standalone NUnit tests cover combat, permissions, resources, ordered story progression, dialogue completion/cancellation, saved beat continuity, version-1 migration, invalid state, and file write failures. All 41 C# files pass syntax parsing. Source checks cover both scenes, profiles, atlases, all five story-to-dialogue references, GUIDs, existing input identities, and 15 audio clips.

Four additional EditMode tests are checked in for actual Unity asset import and JsonUtility serialization/migration. They require Unity and have not run in the authoring environment. The user reported this opening story working in Unity. The subsequent retro/readability and guard-pursuit pass needs a new playtest; see [its checks](RETRO-READABILITY.md).

1. Import in Unity 6000.5.6f1; confirm no red Console errors. Run all EditMode tests in Test Runner.
2. Resume the existing checkpoint with C. Confirm the new arrival dialogue opens and the weapon/resources remain intact.
3. Hold movement/block and press attack/Q/C/F5 while a dialogue is open. Verify gameplay and cooldowns stay paused, no resources are spent, and the scene does not reload. E/Enter advances once; Escape closes and resumes movement/audio normally.
4. Close the arrival or waystone dialogue before its last page. Confirm the objective stays unchanged and reopening starts at page one. Do not award completion until the final page is dismissed.
5. Visit the threshold and marker early. Confirm the objective points back to the expected object. Complete the full route, checking each objective and beacon change.
6. After each completed beat, move away and use C. Verify the objective, resources, position, and beacon restore. Repeat after stopping Play Mode and after closing/reopening a Windows build. Completed arrival dialogue must not autoplay again.
7. Reread the record/marker/threshold. Confirm they do not roll back the objective or overwrite the save. E at the already-read waystone should save normally.
8. Press F5, then C to check the prior story resumes. Complete a new tutorial and verify the opening restarts at arrival.
9. Check dialogue wrapping, objective readability, aim, and paths at 720p, 1080p, narrow, and ultrawide sizes. Build for Windows and repeat the route and continue tests outside the Editor.

Once this approach passes, the next playable extension is the compressed library's loop/anchor/permission mechanics and the Oracle rescue sequence. Asset replacement can proceed through the existing tier profiles; see [art handoff](ART-HANDOFF.md).
