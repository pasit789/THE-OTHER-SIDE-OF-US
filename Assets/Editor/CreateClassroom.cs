using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CreateClassroom
{
    public static void OpenPrototype()
    {
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        new GameObject("Classroom Prototype").AddComponent<ClassroomPrototype>().BuildClassroom();
        EditorSceneManager.SaveScene(scene,"Assets/Scenes/Classroom_Prototype.unity");
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/Classroom_Prototype.unity",true)};
        AssetDatabase.SaveAssets();
    }

    public static void Run()
    {
        OpenPrototype();
        var result=BuildPipeline.BuildPlayer(EditorBuildSettings.scenes,"Builds/ClassroomPrototype/ClassroomPrototype.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
        Debug.Log("PROTOTYPE_BUILD "+result.summary.result);
        EditorApplication.Exit(result.summary.result==UnityEditor.Build.Reporting.BuildResult.Succeeded?0:1);
    }
}
