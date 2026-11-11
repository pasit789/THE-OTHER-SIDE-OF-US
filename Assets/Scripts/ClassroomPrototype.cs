using UnityEngine;
using UnityEngine.InputSystem;

public class ClassroomPrototype : MonoBehaviour
{
    [SerializeField] private CharacterController player;
    [SerializeField] private Camera view;
    [SerializeField] private Light sun;
    [SerializeField] private GameObject clue;
    [SerializeField] private Renderer floor;
    private float pitch;
    private bool otherWorld;
    private int dialogue;
    private static bool messageSent;
    private string focus;
    private GUIStyle textStyle;
    private GUIStyle buttonStyle;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetProgress() => messageSent = false;

    private void Start()
    {
        if (player == null) BuildClassroom();
        ApplyWorld();
        LockCursor(true);
    }

    public void BuildClassroom()
    {
        floor = Box("Floor", new Vector3(0,-.15f,0), new Vector3(12,.3f,14), new Color(.65f,.61f,.5f)).GetComponent<Renderer>();
        Box("Back wall",new Vector3(0,2,7),new Vector3(12,4,.2f),Color.gray);
        Box("Front wall",new Vector3(0,2,-7),new Vector3(12,4,.2f),Color.gray);
        Box("Left wall",new Vector3(-6,2,0),new Vector3(.2f,4,14),Color.gray);
        Box("Right wall",new Vector3(6,2,0),new Vector3(.2f,4,14),Color.gray);
        Box("Door",new Vector3(4.6f,1.25f,-6.8f),new Vector3(1.5f,2.5f,.15f),new Color(.3f,.2f,.12f));
        Box("Blackboard",new Vector3(0,2,6.8f),new Vector3(7,2,.12f),new Color(.08f,.2f,.16f));
        for(int row=0;row<3;row++)
        for(int col=0;col<3;col++)
        {
            Vector3 position=new Vector3((col-1)*3,1, row*2.6f-1);
            Box("Desk",position,new Vector3(1.6f,.16f,.9f),new Color(.55f,.36f,.2f));
            Box("Desk support",position+Vector3.down*.5f,new Vector3(.9f,1,.5f),Color.gray);
        }
        Box("Note",new Vector3(0,1.1f,-1),new Vector3(.5f,.03f,.35f),Color.yellow);
        clue=Box("Reply",new Vector3(.55f,1.15f,-1),new Vector3(.3f,.1f,.3f),Color.cyan);
        clue.SetActive(false);
        var npc=GameObject.CreatePrimitive(PrimitiveType.Capsule);
        npc.name="Other student";
        npc.transform.position=new Vector3(3,1,5);
        npc.GetComponent<Renderer>().material.color=new Color(.4f,.5f,.8f);
        var avatar=new GameObject("Player");
        avatar.transform.position=new Vector3(0,.1f,-5);
        player=avatar.AddComponent<CharacterController>();
        player.height=1.8f;
        player.center=new Vector3(0,.9f,0);
        player.radius=.3f;
        view=new GameObject("Player Camera").AddComponent<Camera>();
        view.tag="MainCamera";
        view.transform.SetParent(avatar.transform,false);
        view.transform.localPosition=new Vector3(0,1.65f,0);
        view.gameObject.AddComponent<AudioListener>();
        sun=new GameObject("Sun").AddComponent<Light>();
        sun.type=LightType.Directional;
        sun.transform.rotation=Quaternion.Euler(45,-25,0);
        ApplyWorld();
    }

    private static GameObject Box(string name,Vector3 position,Vector3 scale,Color color)
    {
        var item=GameObject.CreatePrimitive(PrimitiveType.Cube);
        item.name=name;
        item.transform.position=position;
        item.transform.localScale=scale;
        var shader=Shader.Find("Universal Render Pipeline/Lit");
        item.GetComponent<Renderer>().material=new Material(shader){color=color};
        return item;
    }

    private void Update()
    {
        var keys=Keyboard.current;
        if(keys==null || player==null) return;
        if(keys.escapeKey.wasPressedThisFrame)
        {
            dialogue=0;
            LockCursor(Cursor.lockState!=CursorLockMode.Locked);
        }
        if(dialogue!=0)
        {
            if(keys.digit1Key.wasPressedThisFrame) Choose(1);
            else if(keys.digit2Key.wasPressedThisFrame) Choose(2);
            return;
        }
        if(Cursor.lockState!=CursorLockMode.Locked) return;
        var mouse=Mouse.current;
        if(mouse!=null)
        {
            var delta=mouse.delta.ReadValue()*.12f;
            player.transform.Rotate(0,delta.x,0);
            pitch=Mathf.Clamp(pitch-delta.y,-75,75);
            view.transform.localRotation=Quaternion.Euler(pitch,0,0);
        }
        float x=(keys.dKey.isPressed?1:0)-(keys.aKey.isPressed?1:0);
        float z=(keys.wKey.isPressed?1:0)-(keys.sKey.isPressed?1:0);
        var move=Vector3.ClampMagnitude(player.transform.right*x+player.transform.forward*z,1);
        player.Move((move*3.5f+Vector3.down*9.81f)*Time.deltaTime);
        if(keys.tabKey.wasPressedThisFrame)
        {
            otherWorld=!otherWorld;
            ApplyWorld();
        }
        focus=null;
        if(Physics.Raycast(view.transform.position,view.transform.forward,out var hit,2.6f))
        {
            if(hit.collider.name=="Note" || hit.collider.name=="Reply" || hit.collider.name=="Other student")
                focus=hit.collider.name;
        }
        if(focus!=null && keys.eKey.wasPressedThisFrame)
        {
            dialogue=focus=="Other student"?3:focus=="Reply"?4:1;
            LockCursor(false);
        }
    }

