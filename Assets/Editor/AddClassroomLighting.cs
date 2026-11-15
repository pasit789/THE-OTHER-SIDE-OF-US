using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AddClassroomLighting
{
    [MenuItem("Tools/The Other Side/Add Classroom Ceiling Lights")]
    public static void Run()
    {
        if (EditorApplication.isPlaying) throw new System.InvalidOperationException("Stop Play Mode first.");
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Classroom_Prototype.unity");
        if (GameObject.Find("Classroom ceiling lights") != null) return;
        var root = new GameObject("Classroom ceiling lights").transform;
        var housing = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Hallway/Frames.mat");
        const string panelPath = "Assets/Materials/ClassroomLightPanel.mat";
        var panel = AssetDatabase.LoadAssetAtPath<Material>(panelPath);
        if (panel == null)
        {
            panel = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            panel.color = new Color(1, .95f, .84f);
            panel.EnableKeyword("_EMISSION");
            panel.SetColor("_EmissionColor", new Color(1, .9f, .72f) * 2);
            AssetDatabase.CreateAsset(panel, panelPath);
        }
        for (int row = 0; row < 4; row++)
        for (int col = 0; col < 2; col++)
        {
            Vector3 position = new Vector3(col == 0 ? -3 : 3, 3.9f, -4 + row * 5);
            var fixture = new GameObject("Ceiling fixture " + (row * 2 + col + 1)).transform;
            fixture.SetParent(root); fixture.position = position;
            Box(fixture, "Housing", Vector3.zero, new Vector3(.65f, .14f, 1.3f), housing);
            Box(fixture, "Luminous panel", Vector3.down * .08f, new Vector3(.55f, .035f, 1.2f), panel);
            var light = new GameObject("Classroom ceiling light").AddComponent<Light>();
            light.transform.SetParent(fixture, false);
            light.transform.localPosition = Vector3.down * .25f;
            light.transform.localRotation = Quaternion.Euler(90, 0, 0);
            light.type = LightType.Spot;
            light.color = new Color(1, .92f, .8f);
            light.intensity = 4;
            light.range = 8;
            light.spotAngle = 110;
            light.innerSpotAngle = 70;
            light.shadows = row == 1 ? LightShadows.Soft : LightShadows.None;
            light.shadowBias = .03f;
            light.shadowNormalBias = .2f;
        }
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        if (root.GetComponentsInChildren<Light>().Length != 8) throw new System.Exception("Expected eight ceiling lights.");
        Debug.Log("CLASSROOM_LIGHTS_SAVED: eight fixtures and spotlights, two shadow casters.");
    }
    private static void Box(Transform parent, string name, Vector3 position, Vector3 size, Material material)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name; obj.transform.SetParent(parent, false);
        obj.transform.localPosition = position; obj.transform.localScale = size;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        Object.DestroyImmediate(obj.GetComponent<Collider>());
    }
}
