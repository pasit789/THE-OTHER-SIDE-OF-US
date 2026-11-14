---
name: relationship-system
description: Design or implement NPC affection, trust, relationship progression, and branching consequences in THE OTHER SIDE OF US. Use when choices or events change an NPC relationship or gate story branches.
---

# Relationships

Read the authored NPC and story requirements before defining scales or thresholds. Affection and trust are separate values with distinct meanings. Do not infer that high affection grants trust or overrides a character's boundaries.

Give NPCs stable authored IDs. Keep reusable definitions and progression rules in ScriptableObjects and mutable values in per-session state. Use explicit event IDs for relationship changes so revisiting a node, reloading a scene, or replaying a presentation callback cannot award the same change twice.

Specify each consequence as a condition, value change, progression event, and story flag. Define clamping, threshold equality, downgrade behavior, and mutually exclusive branches when relevant. Centralize condition evaluation so dialogue, exploration, and combat agree on eligibility.

Integrate with the existing dialogue runner through conditions and resolved choice events. Integrate combat through encounter outcomes and memory-system through discovered or interpreted memory IDs. Do not let UI text or scene object names serve as persistence keys.

Preserve progression across scene transitions using the project's state owner. If disk persistence is requested, use save-systems for versioning, defaults for older saves, unknown NPC IDs, and migration. Avoid adding a second persistent singleton when an existing service can own the state.

Verify threshold boundaries, repeated choice events, mutually exclusive outcomes, scene travel, and save round-trips where persistence exists. Describe which branch changed and why without inventing unapproved story canon.
