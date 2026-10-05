# Broken Oracle library: chapter groundwork

The mount, corridor loop, restricted first contact, anchor and archive landing are now implemented in the first playable section. See [delivery/playtest notes](ORACLE-LIBRARY-PLAYTEST.md). The inner archive, warden, relic and mission handoff below remain proposals; they do not establish the Oracle's origin, divine classification, or the next god's identity. The library stays at 16-bit, including its sound and the hero. The larger mission to free other gods begins after this encounter.

## Chapter purpose

Move from following a damaged record to choosing an active mission. Give the Oracle a distinctive presence through how they observe the library's malfunction and respond to the hero. Reveal enough to make the next action understandable while leaving the Engine's full history for later chapters.

## Compact playable sequence

| Beat | Player action | Story/feedback | Proposed checkpoint boundary |
| --- | --- | --- | --- |
| Mount the library (implemented) | Use the completed threshold after the existing approach | Access is granted, but the interior route is virtualized | Entrance |
| Encounter the loop (implemented) | Walk through a corridor and return to its start | A clear repeated landmark and changed system output establish the loop | No progress award for repetition |
| Receive a signal (implemented) | Inspect a responding terminal or book | Oracle makes contact; the channel is restricted | Completed conversation |
| Interrupt the anchor (implemented) | Follow its binding and disable it | The corridor becomes a stable route; an audit is queued, with no warden spawned yet | Anchor state |
| Reach the archive landing (implemented) | Cross the now-open route | Contact holds; inner archive remains the next section | Landing state |
| Enter the inner archive (planned) | Continue beyond the landing | Establish the Oracle's specific limitation | Encounter state |
| Break the active restraint | Resolve a concise warden/permission encounter | Enable access to the relic and the chapter's agreed Oracle outcome | Warden and relic state |
| Accept the wider objective | Finish the Oracle conversation | Other gods' prisons become the next actionable mission | Mission unlocked; return route available |

The anchor mechanic needs a visible change, not just a dialogue flag. Completion/cancellation follows the existing dialogue rules. Earlier actions may be repeated without duplicating rewards. The exact ordering of the relic and warden should follow the relic's eventual function.

## Sample first contact

System messages appear as brief technical output. Dialogue uses the existing readable text system. These lines are a voice sample rather than production dialogue assets.

**SYSTEM:** `MOUNT /oracle/archive: granted. Route integrity: unresolved.`

**AVATAR:** "Same shelf. Same damaged book. The exit routed me back to the entry point."

**UNKNOWN SIGNAL:** "Your position changed. Your destination did not."

**AVATAR:** "Identify yourself."

**UNKNOWN SIGNAL:** "The index calls me the Broken Oracle. The channel gives me very little room to dispute it."

**AVATAR:** "Can you open a route?"

**ORACLE:** "I can trace its binding. I cannot write to it. You reached this channel from outside the library's process. Try what I cannot."

**SYSTEM:** `ANCHOR BINDING FOUND. Interrupt the marked anchor to resolve the loop.`

**Objective:** `Trace the loop anchor and interrupt its binding.`

## Sample containment response

**SYSTEM:** `UNAUTHORIZED WRITE. Containment warden dispatched.`

**AVATAR:** "The route is open. So is the audit log."

**ORACLE:** "Then keep moving. It has your last write location."

This response gives the warden a reason to engage. Its actual pursuit and encounter should communicate that behavior visibly.

## Mission handoff to write after the Oracle is defined

The final conversation must give the player:

1. A reason the other gods need intervention.
2. A reason the hero can enter their prisons and attempt that intervention.
3. One specific next destination, with a readable objective.
4. A reason the Oracle needs or wants this outcome.

Do not claim that the Oracle is a regular god awaiting the same rescue. Do not trigger a 32-bit upgrade here. The proposed 16-bit hero / 8-bit host contrast belongs to the next prison experiment, not to this library.

Potential system wording for that later handoff: `REMOTE SANDBOX: legacy fidelity. Caller capabilities retained.` It describes entry behavior, not an automatic damage bonus.

## Graybox requirements

Use the existing 16-bit profile and placeholder shapes. Include an unmistakable repeated landmark, one clearly marked anchor, a visually altered passage after interruption, a safe dialogue area, and enough space for readable combat. Prevent aimless loops by showing a concise objective after the first repeat. Optional records can deepen the technological lore without hiding mandatory instructions.

Playtest the delivered section's state transitions and resume behavior before extending the inner archive. Production assets remain unnecessary for this graybox milestone.
