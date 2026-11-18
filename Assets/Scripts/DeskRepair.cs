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
    GameObject plank, leg, bench, bridge, barrier;
    Renderer timber;
    readonly Vector2[] slots = { new Vector2(395, 168), new Vector2(430, 228) };
    readonly Vector2[] tray = { new Vector2(120, 155), new Vector2(120, 257) };
    readonly int[] angles = { 90, 0 };
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
        plank = GameObject.Find("Loose repair plank");
        timber = plank.GetComponent<Renderer>();
        leg = Block("Loose desk leg", new Vector3(14,.16f,32.5f), new Vector3(.22f,.22f,1.25f));
        leg.transform.rotation = Quaternion.Euler(0,28,0);
        bench = Block("Broken desk", new Vector3(1.1f,.8f,21.8f), new Vector3(1.4f,.14f,.85f));
        foreach(float x in new[]{.6f,1.6f})
            Block("Desk support", new Vector3(x,.4f,21.9f), new Vector3(.14f,.8f,.48f)).transform.SetParent(bench.transform,true);
        var floor = GameObject.Find("Module 0,6").transform.Find("Floor").gameObject;
        floor.SetActive(false);
        foreach(float z in new[]{22.5f,25.5f})
            Block("Floor edge", new Vector3(0,-.15f,z),new Vector3(4,.3f,1)).GetComponent<Renderer>().sharedMaterial = floor.GetComponent<Renderer>().sharedMaterial;
        bridge = Block("Repaired desk crossing",new Vector3(0,-.08f,24),new Vector3(3.96f,.16f,2.05f));
        barrier = Block("Gap boundary",new Vector3(0,1.2f,24),new Vector3(4,2.4f,2));
        barrier.GetComponent<Renderer>().enabled = false;
        thai = Font.CreateDynamicFontFromOSFont("Tahoma",20);
        sound = gameObject.AddComponent<AudioSource>();
        sound.playOnAwake = false;
        float[] samples = new float[3300];
        for(int i=0;i<samples.Length;i++) samples[i]=Mathf.Sin(i*.11f)*Mathf.Exp(-i/420f)*.65f;
        knock = AudioClip.Create("Wood knock",samples.Length,1,22050,false);
        knock.SetData(samples,0);
        Refresh();
    }
    GameObject Block(string label,Vector3 pos,Vector3 size)
    {
        var block=GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name=label;
        block.transform.position=pos;
        block.transform.localScale=size;
        block.GetComponent<Renderer>().sharedMaterial=timber.sharedMaterial;
        return block;
    }
    public void Refresh()
    {
        plank.SetActive(PrototypeProgress.OtherWorld && (collected & 1)==0);
        leg.SetActive(PrototypeProgress.OtherWorld && (collected & 2)==0);
        bench.SetActive(nails<3);
        barrier.SetActive(nails<3);
        bridge.SetActive(nails==3);
    }
    public bool HandleInput(Keyboard keys)
    {
        if(Open)
        {
            if(keys.escapeKey.wasPressedThisFrame) Close();
            else if(fitted==3 && keys.spaceKey.wasPressedThisFrame)
            {
                if(Mathf.Abs(Beat() - targetCenter) <= zoneWidths[nails] * .5f)
                {
                    sound.PlayOneShot(knock);
                    nails++;
                    feedback="ตอกเข้าแล้ว";
                    start=Time.unscaledTime;
                    if(nails==3) { Close(); Refresh(); }
                    else PickTarget();
                }
                else feedback="พลาดจังหวะ ลองตะปูตัวนี้อีกครั้ง";
            }
            else if(fitted!=3) Drag();
            return true;
        }
        prompt=null;
        if(Cursor.lockState!=CursorLockMode.Locked || nails==3) return false;
        if(!PrototypeThirdPerson.InteractionRay(view,out var hit)) return false;
        var item=hit.collider.gameObject;
        bool atDesk=item.transform.IsChildOf(bench.transform);
        if(item!=plank && item!=leg && !atDesk) return false;
        prompt = !atDesk || collected == 3 ? hit.collider : null;
        if(!keys.eKey.wasPressedThisFrame) return false;
        if(atDesk && collected==3)
        {
            Open=true;
            Cursor.lockState=CursorLockMode.None;
            Cursor.visible=true;
            feedback="";
            start=Time.unscaledTime;
        }
        else if(!atDesk) { collected |= item==leg?2:1; Refresh(); }
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
        Vector2 size=angle%180==0 ? new Vector2(132,26) : new Vector2(26,132);
        return new Rect(p-size/2,size);
    }
    void Drag()
    {
        var mouse=Mouse.current;
        if(mouse==null)return;
        var pos=mouse.position.ReadValue();
        pointer=(new Vector2(pos.x,Screen.height-pos.y)-Origin)/Scale;
        if(mouse.leftButton.wasPressedThisFrame)
            for(int i=0;i<2;i++)
                if((fitted & (1<<i))==0 && Shape(tray[i],angles[i]).Contains(pointer)) dragging=i;
        if(dragging<0)return;
        if(mouse.rightButton.wasPressedThisFrame) angles[dragging]=(angles[dragging]+90)%180;
        if(!mouse.leftButton.wasReleasedThisFrame)return;
        if(Vector2.Distance(pointer,slots[dragging])<32 && angles[dragging]==dragging*90)
        {
            fitted |= 1<<dragging;
            if (fitted == 3) PickTarget();
            feedback="เข้าล็อกแล้ว";
            sound.PlayOneShot(knock,.45f);
            start=Time.unscaledTime;
        }
        else feedback="ลองหมุนไม้แล้ววางตรงกลางกรอบ";
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
            string objective=nails==3?"โต๊ะซ่อมแล้ว ข้ามไปบันไดได้":collected==3?"นำไม้ไปซ่อมโต๊ะหน้าบันได":"สำรวจห้องสมุดในโลก B  •  ชิ้นส่วน "+((collected&1)+(collected>>1))+"/2";
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
        GUI.Label(new Rect(10,8,600,45),fitted==3?"ตอกตะปูยึดโต๊ะ   "+nails+" / 3":"ประกอบโต๊ะเรียน",style);
        GUI.Label(new Rect(5,54,610,40),fitted==3?"กด Space เมื่อขีดสีขาวอยู่ในช่องสีเขียว":"ลากไม้ด้วยคลิกซ้าย • คลิกขวาขณะลากเพื่อหมุน",style);
        if(fitted==3)
        {
            Fill(new Rect(85,180,450,28),Color.gray);
            Fill(new Rect(85+450*(targetCenter-zoneWidths[nails]*.5f),180,450*zoneWidths[nails],28),new Color(.18f,.67f,.38f));
            Fill(new Rect(85+450*Beat()-2,173,4,42),Color.white);
        }
        else
        {
            for(int i=0;i<2;i++) Fill(Shape(slots[i],90*i),new Color(.38f,.49f,.50f));
            for(int i=0;i<2;i++)
            {
                bool fixedPiece=(fitted & (1<<i))!=0;
                var rect=Shape(fixedPiece?slots[i]:dragging==i?pointer:tray[i],fixedPiece?90*i:angles[i]);
                Fill(rect,fixedPiece?new Color(.47f,.64f,.41f):new Color(.79f,.56f,.32f));
                Fill(new Rect(rect.x+4,rect.y+4,rect.width-8,2),new Color(.54f,.34f,.19f));
            }
        }
        GUI.Label(new Rect(10,310,600,46),feedback+"    Esc  พักการซ่อม",style);
        GUI.matrix=previous;
    }
    void OnDestroy(){ if(thai) Destroy(thai); if(knock) Destroy(knock); }
}
