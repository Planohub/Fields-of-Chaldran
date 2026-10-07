# Fields of Chaldran: current-state assessment

**Prepared October 7, 2026.** This assessment separates confirmed direction, what the current branch implements, reported playtest results, and story proposals that still need agreement.

## Coverage and evidence

Reviewed the current development-branch README, story and fidelity roadmap, opening-story notes, Oracle library draft and playtest guide, Unity 6.6 migration notes, and the relevant runtime/save/story assets. The exact-title Library search returned no Fields of Chaldran files. This worker request does not expose a way to search or read every separate historical Project chat; the conversation findings below use the project discussion and playtest reports surfaced in this worker context.

Key source files include `JourneyCheckpoint.cs`, `JourneyStore.cs`, `LibraryProgress.cs`, `LibraryStoryDefinition.cs`, `PrototypeRun.cs`, `PrototypeBootstrap.cs`, and the User Directory and Oracle dialogue assets.

## Current read

The technical foundation is working well enough to support design iteration. The user reports completing the first library section in Unity 6000.6.4f1 with expected behavior and no errors. The story did not work as well: the route felt convoluted and the ending offered no visible Oracle. The present library is therefore **a functionally tested graybox slice, not a satisfying Oracle encounter**.

The most useful next step is a focused story pass before adding a warden, relic, new art, or a longer dungeon. The existing branch already contains a draft of that pass, but it remains a proposal. No revised gameplay has been implemented yet.

## Decisions carried forward

| Topic | Current status |
| --- | --- |
| Premise | The hero is an awakened god held by the Sorruin Engine, framed as a simulated computer environment. The opening personal quarantine leads into the shared User Directory. |
| Presentation | The personal prison is presented as 8-bit; the User Directory advances to 16-bit. Keep the Oracle library at 16-bit, including its hero and sound. |
| Oracle | The Oracle is distinct from the other gods. Do not make them a routine rescue target or settle their exact nature by assumption. The wider call to free other gods follows the Oracle encounter. |
| Tone | Keep the hacker, systems, and simulation language. Mandatory jargon should still tell the player what to do and what will happen. |
| Art and workflow | Continue grayboxing with existing development art/audio. The user has Unity Asset Store assets, but there is no need to import or buy assets for this story work. Use GitHub and PowerShell; no package installer/import workflow. |
| Editor | The shared project is migrated to Unity 6000.6.4f1. The user tested the earlier game through its ending without Console errors. One Editor shutdown error dialog was reported and remains unexplained. |

## What the branch implements

| Area | Current state and evidence |
| --- | --- |
| Quarantine trial | Runtime-built combat/objective sequence: recover a weapon, defeat the sentinel, obtain its token, open the barrier, and exit. Combat, interaction, pause/retry, HUD and basic story persistence exist. The user reports the earlier game loop and tier transition worked. |
| User Directory | Ordered arrival, waystone, damaged signal record, route marker and library threshold. Each completed beat saves. Completing the threshold enables mounting `/oracle/archive`. |
| Oracle library | Separate scene and 16-bit profile. Five ordered states: arrival, loop observed, Oracle contact, anchor interrupted, route reached. A repeated corridor, red book, signal dialogue, anchor interaction, physical barrier and landing are implemented. Version-3 checkpoints preserve location and library progress; valid version-1/2 Directory saves migrate. |
| Visibility of the Oracle | The current contact is a dialogue asset at the restricted channel. The library contains Oracle dialogue, but the reported playthrough did not show a character the user recognized as the Oracle. The revised story draft calls for a visible figure and an actual meeting. |
| Validation | The repository README records 35 standalone C# tests passed, 46 C# sources parsed, and source/asset checks passed. Seven Unity EditMode tests have not run in the authoring environment. A Windows build is also unverified. User playtesting is the evidence for runtime behavior; source checks do not substitute for Unity tests. |

## Conflicts and decisions to resolve

