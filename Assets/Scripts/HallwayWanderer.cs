using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class HallwayWanderer : MonoBehaviour
{
    private HallwayPlayer player;
    private CharacterController controller;
    private Renderer body;
    private Material bodyMaterial;
    private Vector3[] route;
    private int targetIndex = 1;
    private int direction = 1;
    private float speed;
    private float pause;
    private float blockedTime;

    public static void SpawnCrowd(HallwayPlayer player)
    {
        if (GameObject.Find("Hallway students") != null) return;
        var crowd = new GameObject("Hallway students");
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(crowd, player.gameObject.scene);
        Vector3[][] routes =
        {
            new[] { Point(0, 1), Point(0, 4), Point(8, 4), Point(8, 8) },
            new[] { Point(12, 4), Point(24, 4), Point(24, 8) },
            new[] { Point(0, 16), Point(0, 20), Point(8, 20), Point(8, 16) },
            new[] { Point(16, 24), Point(16, 12), Point(20, 12) },
            new[] { Point(32, 20), Point(32, 12), Point(24, 12), Point(24, 20) }
        };
        for (int i = 0; i < routes.Length; i++)
        {
            var avatar = new GameObject("Student " + (i + 1));
            avatar.transform.SetParent(crowd.transform);
            avatar.transform.position = routes[i][0];
            var mover = avatar.AddComponent<CharacterController>();
            mover.height = 1.7f;
            mover.center = Vector3.up * .85f;
            mover.radius = .28f;
            mover.stepOffset = .2f;
            mover.skinWidth = .03f;
            mover.minMoveDistance = 0;
            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Capsule body";
            visual.transform.SetParent(avatar.transform, false);
            visual.transform.localPosition = Vector3.up * .85f;
            visual.transform.localScale = new Vector3(.56f, .85f, .56f);
            var primitiveCollider = visual.GetComponent<Collider>();
            primitiveCollider.enabled = false;
            Destroy(primitiveCollider);
            var npc = avatar.AddComponent<HallwayWanderer>();
            npc.player = player;
            npc.controller = mover;
            npc.body = visual.GetComponent<Renderer>();
            npc.bodyMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            npc.bodyMaterial.color = Color.HSVToRGB(.52f + i * .055f, .35f, .7f);
            npc.body.sharedMaterial = npc.bodyMaterial;
            npc.route = routes[i];
            npc.speed = 1.05f + i * .1f;
            npc.pause = i * .4f;
        }
    }

    private static Vector3 Point(float x, float z) => new Vector3(x, .05f, z);

    private void Update()
    {
        if (player == null || route == null) return;
        bool visible = !player.IsOtherWorld;
        body.enabled = visible;
        controller.enabled = visible;
        if (!visible || SceneFadeTransition.IsTransitioning) return;
        if (pause > 0)
        {
            pause -= Time.deltaTime;
            controller.Move(Vector3.down * 2 * Time.deltaTime);
            return;
        }
        Vector3 offset = route[targetIndex] - transform.position;
        offset.y = 0;
        if (offset.magnitude < .12f)
        {
            Advance();
            pause = Random.Range(.6f, 2f);
            return;
        }
        Vector3 previous = transform.position;
        Vector3 movement = Vector3.ClampMagnitude(offset, speed * Time.deltaTime);
        controller.Move(movement + Vector3.down * 2 * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            Quaternion.LookRotation(offset), 180 * Time.deltaTime);
        Vector3 travelled = transform.position - previous;
        travelled.y = 0;
        blockedTime = travelled.magnitude < movement.magnitude * .15f ? blockedTime + Time.deltaTime : 0;
        if (blockedTime > 2)
        {
            // Retrace the current segment rather than cutting across a corridor corner.
            targetIndex -= direction;
            direction = -direction;
            blockedTime = 0;
            pause = Random.Range(.3f, .8f);
        }
    }

    private void Advance()
    {
        if (targetIndex == route.Length - 1) direction = -1;
        else if (targetIndex == 0) direction = 1;
        targetIndex += direction;
    }

    private void OnDestroy()
    {
        if (bodyMaterial != null) Destroy(bodyMaterial);
    }
}
