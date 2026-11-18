using UnityEngine;
using UnityEngine.InputSystem;

// One repair job per play session. Returning through a doorway rebuilds its physical state.
public sealed class DeskRepair : MonoBehaviour
{
    static int collected, fitted, nails;
    static float targetCenter;
    static readonly float[] zoneWidths = { .30f, .22f, .16f };
    static readonly float[] sweepSpeeds = { .80f, .98f, 1.16f };
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetJob() { collected = fitted = nails = 0; targetCenter = 0; }

    public bool Open { get; private set; }
    Camera view;
    const int FragmentCount = 5;
    const int Complete = (1 << FragmentCount) - 1;
    readonly GameObject[] fragments = new GameObject[FragmentCount];
    GameObject barrier;
    Material stone;
    readonly Vector2[] slots = { new(340,150), new(428,150), new(516,150), new(384,235), new(472,235) };
    readonly Vector2[] tray = { new(60,145), new(143,145), new(225,145), new(98,246), new(190,246) };
    readonly int[] angles = { 90,0,90,0,90 };
    static readonly string[] marks = { "I","II","III","IV","V" };
    int dragging = -1;
    Vector2 pointer;
    Collider prompt;
    string feedback;
    float start, blockedUntil;
    public void WarnGateOverlap() => blockedUntil = Time.unscaledTime + 2;
    const float Width = 620, Height = 370;
    float Scale => Mathf.Min(Screen.width / Width, Screen.height / Height) * .94f;
    Vector2 Origin => new Vector2((Screen.width - Width * Scale)/2, (Screen.height - Height * Scale)/2);
    Font thai;
    AudioSource sound;
    AudioClip knock;

    public void Setup(Camera camera)
    {
        view = camera;
        var oldPlank = GameObject.Find("Loose repair plank");
        if (oldPlank) oldPlank.SetActive(false);
        var floor = GameObject.Find("Module 0,6").transform.Find("Floor");
        stone = floor.GetComponent<Renderer>().sharedMaterial;
        barrier = Block("Sealed masonry wall", new Vector3(0,1.8f,24), new Vector3(4,3.6f,.45f));
        Vector3[] sites = {
            new(1,.25f,3), new(8.9f,.25f,8), new(23,.25f,4),
            new(31,.25f,19.5f), new(15,.25f,23)
        };
        for (int i=0; i<FragmentCount; i++)
        {
            fragments[i] = Block("Masonry fragment " + marks[i], sites[i], new Vector3(.7f,.42f,.48f));
            fragments[i].transform.rotation = Quaternion.Euler(8, i*29, 6);
        }
        thai = Font.CreateDynamicFontFromOSFont("Tahoma",20);
        sound = gameObject.AddComponent<AudioSource>();
        sound.playOnAwake = false;
        float[] samples = new float[3300];
        for(int i=0;i<samples.Length;i++) samples[i]=Mathf.Sin(i*.11f)*Mathf.Exp(-i/420f)*.65f;
        knock = AudioClip.Create("Stone knock",samples.Length,1,22050,false);
        knock.SetData(samples,0);
        Refresh();
    }
    GameObject Block(string label,Vector3 pos,Vector3 size)
    {
        var block=GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name=label;
        block.transform.position=pos;
        block.transform.localScale=size;
        block.GetComponent<Renderer>().sharedMaterial=stone;
        return block;
    }
    public void Refresh()
    {
        for (int i=0; i<FragmentCount; i++)
            fragments[i].SetActive(PrototypeProgress.OtherWorld && (collected & (1<<i)) == 0);
        barrier.SetActive(nails<3);
    }
    public bool HandleInput(Keyboard keys)
    {
        if(Open)
        {
            if(keys.escapeKey.wasPressedThisFrame) Close();
            else if(fitted==Complete && keys.spaceKey.wasPressedThisFrame)
            {
                if(Mathf.Abs(Beat() - targetCenter) <= zoneWidths[nails] * .5f)
                {
                    sound.PlayOneShot(knock);
                    nails++;
                    feedback="ผนึกคลายแล้ว";
                    start=Time.unscaledTime;
                    if(nails==3) { Close(); Refresh(); }
                    else PickTarget();
                }
                else feedback="พลาดจังหวะ ลองเคาะอีกครั้ง";
            }
            else if(fitted!=Complete) Drag();
            return true;
        }
        prompt=null;
        if(Cursor.lockState!=CursorLockMode.Locked || nails==3) return false;
        if(!PrototypeThirdPerson.InteractionRay(view,out var hit)) return false;
        var item=hit.collider.gameObject;
        bool atWall = item == barrier;
        int fragment = System.Array.IndexOf(fragments,item);
        if (!atWall && fragment < 0) return false;
        prompt = !atWall || collected == Complete ? hit.collider : null;
        if (!keys.eKey.wasPressedThisFrame) return false;
        if (atWall && collected == Complete)
        {
            Open=true;
            Cursor.lockState=CursorLockMode.None;
            Cursor.visible=true;
            feedback="";
            start=Time.unscaledTime;
        }
        else if(fragment >= 0) { collected |= 1<<fragment; Refresh(); }
        return true;
    }
    void Close()
    {
        Open=false;
        dragging=-1;
        Cursor.lockState=CursorLockMode.Locked;
        Cursor.visible=false;
    }
    static void PickTarget()
    {
        // Alternate halves with a gap so successive nails never share the same target.
        bool left = targetCenter == 0 ? Random.value < .5f : targetCenter > .5f;
        float halfWidth = zoneWidths[nails] * .5f;
        targetCenter = left ? Random.Range(.04f + halfWidth, .48f - halfWidth)
                            : Random.Range(.52f + halfWidth, .96f - halfWidth);
    }

