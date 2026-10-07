# Unity 6.6: active project baseline

The project uses **Unity 6000.6.4f1**, revision **12bfff696524**, as recorded in `ProjectSettings/ProjectVersion.txt`. The user-tested migration was published in commit `2518a7c67d40afa46dd3de678a7fcb4c424dc0a3`. The migration is complete; continue development in the upgraded folder with this Editor until an explicit later upgrade is agreed.

## Update the working project

Close Unity. In PowerShell:

```powershell
Set-Location -LiteralPath "F:\GameProjects\Fields of Chaldran - Unity66-Test"
git status --short
```

Preserve any listed local changes before continuing. With a clean working tree:

```powershell
git fetch origin --prune
git switch master
git pull --ff-only origin master
Get-Content .\ProjectSettings\ProjectVersion.txt
```

`master` is the canonical branch. If Git reports divergence, inspect and preserve the local work before resolving it. Open this same project in Unity 6000.6.4f1 and let import and compilation finish. Use GitHub/PowerShell directly; no package installer or repeat migration is required.

## Verification still outstanding

The user completed the preceding game content and first Oracle library slice with expected behavior and no reported Console errors. The unclear library puzzle and missing visible Oracle are story issues tracked in the [story spine](STORY-SPINE.md), not reasons to change Editor versions.

Unity EditMode tests and a Windows build remain unverified. An earlier Editor shutdown error remains unexplained. If it repeats, retain the active Editor log and crash report before another launch; use Console's Open Editor Log command to find the active log. Diagnose from that evidence before selecting a fix.

The game's 8-bit and 16-bit presentation tiers are independent of the Unity Editor version.
