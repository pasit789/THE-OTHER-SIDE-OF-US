---
name: memory-system
description: Create discoverable memory items and their exploration, dialogue, and combat effects in THE OTHER SIDE OF US. Use for memory pickup, cross-world clues, memory conditions, or applying a discovered memory during an encounter.
---

# Memories

Inspect current note discovery and cross-world behavior before replacing or extending it. Distinguish discovery, interpretation, and encounter use. Reading a memory is not automatically consuming it.

Author each memory with a stable ID, world and location constraints, readable content or localization key, reveal prerequisites, and explicit effect references. Reusable definitions belong in ScriptableObjects. Discovery flags, interpretations, and per-encounter usage belong in runtime state keyed by stable IDs.

Make discovery idempotent. A player revisiting a scene, switching worlds, or interacting twice receives one unlock event. Presentation may replay while the underlying unlock remains stable. Define behavior for missing or retired IDs and duplicate IDs during authoring.

Expose queries and resolved events to dialogue and psychological-combat. The dialogue runner owns its graph, and combat owns its turn and resource costs. Memory effects should describe intent as data, without directly advancing either system from a pickup callback.

For cross-world content, evaluate the player's current world and discovery prerequisites separately from visibility. Scene reloads and world shifts must reconstruct the same available interactions from state. Keep a playable fallback when the user misses an optional memory.

Use save-systems when persistent saves are requested. Version data and migrate existing note flags deliberately. Verify discovery twice, world switching, return to the scene, locked dialogue choices, encounter usage limits, and the route without the memory.
