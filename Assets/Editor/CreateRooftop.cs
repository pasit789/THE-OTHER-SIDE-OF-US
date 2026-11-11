using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class CreateRooftop
{
    private const string RooftopPath = "Assets/Scenes/Rooftop_Prototype.unity";
    private const string HallwayPath = "Assets/Scenes/Hallway_Prototype.unity";

    [MenuItem("Tools/The Other Side/Open Rooftop Prototype")]
    public static void Open()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(RooftopPath) == null) Create();
        else EditorSceneManager.OpenScene(RooftopPath);
    }

    public static void Create()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(RooftopPath) != null)
            throw new System.InvalidOperationException("Rooftop already exists.");
        EditorSceneManager.OpenScene(HallwayPath);
        AddExit("Rooftop stair exit", new Vector3(0, 3.3f, 31), new Vector3(3.8f, 2.2f, 1),
            "Rooftop_Prototype", new Vector3(0, .05f, -6), 0);
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var floor = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Hallway/Concrete.mat");
        var wall = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Hallway/Plaster.mat");
        var dark = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Hallway/Frames.mat");
        var roof = new GameObject("Rooftop Greybox - 20 x 24 metres").transform;
        Box(roof, "Roof slab", new Vector3(0, -.2f, 0), new Vector3(20, .4f, 24), floor);
        Box(roof, "Parapet west", new Vector3(-9.85f, .65f, 0), new Vector3(.3f, 1.3f, 24), wall);
        Box(roof, "Parapet east", new Vector3(9.85f, .65f, 0), new Vector3(.3f, 1.3f, 24), wall);
        Box(roof, "Parapet north", new Vector3(0, .65f, 11.85f), new Vector3(20, 1.3f, .3f), wall);
        Box(roof, "Parapet south", new Vector3(0, .65f, -11.85f), new Vector3(20, 1.3f, .3f), wall);

        var access = new GameObject("Stairwell access").transform;
        Box(access, "Left wall", new Vector3(-2.5f, 1.6f, -9.7f), new Vector3(.25f, 3.2f, 4), wall);
        Box(access, "Right wall", new Vector3(2.5f, 1.6f, -9.7f), new Vector3(.25f, 3.2f, 4), wall);
        Box(access, "Rear wall", new Vector3(0, 1.6f, -11.6f), new Vector3(5, 3.2f, .25f), wall);
        Box(access, "Roof", new Vector3(0, 3.3f, -9.7f), new Vector3(5.3f, .2f, 4.2f), floor);
        Box(access, "Doorway left", new Vector3(-1.75f, 1.6f, -7.7f), new Vector3(1.5f, 3.2f, .25f), wall);
        Box(access, "Doorway right", new Vector3(1.75f, 1.6f, -7.7f), new Vector3(1.5f, 3.2f, .25f), wall);
        Box(access, "Door lintel", new Vector3(0, 2.9f, -7.7f), new Vector3(2, .6f, .25f), wall);
        AddExit("Return to hallway", new Vector3(0, 1.2f, -9), new Vector3(1.8f, 2.4f, 1),
            "Hallway_Prototype", new Vector3(0, 2.05f, 29.2f), 180);

        var props = new GameObject("Rooftop props").transform;
        Box(props, "Water tank base", new Vector3(6, .3f, 7), new Vector3(3.5f, .6f, 3.5f), floor);
        var tank = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tank.name = "Water tank";
        tank.transform.SetParent(props);
        tank.transform.position = new Vector3(6, 2, 7);
        tank.transform.localScale = new Vector3(2.8f, 1.4f, 2.8f);
        tank.GetComponent<Renderer>().sharedMaterial = dark;
        for (int i = 0; i < 3; i++)
            Box(props, "Vent " + (i + 1), new Vector3(-6, .6f, i * 3), new Vector3(1.7f, 1.2f, 1.3f), dark);
        Box(props, "Bench", new Vector3(0, .45f, 8), new Vector3(2.4f, .9f, .65f), wall);

        var player = new GameObject("Player");
        player.transform.position = new Vector3(0, .05f, -6);
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
        sun.intensity = 1.3f;
        sun.transform.rotation = Quaternion.Euler(35, -40, 0);
        sun.shadows = LightShadows.Soft;
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(.6f, .63f, .68f);
        EditorSceneManager.SaveScene(scene, RooftopPath);
        EditorBuildSettings.scenes = EditorBuildSettings.scenes
            .Where(entry => entry.path != RooftopPath)
            .Append(new EditorBuildSettingsScene(RooftopPath, true)).ToArray();
        AssetDatabase.SaveAssets();
        Debug.Log("ROOFTOP_CREATED: rooftop and hallway triggers saved.");
    }

    private static void AddExit(string name, Vector3 position, Vector3 size, string scene, Vector3 arrival, float yaw)
    {
        var exit = new GameObject(name);
        exit.transform.position = position;
        var trigger = exit.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = size;
        var travel = exit.AddComponent<WalkSceneExit>();
        travel.destinationScene = scene;
        travel.arrivalPosition = arrival;
        travel.arrivalYaw = yaw;
    }

    private static void Box(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
    {
        var item = GameObject.CreatePrimitive(PrimitiveType.Cube);
        item.name = name;
        item.transform.SetParent(parent);
        item.transform.position = position;
        item.transform.localScale = scale;
        item.GetComponent<Renderer>().sharedMaterial = material;
    }
}
