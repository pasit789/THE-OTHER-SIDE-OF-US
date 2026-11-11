using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WalkSceneExit : MonoBehaviour
{
    public string destinationScene;
    public Vector3 arrivalPosition;
    public float arrivalYaw;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HallwayPlayer>() != null)
            SceneFadeTransition.Begin(destinationScene, arrivalPosition, arrivalYaw);
    }
}
