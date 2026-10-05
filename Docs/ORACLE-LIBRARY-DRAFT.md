# Broken Oracle library: revised encounter draft

## Why this revision comes first

Shawn's October 5 playtest reached the current section's end with expected behavior and no errors. The experience nevertheless felt like bouncing off a wall and reading text to open a door. The absent Oracle left the action without a clear point.

The current code implements a redirect, terminal dialogue, anchor release and archive landing. **The sequence below is a proposed replacement for that presentation and story ordering; it has not been implemented.** Its purpose is to deliver a visible, understandable meeting before expanding the dungeon. It supersedes the earlier plan to stop at the landing before the Oracle appears.

See [the story spine](STORY-SPINE.md) for the connection from the personal quarantine to the later rescue mission. The library and hero remain 16-bit. The Oracle's exact identity remains unresolved.

## Scene promise

The hero follows the Oracle's record to learn about their own confinement. On entry, the player sees a distinctive figure at a desk beyond a sealed archive gate. That figure notices them. The immediate objective is **Reach the Oracle beyond the archive gate.** The route is visible before the obstruction becomes a puzzle.

The library permits incoming visitors but rewrites their route back to the entrance. That is the obstacle keeping the visitor and Oracle apart. Show its behavior before explaining its technical name.

## Proposed sequence

| Beat | What the player sees and does | What it means |
| --- | --- | --- |
| Find the Oracle | See the figure beyond the gate and walk toward the marked archive passage | There is someone here worth reaching |
| Experience one redirect | The doorway returns the hero beside the distinctive red book; the camera and a brief trail make the return obvious | The passage loops rather than progressing |
| Receive help | The visible Oracle reacts and addresses the hero automatically after the first redirect | The voice belongs to someone in the scene; repeated attempts are unnecessary |
| Trace the wrong destination | A highlighted link runs from the passage's anchor back to the entrance; inspecting it exposes the assigned target | The route has an understandable cause |
| Change the route | Use a separate interaction to select the archive endpoint and confirm the change; show the link switch and the gate open | The player deliberately changes a rule rather than completing a reading task |
| Meet the Oracle | Cross to the figure and begin a direct conversation | The section pays off its original promise |
| Establish the next problem | The Oracle shows a relevant containment record; a specific blocked operation motivates the later inner archive encounter | The next action grows out of an answer |

This is a short introduction to a system rule. It does not need repeated blind attempts, a timer, an unexplained symbol code or several similar terminals. After the first repeat, movement back through the same route should add no mandatory story step.

Reading a page can reveal a clue; it must not itself commit the route change. On cancellation, the route stays unchanged. Restoring a checkpoint should preserve the discovered clue, confirmed route and meeting state without demanding another loop.

## Permission logic to make explicit

Proposed local rule: visitors can select the endpoint assigned to their own route. The Engine currently points that endpoint back to ENTRY. The Oracle can inspect the assignment but cannot edit a visitor's route from their restricted archive account. The hero can change their own assignment once the Oracle identifies a valid ARCHIVE endpoint.

This is a limited exploit. It does not reuse the consumed sentinel token, grant general root access or depend on being 16-bit. Show the rule in concise inspection output and demonstrate its physical consequence.

## Draft first exchange

Deliver the voice from the visible figure or a clear projection of that figure. An unrelated terminal should not stand in for the promised meeting.

**AVATAR:** "Same red book. The passage sent me back."

**ORACLE:** "It sends every visitor back. Look at the line under the floor. It ends where you started."

**AVATAR:** "You can see the route. Can you change it?"

**ORACLE:** "Read access only. Your visitor slot is writable. I can give you the archive address."

**ROUTE INSPECTION:** `CURRENT TARGET: ENTRY. ARCHIVE ENDPOINT VERIFIED.`

**Objective:** `Change your route's destination to ARCHIVE.`

The route controls now accept a deliberate action. Advancing the final dialogue page only makes the control available.

**After the player confirms:** `ROUTE UPDATED: ARCHIVE. LOOP DETACHED.`

The link changes direction, the return effect disappears and the gate opens. The Oracle turns toward the newly open path.

**ORACLE:** "There. Come through while the route holds."

Use that last line only if a later containment response visibly threatens the route; otherwise use "There. The archive can receive you now." Do not imply a hidden countdown when none exists.

## Draft meeting and payoff

The Oracle should be directly approachable at the destination. Stage the conversation around a visible record that matches a mark or identifier introduced in the personal prison. The hero can point to evidence rather than receive an unexplained lore speech.

**AVATAR:** "That mark was on my cell."

**ORACLE:** "Your containment record. The entry now says the cell is empty. It still lists you inside the Engine."

**AVATAR:** "Then where is the way out?"

**ORACLE:** "The external route is locked from here. I can show you which process owns the lock."

**AVATAR:** "Show me."

The Oracle then opens a small index: the hero's cell is marked EMPTY; several other quarantines remain OCCUPIED. This is evidence the player can see, and the first indication of the wider cast rather than an immediate assignment to rescue everyone.

**AVATAR:** "Those are other cells."

**ORACLE:** "Other gods. Their status still updates. I can read it. I cannot get a message through."

**AVATAR:** "But you reached me."

**ORACLE:** "You reached the archive. That gave us a channel. Tell me how you left your cell."

The meeting now provides two concrete discoveries: escape did not grant external access, and other gods remain confined in separate locations. It also gives the Oracle a personal reason to listen to the hero. Their working relationship begins with exchanging evidence and experience, before the later rescue commitment.

The next inner archive section needs a concrete design for the controlling process, warden and relic before any of them is promised as an actionable objective. A warden should respond to an actual protected operation. A relic should enable a stated action. Keep either only if it serves this chapter; a place on the earlier task list is not sufficient reason to add it. Avoid announcing a dispatch and then spawning nothing.

## Later handoff to the gods' prisons

After the fuller Oracle encounter, show evidence of other occupied quarantines and establish why reaching one helps both its captive and the hero's larger goal. If the partitioned-authority idea in STORY-SPINE.md is adopted, demonstrate the missing authority before asking the hero to restore it.

The chapter handoff must name one reachable prison, identify what the hero can attempt there and give the Oracle a reason to support that attempt. The first god's identity and prison rule remain to be chosen. Do not classify the Oracle as the first ordinary god rescue. The proposed 16-bit hero in later 8-bit prisons belongs to that subsequent experiment.

## Acceptance for the story revision

A new player should be able to answer these without rereading a log:

1. Why did I come to the library?
2. Who am I trying to reach, and where are they?
3. Why did the doorway return me to the entrance?
4. What did I change, and why was I able to change it?
5. What did meeting the Oracle tell me, and what do I want to do next?

Graybox a distinct figure, a visible route connection, one repeated landmark and a direct meeting space. Label any development stopping point outside the fictional system dialogue. The first revised delivery should end after a satisfying exchange with the Oracle, with its current content boundary clearly identified.
