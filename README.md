# The Other Side of Us

โปรเจกต์ Unity สำหรับต้นแบบเกม 3D Romance Mystery ที่ใช้การสำรวจ สลับโลก และต่อสู้แบบเทิร์นเบส

## เปิดโปรเจกต์

1. ติดตั้ง Unity Editor **6000.6.5f1 (Unity 6.6)** ผ่าน Unity Hub
2. Clone repository แล้วเปิด Unity Hub → Projects → Add → Add project from disk
3. เลือกโฟลเดอร์ที่มี `Assets`, `Packages` และ `ProjectSettings` แล้วรอให้ Unity นำเข้าแพ็กเกจ
4. เปิด `Assets/Scenes/SampleScene.unity` แล้วกด **Play** ฉากนี้ใช้ตรวจว่าโปรเจกต์ 3D URP เปิดและรันได้ก่อนเริ่มทำต้นแบบห้องเรียน

## การเก็บไฟล์ใน Git

Commit `Assets/` พร้อมไฟล์ `.meta`, `Packages/` และ `ProjectSettings/` เพื่อรักษาการอ้างอิง Asset และเวอร์ชัน Editor ไฟล์แคช ไฟล์ Build และการตั้งค่าเฉพาะเครื่องถูกแยกด้วย `.gitignore`
