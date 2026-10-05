# Next milestones from the October 3 playtest

## Confirmed working

The user tested the quarantine trial, graphics/sound transition, Path to the Oracle objective, and subsequent retro/readability pass and reported that they behaved as intended. Current art is development art; the game is establishing its structure.

## October 4 story direction

The Oracle is distinct from the other gods. Keep the Oracle library at 16-bit; the broader call to free the other gods follows that encounter. Later 8-bit god prisons with a visiting 16-bit hero are a proposed next experiment. Do not immediately advance to 32-bit. Keep hacker language and simulation jargon prominent. Continue grayboxing.

See [the delivery roadmap](STORY-AND-FIDELITY-ROADMAP.md), [Oracle chapter draft](ORACLE-LIBRARY-DRAFT.md), and [isolated Unity 6.6 upgrade steps](UNITY-6.6-UPGRADE.md). The Unity migration and first library graybox are now delivered; the next order is library playtest, inner archive writing/encounter, then a mixed-fidelity proof. Opening expansion and avatar/sword animation remain on the roadmap.

The 6.6 gameplay trial is now user-tested through its ending with no Console errors. An error dialog on Editor exit remains unresolved. Use the upgraded copy for development and preserve the original as a fallback. The actual migration is committed on the shared branch: Unity **6000.6.4f1**, revision **12bfff696524**. The new library playtest and Windows build verification remain pending.

## Avatar animation

Build visible idle, directional walking, sword windup/swing/recovery, block, hit, and defeat poses. Even the primitive tutorial should show an actual sword swing. Tie damage timing to the attack's visible contact rather than making the whole action an instant effect. Preserve aiming, cooldowns, wall checks, and directional block. Provide simple low-tier frames and richer later-tier versions with recognizable silhouettes. Use the user's production assets when supplied; the current walking/arc effects are only temporary feedback.

## A longer, purposeful prison/tutorial

Give the player several connected things to discover and do before escape. Establish why the guard controls escape and why fighting it matters. Build a short cause-and-effect chain through environmental discovery, a weapon anomaly, a containment/permission interaction, combat, token use, and release. Include opportunities to learn blocking, burst, restoration, and interaction through play. Add duration by decisions, exploration, and mechanics rather than slow text or repeated guards.

The exact prison layout, puzzle, and dialogue are upcoming design work. Those connective details are not locked game canon. Keep the avatar's personal quarantine and release within the Sorruin Engine, the medieval OS overland, and story-driven fidelity changes intact. Preserve the compressed library/Oracle sequence and keep its dungeon mechanics coherent with whatever the tutorial teaches.

## Nostalgia and readable feedback

Keep the opening's NES-style top text box, quick letter reveal, sound, and instant-reveal input. Use coherent pixel-font, border, palette, animation, and chiptune choices when replacing art. Important information should reach the player's focus: persistent objectives, contextual prompts, brief readable notices, and clear enemy tells. Avoid requiring a second playthrough to notice a necessary instruction.

## Compressed library

The User Directory now mounts a playable 16-bit library opening: corridor redirect, repeated book, restricted Oracle contact, anchor interruption, physical barrier release and archive landing. Version-3 checkpoints retain the library scene and route state. Follow [the acceptance checks](ORACLE-LIBRARY-PLAYTEST.md).

Next build the inner archive, define the relic and mechanical warden encounter, and write the Oracle outcome and wider mission handoff. The Oracle's precise outcome remains a story decision; do not assume the same rescue as the other gods. Keep visual-tier progress separate from divine advancement. The library remains 16-bit.