    float Beat() => Mathf.PingPong((Time.unscaledTime - start) * sweepSpeeds[nails], 1);
    static Rect Shape(Vector2 p,int angle)
    {
        Vector2 size=angle%180==0 ? new Vector2(66,36) : new Vector2(36,66);
        return new Rect(p-size/2,size);
    }
    void Drag()
    {
        var mouse=Mouse.current;
        if(mouse==null)return;
        var pos=mouse.position.ReadValue();
        pointer=(new Vector2(pos.x,Screen.height-pos.y)-Origin)/Scale;
        if(mouse.leftButton.wasPressedThisFrame)
            for(int i=0;i<FragmentCount;i++)
                if((fitted & (1<<i))==0 && Shape(tray[i],angles[i]).Contains(pointer)) dragging=i;
        if(dragging<0)return;
        if(mouse.rightButton.wasPressedThisFrame) angles[dragging]=(angles[dragging]+90)%180;
        if(!mouse.leftButton.wasReleasedThisFrame)return;
        if(Vector2.Distance(pointer,slots[dragging])<32 && angles[dragging]==(dragging%2)*90)
        {
            fitted |= 1<<dragging;
            if (fitted == Complete) PickTarget();
            feedback="เข้าล็อกแล้ว";
            sound.PlayOneShot(knock,.45f);
            start=Time.unscaledTime;
        }
        else feedback="ลองหมุนชิ้นส่วนแล้ววางตรงกลางกรอบ";
        dragging=-1;
    }
    static void Fill(Rect r, Color c)
    {
        var old=GUI.color; GUI.color=c; GUI.DrawTexture(r,Texture2D.whiteTexture); GUI.color=old;
    }
    void OnGUI()
    {
        if(!view || SceneFadeTransition.IsTransitioning)return;
        var style=new GUIStyle(GUI.skin.label){font=thai,fontSize=19,alignment=TextAnchor.MiddleCenter};
        if(!Open)
        {
            int count = 0;
            for (int i=0; i<FragmentCount; i++) count += (collected >> i) & 1;
            string objective=nails==3?"ผนึกเปิดแล้ว ขึ้นบันไดได้":collected==Complete?"นำเศษกำแพงไปประกอบที่ผนึกหน้าบันได":"ค้นหาเศษกำแพงในโถงทางเดินโลก B  "+count+"/5";
            float width = Mathf.Min(410, Screen.width * .46f);
            Rect hud = new Rect(Screen.width - width - 12, 12, width, 76);
            Fill(hud,new Color(.05f,.07f,.08f,.92f));
            style.wordWrap = true;
            GUI.Label(hud,"ภารกิจ\n" + objective,style);
            if (Time.unscaledTime < blockedUntil)
                GUI.Label(new Rect(hud.x, hud.yMax, width, 60), "เดินพ้นช่องประตูก่อนสลับกลับโลก A", style);
            if (prompt != null && Cursor.lockState == CursorLockMode.Locked)
                PrototypeThirdPerson.DrawInteractionKey(view, prompt);
            return;
        }
        GUI.depth=-500;
        Fill(new Rect(0,0,Screen.width,Screen.height),new Color(.03f,.04f,.05f,.92f));
        var previous=GUI.matrix;
        GUI.matrix=Matrix4x4.TRS(Origin,Quaternion.identity,new Vector3(Scale,Scale,1));
        Fill(new Rect(0,0,Width,Height),new Color(.14f,.18f,.19f));
        GUI.Label(new Rect(10,8,600,45),fitted==Complete?"เคาะปลดผนึก   "+nails+" / 3":"ประกอบเศษกำแพงตามรอยสลัก",style);
        GUI.Label(new Rect(5,54,610,40),fitted==Complete?"กด Space เมื่อขีดสีขาวอยู่ในช่องสีเขียว":"ลากเศษกำแพงด้วยคลิกซ้าย • คลิกขวาขณะลากเพื่อหมุน",style);
        if(fitted==Complete)
        {
            Fill(new Rect(85,180,450,28),Color.gray);
            Fill(new Rect(85+450*(targetCenter-zoneWidths[nails]*.5f),180,450*zoneWidths[nails],28),new Color(.18f,.67f,.38f));
            Fill(new Rect(85+450*Beat()-2,173,4,42),Color.white);
        }
        else
        {
            for(int i=0;i<FragmentCount;i++)
            {
                Rect slot = Shape(slots[i],90*(i%2));
                Fill(slot,new Color(.32f,.38f,.40f));
                GUI.Label(slot,marks[i],style);
            }
            for(int i=0;i<FragmentCount;i++)
            {
                bool fixedPiece=(fitted & (1<<i))!=0;
                var rect=Shape(fixedPiece?slots[i]:dragging==i?pointer:tray[i],fixedPiece?90*(i%2):angles[i]);
                Fill(rect,fixedPiece?new Color(.47f,.64f,.41f):new Color(.48f,.50f,.54f));
                GUI.Label(rect,marks[i],style);
            }
        }
        GUI.Label(new Rect(10,310,600,46),feedback+"    Esc  พักการซ่อม",style);
        GUI.matrix=previous;
    }
    void OnDestroy(){ if(thai) Destroy(thai); if(knock) Destroy(knock); }
}
