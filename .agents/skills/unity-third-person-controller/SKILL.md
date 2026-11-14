---
name: unity-third-person-controller
description: Build or adapt a third-person character controller for this Unity 6 project using the installed Input System and camera-relative movement. Use when third-person locomotion is explicitly requested, including collision, slopes, stairs, animation, and camera integration.
---

# Third-person controller

Inspect ClassroomPrototype and HallwayPlayer and the scenes that reference them before changing locomotion. Existing gameplay uses first-person controls. A request to install this skill does not authorize converting those controllers.

Read unity-input-system, unity-physics, and camera-systems for the affected layers. Use the actual installed versions. Cinemachine is optional and was absent at installation. Reuse an existing camera rig or use a small camera follow component if it satisfies the request.

Choose one authority for player motion. A CharacterController owns kinematic displacement and grounding. A dynamic Rigidbody owns force-driven movement. A root-motion Animator may drive displacement only when the chosen motion model explicitly consumes it. Avoid competing position writes.

Project camera forward onto the movement plane, normalize diagonal input, and separate facing from displacement. Treat mouse delta and gamepad angular speed according to their units. Clamp pitch and handle camera collision without snapping through walls. Update follow after player movement.

Define supported slopes, step height, gravity, movement lock, and spawn orientation from the actual level dimensions. Coordinate cursor and action-map ownership with dialogue, menus, and SceneFadeTransition. Reset vertical velocity after teleportation when needed. Preserve the door and stair scene-travel entry points.

Use unity-animation for locomotion parameters when a rig exists. Do not add an Animator or animation package solely to move a capsule.

Verify walking diagonally, stairs in Hallway_Prototype, slopes and corners, camera obstruction, varying frame rates, input focus, and travel between classroom, hallway, and rooftop. Report which device inputs and visual behaviors were actually exercised.
