# University Portal System

The **University Portal System** is a web-based application developed for a **Web Programming** course by **Mustafa Waqar**. 
Built using **ASP.NET MVC**, it streamlines academic processes for students, teachers, and administrators. 
While real-world university portals require addressing complex constraints (e.g., scalability, security, compliance), this project serves as a simplified representation, showcasing core functionalities and robust design.

## Table of Contents
- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Setup Instructions](#setup-instructions)
- [Usage](#usage)
- [Contributions](#contributions)
- [License](#license)

## Overview
The University Portal System enables efficient management of academic tasks. Students can enroll in courses and view grades/attendance, teachers can manage grades and attendance, and admins can oversee users, courses, and departments. Developed as a course project, it demonstrates **full-stack development**, **database design**, and **UI/UX** skills using modern web technologies.

## Features
- **User Authentication**: Secure registration and login for students, teachers, and admins (`/Account/Login`, `/Account/Register`).
- **Course Management**: Admins create/edit courses; students enroll (`/Admin/CourseList`, `/Student/RegisterCourse`).
- **Grade & Attendance Tracking**: Teachers update grades and attendance; students view records (`/Teacher/Dashboard`, `/Student/Dashboard`).
- **Admin Dashboard**: Manage users, courses, departments, and registrations (`/Admin/Index`).
- **Responsive UI**: Built with Bootstrap 5 for a seamless experience across devices (`Views/Shared/_Layout.cshtml`).

## Technology Stack
- **Frontend**: Bootstrap 5, jQuery, HTML, CSS (`Content/Site.css`).
- **Backend**: ASP.NET MVC, C# (`Controllers/AccountController.cs`, `AdminController.cs`).
- **Database**: SQL Server, Entity Framework (`App_Data/StudentPortalSystem.mdf`, `Models/StudentPortalModel.edmx`).
- **Tools**: Visual Studio, SQL Server Management Studio.

## Architecture
The system follows the **MVC (Model-View-Controller)** pattern:
- **Models**: Define data structures (`USER.cs`, `COURSE.cs`, `ENROLLMENT.cs`, `ATTENDANCE.cs`, `GRADE.cs`).
  - `USER`: `USER_ID (PK)`, `USERNAME`, `PASSWORD_HASH`, `EMAIL`, `ROLE`, `FIRST_NAME`, `LAST_NAME`, `CONTACT`, `ADDRESS`.
  - `COURSE`: `COURSE_ID (PK)`, `COURSE_NAME`, `CREDITS`, `TEACHER_ID (FK)`.
  - Relationships: `USER` teaches `COURSE` (1:N), `USER` and `COURSE` link via `ENROLLMENT`.
- **Controllers**: Handle logic (`AccountController.cs` for login, `AdminController.cs` for management).
- **Views**: Render UI (`Login.cshtml`, `Student/Dashboard.cshtml`, `Admin/Index.cshtml`) using Razor and Bootstrap.
- **Database**: SQL Server with Entity Framework for ORM, including tables: `USERs`, `COURSEs`, `ENROLLMENTs`, `ATTENDANCEs`, `GRADEs`.

## Setup Instructions
1. **Prerequisites**:
   - Visual Studio 2019/2022 with ASP.NET and web development workload.
   - SQL Server Express or LocalDB.
   - .NET Framework 4.8 or later.

2. **Clone Repository**:
   ```bash
   git clone https://github.com/MusW02/university-portal-system.git
   cd university-portal-system
   ```

3. **Restore Dependencies**:
   - Open `UniversityPortalSystem.sln` in Visual Studio.
   - Restore NuGet packages (e.g., Entity Framework, Bootstrap).

4. **Database Setup**:
   - Update connection string in `Web.config`:
     ```xml
     <connectionStrings>
       <add name="StudentPortalSystemEntities" connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\StudentPortalSystem.mdf;Integrated Security=True" providerName="System.Data.SqlClient" />
     </connectionStrings>
     ```
   - Run migrations or attach `App_Data/StudentPortalSystem.mdf` in SQL Server.

5. **Run Application**:
   - Set `UniversityPortalSystem` as the startup project.
   - Press `F5` to build and run.
   - Access at `http://localhost:[port]` (e.g., `http://localhost:44398`).

## Usage
- **Login**:
  - Admin: Use credentials (e.g., username: `admin`, password: `admin123`) or create via `/Account/Register`.
  - Student/Teacher: Register or use test accounts.
- **Navigate**:
  - **Student**: Enroll in courses, view grades/attendance (`/Student/Dashboard`).
  - **Teacher**: Manage grades/attendance (`/Teacher/Dashboard`).
  - **Admin**: Manage users, courses, departments (`/Admin/Index`).
- **Test Features**: Explore responsive UI on different devices.

## Contributions
- **Mustafa Waqar**:
  - Designed and implemented frontend (Bootstrap 5, Razor views).
  - Developed backend logic (Controllers).
  - Configured database schema and Entity Framework (`StudentPortalModel.edmx`).
  - Planned LinkedIn post to showcase the project.
  - Worked on feature development (e.g., course management, admin dashboard).
  - Contributed to UI/UX design and testing.

## License
This project is licensed under the [MIT License](LICENSE). Feel free to use and modify, with attribution to Mustafa Waqar.
---
