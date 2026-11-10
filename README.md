# The Other Side of Us

โปรเจกต์ Unity สำหรับต้นแบบเกม 3D Romance Mystery ที่ใช้การสำรวจ สลับโลก และต่อสู้แบบเทิร์นเบส

## เปิดโปรเจกต์

1. ติดตั้ง Unity Editor **6000.6.5f1 (Unity 6.6)** ผ่าน Unity Hub
2. Clone repository แล้วเปิด Unity Hub → Projects → Add → Add project from disk
3. เลือกโฟลเดอร์ที่มี `Assets`, `Packages` และ `ProjectSettings` แล้วรอให้ Unity นำเข้าแพ็กเกจ
4. เปิด `Assets/Scenes/SampleScene.unity` แล้วกด **Play** ฉากนี้ใช้ตรวจว่าโปรเจกต์ 3D URP เปิดและรันได้ก่อนเริ่มทำต้นแบบห้องเรียน

## การเก็บไฟล์ใน Git

Commit `Assets/` พร้อมไฟล์ `.meta`, `Packages/` และ `ProjectSettings/` เพื่อรักษาการอ้างอิง Asset และเวอร์ชัน Editor ไฟล์แคช ไฟล์ Build และการตั้งค่าเฉพาะเครื่องถูกแยกด้วย `.gitignore`

## Classroom prototype

เปิดฉาก Assets/Scenes/Classroom_Prototype.unity แล้วกด Play ห้องเรียนใช้รูปทรงพื้นฐานและบันทึกวัตถุไว้ในฉาก สามารถเลือกและย้ายโต๊ะ ผนัง โน้ต และ NPC ใน Scene view ได้

- WASD เดิน และเมาส์หมุนกล้อง
- E โต้ตอบเมื่อเล็งโน้ตหรือ NPC ในระยะ 2.6 เมตร
- Tab สลับโลก โดยใช้พื้นและสิ่งกีดขวางชุดเดียวกัน
- 1 / 2 หรือคลิกเพื่อเลือกบทสนทนา
- Esc ปิดบทสนทนาและปล่อยเมาส์

เดินไปที่โต๊ะกลางแถวแรก อ่านโน้ตสีเหลือง เลือกฝากข้อความ แล้วสลับโลกเพื่ออ่านข้อความตอบกลับสีฟ้าบนโต๊ะ นักเรียนด้านหลังกระดานให้คำใบ้ได้

Windows build อยู่ที่ Builds/ClassroomPrototype/ClassroomPrototype.exe หลังเรียก CreateClassroom.Run ผ่าน Unity batch mode ต้องเก็บไฟล์ทั้งหมดในโฟลเดอร์ build ไว้ด้วยกัน ต้นแบบใช้ฟอนต์ Tahoma ของ Windows และยังไม่บันทึกความคืบหน้าข้ามการเปิดเกม

