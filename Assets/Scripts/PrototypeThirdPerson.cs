using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PrototypeThirdPerson : MonoBehaviour
{
    private Camera view;
    private CharacterController controller;
    private Transform body;
    private Renderer bodyRenderer;
    private Material material;
    private float pitch = 15;
    private float verticalSpeed;
    private readonly RaycastHit[] hits = new RaycastHit[32];
    public const float WalkSpeed = 3.5f;
    public const float RunSpeed = 6f;

    public static PrototypeThirdPerson Attach(CharacterController player, Camera camera)
    {
        var motor = player.GetComponent<PrototypeThirdPerson>();
        if (motor != null) return motor;
        motor = player.gameObject.AddComponent<PrototypeThirdPerson>();
        motor.controller = player;
        player.minMoveDistance = 0;
        motor.view = camera;
        camera.nearClipPlane = .05f;
        var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        visual.name = "Player capsule";
        visual.transform.SetParent(player.transform, false);
        visual.transform.localPosition = Vector3.up * .9f;
        visual.transform.localScale = new Vector3(.6f, .9f, .6f);
        var collider = visual.GetComponent<Collider>();
        collider.enabled = false;
        Destroy(collider);
        motor.body = visual.transform;
        motor.bodyRenderer = visual.GetComponent<Renderer>();
        motor.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        motor.material.color = new Color(.25f, .45f, .65f);
        motor.bodyRenderer.sharedMaterial = motor.material;
        motor.LateUpdate();
        return motor;
    }

    public void Tick(Keyboard keys)
    {
        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue() * .12f;
            transform.Rotate(0, delta.x, 0);
            pitch = Mathf.Clamp(pitch - delta.y, -30, 65);
        }
        float x = (keys.dKey.isPressed ? 1 : 0) - (keys.aKey.isPressed ? 1 : 0);
        float z = (keys.wKey.isPressed ? 1 : 0) - (keys.sKey.isPressed ? 1 : 0);
        Vector3 movement = Vector3.ClampMagnitude(transform.right * x + transform.forward * z, 1);
        float speed = keys.leftShiftKey.isPressed ? RunSpeed : WalkSpeed;
        verticalSpeed = controller.isGrounded ? -2 : verticalSpeed + Physics.gravity.y * Time.deltaTime;
        controller.Move((movement * speed + Vector3.up * verticalSpeed) * Time.deltaTime);
        if (movement.sqrMagnitude > .01f)
            body.rotation = Quaternion.RotateTowards(body.rotation, Quaternion.LookRotation(movement), 540 * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (view == null) return;
        Quaternion rotation = transform.rotation * Quaternion.Euler(pitch, 0, 0);
        Vector3 pivot = transform.position + Vector3.up * 1.55f;
        Vector3 direction = -(rotation * Vector3.forward);
        float distance = 3f;
        int count = Physics.SphereCastNonAlloc(pivot, .18f, direction, hits, distance, ~0, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < count; i++)
        {
            if (hits[i].collider.transform.IsChildOf(transform)) continue;
            distance = Mathf.Min(distance, Mathf.Max(0, hits[i].distance - .05f));
        }
        view.transform.SetPositionAndRotation(pivot + direction * distance, rotation);
        bodyRenderer.enabled = distance > .65f;
    }

    public static bool InteractionRay(Camera camera, out RaycastHit hit, float range = 2.6f)
    {
        var player = camera.GetComponentInParent<CharacterController>();
        hit = default;
        if (player == null) return false;
        var motor = player.GetComponent<PrototypeThirdPerson>();
        if (motor == null) return Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, range);
        int count = Physics.RaycastNonAlloc(camera.transform.position, camera.transform.forward, motor.hits, range + 3.5f, ~0, QueryTriggerInteraction.Ignore);
        float nearest = float.PositiveInfinity;
        for (int i = 0; i < count; i++)
        {
            var candidate = motor.hits[i];
            if (candidate.collider.transform.IsChildOf(player.transform) || candidate.distance >= nearest) continue;
            nearest = candidate.distance;
            hit = candidate;
        }
        return hit.collider != null && Vector3.Distance(player.transform.position + Vector3.up, hit.point) <= range;
    }

    private void OnDestroy()
    {
        if (material != null) Destroy(material);
    }
}