    private void ApplyWorld()
    {
        sun.color=otherWorld?new Color(.45f,.6f,1):new Color(1,.86f,.65f);
        sun.intensity=otherWorld?.8f:1.3f;
        RenderSettings.ambientLight=otherWorld?new Color(.15f,.2f,.4f):new Color(.5f,.5f,.5f);
        view.backgroundColor=otherWorld?new Color(.05f,.08f,.2f):new Color(.5f,.7f,.85f);
        floor.sharedMaterial.color=otherWorld?new Color(.2f,.25f,.4f):new Color(.65f,.61f,.5f);
        clue.SetActive(otherWorld && messageSent);
    }

    private void Choose(int choice)
    {
        if(dialogue==1 && choice==1)
        {
            messageSent=true;
            dialogue=2;
            ApplyWorld();
            return;
        }
        dialogue=0;
        LockCursor(true);
    }

    private static void LockCursor(bool locked)
    {
        Cursor.lockState=locked?CursorLockMode.Locked:CursorLockMode.None;
        Cursor.visible=!locked;
    }

    private void OnGUI()
    {
        if(textStyle==null)
        {
            var font=Font.CreateDynamicFontFromOSFont(new[]{"Tahoma","Arial"},22);
            textStyle=new GUIStyle(GUI.skin.label){font=font,fontSize=22,wordWrap=true};
            textStyle.normal.textColor=Color.white;
            buttonStyle=new GUIStyle(GUI.skin.button){font=font,fontSize=20,wordWrap=true};
        }
        GUI.Box(new Rect(12,12,620,160),"");
        GUI.Label(new Rect(25,20,590,145),
            (otherWorld?"โลกคู่ขนาน":"โลกจริง")+" — ห้องเรียนต้นแบบ\nWASD เดิน · เมาส์มอง · Tab สลับโลก · E โต้ตอบ · Esc เมาส์\n"+
            (messageSent?"สลับโลกแล้วค้นหาคำตอบบนโต๊ะ":"หาโน้ตสีเหลืองบนโต๊ะ แล้วฝากข้อความ"),textStyle);
        GUI.Label(new Rect(Screen.width/2-8,Screen.height/2-15,30,30),"+",textStyle);
        if(focus!=null && dialogue==0) GUI.Label(new Rect(Screen.width/2-130,Screen.height/2+30,300,40),"E — อ่าน / พูดคุย",textStyle);
        if(dialogue==0)
        {
            if(Cursor.lockState!=CursorLockMode.Locked && GUI.Button(new Rect(Screen.width/2-120,Screen.height/2+80,240,50),"กลับไปเล่น",buttonStyle)) LockCursor(true);
            return;
        }
        float width=Mathf.Min(Screen.width-40,850);
        var area=new Rect((Screen.width-width)/2,Screen.height-270,width,250);
        GUI.Box(area,"");
        string line=dialogue==1?"โน้ตบนโต๊ะ — ถ้าเธอได้ยินฉัน ฝากข้อความไว้ตรงนี้ได้ไหม?":
            dialogue==2?"คุณเขียนว่า ‘ฉันยังอยู่ตรงนี้’ ลองสลับโลกเพื่อค้นหาคำตอบ":
            dialogue==3?"นักเรียน — โต๊ะตัวเดิมอาจมีบางอย่างต่างออกไป เมื่อมองจากอีกโลก":
            "ข้อความจากอีกโลก — ฉันเห็นข้อความแล้ว ขอบคุณที่ยังไม่ลืมกัน";
        GUI.Label(new Rect(area.x+20,area.y+20,width-40,125),line,textStyle);
        if(GUI.Button(new Rect(area.x+20,area.y+150,width/2-30,75),dialogue==1?"1 — ฝากข้อความ":"1 — เข้าใจแล้ว",buttonStyle)) Choose(1);
        if(GUI.Button(new Rect(area.x+width/2+10,area.y+150,width/2-30,75),dialogue==1?"2 — ไว้ก่อน":"2 — กลับไปสำรวจ",buttonStyle)) Choose(2);
    }
}
