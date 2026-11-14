using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class HallwayPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    private CharacterController controller;
    private PrototypeThirdPerson movement;
    private bool otherWorld;
    public bool IsOtherWorld => otherWorld;
    private Camera view;
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
        SetCursor(true);
        if (gameObject.scene.name == "Hallway_Prototype") HallwayWanderer.SpawnCrowd(this);
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
    }

    private void Update()
    {
        if (SceneFadeTransition.IsTransitioning) return;
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        if (keyboard.escapeKey.wasPressedThisFrame)
            SetCursor(Cursor.lockState != CursorLockMode.Locked);
        if (Cursor.lockState != CursorLockMode.Locked) return;

        if (keyboard.qKey.wasPressedThisFrame)
        {
            otherWorld = !otherWorld;
            ApplyWorld();
        }

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
        GUI.Box(new Rect(16, 16, 420, 70), UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Replace("_Prototype", "").ToUpperInvariant() +
            (otherWorld ? " / OTHER WORLD" : " / REAL WORLD") +
            "\nWASD - Walk    Mouse - Look    Esc - Cursor\nQ - Switch world    Left Shift - Run");
        if (Cursor.lockState != CursorLockMode.Locked &&
            GUI.Button(new Rect(16, 94, 160, 36), "Resume"))
            SetCursor(true);
    }
}
