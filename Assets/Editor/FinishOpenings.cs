using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class FinishOpenings
{
    private static Material frame;
    private static Material glass;
    private static Material plaster;
    private static Transform details;

    public static void Run()
    {
        frame = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Hallway/Frames.mat");
        plaster = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Hallway/Plaster.mat");
        const string glassPath = "Assets/Materials/Hallway/WindowGlass.mat";
        glass = AssetDatabase.LoadAssetAtPath<Material>(glassPath);
        if (glass == null)
        {
            glass = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            glass.color = new Color(.55f, .7f, .75f, .22f);
            glass.SetFloat("_Surface", 1);
            glass.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            glass.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            glass.SetFloat("_ZWrite", 0);
            glass.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            glass.SetOverrideTag("RenderType", "Transparent");
            glass.renderQueue = 3000;
            AssetDatabase.CreateAsset(glass, glassPath);
        }
        foreach (string sceneName in new[] { "Classroom", "Hallway", "Rooftop" })
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/" + sceneName + "_Prototype.unity");
            if (GameObject.Find("Doors and windows") != null) continue;
            details = new GameObject("Doors and windows").transform;
            if (sceneName == "Classroom")
            {
                WindowWall(GameObject.Find("Left wall"), 4, 14, 4, -6, 0, 1, 3.2f);
                DoorFrame(new Vector3(4.6f, 0, -6.65f), 1.5f, 2.5f);
                Box("Door handle", new Vector3(5.1f, 1.05f, -6.63f), new Vector3(.2f, .06f, .1f), frame, false);
                Box("Door window", new Vector3(4.6f, 1.85f, -6.63f), new Vector3(.75f, .65f, .04f), glass, false);
            }
            else if (sceneName == "Hallway")
            {
                for (int room = 0; room < 4; room++)
                {
                    float z = 1 + room * 6;
                    Box("Door jamb " + (201 + room) + " A", new Vector3(-1.84f, 1.25f, z - .73f), new Vector3(.12f, 2.5f, .1f), frame, false);
                    Box("Door jamb " + (201 + room) + " B", new Vector3(-1.84f, 1.25f, z + .73f), new Vector3(.12f, 2.5f, .1f), frame, false);
                    Box("Door window " + (201 + room), new Vector3(-1.87f, 1.9f, z), new Vector3(.02f, .6f, .7f), glass, false);
                }
                for (int bay = 0; bay < 7; bay++)
                    Box("Window glass " + bay, new Vector3(2.15f, 2, bay * 4), new Vector3(.02f, 1.95f, 3.75f), glass, false);
                DoorFrame(new Vector3(0, 2.16f, 30.6f), 2, 2.6f);
                Box("Roof door - open leaf", new Vector3(1.1f, 3.46f, 31.1f), new Vector3(.12f, 2.6f, 1), frame, true);
            }
            else
            {
                WindowWall(GameObject.Find("Left wall"), 2, 4, 3.2f, -2.5f, -9.7f, 1.1f, 2.6f);
                WindowWall(GameObject.Find("Right wall"), 2, 4, 3.2f, 2.5f, -9.7f, 1.1f, 2.6f);
                DoorFrame(new Vector3(0, 0, -7.55f), 2, 2.6f);
                Box("Stairwell door - open leaf", new Vector3(1.08f, 1.3f, -8.2f), new Vector3(.12f, 2.6f, 1.3f), frame, true);
            }
            EditorSceneManager.SaveScene(scene);
        }
        AssetDatabase.SaveAssets();
        Debug.Log("OPENINGS_SAVED: all three scenes.");
    }

    private static void WindowWall(GameObject wall, int count, float length, float height, float x, float z, float sill, float top)
    {
        wall.transform.position = new Vector3(x, sill / 2, z);
        wall.transform.localScale = new Vector3(.25f, sill, length);
        Box("Window header", new Vector3(x, (height + top) / 2, z), new Vector3(.25f, height - top, length), plaster, true);
        float width = length / count;
        for (int index = 0; index <= count; index++)
            Box("Window pier", new Vector3(x, (top + sill) / 2, z - length / 2 + index * width),
                new Vector3(.25f, top - sill, .2f), plaster, true);
        for (int index = 0; index < count; index++)
        {
            float center = z - length / 2 + (index + .5f) * width;
            Box("Glass pane", new Vector3(x, (top + sill) / 2, center), new Vector3(.03f, top - sill, width - .2f), glass, true);
            Box("Window mullion", new Vector3(x, (top + sill) / 2, center), new Vector3(.1f, top - sill, .06f), frame, false);
            Box("Window sill", new Vector3(x, sill, center), new Vector3(.38f, .08f, width), frame, false);
            Box("Window top frame", new Vector3(x, top, center), new Vector3(.15f, .08f, width), frame, false);
        }
    }

    private static void DoorFrame(Vector3 bottom, float width, float height)
    {
        Box("Door jamb left", bottom + new Vector3(-width / 2, height / 2, 0), new Vector3(.12f, height, .15f), frame, false);
        Box("Door jamb right", bottom + new Vector3(width / 2, height / 2, 0), new Vector3(.12f, height, .15f), frame, false);
        Box("Door frame top", bottom + new Vector3(0, height, 0), new Vector3(width + .12f, .12f, .15f), frame, false);
    }

    private static void Box(string name, Vector3 position, Vector3 scale, Material material, bool collision)
    {
        var item = GameObject.CreatePrimitive(PrimitiveType.Cube);
        item.name = name;
        item.transform.SetParent(details);
        item.transform.position = position;
        item.transform.localScale = scale;
        item.GetComponent<Renderer>().sharedMaterial = material;
        if (!collision) Object.DestroyImmediate(item.GetComponent<Collider>());
    }
}
