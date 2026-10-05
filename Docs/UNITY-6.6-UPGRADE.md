# Test Unity 6.6 on a project copy

Decision recorded October 4, 2026: recommend a trial upgrade while the project is still grayboxing. The checked-in project currently specifies **6000.5.6f1**. This document does not perform an Editor migration or establish compatibility.

## User's 6.6 playtest result

Shawn reports completing the current game through its ending in the upgraded project copy, with expected behavior and no Console errors. He also reports an Editor error/crash dialog on exit. Record the gameplay playtest as successful and the shutdown issue as unresolved; the dialog alone does not identify the cause. The exact 6000.6 patch, migrated package/settings changes, EditMode results, and Windows build results have not yet been supplied.

Use the upgraded copy as the active development folder. Renaming it is optional. Keep the original 6.5 project and Editor as a fallback until the exit behavior and a Windows build are checked. Avoid developing independently in both copies.

Before syncing future gameplay changes, run these read-only commands from the upgraded project's PowerShell window:

```powershell
git status --short
git branch --show-current
Get-Content .\ProjectSettings\ProjectVersion.txt
```

Use the results to preserve and review the actual migration changes. Do not invent the Editor revision or overwrite its generated project version from this authoring workspace. Publish the migration separately and integrate it into the shared development branch before expecting the repository's Editor version to match the upgraded working copy.

For a repeated exit crash, stop Play Mode, save, exit normally, and retain the Editor log before another launch. In 6.6 the default is the project's Logs/Editor.log; Console → More (⋮) → Open Editor Log finds the active file. A global-log configuration instead uses %LOCALAPPDATA%\Unity\Editor\Editor.log on Windows. Crash files are normally under %TMP%\Unity\Editor\Crashes. Unity 6000.6.3f1 release notes list fixes for particular Editor-quit crashes, but they do not establish that Shawn encountered one of those cases. Check the exact patch and log before diagnosing or selecting a fix.

Unity 6.6 is a supported update release. The game's 8-bit/16-bit presentation is independent of the Unity Editor version. Our next story chapter does not require a 6.6-only feature, so a failed trial can return to 6.5 without blocking story development.

## Save and copy

1. In Unity, save the scene and project, then close the Editor.
2. Open PowerShell in the actual project folder containing Assets, Packages, and ProjectSettings. Run `git status` and `git branch --show-current`.
3. Preserve any listed local work by committing the files you intend to keep. Do not discard changes to obtain a clean status. If already clean, no extra commit is needed. Record `git rev-parse HEAD` as the baseline. When the working branch is the existing `codex/quarantine-trial-recovered` branch and it has no unpublished local divergence, update it with `git pull --ff-only` before copying.
4. In File Explorer, go to the folder that contains the project folder (for example F:\Game Projects). Select the entire Fields of Chaldran folder, press Ctrl+C, then Ctrl+V. Rename the copy **Fields of Chaldran - Unity66-Test**. If your actual folder is spelled Chaldron, use that actual folder. This is a folder copy; do not create a new Unity project and transplant only Assets.
5. Keep the original folder and the 6.5 Editor. The copy must contain Assets (including .meta files), Packages, ProjectSettings, and the Git history for the branch workflow below. Windows may hide the .git folder; an entire folder copy includes it.

Copying the whole folder is simplest. Its generated Library cache may be large. Unity can regenerate that cache; omitting it from an intentional selective backup saves space, but preserve all authored content and metadata. Do not delete files from the original working project.

## Open the copy in 6.6

1. In Unity Hub, open **Installs → Install Editor** and install a stable Unity 6.6 release (6000.6.x, not alpha/beta). Include the same target support used by this project, including Windows Build Support for a Windows player. Keep 6.5 installed.
2. In Hub's Projects tab, use **Add → Add project from disk** (wording may vary by Hub version). Select the copied project folder.
3. In the copied project's row, select its Editor version and choose the installed 6.6 version. Open it and accept the version-change prompt for the copy.
4. Let package resolution, asset import, and compilation complete. Accept the API updater if Unity requests it. If Unity offers Safe Mode because of compile errors, enter Safe Mode and capture the first red Console error with its file and line.
5. Record the exact 6000.6 patch that succeeds. Do not manually edit ProjectVersion.txt to simulate an upgrade. Avoid unrelated package upgrades or changes to rendering options while diagnosing the migration.

## Isolate the resulting changes

In PowerShell, change to the **copied** folder, then create an upgrade branch:

```powershell
git switch -c upgrade/unity-6.6-trial
git status
```

If that branch already exists, use `git switch upgrade/unity-6.6-trial`. Review the changed source/settings/package files before committing. Commit the migration separately from new gameplay. Pushing this branch preserves it for review; do not merge the upgrade until the checks below pass.

## Required playtest

- Open **Fields of Chaldran → Open Quarantine Trial**.
- Verify the top dialogue box, typing sound, instant reveal, objective/prompt placement, movement, aim, sword attack, block, burst, sentinel routing, defeat, and retry.
- Complete the 8-bit → 16-bit transition. Check sprite scale/detail, sound, resources, weapon and objective continuity.
- Complete the Oracle approach; cancel and reopen a conversation.
- Save at the waystone and resume with C; replay with F5 and return to the checkpoint. Repeat Play/Stop more than once to catch retained static state.
- Run the existing EditMode suites in **Window → General → Test Runner**.
- Make a Windows development build and test the transition, dialogue, input and continue there too.

Prototype checkpoints live outside the project folder in Unity's persistent data directory. Copies with the same application identity may share that save location. Back up the existing prototype checkpoint before replaying if you want to preserve its story position; F5 alone retains it, but completing a fresh tutorial replaces it.

## Project-specific review

The manifest uses URP 17.6.0, Input System 1.20.0, uGUI 2.5.0 and 2D packages. Let the Editor resolve compatibility and inspect the resulting Packages changes. A source scan found no Cinemachine references, legacy Rendering Debugger types, UNITY_64 or DEVELOPMENT_BUILD directives in the authored Assets at this checkpoint. That is a limited preflight, not proof of Editor/package compatibility or compatibility for unimported Asset Store content.

The Unity 6.6 guide calls out Cinemachine becoming core, removal of legacy rendering debug APIs and dynamic batching, serialization-related YAML changes, and deprecated scripting symbols. Revisit those changes when importing third-party packages. Extra serialized changes after migration are possible; review them rather than assuming every changed line represents gameplay work.

If the copy fails, close it and reopen the untouched original with 6000.5.6f1. Do not try to downgrade the migrated copy by opening it in 6.5. After the trial passes, adopt the reviewed migration branch/commit into the working project and consistently use the verified Editor version.

## Official references

- [Unity 6.6 release announcement](https://discussions.unity.com/t/unity-6-6-is-now-available/1735357)
- [Unity 6.5 to 6.6 upgrade guide](https://docs.unity.com/en-us/engine/6000.6/manual/upgrade-guides/upgrade-guide-unity66)
- [Upgrade your Unity project](https://docs.unity.com/en-us/engine/6000.6/manual/upgrade-guides/upgrade-project)
- [Unity 6.6 log locations](https://docs.unity.com/en-us/engine/6000.6/manual/scripting/debugging-and-diagnostics/log-files)
- [Unity 6000.6.3f1 release notes](https://unity.com/releases/editor/whats-new/6000.6.3f1)
