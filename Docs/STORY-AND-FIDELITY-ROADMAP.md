# Story and fidelity direction

Recorded from Shawn's October 4, 2026 direction. This document supersedes earlier suggestions to soften the computer terminology or treat the Oracle as an ordinary imprisoned god. It is design groundwork, not a claim that the library or mixed-fidelity rendering is playable.

## Established direction

- The starting prison uses an intentionally primitive 8-bit presentation. Escape opens the 16-bit User Directory.
- The Broken Oracle is different from the other gods. Keep the library and the avatar at 16-bit throughout this encounter. Do not define the Oracle's exact nature from the title alone.
- The broader call to free the other gods follows the Oracle encounter.
- Keep hacker language and technical simulation terminology prominent: quarantine, permissions, processes, sandbox, corruption, anchors, write access, filters, and overrides.
- No immediate move to 32-bit. Later fidelity advances need their own story milestones.
- Continue grayboxing with current development assets. Owned Asset Store packs can be evaluated later, when a specific scene needs them.

## Proposed mixed-fidelity rule

Shawn is considering later god prisons at 8-bit while the visiting hero retains 16-bit presentation and capabilities. Treat this as the next rendering experiment, not a universal rule already implemented.

| Location | Environment | Hero | Story purpose |
| --- | --- | --- | --- |
| Personal prison | 8-bit | 8-bit | Learn the Engine's restraints and escape |
| User Directory | 16-bit | 16-bit | Orient and follow the Oracle signal |
| Oracle library | 16-bit | 16-bit | Establish the Oracle's distinct role and the rescue mission |
| First other god's prison, proposed | 8-bit | 16-bit | Show an intruding process exceeding the sandbox's expectations |
| Later chapters | Undecided | Story-dependent | Earn each additional fidelity advance |

Audio can belong to the host prison while the hero's action cues retain their richer sound. Which effects cross that boundary is a testable art decision. Keep dialogue readable regardless of host fidelity.

An advantage should open new interactions, not make every guard harmless. First experiment: the hero can inspect or alter a permission anchor that local processes cannot address; doing so raises a containment response. The specific power, cost, and response are draft mechanics. Artistic fidelity itself is not a damage multiplier.

## Implementation groundwork

The current prototype has one PresentationProfile, one sprite source, and one world RenderTexture per scene. AvatarPresentation reads that same profile. A 32-pixel hero sprite drawn into the existing primitive world buffer will lose detail; swapping only the atlas will not prove the proposed effect.

Before implementing the first other god's prison, separate:

| Concern | Responsibility |
| --- | --- |
| Location/host presentation | Environment atlas, host pixels, palette, ambience, music, dialogue framing |
| Avatar presentation | Hero atlas, animation frames, hero effects and action sounds |
| Unlocked presentation | Story-earned hero fidelity, retained when entering a primitive host |
| Gameplay capability | Abilities and permissions, independent of sprite resolution |
| Chapter state | Oracle encounter, prison identity, anchors, warden, relic and rescue progress |

Prototype a shared final canvas with independently rendered or quantized world and hero layers. Maintain the same camera position, viewport, collision scale, aiming coordinates, and consistent foreground occlusion. Trees/walls must still occlude the higher-fidelity hero correctly. Do not simply put the hero on top of everything. Compare separate camera buffers with a shared high-resolution buffer plus selective world quantization before choosing the production method.

Use stable chapter/location IDs for future saves. Preserve the numeric meaning of existing PresentationTier values and the current version-2 checkpoint. That enum currently identifies the tutorial/overland profile, not literal hardware bitness. Introduce an explicit migration when dungeon state and separate hero/host fidelity are added. Returning from a prison must retain the hero's unlocked fidelity and capabilities.

## Delivery order

1. **Adopt the tested Unity 6.6 copy.** Shawn completed the current game in 6.6 with no Console errors. Keep the reported exit crash open for diagnosis; collect the exact patch and actual migration changes, verify a Windows build, and adopt the migration in its own commit. Story development can proceed on the active 6.6 copy. See UNITY-6.6-UPGRADE.md.
2. **Oracle chapter writing.** Resolve what the Oracle knows, what prevents communication, and how the encounter leads to the rescue mission. Review the draft in ORACLE-LIBRARY-DRAFT.md.
3. **Playable 16-bit library.** Graybox a compact loop, one anchor, a permission barrier, the Oracle encounter, and the warden/relic sequence. Extend saves before expecting persistent dungeon progress. Keep the first implementation small.
4. **First mixed-fidelity experiment.** Use a disposable 8-bit host room with the 16-bit hero. Verify silhouette/detail, occlusion, mouse aim, dialogue, audio, enter/exit, and continue. Add one meaningful permission advantage.
5. **First god rescue.** Choose the god and their specific prison restriction with Shawn; connect that restriction to their identity. Reuse the presentation and save foundation rather than repeating the Oracle chapter.
6. **Opening expansion and animation.** Turn the personal prison into a purposeful sequence, with visible sword/block actions. Connect what it teaches to the permission mechanics used later. Keep the fast retro dialogue and readable prompts.
7. **Production assets.** Establish scales and animation requirements, then evaluate owned assets or create tier-appropriate replacements. Preserve recognizable identity across fidelity levels.

## Writing rules

Keep the technological language. Use a distinct voice for system messages, the avatar, and the Oracle. System output is terse and procedural. The avatar reacts and attempts exploits. The Oracle is precise, observant, and personally motivated; establish that motivation in the chapter draft rather than assigning omniscience.

Every mandatory technical message must also communicate an actionable consequence. Example: `WRITE DENIED: anchor process owns this route. Locate its binding and interrupt it.` Optional records can carry deeper jargon and lore. Put essential guidance in the current objective and contextual prompt so it does not require a second playthrough.

## Open decisions

- The Oracle's exact identity, limitations, motives, and permitted revelations.
- Whether the library encounter restores a connection, releases confinement, or accomplishes another specific outcome.
- The relic's identity and function; do not invent a permanent ability before its role is approved.
- The first other god and their prison's rule.
- Whether mixed fidelity is universal for later prisons or varies by prison.
- The story event that eventually unlocks 32-bit presentation.
