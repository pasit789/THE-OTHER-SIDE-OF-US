using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadeTransition : MonoBehaviour
{
    public static bool IsTransitioning { get; private set; }
    private float opacity;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState() => IsTransitioning = false;

    public static void Begin(string destination, Vector3 position, float yaw)
    {
        if (IsTransitioning) return;
        if (!Application.CanStreamedLevelBeLoaded(destination))
        {
            Debug.LogError("Scene is not enabled in Build Settings: " + destination);
            return;
        }
        IsTransitioning = true;
        var transition = new GameObject("Scene fade").AddComponent<SceneFadeTransition>();
        DontDestroyOnLoad(transition.gameObject);
        transition.StartCoroutine(transition.Travel(destination, position, yaw));
    }

    private IEnumerator Travel(string destination, Vector3 position, float yaw)
    {
        yield return Fade(1);
        // Keep the fully black frame visible before replacing the scene.
        yield return null;
        yield return SceneManager.LoadSceneAsync(destination);
        var player = FindFirstObjectByType<HallwayPlayer>();
        if (player != null)
        {
            var controller = player.GetComponent<CharacterController>();
            controller.enabled = false;
            player.transform.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));
            controller.enabled = true;
        }
        else Debug.LogError("Transition destination has no HallwayPlayer.");
        yield return new WaitForSecondsRealtime(.2f);
        yield return Fade(0);
        IsTransitioning = false;
        Destroy(gameObject);
    }

    private IEnumerator Fade(float target)
    {
        while (!Mathf.Approximately(opacity, target))
        {
            opacity = Mathf.MoveTowards(opacity, target, Time.unscaledDeltaTime / .4f);
            yield return null;
        }
    }

    private void OnGUI()
    {
        GUI.depth = -10000;
        Color previous = GUI.color;
        GUI.color = new Color(0, 0, 0, opacity);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = previous;
    }

    private void OnDestroy() => IsTransitioning = false;
}
