using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Installs the same doorway interaction in both prototype scenes.
public class PrototypeDoorTravel : MonoBehaviour
{
    private const string Classroom = "Classroom_Prototype";
    private const string Hallway = "Hallway_Prototype";
    private static string arrivalScene;
    private Camera playerCamera;
    private Collider[] doorways;
    private Collider focusedDoor;
    private static float hallwayReturnZ = 1;
    private string destination;
    private bool loading;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSession()
    {
        arrivalScene = null;
        hallwayReturnZ = 1;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitializeCurrentScene()
    {
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != Classroom && scene.name != Hallway) return;
        var camera = Camera.main;
        if (camera == null) return;
        var player = camera.GetComponentInParent<CharacterController>();
        if (player == null) return;
        if (player.GetComponent<PrototypeDoorTravel>() != null) return;
        var travel = player.gameObject.AddComponent<PrototypeDoorTravel>();
        travel.playerCamera = camera;
        bool inClassroom = scene.name == Classroom;
        travel.destination = inClassroom ? Hallway : Classroom;
        travel.doorways = inClassroom
            ? new[] { GameObject.Find("Door").GetComponent<Collider>() }
            : new[]
            {
                GameObject.Find("Classroom 201").GetComponent<Collider>(),
                GameObject.Find("Classroom 202").GetComponent<Collider>(),
                GameObject.Find("Classroom 203").GetComponent<Collider>(),
                GameObject.Find("Classroom 204").GetComponent<Collider>()
            };
        if (arrivalScene != scene.name) return;

        player.enabled = false;
        player.transform.SetPositionAndRotation(
            inClassroom ? new Vector3(4.6f, .05f, -5.5f) : new Vector3(-.8f, .05f, hallwayReturnZ),
            Quaternion.Euler(0, inClassroom ? 0 : 90, 0));
        player.enabled = true;
        arrivalScene = null;
    }

    private bool FacingDoor()
    {
        focusedDoor = null;
        if (loading || SceneFadeTransition.IsTransitioning ||
            !PrototypeThirdPerson.InteractionRay(playerCamera, out var hit))
            return false;
        if (System.Array.IndexOf(doorways, hit.collider) < 0) return false;
        focusedDoor = hit.collider;
        return true;
    }

    private void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            TryTravel();
    }

    public bool TryTravel()
    {
        if (!FacingDoor()) return false;
        if (!Application.CanStreamedLevelBeLoaded(destination))
        {
            Debug.LogError("Add " + destination + " to the enabled Build Settings scenes.");
            return false;
        }
        if (destination == Classroom) hallwayReturnZ = focusedDoor.transform.position.z;
        loading = true;
        arrivalScene = destination;
        SceneManager.LoadScene(destination);
        return true;
    }

    private void OnGUI()
    {
        if (Cursor.lockState != CursorLockMode.Locked || !FacingDoor()) return;
        GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 45, 300, 42),
            destination == Hallway ? "E - Go to hallway" : "E - Enter " + focusedDoor.name);
    }
}
