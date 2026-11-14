# บันทึกการติดตั้ง AI Agent Skills

วันที่ดำเนินการ 14 พฤศจิกายน 2026

โปรเจกต์ THE OTHER SIDE OF US

ตำแหน่ง Unity project และ Git root คือ `C:/672110109/GitHub/THE OTHER SIDE OF US/THE-OTHER-SIDE-OF-US`

## ผลงานรอบนี้

ติดตั้ง skills จาก GitHub 28 ตัว และสร้าง project-specific skills 5 ตัว รวม 33 ตัวใน `.agents/skills/` พร้อม `AGENTS.md` สำหรับกติกาการพัฒนาและการเลือก skill

งานรอบนี้เป็นการติดตั้งคำแนะนำให้ Codex ไม่มีการเปลี่ยน gameplay, scenes, prefabs, assets, `.meta`, package manifest หรือ ProjectSettings ตรวจ SHA-256 ของไฟล์เดิมที่ Git ติดตาม 109 ไฟล์แล้วตรงกับก่อนเริ่มงานทั้งหมด รายละเอียดอยู่ใน `.agents/skill-sources/verification.json`

## ตรวจโปรเจกต์ก่อนติดตั้ง

ก่อนเริ่มไม่พบ `AGENTS.md` และ `.agents/skills/` ใน Unity project root สถานะ Git ไม่มีไฟล์แก้ไขค้างอยู่ในขณะตรวจ

| รายการ | เวอร์ชันที่พบ |
| --- | --- |
| Unity Editor | 6000.6.5f1 หรือ Unity 6.6 |
| Input System | 1.19.0 |
| AI Navigation | 2.0.12 |
| Universal Render Pipeline | 17.6.0 |
| Shader Graph จาก packages-lock.json | 17.6.0 |
| uGUI | 2.6.0 |
| Timeline | 6.6.0 |
| Test Framework | 1.8.0 |
| Visual Scripting | 1.9.11 |
| Rider integration | 3.0.38 |
| Visual Studio integration | 2.0.26 |
| Version Control integration | 2.12.4 |

โครงสร้างเดิมประกอบด้วย `Assets/Scripts`, `Assets/Editor`, `Assets/Scenes`, `Assets/Materials`, `Assets/Settings`, `Packages` และ `ProjectSettings` มีฉาก SampleScene, Classroom_Prototype, Hallway_Prototype และ Rooftop_Prototype รวมถึงระบบประตู การเฟดเปลี่ยนฉาก และตัวควบคุมผู้เล่นเดิม

## แหล่งติดตั้งและการตรึงเวอร์ชัน

ติดตั้งจาก commit ที่ตรวจพบจริง เพื่อให้ตรวจย้อนกลับและติดตั้งซ้ำได้โดยไม่ขึ้นกับการเปลี่ยนแปลงของ main

| Repository | Commit | สิทธิ์การใช้งาน | จำนวน |
| --- | --- | --- | --- |
| Unity-Technologies/unity-agent-plugin | `2d68641b24055d9845cb2804bf5a331cc2ff98d9` | Unity Companion License สำหรับ Unity-dependent projects | 10 |
| gamedev-skills/awesome-gamedev-agent-skills | `c6ca6c41c839b68319ffc08430b99ab768debc32` | Apache-2.0 พร้อม NOTICE | 18 |

