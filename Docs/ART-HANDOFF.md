# First art handoff

The current trial and clearing use development pixel art. Replace a small coherent set first: avatar, ground/path, vegetation, waystone, and library approach. The quarantine and overland versions should share recognizable shapes while expressing different story-driven fidelity. Keep HUD/dialogue text readable at either tier.

Both presentation profiles are in `Assets/Chaldran/Data`. They select the atlas, cell size/layout, render-buffer size, walking frames, colors, looping music, and action cues. Current quarantine uses sixteen 16px cells in a 128x32 atlas; overland uses sixteen 32px cells in a 256x64 atlas. Point filtering and matching pixels-per-unit maintain the same world size across these tiers. Keep the current layout when replacing the first atlas so existing gameplay references remain valid.

| Overland frame | Current role |
| --- | --- |
| 0 | Grass floor |
| 1 | Trees and perimeter vegetation |
| 2, 11, 15 | Avatar idle and walking poses |
| 4 | Library route marker |
| 5 | Restoration spring |
| 6 | Damaged record and small objective beacon |
| 8 | Waystone and library threshold |
| 9, 10, 13 | Melee arc, burst, hit feedback |
| 12 | Library entrance stones |
| 14 | Path |

Keep artwork centered within each cell and use transparent backgrounds for objects. Collisions and interaction radii are separate from artwork: replacement art should fit the existing one-unit avatar/props, larger trees, and entrance stones before changing those dimensions. Walking frame indices and cadence are prototype settings; expanded directional/action animation will need additional frames and animation logic.

Use a separate atlas/profile for each visual tier. Music can retain a recognizable motif with increasing instrumentation; action cues should remain recognizable as their timbre improves. Keep Blender and Audacity source files with production assets when available. Checked-in atlases/WAVs let collaborators play without those authoring tools.

The new story copy is independent of these visuals. Polish dialogue in `Data/Story`, and replace presentation through the profiles. No production assets supplied by the user have been substituted in this milestone.
