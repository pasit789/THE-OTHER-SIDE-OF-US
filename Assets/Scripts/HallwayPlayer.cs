using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class HallwayPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    private CharacterController controller;
    private float pitch;
    private float verticalSpeed;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>().transform;
    }

    private void Start() => SetCursor(true);

    private void Update()
    {
        if (SceneFadeTransition.IsTransitioning) return;
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        if (keyboard.escapeKey.wasPressedThisFrame)
            SetCursor(Cursor.lockState != CursorLockMode.Locked);
        if (Cursor.lockState != CursorLockMode.Locked) return;

        if (Mouse.current != null)
        {
            Vector2 look = Mouse.current.delta.ReadValue() * 0.12f;
            transform.Rotate(0, look.x, 0);
            pitch = Mathf.Clamp(pitch - look.y, -75, 75);
            playerCamera.localRotation = Quaternion.Euler(pitch, 0, 0);
        }

        float horizontal = (keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0);
        float forward = (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0);
        Vector3 movement = Vector3.ClampMagnitude(transform.right * horizontal + transform.forward * forward, 1);
        verticalSpeed = controller.isGrounded ? -2 : verticalSpeed + Physics.gravity.y * Time.deltaTime;
        controller.Move((movement * 3.5f + Vector3.up * verticalSpeed) * Time.deltaTime);
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
        GUI.Box(new Rect(16, 16, 360, 70), UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Replace("_Prototype", "").ToUpperInvariant() + " / GREYBOX\nWASD - Walk    Mouse - Look    Esc - Cursor");
        if (Cursor.lockState != CursorLockMode.Locked &&
            GUI.Button(new Rect(16, 94, 160, 36), "Resume"))
            SetCursor(true);
    }
}
