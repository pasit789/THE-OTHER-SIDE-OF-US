using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class VerifyHallwayPuzzle
{
    const string Key = "VerifyHallwayPuzzle.Active";
    static int stage, assertions;
    static float next;
    static double deadline;
    static VerifyHallwayPuzzle() { EditorApplication.update += Tick; }
    public static void Run()
    {
        SessionState.SetBool(Key, true);
        EditorSceneManager.OpenScene("Assets/Scenes/Hallway_Prototype.unity");
        EditorApplication.isPlaying = true;
    }
    static void Assert(bool value, string label)
    {
        if (!value) throw new Exception(label);
        assertions++;
        Debug.Log("PUZZLE_CHECK: " + label);
    }
    static void Move(HallwayPlayer p, Vector3 position)
    {
        var cc = p.GetComponent<CharacterController>();
        cc.enabled = false; p.transform.position = position; cc.enabled = true;
        Physics.SyncTransforms();
    }
    static T Field<T>(object o, string name) => (T)o.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(o);
    static void Aim(HallwayPlayer p, Collider target, Vector3 position)
    {
        Move(p, position);
        var camera = p.GetComponentInChildren<Camera>();
        camera.transform.position = position + new Vector3(0, 1.6f, -.6f);
        camera.transform.LookAt(target.bounds.center);
        Assert(PrototypeThirdPerson.InteractionRay(camera, out var hit) && hit.collider == target, "Third-person interaction reaches " + target.name);
    }
    static void Tick()
    {
        if (!SessionState.GetBool(Key, false) || !EditorApplication.isPlaying || EditorApplication.isCompiling) return;
        Application.runInBackground = true;
        if (deadline == 0) { deadline = EditorApplication.timeSinceStartup + 90; next = Time.time + 1; }
        if (EditorApplication.timeSinceStartup > deadline) { Fail(new Exception("Play test timeout")); return; }
        if (Time.time < next) return;
        try
        {
            var p = UnityEngine.Object.FindFirstObjectByType<HallwayPlayer>();
            var puzzle = UnityEngine.Object.FindFirstObjectByType<HallwayLightPuzzle>();
            var gate = GameObject.Find("Closed school wing partition");
            if (stage == 0)
            {
                Assert(p != null && puzzle != null, "Hallway player and puzzle load");
                Assert(!p.IsOtherWorld && gate.GetComponent<Collider>().enabled, "World A gate solid");
                var cc = p.GetComponent<CharacterController>();
                Move(p, new Vector3(0, .05f, 9));
                cc.Move(Vector3.forward * 2);
                Assert(p.transform.position.z < 10, "World A blocks capsule movement");
                Assert(p.TrySwitchWorld() && p.IsOtherWorld && !gate.GetComponent<Collider>().enabled, "World B opens gate");
                Move(p, new Vector3(0, .05f, 10));
                for (int i = 0; i < 5; i++) Assert(!p.TrySwitchWorld() && p.IsOtherWorld, "Repeated Q inside gate rejected " + i);
                Move(p, new Vector3(0, .05f, 10.2f));
                Assert(!p.TrySwitchWorld(), "Capsule edge intersection rejected even when centre outside gate");
                Move(p, new Vector3(0, .05f, 9));
                cc.Move(Vector3.forward * 2);
                Assert(p.transform.position.z > 10.8f, "World B capsule crosses gate");
                Assert(p.TrySwitchWorld() && !p.IsOtherWorld, "Clear landing allows return to World A");
                var entry = Field<Collider>(puzzle, "entryNote");
                var request = Field<Collider>(puzzle, "requestNote");
                Aim(p, entry, new Vector3(0, .05f, 7));
                puzzle.ReadEntry();
                Assert(!puzzle.PressSwitch(1) && PrototypeProgress.LightSequenceStep == 0, "Request required before switch sequence");
                Aim(p, request, new Vector3(0, .05f, 11));
                puzzle.ReadRequest();
                Assert(PrototypeProgress.LightRequestRead, "Request accepted");
                var buttons = Field<Collider[]>(puzzle, "switches");
                for (int i = 0; i < 3; i++) Aim(p, buttons[i], new Vector3(0, .05f, buttons[i].transform.position.z));
                Assert(p.TrySwitchWorld(), "Switch to B for clue");
                Assert(!puzzle.PressSwitch(1) && PrototypeProgress.LightSequenceStep == 0, "World B cannot operate switches");
                Assert(p.TrySwitchWorld(), "Return to A at switch panel");
                puzzle.PressSwitch(0); puzzle.PressSwitch(0);
                Assert(PrototypeProgress.LightSequenceStep == 0 && PrototypeProgress.LightMistakes == 2, "Wrong input resets sequence without softlock");
                Assert(Field<string>(puzzle, "message").Contains("■"), "Two mistakes reveal hint");
                puzzle.PressSwitch(1);
                Assert(PrototypeProgress.LightSequenceStep == 1, "First correct switch recorded");
                p.TrySwitchWorld();
                // Use the real door travel entry point; aim from within interaction range.
                var door = GameObject.Find("Classroom 203").GetComponent<Collider>();
                Aim(p, door, new Vector3(-.6f, .05f, 13));
                Assert(p.GetComponent<PrototypeDoorTravel>().TryTravel(), "Hallway door loads classroom");
                stage = 1; next = Time.time + 1;
            }
            else if (stage == 1)
            {
                Assert(SceneManager.GetActiveScene().name == "Classroom_Prototype", "Classroom destination");
                var classroom = UnityEngine.Object.FindFirstObjectByType<ClassroomPrototype>();
                Assert(Field<bool>(classroom, "otherWorld") && PrototypeProgress.LightSequenceStep == 1, "World and partial progress survive classroom travel");
                var cc = UnityEngine.Object.FindFirstObjectByType<CharacterController>();
                var camera = cc.GetComponentInChildren<Camera>();
                camera.transform.position = cc.transform.position + Vector3.up * 1.5f;
                camera.transform.LookAt(GameObject.Find("Door").transform.position);
                Assert(cc.GetComponent<PrototypeDoorTravel>().TryTravel(), "Classroom door returns to hallway");
                stage = 2; next = Time.time + 1;
            }
            else if (stage == 2)
            {
                Assert(p.IsOtherWorld && !gate.GetComponent<Collider>().enabled && PrototypeProgress.LightSequenceStep == 1, "Partial puzzle restored on return");
                Assert(Mathf.Abs(p.transform.position.z - 13) < .2f, "Door return position retained");
                p.TrySwitchWorld(); puzzle.PressSwitch(0); puzzle.PressSwitch(2);
                Assert(PrototypeProgress.HallwayLightsSolved && PrototypeProgress.LightSequenceStep == 3 && !gate.GetComponent<Collider>().enabled, "Complete sequence opens permanent shortcut in A");
                foreach (var lamp in Field<Light[]>(puzzle, "pathLights")) Assert(lamp.enabled, "Route light enabled");
                Assert(!puzzle.PressSwitch(0) && PrototypeProgress.LightSequenceStep == 3, "Solved state ignores repeated switches");
                puzzle.ReadRequest(); Assert(!PrototypeProgress.LightReplyRead, "Reply requires B");
                p.TrySwitchWorld(); puzzle.ReadRequest(); Assert(PrototypeProgress.LightReplyRead, "Reply readable in B");
                Move(p, new Vector3(0, .05f, 10));
                Assert(p.TrySwitchWorld(), "Solved gate permits switching inside former obstacle");
                SceneFadeTransition.Begin("Rooftop_Prototype", new Vector3(0, .05f, -6), 0);
                Assert(!p.TrySwitchWorld(), "World input blocked during fade");
                stage = 3; next = Time.time + 2;
            }
            else if (stage == 3)
            {
                if (SceneFadeTransition.IsTransitioning) return;
                Assert(SceneManager.GetActiveScene().name == "Rooftop_Prototype", "Fade reaches rooftop");
                Assert(!p.IsOtherWorld && PrototypeProgress.HallwayLightsSolved, "Rooftop retains A and solved state");
                Assert(p.TrySwitchWorld(), "Rooftop Q remains available");
                SceneFadeTransition.Begin("Hallway_Prototype", new Vector3(0, 2.05f, 29.2f), 180);
                stage = 4; next = Time.time + 2;
            }
            else if (stage == 4)
            {
                if (SceneFadeTransition.IsTransitioning) return;
                Assert(SceneManager.GetActiveScene().name == "Hallway_Prototype" && p.IsOtherWorld, "Rooftop return retains B");
                Assert(PrototypeProgress.HallwayLightsSolved && PrototypeProgress.LightReplyRead && !gate.GetComponent<Collider>().enabled, "Solved gate and reply persist after rooftop return");
                Assert(GameObject.Find("Hallway students").transform.childCount == 5, "Existing five hallway NPCs retained");
                SessionState.SetBool(Key, false);
                Debug.Log("PUZZLE_PLAY_PASS: " + assertions + " assertions");
                EditorApplication.Exit(0);
            }
        }
        catch (Exception error) { Fail(error); }
    }
    static void Fail(Exception error)
    {
        SessionState.SetBool(Key, false);
        Debug.LogError("PUZZLE_PLAY_FAIL: " + error);
        EditorApplication.Exit(1);
    }
}

