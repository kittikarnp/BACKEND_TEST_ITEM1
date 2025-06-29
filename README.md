 ผู้พัฒนา
👨‍💻 ชื่อ: Kittikarn Panyu
📧 อีเมล: kittikarnpanyu@gmail.com

ไฟล์นี้ประกอบด้วย ข้อที่ 1  

โครงสร้างโปรเจ็กต์ 
BACKEND_TEST_ITEM1/
│
├── Controllers/
│   └── QuizController.cs             # Logic API สำหรับ session, คำถาม, คำตอบ, สรุปผล
│
├── Data/
│   └── AppDbContext.cs               # DbContext สำหรับเชื่อมฐานข้อมูล
│
├── Models/
│   ├── DTOs/                     
│   │   ├── AnswerDto.cs
│   │   ├── ChoiceDto.cs
│   │   ├── QuestionDto.cs
│   │   ├── SubmitAnswerDto.cs
│   │   └── SummaryDto.cs
│   ├── Answer.cs
│   ├── Choice.cs
│   ├── Question.cs
│   └── Session.cs
│
├── Services/
├── appsettings.json
├── Program.cs
├── Startup.cs 


1. Clone โปรเจกต์นี้
   https://github.com/kittikarnp/BACKEND_TEST_ITEM1.git
1. เตรียมเครื่องมือ
   .NET SDK 9.0
   SQL Server 
   Visual Studio 
2. ตั้งค่า Database Connection
แก้ไขใน appsettings.json:
   {
    "ConnectionStrings": {
      "DefaultConnection": "Server=.;Database=QuizIslandDB;Trusted_Connection=True;"
    }
  }
3. รันโปรเจกต์
    dotnet restore
    dotnet build
    dotnet run
API จะเริ่มที่: https://localhost:7209/swagger/index.html

4. BackEndQuiz.bak  ===> Add Database
