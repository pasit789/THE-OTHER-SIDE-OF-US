---
name: horror-director
description: Design or implement psychological horror pacing in THE OTHER SIDE OF US using authored events, atmospheric URP lighting, spatial sound, and tension cooldowns. Use for environmental scares, world-shift atmosphere, or tension scheduling.
---

# Horror director

Read the scene route, interaction objectives, existing lighting, and audio configuration. Define the intended emotional beat and what the player must still be able to see or hear. Preserve door readability, navigation cues, and dialogue legibility.

Author event definitions with stable IDs, location or world eligibility, prerequisites, duration, cooldown, repeat policy, and recovery behavior. Store runtime scheduling state separately from ScriptableObject definitions. Begin with authored sequencing and a small explicit state machine. Introduce procedural selection only when the request needs it.

Prevent incompatible events from overlapping. Pause new scares during loading, screen fades, dialogue, or turn resolution unless the story explicitly calls for one. Separate anticipation, peak, and recovery so tension can release. If randomness is used, permit a seeded test sequence and record the selected event IDs for diagnosis.

For lighting, use the installed URP version and urp-postprocessing guidance. Apply temporary overrides to runtime instances, and restore the prior state on completion, cancellation, scene unload, or disable. Avoid mutating shared materials and VolumeProfile assets during Play Mode.

Use audio-design for spatial cues and audio-setup-mixers only when mixer routing is requested and its prerequisites exist. Cap simultaneous voices and use the existing mixer groups. Implement requested intensity and accessibility controls without hiding essential gameplay cues behind the horror effect.

Verify event eligibility, cooldowns, cancellation during travel, restoration of lighting and audio, and repeated scene entry. Test readability and sound levels in a visible or audible run when possible. Label automated state checks separately from visual or audio validation.
