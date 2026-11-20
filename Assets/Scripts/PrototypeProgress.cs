using UnityEngine;

// Progress lasts for one play session, including travel between prototype scenes.
public static class PrototypeProgress
{
    public static bool OtherWorld { get; set; }
    private static bool notebookRead;
    public static string OpenNotebook()
    {
        string label = notebookRead ? (OtherWorld ? "กลับโลกเดิม" : "ข้ามไปอีกโลก") : "พลิกไปอีกหน้า";
        notebookRead = true;
        return label;
    }

    public static bool LightRequestRead { get; set; }
    public static bool HallwayLightsSolved { get; set; }
    public static bool LightReplyRead { get; set; }
    public static int LightSequenceStep { get; set; }
    public static int LightMistakes { get; set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        notebookRead = false;
        OtherWorld = false;
        LightRequestRead = false;
        HallwayLightsSolved = false;
        LightReplyRead = false;
        LightSequenceStep = 0;
        LightMistakes = 0;
    }
}