1. **“Section complete” versus the missing meeting.** The current library progression and landing message say the first section is complete. The later draft says the encounter is incomplete until the Oracle appears. Resolve the labels: the loop/anchor graybox passed its functional test; the story section should not be called narratively complete before the promised meeting.

2. **Oracle contact versus an Oracle character.** The current milestone calls a terminal exchange “Oracle contact.” The user expected to meet the Oracle and found no Oracle there. Reserve “signal/contact” for the remote exchange and use “meeting” for the visible character or projection.

3. **The obstacle has no clear dramatic purpose.** Walking into the loop, reading terminal text, and opening the door did not explain why reaching the Oracle mattered or what the hero accomplished. The current story proposal offers a simpler interaction: one obvious redirect, an explanation of its target, and a deliberate route change with a visible result. That proposal is not yet implemented.

4. **The long-term fidelity endpoint is unsettled.** Recent direction says not to jump from 16-bit to 32-bit immediately and considers 8-bit prisons with a 16-bit hero. Older project context also mentions a 64-bit/near-photoreal finale; the current roadmap describes a future 32-bit tier. Keep the current Oracle chapter at 16-bit and settle the eventual tier when it becomes a design decision.

5. **“Quarantine” remains a working term.** It is central to current code and docs. Earlier feedback singled out the name as something to revisit, while later discussion continued to use it. Treat the label as provisional until the opening story terminology is polished.

6. **Some docs retain the old Editor baseline.** `README.md` and `UNITY-6.6-UPGRADE.md` name 6000.6.4f1 as current, while older verification instructions in `QUARANTINE-TRIAL.md` still say 6000.5.6f1. Label those checks as historical or update their active instructions.

7. **The proposed lore is not canon yet.** The story-spine document suggests the Engine partitions divine authority so the gods cannot act together. That is a promising explanation for the rescue arc, but it is a proposal; the Engine’s origin/motive, Oracle identity/motivation, relic, warden, and first rescued god remain open.

## Recommended work order

1. **Lock the story spine before expanding mechanics.** Keep the near-term player goals in plain terms: escape the cell, learn whether escape really made them free, then decide what to do about the other captives. Decide how much the Oracle knows, what they personally want, and what specific answer the first meeting gives.
2. **Strengthen the lead from the User Directory.** Tie the damaged record to a mark or identifier established in the prison, so the Oracle is a credible source about the hero’s own containment.
3. **Revise the library around a visible destination.** Show the Oracle beyond the gate on arrival. Demonstrate the loop once, explain it in the scene, and make route editing a separate deliberate action. Let the player cross the newly opened route and meet the Oracle.
4. **End on a payoff and a specific next question.** Let the Oracle provide evidence about the hero’s status and reveal that other gods remain confined. Save the larger rescue commitment for after the fuller Oracle encounter, with a concrete destination and reason to act.
5. **Then decide whether the inner archive needs a warden or relic.** Keep each only if it creates a meaningful obstacle or enables an action that the story needs.
6. **Preserve saves if the beat order changes.** The library uses stable beat IDs and version-3 checkpoints. Changing what those beat values mean requires an explicit save migration; do not silently reinterpret existing progress.
7. **After the story revision, test it in Unity 6.6.** Run the EditMode tests, verify dialogue cancellation and checkpoint continuation, then make and test a Windows build. Keep the reported Editor shutdown issue separate until there is a log that explains it.
8. **Clean the project notes.** Resolve the 32/64-bit endpoint question and mark the old 6.5 instructions as historical. Keep the PowerShell/GitHub pull steps current.

## Immediate recommendation

Implement the clearer Oracle meeting before extending the inner archive. Keep the current slice as the tested mechanics prototype, keep the proposed partitioned-authority explanation marked as a choice for discussion, and use the next playtest to answer one question: **does the player understand why they came, who they are trying to reach, what they changed, and what they learned?**
