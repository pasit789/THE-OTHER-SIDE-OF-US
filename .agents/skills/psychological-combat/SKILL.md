---
name: psychological-combat
description: Design or implement THE OTHER SIDE OF US turn-based psychological encounters with Attack, Defend, Talk, Memory, emotional enemies, and nonviolent resolution. Use for battle rules, encounter data, turn resolution, or combat-dialogue integration.
---

# Psychological combat

Read the current encounter rules and existing runtime state before proposing mechanics. The prototype currently focuses on exploration. Treat new battle numbers and emotional thresholds as proposals until the user accepts them.

Use ScriptableObjects for reusable encounter definitions, enemy intent patterns, action costs, dialogue responses, and memory effects. Keep mutable health, emotional state, cooldowns, turn ownership, and used-memory IDs in encounter instances. Never write play-session values into definition assets.

Model a turn as command selection, validation, resolution, presentation, and completion. A command must resolve once even if a UI button is pressed repeatedly. Deduct resources only after validation. Animation and sound acknowledge resolved events and do not independently apply damage or advance turns. End-of-battle checks run before scheduling the next actor.

Define all four actions for the requested encounter. Attack applies the agreed physical and emotional effects. Defend has an explicit expiry and stacking rule. Talk evaluates authored conditions and consequences. Memory validates discovery and usage rules through memory-system state. Missing memories must leave at least one viable route through the encounter.

Represent emotional monsters through authored motives, telegraphed intent, and state-dependent responses. Avoid turning named mental health conditions into automatic enemy labels. Define victory, de-escalation, defeat, retry, and retreat behavior only to the extent the requested encounter needs them.

Use dialogue-systems for conversation graph execution and memory-system for memory ownership. Combat owns turn order and effect application. Relationship consequences are emitted once at the agreed outcome boundary.

Verify a full encounter through its normal command entry point. Cover repeated input, invalid or unavailable Memory, Talk conditions, Defend expiry, defeat and retry, and both violent and de-escalation outcomes when supported. Report the tested paths and any untested presentation.
