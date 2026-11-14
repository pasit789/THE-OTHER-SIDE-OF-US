using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ExpandPrototypeSpaces
{
    private static Transform root;
    private static Material wall, floor, trim;

    [MenuItem("Tools/The Other Side/Add Prototype Roofs")]
    public static void AddRoofs()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play Mode first.");
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        wall = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Hallway/Ceiling.mat");
        foreach (string sceneName in new[] { "Classroom", "Hallway" })
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/" + sceneName + "_Prototype.unity");
            if (GameObject.Find(sceneName == "Classroom" ? "Classroom roof" : "Stairwell roof") != null) continue;
            root = new GameObject("Prototype roof additions").transform;
            if (sceneName == "Classroom")
            {
                Box("Classroom roof", new Vector3(0, 4.1f, 3), new Vector3(12.4f, .2f, 20.4f), wall);
                for (int row = 0; row < 3; row++)
                for (int col = 0; col < 2; col++)
                {
                    var position = new Vector3(col == 0 ? -3 : 3, 3.6f, -3 + row * 6);
                    Box("Ceiling fixture", position + Vector3.up * .3f, new Vector3(.3f, .08f, 1.2f), wall);
                    var light = new GameObject("Classroom ceiling light").AddComponent<Light>();
                    light.transform.SetParent(root); light.transform.position = position;
                    light.type = LightType.Point; light.range = 8; light.intensity = 1.5f;
                    light.color = new Color(1, .9f, .75f); light.shadows = LightShadows.None;
                }
            }
            else
            {
                Box("Stairwell roof", new Vector3(0, 5.65f, 28.92f), new Vector3(4.6f, .2f, 6.04f), wall);
                Box("Stairwell upper left", new Vector3(-2.15f, 4.775f, 28.92f), new Vector3(.3f, 1.55f, 5.84f), wall);
                Box("Stairwell upper right", new Vector3(2.15f, 4.775f, 28.92f), new Vector3(.3f, 1.55f, 5.84f), wall);
                Box("Stairwell front infill", new Vector3(0, 4.575f, 26), new Vector3(4.4f, 1.95f, .2f), wall);
            }
            Physics.SyncTransforms();
            Vector3 probe = sceneName == "Classroom" ? new Vector3(0, 2, 3) : new Vector3(0, 4.5f, 30.8f);
            if (!Physics.Raycast(probe, Vector3.up, 3)) throw new InvalidOperationException("Missing roof collision.");
            EditorSceneManager.SaveScene(scene);
        }
        Debug.Log("ROOFS_SAVED: classroom and staircase roof collisions verified.");
    }

    [MenuItem("Tools/The Other Side/Expand Prototype Spaces")]
    public static void Run()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play Mode first.");
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        wall = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Hallway/Plaster.mat");
        floor = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Hallway/Concrete.mat");
        trim = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Hallway/Frames.mat");
        foreach (string name in new[] { "Classroom", "Hallway", "Rooftop" })
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/" + name + "_Prototype.unity");
            if (GameObject.Find("Expanded prototype spaces") != null) continue;
            root = new GameObject("Expanded prototype spaces").transform;
            if (name == "Classroom") Classroom();
            if (name == "Hallway") Hallway();
            if (name == "Rooftop") Rooftop();
            Physics.SyncTransforms();
            EditorSceneManager.SaveScene(scene);
        }
        AssetDatabase.SaveAssets();
        EditorSceneManager.OpenScene("Assets/Scenes/Hallway_Prototype.unity");
        Debug.Log("SPACES_EXPANDED: classroom 12x20m, hallway maze, rooftop 32x36m. Existing travel anchors preserved.");
    }

    private static void Classroom()
    {
        foreach (var t in UnityEngine.Object.FindObjectsByType<Transform>())
        {
            if (t.name == "Floor") { t.position = new Vector3(0, -.15f, 3); t.localScale = new Vector3(12, .3f, 20); }
            else if (t.name == "Back wall" || t.name == "Blackboard" || t.name == "Other student") t.position += Vector3.forward * 6;
            else if (t.name == "Right wall") { t.position += Vector3.forward * 3; t.localScale = new Vector3(.2f, 4, 20); }
            else if (t.position.x < -5.7f && t.GetComponent<Renderer>() != null)
            {
                var p = t.position; p.z = p.z * 20f / 14f + 3; t.position = p;
                var s = t.localScale; s.z *= 20f / 14f; t.localScale = s;
            }
        }
        for (int row = 0; row < 2; row++)
        for (int col = 0; col < 3; col++)
        {
            Vector3 p = new Vector3((col - 1) * 3, .76f, 7 + row * 2.5f);
            Box("Additional desk", p, new Vector3(1.4f, .1f, .7f), trim);
            Box("Desk support", p + Vector3.down * .38f, new Vector3(.8f, .7f, .45f), wall);
        }
        Marker("Puzzle - classroom notebook follow-up", new Vector3(3, 0, 10));
    }

    private static void Hallway()
    {
        // Retain the previous authored layout for inspection and rollback.
        foreach (string name in new[] { "Upper landing", "Landing end wall", "Stair side left", "Stair side right" })
            GameObject.Find(name).transform.SetParent(root, true);
        GameObject.Find("Hallway - Structure").SetActive(false);
        GameObject.Find("Doors and windows").SetActive(false);
        var cells = new HashSet<Vector2Int>();
        for (int z = 0; z <= 6; z++) cells.Add(new Vector2Int(0, z));
        foreach (int z in new[] { 1, 3, 5 })
            for (int x = 1; x <= 6; x++) cells.Add(new Vector2Int(x, z));
        foreach (int x in new[] { 2, 6 })
            for (int z = 1; z <= 5; z++) cells.Add(new Vector2Int(x, z));
        for (int z = 3; z <= 6; z++) cells.Add(new Vector2Int(4, z));
        for (int x = 6; x <= 8; x++) cells.Add(new Vector2Int(x, 3));
        cells.Add(new Vector2Int(8, 4));
        cells.Add(new Vector2Int(8, 5));
        Vector2Int[] dirs = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
        foreach (var cell in cells.OrderBy(c => c.y).ThenBy(c => c.x))
        {
            var group = new GameObject("Module " + cell.x + "," + cell.y).transform;
            group.SetParent(root);
            Vector3 center = new Vector3(cell.x * 4, 0, cell.y * 4);
            Box("Floor", center + Vector3.down * .15f, new Vector3(4, .3f, 4), floor, group);
            Box("Ceiling", center + Vector3.up * 3.7f, new Vector3(4, .2f, 4), wall, group);
            foreach (var dir in dirs)
            {
                if (cells.Contains(cell + dir)) continue;
                if (cell == new Vector2Int(0, 6) && dir == Vector2Int.up) continue;
                Vector3 offset = new Vector3(dir.x, 0, dir.y) * 2;
                if (dir == Vector2Int.right && cell.x >= 6)
                {
                    var glass = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Hallway/WindowGlass.mat");
                    Box("Window sill wall", center + offset + Vector3.up * .5f, new Vector3(.2f, 1, 4), wall, group);
                    Box("Window header", center + offset + Vector3.up * 3.2f, new Vector3(.2f, .8f, 4), wall, group);
                    Box("Window glass", center + offset + Vector3.up * 1.9f, new Vector3(.04f, 1.8f, 4), glass, group);
                    Box("Window mullion", center + offset + Vector3.up * 1.9f, new Vector3(.12f, 1.8f, .08f), trim, group);
                    continue;
                }
                Box("Wall", center + offset + Vector3.up * 1.8f,
                    dir.x == 0 ? new Vector3(4.2f, 3.6f, .2f) : new Vector3(.2f, 3.6f, 4.2f), wall, group);
            }
            if ((cell.x + cell.y) % 2 == 0)
            {
                var lamp = new GameObject("Ceiling light").AddComponent<Light>();
                lamp.transform.SetParent(group); lamp.transform.position = center + Vector3.up * 3.1f;
                lamp.type = LightType.Point; lamp.range = 7; lamp.intensity = 1.5f; lamp.shadows = LightShadows.None;
                Box("Light fixture", center + Vector3.up * 3.5f, new Vector3(.3f, .08f, 1.2f), wall, group);
            }
        }
        // Break the direct route to the stairs. Both side loops remain traversable.
        Box("Closed school wing partition", new Vector3(0, 1.8f, 10), new Vector3(4, 3.6f, .25f), wall);
        Marker("Puzzle 01 - world shift gate", new Vector3(8, 0, 8));
        Marker("Puzzle 02 - echo object alcove", new Vector3(16, 0, 24));
        Marker("Puzzle 03 - memory dead end", new Vector3(32, 0, 20));
        Marker("Encounter staging", new Vector3(24, 0, 12));
        Box("Library landmark shelf", new Vector3(17.5f, 1, 24), new Vector3(.4f, 2, 2), trim);
        Box("Science landmark cabinet", new Vector3(33.5f, 1, 20), new Vector3(.4f, 2, 2), trim);
        Physics.SyncTransforms();
        foreach (var cell in cells)
        {
            if (!Physics.Raycast(new Vector3(cell.x * 4, 1, cell.y * 4), Vector3.down, 2))
                throw new InvalidOperationException("Missing maze floor " + cell);
        }
        var reached = new HashSet<Vector2Int> { Vector2Int.zero };
        var pending = new Queue<Vector2Int>(); pending.Enqueue(Vector2Int.zero);
        while (pending.Count > 0)
        {
            var cell = pending.Dequeue();
            foreach (var dir in dirs)
            {
                var next = cell + dir;
                if (!cells.Contains(next) || reached.Contains(next)) continue;
                Vector3 start = new Vector3(cell.x * 4, .9f, cell.y * 4);
                Vector3 direction = new Vector3(dir.x, 0, dir.y);
                if (Physics.CapsuleCast(start + Vector3.up * .55f, start - Vector3.up * .55f,
                    .3f, direction, 4, ~0, QueryTriggerInteraction.Ignore)) continue;
                reached.Add(next); pending.Enqueue(next);
            }
        }
        if (reached.Count != cells.Count)
            throw new InvalidOperationException("Maze has unreachable modules: " + reached.Count + "/" + cells.Count);
        Debug.Log("MAZE_VALIDATED: " + reached.Count + " modules reachable using player-sized capsule casts.");
    }

    private static void Rooftop()
    {
        var slab = GameObject.Find("Roof slab").transform; slab.localScale = new Vector3(32, .4f, 36);
        foreach (string side in new[] { "west", "east" })
        {
            var t = GameObject.Find("Parapet " + side).transform;
            t.position = new Vector3(side == "west" ? -15.85f : 15.85f, .65f, 0);
            t.localScale = new Vector3(.3f, 1.3f, 36);
        }
        foreach (string side in new[] { "north", "south" })
        {
            var t = GameObject.Find("Parapet " + side).transform;
            t.position = new Vector3(0, .65f, side == "north" ? 17.85f : -17.85f);
            t.localScale = new Vector3(32, 1.3f, .3f);
        }
        GameObject.Find("Rooftop Greybox - 20 x 24 metres").name = "Rooftop Greybox - 32 x 36 metres";
        Marker("Puzzle - rooftop final message", new Vector3(0, 0, 14));
        Marker("Memory - water tank", new Vector3(10, 0, 10));
        Box("Meeting bench seat", new Vector3(0, .45f, 14), new Vector3(2.4f, .12f, .6f), trim);
        Box("Meeting bench support", new Vector3(0, .2f, 14), new Vector3(2, .4f, .4f), wall);
    }

    private static void Marker(string name, Vector3 position)
    {
        var marker = new GameObject(name); marker.transform.SetParent(root); marker.transform.position = position;
    }
    private static void Box(string name, Vector3 position, Vector3 scale, Material material, Transform parent = null)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube); obj.name = name;
        obj.transform.SetParent(parent == null ? root : parent); obj.transform.position = position;
        obj.transform.localScale = scale; obj.GetComponent<Renderer>().sharedMaterial = material;
    }
}
