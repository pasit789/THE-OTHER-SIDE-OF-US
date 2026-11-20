using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class HallwayPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    private CharacterController controller;
    private PrototypeThirdPerson movement;
    private bool otherWorld;
    private HallwayLightPuzzle puzzle;
    private DeskRepair repair;
    public bool IsOtherWorld => otherWorld;
    private Camera view;
    private Collider notebook;
    private bool lookingAtNotebook;
    private string notebookChoice;
    private GUIStyle notebookText, notebookButton;

    private void PlaceNotebook()
    {
        string surfaceName = gameObject.scene.name == "Hallway_Prototype" ? "Reading table" : "Meeting bench seat";
        var surface = GameObject.Find(surfaceName);
        if (!surface) return;
        var bounds = surface.GetComponent<Renderer>().bounds;
        var book = GameObject.CreatePrimitive(PrimitiveType.Cube);
        book.name = "World notebook";
        book.transform.position = new Vector3(bounds.center.x, bounds.max.y + .025f, bounds.center.z);
        book.transform.localScale = new Vector3(.38f, .05f, .29f);
        book.GetComponent<Renderer>().material.color = new Color(.85f, .65f, .28f);
        notebook = book.GetComponent<Collider>();
    }

    private void CloseNotebook(bool cross)
    {
        if (cross && !TrySwitchWorld()) return;
        notebookChoice = null;
        SetCursor(true);
    }

    private Light[] sceneLights;
    private Color[] originalLightColors;
    private float[] originalLightIntensities;
    private Color originalAmbientLight;
    private UnityEngine.Rendering.AmbientMode originalAmbientMode;
    private Color originalBackground;
    private CameraClearFlags originalClearFlags;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>().transform;
    }

    private void Start()
    {
        view = playerCamera.GetComponent<Camera>();
        movement = PrototypeThirdPerson.Attach(controller, view);
        sceneLights = FindObjectsByType<Light>();
        originalLightColors = new Color[sceneLights.Length];
        originalLightIntensities = new float[sceneLights.Length];
        for (int i = 0; i < sceneLights.Length; i++)
        {
            originalLightColors[i] = sceneLights[i].color;
            originalLightIntensities[i] = sceneLights[i].intensity;
        }
        originalAmbientLight = RenderSettings.ambientLight;
        originalAmbientMode = RenderSettings.ambientMode;
        originalBackground = view.backgroundColor;
        originalClearFlags = view.clearFlags;
        otherWorld = PrototypeProgress.OtherWorld;
        puzzle = FindFirstObjectByType<HallwayLightPuzzle>();
        ApplyWorld();
        if (puzzle != null) puzzle.Initialize(this);
        PlaceNotebook();
        SetCursor(true);
        if (gameObject.scene.name == "Hallway_Prototype") {
            repair = gameObject.AddComponent<DeskRepair>();
            repair.Setup(view);
            HallwayWanderer.SpawnCrowd(this);
        }
    }

    private void ApplyWorld()
    {
        for (int i = 0; i < sceneLights.Length; i++)
        {
            if (sceneLights[i] == null) continue;
            sceneLights[i].color = otherWorld ? new Color(.45f, .6f, 1f) : originalLightColors[i];
            sceneLights[i].intensity = originalLightIntensities[i] * (otherWorld ? .45f : 1f);
        }
        RenderSettings.ambientMode = otherWorld ? UnityEngine.Rendering.AmbientMode.Flat : originalAmbientMode;
        RenderSettings.ambientLight = otherWorld ? new Color(.15f, .2f, .4f) : originalAmbientLight;
        view.clearFlags = otherWorld ? CameraClearFlags.SolidColor : originalClearFlags;
        view.backgroundColor = otherWorld ? new Color(.05f, .08f, .2f) : originalBackground;
        if (puzzle != null) puzzle.RefreshWorld();
        if (repair != null) repair.Refresh();
    }

    public bool TrySwitchWorld()
    {
        if (SceneFadeTransition.IsTransitioning) return false;
        bool target = !otherWorld;
        if (repair != null && repair.Open) return false;
        if (puzzle != null && !puzzle.CanSwitchTo(target, controller))
        {
            repair?.WarnGateOverlap();
            return false;
        }
        otherWorld = target;
        PrototypeProgress.OtherWorld = target;
        ApplyWorld();
        return true;
    }

    private void Update()
    {
        if (SceneFadeTransition.IsTransitioning) return;
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        if (notebookChoice != null)
        {
            if (keyboard.digit1Key.wasPressedThisFrame) CloseNotebook(true);
            else if (keyboard.digit2Key.wasPressedThisFrame || keyboard.escapeKey.wasPressedThisFrame) CloseNotebook(false);
            return;
        }
        lookingAtNotebook = Cursor.lockState == CursorLockMode.Locked && !(repair != null && repair.Open)
            && PrototypeThirdPerson.InteractionRay(view, out var hit) && hit.collider == notebook;
        if (lookingAtNotebook && keyboard.eKey.wasPressedThisFrame)
        {
            notebookChoice = PrototypeProgress.OpenNotebook();
            SetCursor(false);
            return;
        }
        if (repair != null && repair.HandleInput(keyboard)) return;
        if (keyboard.escapeKey.wasPressedThisFrame)
            SetCursor(Cursor.lockState != CursorLockMode.Locked);
        if (Cursor.lockState != CursorLockMode.Locked) return;

        movement.Tick(keyboard);
    }

    private static void SetCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void OnDisable() => SetCursor(false);

    private void OnGUI()
    {
        if (SceneFadeTransition.IsTransitioning) return;
        if (repair != null && repair.Open) return;
        if (notebookChoice != null)
        {
            if (notebookText == null)
            {
                var font = Font.CreateDynamicFontFromOSFont(new[]{"Tahoma","Arial"}, 20);
                notebookText = new GUIStyle(GUI.skin.label){ font = font, fontSize = 20 };
                notebookButton = new GUIStyle(GUI.skin.button){ font = font, fontSize = 20 };
            }
            float width = Mathf.Min(680, Screen.width - 32);
            GUILayout.BeginArea(new Rect((Screen.width-width)/2,Screen.height-140,width,120),GUI.skin.box);
            GUILayout.Label("มีรอยเขียนจาง ๆ อยู่อีกด้านของหน้านี้", notebookText);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(notebookChoice, notebookButton)) CloseNotebook(true);
            if (GUILayout.Button("ปิดสมุด", notebookButton)) CloseNotebook(false);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            return;
        }
        if (lookingAtNotebook) PrototypeThirdPerson.DrawInteractionKey(view, notebook);
        GUI.Box(new Rect(16, 16, Mathf.Min(420, Screen.width * .46f), 70), UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Replace("_Prototype", "").ToUpperInvariant() +
            (otherWorld ? " / OTHER WORLD (B)" : " / REAL WORLD (A)") +
            "\nWASD - Walk    Mouse - Look    Esc - Cursor\nLeft Shift - Run");
        if (Cursor.lockState != CursorLockMode.Locked &&
            GUI.Button(new Rect(16, 94, 160, 36), "Resume"))
            SetCursor(true);
    }
}
