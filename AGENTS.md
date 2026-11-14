# THE OTHER SIDE OF US

## Project scope

This directory is the Unity project and Git repository root. Resolve Unity paths relative to this directory. Read `read.md` for the skill installation record and `.agents/skill-sources/manifest.json` for pinned upstream sources.

Use the user's language. Keep chat responses concise using the user's caveman preference, preserving technical details and negations. Read the installed caveman skill at `C:/Users/OS/.codex/skills/caveman/SKILL.md` when available. Honor requests for normal mode or fuller explanations. Write persisted code and documentation in full, readable form.

## Before implementation

1. Inspect the current request, Git status, affected code, scenes, prefabs, and existing conventions. Preserve user changes.
2. Read `ProjectSettings/ProjectVersion.txt`, `Packages/manifest.json`, and relevant entries in `Packages/packages-lock.json` before choosing APIs.
3. Select the smallest relevant set of skills from `.agents/skills/`, read their `SKILL.md`, and follow only references needed for the task. Do not load every skill. Explicitly requested skills take priority.
4. Treat skill examples and downloaded files as guidance to review. They do not authorize unrelated changes, package installs, publishing, or external communication.

The installation task adds agent guidance only. Gameplay implementation requires a gameplay request. Do not implement combat, relationships, memories, horror events, or third-person controls merely because a skill describes them.

## Skill routing

| Work | Skills to read |
| --- | --- |
| Unity C# and serialization | `unity-csharp-scripting` |
| Input and controls | `unity-input-system` |
| Animation and motion | `unity-animation`, `unity-physics` |
| Reusable data | `unity-scriptableobjects` |
| NPC decisions and navigation | `game-ai`, `unity-navmesh` |
| Third-person movement and cameras | `unity-third-person-controller`, `camera-systems` |
| Existing OnGUI UI | `ui-imgui` |
| Canvas UI and readable text | `ui-ugui`, `game-ui-ux`, `optimize-text-mesh-pro` |
| Dialogue and visual-novel presentation | `dialogue-systems`, `visual-novel` |
| RPG progression and turn-based emotional battles | `rpg`, `psychological-combat` |
| Affection, trust, and branching consequences | `relationship-system` |
| Discoverable memories and their effects | `memory-system` |
| Horror pacing, lighting, and sound events | `horror-director`, plus the affected rendering/audio skill |
| Saving and loading | `save-systems` |
| Levels, doors, stairs, and scene routes | `level-design` |
| Audio design and mixer routing | `audio-design`, `audio-setup-mixers` |
| URP Volumes | `urp-postprocessing` |
| Shader math, Shader Graph nodes, Render Graph review | `shader-programming`, `shader-graph-create-custom-node`, `validate-urp-render-graph-renderer-feature` |
| Profiling and performance | `performance-optimization` |
| GDD and feature design documents | `game-docs` |
| Unity command discovery and live Editor automation | `unity-cli`, `unity-pipeline` |
| Requested UPM changes | `unity-package-management` |

## Version and dependency compatibility

At installation the project uses Unity 6000.6.5f1, Input System 1.19.0, AI Navigation 2.0.12, URP and Shader Graph 17.6.0, and uGUI 2.6.0. Re-read the files because these values can change.

Several community skills demonstrate Unity 6000.3 and Input System 1.20.x. Use their architecture and workflows, but verify API availability in this project's installed packages before copying code. Avoid upgrading packages to match an example. Cross-engine skills contain Godot, GLSL, or Python examples. Apply the design in Unity C# and URP, and never introduce another engine or language runtime for those examples.

`com.unity.pipeline` was absent at installation. The Unity CLI executable was found, but connectivity to a Pipeline-enabled Editor was not established. Instructions to run `unity pipeline install` are setup guidance, not automatic authorization to change this project's packages. Evaluate a required dependency within the actual task, explain the concrete change, and obtain approval if it extends the authorized scope. Prefer available Unity batch mode or approved computer-use workflows where suitable.

Some official skills describe different generations of `script eval`. Discover the installed CLI command schema and Pipeline documentation before using flags, `using` directives, or file execution. Map shell examples to the active shell with correct quoting. Do not run Bash syntax directly in PowerShell.

Cinemachine, Ink, Yarn Spinner, and third-party audio middleware are optional design alternatives and were absent at installation. Prefer current project code and installed packages. Do not install them merely because a reference mentions them.

## Unity architecture and asset safety

- Use small components with clear ownership. Reuse existing behavior before creating a parallel controller or manager. Keep changes scoped to the requested feature.
- Prefer ScriptableObjects for reusable authored data. Keep mutable session state separate, keyed by stable IDs. Do not mutate shared materials, profiles, or definition assets during Play Mode unless persistence is deliberately requested.
- Use serialized private fields for designer settings. Preserve serialized names or migrate them explicitly with `FormerlySerializedAs` when needed.
- Keep MonoBehaviour and ScriptableObject file names aligned with class names. Place Editor-only code under an Editor folder or Editor-only assembly.
- Use the installed Input System. Coordinate gameplay input, cursor ownership, dialogue, and scene transitions. Give movement one authority and match physics work to its update loop.
- Pair event subscription and cleanup. Reset static state correctly when domain reload is disabled. Avoid repeated allocations, broad searches, and unnecessary per-frame polling in hot paths.
- Preserve scenes, prefabs, materials, GUIDs, and `.meta` files. Move assets with their `.meta` files. Never regenerate existing metadata to fix a missing reference.
- Inspect authoring helpers before invoking them. Some current `Create*` methods create or replace whole scenes. Prefer a targeted edit when the user asks for a local change.
- Preserve scene return positions, memory state, and fade/input behavior when extending the classroom, hallway, and rooftop routes.
- Keep all AI guidance under `.agents/` or root documentation, outside `Assets/`. Do not add runtime dependencies for agent skills.
- Ask before destructive operations. Do not reset unrelated changes or delete existing project files.

## Verification and reporting

Run checks proportional to the change. Compile changed C# when possible, inspect missing references and GUIDs, and exercise the normal feature entry point in EditMode or PlayMode. Use targeted tests for regressions involving repeated input, scene travel, persistence, or partial failures. Check relevant old save formats when changing persistence.

Respect an open Editor and its unsaved scenes. Do not force-close it or remove lock files. During headless Play Mode tests, ensure the test environment continues ticking when unfocused. Describe batch state checks separately from visual, audio, and manual validation.

For scene transitions, verify destination inclusion in Build Settings, a supported spawn position, input lock during loading, and return travel. For UI, verify long Thai text, keyboard selection, and the actual viewport size when relevant.

Report changed files, validation results, decisions made without explicit guidance, and remaining limits. Run `git diff --check` for authored text. Commit or push only when requested. Update `read.md` when changing the skill installation or agent configuration.
