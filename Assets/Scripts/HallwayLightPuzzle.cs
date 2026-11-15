using UnityEngine;
using UnityEngine.InputSystem;

public class HallwayLightPuzzle : MonoBehaviour
{
    [SerializeField] private BoxCollider gate;
    [SerializeField] private Renderer ghost;
    [SerializeField] private Collider entryNote;
    [SerializeField] private Collider requestNote;
    [SerializeField] private Collider[] switches;
    [SerializeField] private Light[] lamps;
    [SerializeField] private Light[] pathLights;
    [SerializeField] private int[] sequence = { 1, 0, 2 };
    private HallwayPlayer player;
    private Camera view;
    private Collider focus;
    private string message;
    private float messageUntil;
    private float blockedUntil;
    private Material ghostMaterial;
    private GUIStyle text;
    private Font font;
    private BoxCollider gateSensor;
    private readonly Collider[] overlaps = new Collider[32];

    public void Configure(BoxCollider barrier, Renderer preview, Collider before, Collider after,
        Collider[] buttons, Light[] indicators, Light[] route)
    {
        gate = barrier; ghost = preview; entryNote = before; requestNote = after;
        switches = buttons; lamps = indicators; pathLights = route;
    }

    public void Initialize(HallwayPlayer owner)
    {
        player = owner;
        view = owner.GetComponentInChildren<Camera>();
        ghostMaterial = ghost.material;
        // Keep a trigger-only copy available for queries while the solid gate is disabled.
        var probe = new GameObject("Gate occupancy sensor") { layer = 2 };
        probe.transform.SetParent(gate.transform, false);
        gateSensor = probe.AddComponent<BoxCollider>();
        gateSensor.center = gate.center;
        gateSensor.size = gate.size;
        gateSensor.isTrigger = true;
        RefreshWorld();
    }

    public bool CanSwitchTo(bool otherWorld, CharacterController character)
    {
        if (otherWorld || PrototypeProgress.HallwayLightsSolved) return true;
        Vector3 scale = character.transform.lossyScale;
        float radius = character.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
        float halfSegment = Mathf.Max(0, character.height * Mathf.Abs(scale.y) * .5f - radius);
        Vector3 center = character.transform.TransformPoint(character.center);
        Vector3 offset = character.transform.up * halfSegment;
        Physics.SyncTransforms();
        int count = Physics.OverlapCapsuleNonAlloc(center - offset, center + offset, radius + .02f,
            overlaps, 1 << 2, QueryTriggerInteraction.Collide);
        bool blocked = count == overlaps.Length;
        for (int i = 0; i < count; i++) blocked |= overlaps[i] == gateSensor;
        if (blocked)
        {
            blockedUntil = Time.unscaledTime + 2;
            Tell("ขยับออกจากช่องประตูก่อน — โลกจริงมีประตูอยู่ตรงนี้");
            return false;
        }
        return true;
    }

    public void RefreshWorld()
    {
        bool other = PrototypeProgress.OtherWorld;
        bool solved = PrototypeProgress.HallwayLightsSolved;
        gate.enabled = !other && !solved;
        gate.GetComponent<Renderer>().enabled = gate.enabled;
        ghost.enabled = other && !solved;
        for (int i = 0; i < lamps.Length; i++)
        {
            bool lit = solved;
            for (int step = 0; step < PrototypeProgress.LightSequenceStep; step++) lit |= sequence[step] == i;
            lamps[i].enabled = true;
            lamps[i].color = other ? new Color(.35f, .65f, 1) : lit ? new Color(.55f, 1, .65f) : new Color(1, .7f, .3f);
            lamps[i].intensity = solved ? 2 : lit ? 1.5f : .25f;
        }
        foreach (var lamp in pathLights) lamp.enabled = solved;
    }

    public void ReadEntry()
    {
        Tell("โน้ตข้างประตู\n“ฉันเปิดทางฝั่งนี้ให้แล้ว ลองมองจากที่ที่ฉันอยู่”\nกด Q แล้วเดินผ่านประตู");
    }

    public void ReadRequest()
    {
        if (PrototypeProgress.HallwayLightsSolved)
        {
            if (!PrototypeProgress.OtherWorld) { Tell("ข้อความจางหายไป… ลองอ่านสมุดในโลก B"); return; }
            PrototypeProgress.LightReplyRead = true;
            Tell("ข้อความตอบกลับ\n“เห็นแล้ว… ขอบคุณที่ยังอยู่ตรงนั้น”\nไฟนำทางไปบันไดติดแล้ว และประตูโลกจริงเปิดเป็นทางลัด");
        }
        else
        {
            PrototypeProgress.LightRequestRead = true;
            Tell("สมุดอีกฝั่ง\n“ช่วยเปิดไฟให้ฉันสามดวงได้ไหม ฉันมองไม่เห็นทางกลับ”\nดูสัญลักษณ์ที่แผงสวิตช์ในโลก B แล้วกลับไปกดในโลก A");
        }
    }