ดูแหล่งต้นทางได้ที่ [Unity Agent Plugin](https://github.com/Unity-Technologies/unity-agent-plugin) และ [Awesome Gamedev Agent Skills](https://github.com/gamedev-skills/awesome-gamedev-agent-skills)

เก็บชื่อ repository, commit, path ต้นทาง, SHA-256 และการปรับ metadata ไว้ใน `.agents/skill-sources/manifest.json` เก็บข้อความ license และ NOTICE ที่ `.agents/skill-sources/licenses/`

## Installed Skills

### จาก Unity Technologies

| Skill | หน้าที่ |
| --- | --- |
| `ui-imgui` | แก้ OnGUI, EditorWindow และ Inspector เหมาะกับ UI ต้นแบบเดิม |
| `ui-ugui` | Canvas, RectTransform, layout และ prefab UI |
| `optimize-text-mesh-pro` | ฟอนต์ fallback, atlas, คุณภาพและประสิทธิภาพ TMP |
| `urp-postprocessing` | Volume, bloom, tonemapping, vignette และการตั้งค่า URP |
| `shader-graph-create-custom-node` | สร้าง Shader Graph node จาก HLSL reflection |
| `validate-urp-render-graph-renderer-feature` | ตรวจ RendererFeature และ Render Graph ของ Unity 6 |
| `audio-setup-mixers` | จัด AudioSource เข้ากลุ่ม Audio Mixer ที่มีอยู่ |
| `unity-cli` | ค้นคำสั่ง Unity CLI และ workflow สำหรับ build/test/editor |
| `unity-pipeline` | ควบคุม Editor สดเมื่อมี Pipeline package และ connection พร้อม |
| `unity-package-management` | ตรวจและจัดการ UPM packages เมื่องานนั้นต้องการ |

### จาก Gamedev Skills

| Skill | หน้าที่ |
| --- | --- |
| `unity-csharp-scripting` | MonoBehaviour, lifecycle, serialization และตรวจ GUID |
| `unity-input-system` | Input Actions, PlayerInput และ action maps |
| `unity-animation` | Animator, blend tree, root motion และ humanoid IK |
| `unity-physics` | Rigidbody, CharacterController, collision และ physics timing |
| `unity-scriptableobjects` | reusable data และการแยก definition ออกจาก runtime state |
| `unity-navmesh` | NavMeshSurface, NavMeshAgent, links และการตรวจเส้นทาง |
| `camera-systems` | follow, orbit, camera collision และความลื่นตาม frame rate |
| `dialogue-systems` | branching dialogue, conditions, choices และ variables |
| `visual-novel` | presentation ของเรื่องเล่า backlog, skip และเส้นทางเรื่อง |
| `rpg` | progression, quest และโครงสร้างระบบ RPG |
| `game-ai` | เลือก FSM, behavior tree, utility หรือ GOAP และระบบรับรู้ |
| `save-systems` | save slots, atomic writes และ versioned migration |
| `level-design` | ขนาดพื้นที่ตาม controller, gating และ pacing |
| `audio-design` | ออกแบบเสียง spatial cues และการจัดเสียงตามการเล่น |
| `game-ui-ux` | HUD, readability, focus, scaling และ accessibility |
| `performance-optimization` | profiling และแยกสาเหตุ CPU/GPU/GC |
| `shader-programming` | คณิตศาสตร์ shader และแนวทางแปลง GLSL/HLSL |
| `game-docs` | GDD และเอกสาร feature ที่เชื่อมกับสถานะโค้ดจริง |

## Created Custom Skills

ตรวจรายการ skills ในสอง repository แล้วไม่พบตัวที่ตรงขอบเขตเฉพาะเกมครบถ้วน จึงสร้างดังนี้

| Skill | เงื่อนไขใช้งานและขอบเขต |
| --- | --- |
| `psychological-combat` | งาน turn-based psychological encounter ใช้ Attack, Defend, Talk และ Memory พร้อม emotional enemies และทางจบด้วยการสื่อสาร |
| `relationship-system` | งาน affection, trust, relationship progression และผลของตัวเลือกต่อเนื้อเรื่อง |
| `memory-system` | งานค้นพบความทรงจำ การตีความ และการใช้ใน dialogue/combat |
| `horror-director` | งาน horror events, URP atmosphere, sound triggers, cooldown และ tension pacing |
| `unity-third-person-controller` | งาน third-person movement ที่ผู้ใช้ร้องขอ ใช้ Input System และระบบชนเดิมร่วมกับ camera-relative movement |

ตัวที่ห้าเพิ่มเพื่อครอบคลุมหัวข้อ third-person controller ที่ร้องขอ เนื่องจากพบ camera-systems แต่ไม่พบ skill controller แบบ third-person โดยตรง การติดตั้ง skill นี้ไม่ได้เปลี่ยนตัวควบคุมผู้เล่นจาก first-person

ทุก custom skill มี YAML frontmatter, activation description, ขั้นตอนทำงาน, ขอบเขตความรับผิดชอบ, การเชื่อมกับระบบอื่น และแนวทางตรวจผล

## Folder Structure

```text
THE-OTHER-SIDE-OF-US/
├── AGENTS.md
├── read.md
├── README.md                       คู่มือเกมเดิม
├── .agents/
│   ├── skills/
│   │   ├── unity-csharp-scripting/SKILL.md
│   │   ├── unity-input-system/SKILL.md
│   │   ├── psychological-combat/SKILL.md
│   │   ├── relationship-system/SKILL.md
│   │   ├── memory-system/SKILL.md
│   │   ├── horror-director/SKILL.md
│   │   ├── unity-third-person-controller/SKILL.md
│   │   └── skill อื่นตามตาราง รวม 33 โฟลเดอร์
│   └── skill-sources/
│       ├── selections.json
│       ├── manifest.json
│       ├── project-before.json
│       ├── verification.json
│       └── licenses/
├── Assets/
├── Packages/
└── ProjectSettings/
```

ไฟล์ references, scripts และ agents/openai.yaml ของ upstream อยู่ใน skill ของตัวเองตามที่ต้นทางจัดไว้ ไม่มี skill ติดตั้งใน Assets และไม่มีการติดตั้ง skills แบบ global

## AGENTS.md Configuration

`AGENTS.md` ระบุให้เลือกและอ่าน relevant skills ก่อนลงมือ ยึด Unity และ package versions จริง ใช้ C# ที่แยกความรับผิดชอบชัดเจน และใช้ ScriptableObjects สำหรับ reusable authored data

กำหนดให้รักษา scene, prefab, asset, `.meta`, GUID และงานเดิมของผู้ใช้ ตรวจ callback lifecycle, event cleanup, static reset, input ownership และ scene transitions รวมถึงหลีกเลี่ยง dependency ที่ไม่จำเป็น

มีตารางแยกหน้าที่ของ skills ที่มีเนื้อหาเกี่ยวข้องกัน เช่น game-ai ดูการตัดสินใจ ส่วน unity-navmesh ดูการเดินทาง, audio-design ดูการออกแบบ ส่วน audio-setup-mixers ดู routing และ psychological-combat ดู turn resolution ส่วน dialogue-systems ดู conversation graph

ระบุให้ตรวจ schema ของ Unity CLI จริงก่อนใช้คำสั่ง เพราะเอกสารบาง skill อ้างอิง Pipeline คนละรุ่น โดยเฉพาะรูปแบบ script eval พร้อมระบุให้ทดสอบตามผลกระทบและแยกผลตรวจอัตโนมัติออกจากการดูภาพหรือฟังเสียงจริง

## Installation Verification

- ตรวจ `SKILL.md` ครบ 33 ตัวด้วย `quick_validate.py` จาก skill-creator ผ่านทั้งหมด
- ชื่อ skill ไม่ซ้ำ และชื่อโฟลเดอร์ตรงกับ frontmatter ทุกตัว
- เก็บ SHA-256 ของไฟล์ทั้งหมดในแต่ละ skill และ source commit ใน manifest
- ตรวจ relative links ของเอกสารประกอบ โดยแยก links ตัวอย่างใน game-docs templates ออกจากไฟล์ที่ต้องมีจริง
- ตรวจ syntax และ imports ของ Python helpers สองไฟล์ที่มากับ upstream โดยไม่ได้เรียกใช้ helpers เหล่านั้นกับโปรเจกต์
- ตรวจรูปแบบตำแหน่งติดตั้งตามเอกสาร discovery ของ Codex และตรวจการอ้างอิงใน AGENTS.md
- ตรวจ SHA-256 ไฟล์เดิมที่ Git ติดตาม 109 ไฟล์ ไม่มีไฟล์เปลี่ยนแปลงจากงานติดตั้งนี้
- ไม่ได้เปิด Unity เพื่อ compile หรือ Play Mode เพราะรอบนี้เพิ่มเฉพาะ agent guidance

Codex ค้น `.agents/skills/` จาก working directory ไล่ขึ้นไปยัง repository root ดังนั้นควรเปิด Codex ที่โฟลเดอร์ Unity project `THE-OTHER-SIDE-OF-US` โดยตรง แชตที่เริ่มจากโฟลเดอร์แม่ `THE OTHER SIDE OF US` จะไม่ค้น skills ในโฟลเดอร์ลูกโดยอัตโนมัติ การตรวจครั้งนี้ยืนยันโครงสร้างไฟล์ตามกติกา discovery แต่ยังไม่ได้ยืนยันรายการ loaded skills ของแชตใหม่ใน Unity root [เอกสาร Codex Skills](https://learn.chatgpt.com/docs/build-skills)

หลังเปิดโปรเจกต์ถูก root ให้ใช้ skills ในเทิร์นถัดไป เช่น `$unity-input-system` หรือ `$psychological-combat` หากรายการยังไม่ปรากฏ ให้เปิดแชตใหม่หรือ restart Codex

## Errors, Compatibility and Missing Dependencies

1. คำสั่ง `python` ชี้ไป WindowsApps alias ที่เรียกไม่ได้ จึงใช้ Python ที่ติดตั้งจริง `C:/Users/OS/AppData/Local/Python/pythoncore-3.15-64/python.exe`
2. Python 3.15 ไม่มี PyYAML และไม่พบ binary wheel ที่ตรงรุ่น จึงดาวน์โหลด wheel ของ PyYAML 6.0.3 แล้วใช้เฉพาะไฟล์ Pure Python ในพื้นที่ตรวจสอบชั่วคราว ไม่ได้โหลด binary extension หรือติดตั้ง global ตัว validator ทำงานผ่านแล้ว
3. Upstream สอง skills ใช้ top-level `required_packages` ซึ่ง validator ของ Codex ไม่รองรับ จึงย้ายคีย์ไป `metadata.required_packages` ใน shader-graph-create-custom-node และ urp-postprocessing โดยรักษาค่า requirement เดิม และบันทึกการปรับไว้ใน manifest
4. Skills ชุมชนหลายตัวอ้างอิง Unity 6.3 และ Input System 1.20.x ขณะที่โปรเจกต์ใช้ 6.6 และ 1.19.0 จึงต้องตรวจ API กับแพ็กเกจจริงก่อนใช้ตัวอย่าง ไม่ได้อัปเกรดโปรเจกต์เพื่อให้ตรงตัวอย่าง
5. มี Unity CLI executable แต่ไม่พบ `com.unity.pipeline` ใน manifest/lock จึงยังไม่รับรอง workflow ควบคุม Editor สดของ unity-pipeline และ audio-setup-mixers การติดตั้ง package นี้อยู่นอกงานเพิ่ม skills รอบนี้
6. Cinemachine, Ink, Yarn Spinner และ audio middleware ไม่ใช่ dependency ที่จำเป็นสำหรับการโหลด skills และยังไม่ได้ติดตั้ง ตัวอย่างที่ใช้เครื่องมือเหล่านี้ต้องเลือกตามงานภายหลัง
7. บาง discipline skills มีตัวอย่าง Godot หรือ GLSL ใช้เป็นแนวทางการออกแบบและแปลงให้เข้ากับ Unity C#/URP ตาม AGENTS.md
8. ความพร้อมในการอ่าน skill และผ่าน frontmatter validation ไม่ได้พิสูจน์ว่า code example ทุกตัวทำงานบน Unity 6000.6.5f1 แล้ว ต้อง compile และทดสอบเมื่อมีงาน implementation จริง

การติดตั้งจาก GitHub ทั้งสองแหล่งสำเร็จ ไม่มี skill ที่เลือกไว้ติดตั้งล้มเหลว รายการที่ขาดจากต้นทางถูกครอบคลุมด้วย custom skills และบันทึกชื่อแยกจาก upstream อย่างชัดเจน


## Expanded prototype layout — 2026-11-14

The playable Classroom, Hallway and Rooftop scenes are expanded using `Assets/Editor/ExpandPrototypeSpaces.cs`. The editor menu is Tools > The Other Side > Expand Prototype Spaces. Each scene has an `Expanded prototype spaces` marker, so rerunning the menu skips already processed scenes.

- Classroom grows from 12 × 14 to 12 × 20 metres. The original notebook, reply, front door and arrival point stay in place. Two extra rows of desks occupy the expanded area.
- Hallway uses 4-metre modules, 3.6-metre wall height, loops, intersections and two dead ends. The direct corridor is partitioned at z = 10, so reaching the stairs requires a side route. The former hallway structure is retained inactive. Stair steps, landing, classroom door objects and scene-exit triggers remain active.
- The route starts in classroom wing A, crosses into the library junction at (8, 4), then can return through (8, 12) to the northern classroom corridor. The longer loop reaches science wing C at (24, 12), reconnecting at z = 20. Dead ends at (16, 24) and (32, 20) provide space for echo objects and memories. Coordinates here are Unity x/z metres.
- Empty, named GameObjects mark candidate world-shift gate, echo-object, memory and encounter locations. These are authoring anchors for future puzzles.
- Rooftop grows from 20 × 24 to 32 × 36 metres, with a meeting bench and final-message marker. Existing stairwell and scene-travel positions remain in place.

The editor helper checks floors and uses capsule casts with the player's 0.3-metre radius to verify reachability of all hallway modules before saving. The school layout remains a greybox for playtesting.


### World-space labels removed — 2026-11-14

Disabled all eight Wayfinding text objects in Hallway_Prototype at the user’s request because the labels appeared reversed. Removed the label-generation calls and TextMesh helper from ExpandPrototypeSpaces. Puzzle authoring markers remain named in the Hierarchy.


### Prototype roofs — 2026-11-14

Added a classroom roof covering 12.4 × 20.4 metres at y = 4.1 and a staircase roof with upper wall infill in Hallway. Roof meshes use the existing Ceiling material and BoxColliders. Hallway modules already have ceilings. Scene object IDs and root references were checked after editing. Unity execution was unavailable because the temporary validation project had been removed, so visual and Play Mode verification remain pending.


### Roof reference repair — 2026-11-14

The initial direct scene edit assigned roof file IDs above the signed 64-bit limit by incrementing the reserved SceneRoots ID. This prevented reliable Unity loading. Reassigned only the newly added roof objects and their components to unique valid IDs, updating all matching references in Classroom and Hallway.
`VerifyRoofs.Run` completed in Unity 6000.6.5f1 with exit code 0. Both scenes loaded successfully and the roofs passed active-renderer, mesh, material and upward collision checks. Visual Play Mode review remains pending.


### Q world shift and hallway crowd — 2026-11-14

World switching uses Q in Classroom, Hallway and Rooftop, with matching HUD instructions. HallwayPlayer spawns five Capsule students when the hallway starts. Each uses a CharacterController capsule for solid collisions, a waypoint patrol, varied walking speed and short pauses. Blocked students wait and retrace their current segment. Students hide and disable collision in the other world, then return to their previous positions in the real world. The crowd is generated during Play Mode and is scoped to the hallway scene.
Unity 6000.6.5f1 Play Mode validation passed with exit code 0. All five NPCs moved from their spawn positions, remained above the floor, hid with their colliders in the other world, and returned in the real world. CharacterController.minMoveDistance is zero to support small frame times. Physical keyboard input and visual appearance still need manual review.
