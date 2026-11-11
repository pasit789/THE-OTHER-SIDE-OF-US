using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class CreateHallway
{
    private const string ScenePath = "Assets/Scenes/Hallway_Prototype.unity";
    private const string MaterialFolder = "Assets/Materials/Hallway";
    private static Transform structure;
    private static Material wall;
    private static Material dark;
    private static Material floor;

    [MenuItem("Tools/The Other Side/Open Hallway Prototype")]
    public static void Open()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null)
        {
            EditorSceneManager.OpenScene(ScenePath);
            return;
        }
        Create();
    }

    public static void Create()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null)
            throw new System.InvalidOperationException("Hallway scene already exists. Open it through the Tools menu.");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        System.IO.Directory.CreateDirectory(MaterialFolder);
        AssetDatabase.Refresh();
        wall = Material("Plaster", new Color(0.72f, 0.73f, 0.72f));
        dark = Material("Frames", new Color(0.26f, 0.29f, 0.30f));
        floor = Material("Concrete", new Color(0.45f, 0.47f, 0.48f));
        var pale = Material("Ceiling", new Color(0.85f, 0.86f, 0.85f));
        structure = new GameObject("Hallway - Structure").transform;

        Box("Floor - 4 x 28 metres", new Vector3(0, -.15f, 12), new Vector3(4.4f, .3f, 28), floor);
        Box("Ceiling", new Vector3(0, 3.65f, 12), new Vector3(4.4f, .2f, 28), pale);
        Box("Classroom wall", new Vector3(-2.15f, 1.8f, 12), new Vector3(.3f, 3.6f, 28), wall);
        Box("Entrance wall", new Vector3(0, 1.8f, -2), new Vector3(4.4f, 3.6f, .25f), wall);
        Box("Window sill wall", new Vector3(2.15f, .5f, 12), new Vector3(.3f, 1, 28), wall);
        Box("Window header", new Vector3(2.15f, 3.3f, 12), new Vector3(.3f, .6f, 28), wall);
        Box("Skirting left", new Vector3(-1.96f, .12f, 12), new Vector3(.08f, .24f, 28), dark);

        for (int bay = 0; bay < 7; bay++)
        {
            float z = bay * 4;
            Box("Window pier " + bay, new Vector3(2.15f, 2, z - 2), new Vector3(.35f, 2, .25f), wall);
            Box("Window mullion " + bay, new Vector3(2.15f, 2, z), new Vector3(.08f, 2, .08f), dark);
            Box("Window crossbar " + bay, new Vector3(2.15f, 2.1f, z), new Vector3(.08f, .08f, 3.8f), dark);
            var barrier = new GameObject("Window collision " + bay);
            barrier.transform.SetParent(structure);
            barrier.transform.position = new Vector3(2.15f, 2, z);
            barrier.AddComponent<BoxCollider>().size = new Vector3(.1f, 2, 3.8f);
            Box("Ceiling beam " + bay, new Vector3(0, 3.4f, z), new Vector3(4, .25f, .25f), pale);
            Box("Light fixture " + bay, new Vector3(0, 3.48f, z + 1), new Vector3(.35f, .08f, 1.1f), pale);
            var lamp = new GameObject("Corridor light " + bay).AddComponent<Light>();
            lamp.transform.SetParent(structure);
            lamp.transform.position = new Vector3(0, 3.1f, z + 1);
            lamp.type = LightType.Point;
            lamp.range = 7;
            lamp.intensity = 1.4f;
            lamp.shadows = LightShadows.None;
        }

        var doors = new GameObject("Classroom doors - blockouts").transform;
        for (int room = 0; room < 4; room++)
        {
            float z = 1 + room * 6;
            var door = Box("Classroom " + (201 + room), new Vector3(-1.94f, 1.25f, z), new Vector3(.12f, 2.5f, 1.35f), dark);
            door.transform.SetParent(doors);
            Box("Door frame top", new Vector3(-1.86f, 2.55f, z), new Vector3(.16f, .12f, 1.6f), pale);
            Box("Door handle", new Vector3(-1.8f, 1.1f, z + .45f), new Vector3(.12f, .08f, .25f), pale);
            Box("Room sign", new Vector3(-1.9f, 2.95f, z), new Vector3(.08f, .35f, .85f), dark);
        }
        Box("Noticeboard", new Vector3(-1.9f, 1.8f, 23), new Vector3(.15f, 1.3f, 2), dark);
        Box("Bench seat", new Vector3(-1.45f, .48f, 11), new Vector3(.6f, .12f, 2), dark);
        Box("Bench base", new Vector3(-1.45f, .22f, 11), new Vector3(.45f, .44f, 1.7f), wall);

        var stairs = new GameObject("Staircase - 12 steps").transform;
        for (int step = 0; step < 12; step++)
        {
            float height = (step + 1) * .18f;
            Box("Step " + (step + 1), new Vector3(0, height / 2, 26 + step * .32f + .16f),
                new Vector3(4, height, .32f), floor).transform.SetParent(stairs);
        }
        Box("Upper landing", new Vector3(0, 2.01f, 30.84f), new Vector3(4.4f, .3f, 2), floor);
        Box("Landing end wall", new Vector3(0, 3.86f, 31.84f), new Vector3(4.4f, 3.4f, .25f), wall);
        Box("Stair side left", new Vector3(-2.15f, 2, 28.92f), new Vector3(.3f, 4, 5.84f), wall);
        Box("Stair side right", new Vector3(2.15f, 2, 28.92f), new Vector3(.3f, 4, 5.84f), wall);

        var player = new GameObject("Player");
        player.transform.position = new Vector3(0, .05f, 0);
        var controller = player.AddComponent<CharacterController>();
        controller.height = 1.8f;
        controller.center = new Vector3(0, .9f, 0);
        controller.radius = .3f;
        controller.stepOffset = .3f;
        var camera = new GameObject("Player Camera").AddComponent<Camera>();
        camera.transform.SetParent(player.transform, false);
        camera.transform.localPosition = new Vector3(0, 1.65f, 0);
        camera.tag = "MainCamera";
        camera.nearClipPlane = .05f;
        camera.gameObject.AddComponent<AudioListener>();
        player.AddComponent<HallwayPlayer>();
        var sun = new GameObject("Sun").AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.2f;
        sun.transform.rotation = Quaternion.Euler(38, -65, 0);
        sun.shadows = LightShadows.Soft;
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(.55f, .57f, .6f);

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = EditorBuildSettings.scenes
            .Concat(new[] { new EditorBuildSettingsScene(ScenePath, true) }).ToArray();
        AssetDatabase.SaveAssets();
        if (SceneView.lastActiveSceneView != null)
            SceneView.lastActiveSceneView.LookAt(new Vector3(0, 1.7f, 10), Quaternion.Euler(25, -35, 0), 24);
        Physics.SyncTransforms();
        if (!Physics.Raycast(player.transform.position + Vector3.up, Vector3.down, 2))
            throw new System.InvalidOperationException("Spawn has no floor.");
        Debug.Log("HALLWAY_CREATED: saved scene, materials, player, floor collision and staircase.");
    }

    private static Material Material(string name, Color color)
    {
        string path = MaterialFolder + "/" + name + ".mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null) return material;
        material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = color };
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static GameObject Box(string name, Vector3 position, Vector3 scale, Material material)
    {
        var item = GameObject.CreatePrimitive(PrimitiveType.Cube);
        item.name = name;
        item.transform.SetParent(structure);
        item.transform.position = position;
        item.transform.localScale = scale;
        item.GetComponent<Renderer>().sharedMaterial = material;
        return item;
    }
}