    public bool PressSwitch(int index)
    {
        if (index < 0 || index >= switches.Length || PrototypeProgress.HallwayLightsSolved) return false;
        if (!PrototypeProgress.LightRequestRead) { Tell("มีสมุดอยู่หลังประตู ลองอ่านก่อน"); return false; }
        if (PrototypeProgress.OtherWorld) { Tell("เงาสัญลักษณ์บนผนัง\n■  →  ●  →  ◆\nกลับโลก A เพื่อกดสวิตช์ตามลำดับนี้"); return false; }
        if (sequence[PrototypeProgress.LightSequenceStep] != index)
        {
            PrototypeProgress.LightSequenceStep = 0;
            PrototypeProgress.LightMistakes++;
            Tell(PrototypeProgress.LightMistakes >= 2 ? "ลองใหม่ได้เลย — ลำดับคือ ■  →  ●  →  ◆" : "ไฟดับลง… ลองดูคำใบ้ในโลก B อีกครั้ง");
        }
        else
        {
            PrototypeProgress.LightSequenceStep++;
            PrototypeProgress.HallwayLightsSolved = PrototypeProgress.LightSequenceStep == sequence.Length;
            Tell(PrototypeProgress.HallwayLightsSolved ? "ไฟทั้งสามติดแล้ว! สลับโลก B กลับไปอ่านสมุด" : "ไฟติดแล้ว " + PrototypeProgress.LightSequenceStep + "/3");
        }
        RefreshWorld();
        return true;
    }

    private void Update()
    {
        if (player == null) return;
        if (ghostMaterial != null)
            ghostMaterial.color = Time.unscaledTime < blockedUntil ? new Color(1, .2f, .15f, .35f) : new Color(.3f, .7f, 1, .16f);
        focus = null;
        if (SceneFadeTransition.IsTransitioning || Cursor.lockState != CursorLockMode.Locked) return;
        if (PrototypeThirdPerson.InteractionRay(view, out var hit)) focus = hit.collider;
        if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame) return;
        if (focus == entryNote) ReadEntry();
        else if (focus == requestNote) ReadRequest();
        else
        {
            int index = System.Array.IndexOf(switches, focus);
            if (index >= 0) PressSwitch(index);
        }
    }

    private void Tell(string value)
    {
        message = value;
        messageUntil = Time.unscaledTime + 10;
    }

    private void OnGUI()
    {
        if (player == null || SceneFadeTransition.IsTransitioning) return;
        if (text == null)
        {
            font = Font.CreateDynamicFontFromOSFont(new[] { "Tahoma", "Arial" }, 20);
            text = new GUIStyle(GUI.skin.label) { font = font, fontSize = 20, wordWrap = true };
            text.normal.textColor = Color.white;
        }
        float width = Mathf.Min(720, Screen.width - 32);
        string objective = PrototypeProgress.LightReplyRead ? "ตามแสงไฟไปบันไดดาดฟ้า" :
            PrototypeProgress.HallwayLightsSolved ? "อ่านคำตอบในสมุด — โลก B" :
            PrototypeProgress.LightRequestRead ? "เปิดไฟให้อีกฝั่ง  " + PrototypeProgress.LightSequenceStep + "/3 — ดูคำใบ้โลก B / กดสวิตช์โลก A" :
            "สำรวจโน้ตข้างประตูเหล็ก — Q สลับโลก / E อ่าน";
        GUI.Box(new Rect(16, Screen.height - 70, width, 58), "");
        GUI.Label(new Rect(26, Screen.height - 65, width - 20, 52), objective, text);
        GUI.Label(new Rect(Screen.width / 2 - 7, Screen.height / 2 - 14, 25, 30), "+", text);
        int index = switches == null ? -1 : System.Array.IndexOf(switches, focus);
        if (focus != null && (focus == entryNote || focus == requestNote || index >= 0))
        {
            string prompt = index >= 0 ? "E — " + new[] { "●", "■", "◆" }[index] + (PrototypeProgress.OtherWorld ? " ดูร่องรอย" : " กดสวิตช์") : "E — อ่านสมุด";
            GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height / 2 + 25, 320, 40), prompt, text);
            if (index >= 0 && PrototypeProgress.OtherWorld && !PrototypeProgress.HallwayLightsSolved)
                GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height / 2 + 65, 350, 40), "ร่องรอย  ■  →  ●  →  ◆", text);
        }
        if (Time.unscaledTime < messageUntil)
        {
            GUI.Box(new Rect(16, Screen.height - 240, width, 160), "");
            GUI.Label(new Rect(28, Screen.height - 232, width - 24, 145), message, text);
        }
    }

    private void OnDestroy()
    {
        if (ghostMaterial != null) Destroy(ghostMaterial);
        if (font != null) Destroy(font);
    }
}
