using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class CreateHallwayLightPuzzle
{
    [MenuItem("Tools/The Other Side/Add Hallway Light Puzzle")]
    public static void Run()
    {
        if (EditorApplication.isPlaying) throw new System.InvalidOperationException("Stop Play Mode first.");
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Hallway_Prototype.unity");
        if (Object.FindFirstObjectByType<HallwayLightPuzzle>() != null) return;
        var gate = GameObject.Find("Closed school wing partition").GetComponent<BoxCollider>();
        var root = new GameObject("Hallway light puzzle").transform;
        if (!AssetDatabase.IsValidFolder("Assets/Materials/HallwayPuzzle"))
            AssetDatabase.CreateFolder("Assets/Materials", "HallwayPuzzle");
        var metal = Material("Metal", new Color(.19f, .24f, .29f));
        var paper = Material("Paper", new Color(1, .86f, .52f));
        var blue = Material("Symbols", new Color(.35f, .8f, 1));
        var preview = Material("GatePreview", new Color(.3f, .7f, 1, .16f));
        preview.SetFloat("_Surface", 1);
        preview.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
        preview.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
        preview.SetFloat("_ZWrite", 0);
        preview.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        preview.SetOverrideTag("RenderType", "Transparent");
        preview.renderQueue = (int)RenderQueue.Transparent;
        gate.GetComponent<Renderer>().sharedMaterial = metal;
        var ghost = Box(root, "Gate preview", gate.transform.position, gate.transform.lossyScale, preview, false).GetComponent<Renderer>();
        ghost.shadowCastingMode = ShadowCastingMode.Off;
        ghost.enabled = false;
        // Frames remain in both worlds. The centre stays wide enough for the player capsule.
        foreach (float x in new[] { -1.85f, 1.85f })
            Box(root, "Gate frame", new Vector3(x, 1.8f, 10), new Vector3(.15f, 3.6f, .35f), blue, true);
        var entry = Note(root, "Entry note", new Vector3(1.25f, 0, 8), metal, paper);
        var request = Note(root, "Request notebook", new Vector3(1.25f, 0, 12), metal, paper);
        var buttons = new Collider[3];
        var lamps = new Light[3];
        for (int i = 0; i < 3; i++)
        {
            float z = 15.2f + i * .8f;
            buttons[i] = Box(root, "Puzzle switch " + i, new Vector3(1.78f, 1.35f, z), new Vector3(.2f, .5f, .55f), metal, true).GetComponent<Collider>();
            var symbol = GameObject.CreatePrimitive(i == 0 ? PrimitiveType.Sphere : PrimitiveType.Cube);
            symbol.name = "Switch symbol";
            symbol.transform.SetParent(root);
            symbol.transform.position = new Vector3(1.66f, 1.35f, z);
            symbol.transform.localScale = new Vector3(.04f, .22f, .22f);
            if (i == 2) symbol.transform.rotation = Quaternion.Euler(45, 0, 0);
            Object.DestroyImmediate(symbol.GetComponent<Collider>());
            symbol.GetComponent<Renderer>().sharedMaterial = blue;
            lamps[i] = Lamp(root, "Puzzle indicator " + i, new Vector3(1.45f, 2.4f, z), new Color(1, .7f, .3f), .25f, 3);
            Box(root, "Indicator fixture", new Vector3(1.8f, 2.4f, z), new Vector3(.2f, .18f, .35f), paper, false);
        }
        var route = new Light[3];
        for (int i = 0; i < route.Length; i++)
        {
            route[i] = Lamp(root, "Stair route light " + i, new Vector3(0, 2.8f, 18 + i * 3.5f), new Color(.5f, 1, .7f), 2, 5);
            route[i].enabled = false;
        }
        root.gameObject.AddComponent<HallwayLightPuzzle>().Configure(gate, ghost, entry, request, buttons, lamps, route);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("HALLWAY_PUZZLE_SAVED: existing gate, two notes, three switches, three route lights.");
    }

    private static Material Material(string name, Color color)
    {
        string path = "Assets/Materials/HallwayPuzzle/" + name + ".mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null) return material;
        material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = color };
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static GameObject Box(Transform parent, string name, Vector3 position, Vector3 size, Material material, bool solid)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.position = position;
        obj.transform.localScale = size;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        if (!solid) Object.DestroyImmediate(obj.GetComponent<Collider>());
        return obj;
    }

    private static Collider Note(Transform root, string name, Vector3 position, Material metal, Material paper)
    {
        Box(root, name + " stand", position + Vector3.up * .5f, new Vector3(.4f, 1, .4f), metal, true);
        var note = Box(root, name, position + Vector3.up * 1.1f, new Vector3(.6f, .12f, .5f), paper, true);
        note.transform.rotation = Quaternion.Euler(12, -30, 0);
        return note.GetComponent<Collider>();
    }

    private static Light Lamp(Transform parent, string name, Vector3 position, Color color, float intensity, float range)
    {
        var lamp = new GameObject(name).AddComponent<Light>();
        lamp.transform.SetParent(parent);
        lamp.transform.position = position;
        lamp.type = LightType.Point;
        lamp.color = color;
        lamp.intensity = intensity;
        lamp.range = range;
        lamp.shadows = LightShadows.None;
        return lamp;
    }
}
