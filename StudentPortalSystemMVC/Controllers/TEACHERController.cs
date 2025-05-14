using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StudentPortalSystemMVC.Models;

namespace StudentPortalSystemMVC.Controllers
{
    public class TeacherController : Controller
    {
        private StudentPortalSystemEntities db = new StudentPortalSystemEntities();

        // GET: Teacher/Dashboard
        public ActionResult Dashboard()
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Teacher")
            {
                TempData["Error"] = "Please log in as a teacher.";
                return RedirectToAction("Login", "Account");
            }
            int userId = (int)Session["UserId"];
            var teacher = db.TEACHERS.FirstOrDefault(t => t.USER_ID == userId);
            if (teacher == null)
            {
                TempData["Error"] = "Teacher profile not found.";
                return RedirectToAction("Login", "Account");
            }
            try
            {
                // Fetch raw data without complex expressions
                var rawCourses = db.COURSES
                    .Where(c => c.TEACHER_ID == teacher.TEACHER_ID)
                    .Select(c => new
                    {
                        c.COURSE_ID,
                        c.COURSE_NAME,
                        c.CREDITS,
                        TeacherUser = c.TEACHER != null ? c.TEACHER.USER : null
                    })
                    .ToList();

                // Transform data in memory
                var courses = rawCourses.Select(c => new CourseViewModel
                {
                    COURSE_ID = c.COURSE_ID,
                    COURSE_NAME = c.COURSE_NAME ?? "N/A",
                    CREDITS = c.CREDITS,
                    TeacherName = c.TeacherUser != null
                        ? $"{c.TeacherUser.FIRST_NAME ?? "N/A"} {c.TeacherUser.LAST_NAME ?? "N/A"}"
                        : "N/A"
                }).ToList();

                var dashboard = new TeacherDashboardViewModel
                {
                    Teacher = db.USERS.FirstOrDefault(u => u.ID == userId) ?? new USER(),
                    Department = teacher.DEPARTMENT != null
                        ? db.DEPARTMENTS.FirstOrDefault(d => d.DEP_ID == teacher.DEPARTMENT.DEP_ID)?.DEP_NAME ?? "N/A"
                        : "N/A",
                    Courses = courses
                };

                return View(dashboard);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Dashboard: {ex.Message}\nStack Trace: {ex.StackTrace}");
                TempData["Error"] = "An error occurred while loading the dashboard.";
                return RedirectToAction("Login", "Account");
            }
        }


        // GET: Teacher/ManageAttendance/5 (CourseId)
        public ActionResult ManageAttendance(int? courseId)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Teacher")
            {
                TempData["Error"] = "Please log in as a teacher.";
                return RedirectToAction("Login", "Account");
            }

            if (courseId == null)
            {
                TempData["Error"] = "Course not specified.";
                return RedirectToAction("Dashboard");
            }

            int userId = (int)Session["UserId"];
            var teacher = db.TEACHERS.FirstOrDefault(t => t.USER_ID == userId);
            if (teacher == null)
            {
                TempData["Error"] = "Teacher profile not found.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var course = db.COURSES.FirstOrDefault(c => c.COURSE_ID == courseId && c.TEACHER_ID == teacher.TEACHER_ID);
                if (course == null)
                {
                    TempData["Error"] = "Course not found or not assigned to you.";
                    return RedirectToAction("Dashboard");
                }

                var rawAttendance = db.ENROLLMENTS
                    .Where(e => e.COURSE_ID == courseId)
                    .SelectMany(e => e.ATTENDANCEs, (e, a) => new
                    {
                        a.ATTENDANCE_ID,
                        e.ENROLLMENT_ID,
                        StudentFirstName = e.STUDENT.USER != null ? e.STUDENT.USER.FIRST_NAME : null,
                        StudentLastName = e.STUDENT.USER != null ? e.STUDENT.USER.LAST_NAME : null,
                        CourseName = e.COURS.COURSE_NAME,
                        a.ATTENDANCE_DATE,
                        a.STATUS
                    })
                    .ToList();

                var attendanceList = rawAttendance.Select(a => new StudentAttendanceViewModel
                {
                    ATTENDANCE_ID = a.ATTENDANCE_ID,
                    ENROLLMENT_ID = a.ENROLLMENT_ID,
                    StudentName = a.StudentFirstName != null && a.StudentLastName != null
                        ? $"{a.StudentFirstName} {a.StudentLastName}"
                        : "N/A",
                    CourseName = a.CourseName ?? "N/A",
                    AttendanceDate = a.ATTENDANCE_DATE,
                    Status = a.STATUS ?? "N/A"
                }).ToList();

                ViewBag.CourseId = courseId;
                ViewBag.CourseName = course.COURSE_NAME;
                return View(attendanceList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ManageAttendance: {ex.Message}");
                TempData["Error"] = "An error occurred while loading attendance.";
                return RedirectToAction("Dashboard");
            }
        }

        // GET: Teacher/AddAttendance/5 (CourseId)
        public ActionResult AddAttendance(int? courseId)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Teacher")
            {
                TempData["Error"] = "Please log in as a teacher.";
                return RedirectToAction("Login", "Account");
            }

            if (courseId == null)
            {
                TempData["Error"] = "Course not specified.";
                return RedirectToAction("Dashboard");
            }

            int userId = (int)Session["UserId"];
            var teacher = db.TEACHERS.FirstOrDefault(t => t.USER_ID == userId);
            if (teacher == null)
            {
                TempData["Error"] = "Teacher profile not found.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var course = db.COURSES.FirstOrDefault(c => c.COURSE_ID == courseId && c.TEACHER_ID == teacher.TEACHER_ID);
                if (course == null)
                {
                    TempData["Error"] = "Course not found or not assigned to you.";
                    return RedirectToAction("Dashboard");
                }

                var rawStudents = db.ENROLLMENTS
                    .Where(e => e.COURSE_ID == courseId)
                    .Select(e => new
                    {
                        e.ENROLLMENT_ID,
                        StudentFirstName = e.STUDENT.USER != null ? e.STUDENT.USER.FIRST_NAME : null,
                        StudentLastName = e.STUDENT.USER != null ? e.STUDENT.USER.LAST_NAME : null,
                        CourseName = course.COURSE_NAME
                    })
                    .ToList();

                var students = rawStudents.Select(e => new StudentAttendanceViewModel
                {
                    ENROLLMENT_ID = e.ENROLLMENT_ID,
                    StudentName = e.StudentFirstName != null && e.StudentLastName != null
                        ? $"{e.StudentFirstName} {e.StudentLastName}"
                        : "N/A",
                    CourseName = e.CourseName,
                    AttendanceDate = DateTime.Today,
                    Status = "P"
                }).ToList();

                ViewBag.CourseId = courseId;
                ViewBag.CourseName = course.COURSE_NAME;
                return View(students);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddAttendance: {ex.Message}");
                TempData["Error"] = "An error occurred while loading students.";
                return RedirectToAction("Dashboard");
            }
        }

        // POST: Teacher/AddAttendance
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAttendance(int courseId, List<StudentAttendanceViewModel> attendances)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Teacher")
            {
                TempData["Error"] = "Please log in as a teacher.";
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["UserId"];
            var teacher = db.TEACHERS.FirstOrDefault(t => t.USER_ID == userId);
            if (teacher == null)
            {
                TempData["Error"] = "Teacher profile not found.";
                return RedirectToAction("Dashboard");
            }

            try
            {
                var course = db.COURSES.FirstOrDefault(c => c.COURSE_ID == courseId && c.TEACHER_ID == teacher.TEACHER_ID);
                if (course == null)
                {
                    TempData["Error"] = "Course not found or not assigned to you.";
                    return RedirectToAction("Dashboard");
                }

                if (attendances == null || !attendances.Any())
                {
                    TempData["Error"] = "No attendance data provided.";
                    return RedirectToAction("ManageAttendance", new { courseId });
                }

                foreach (var att in attendances)
                {
                    if (att.ENROLLMENT_ID > 0)
                    {
                        db.ATTENDANCEs.Add(new ATTENDANCE
                        {
                            ENROLLMENT_ID = att.ENROLLMENT_ID,
                            ATTENDANCE_DATE = att.AttendanceDate,
                            STATUS = att.Status
                        });
                    }
                }

                db.SaveChanges();
                TempData["Success"] = "Attendance recorded successfully.";
                return RedirectToAction("ManageAttendance", new { courseId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddAttendance: {ex.Message}");
                TempData["Error"] = "An error occurred while saving attendance.";
                return RedirectToAction("ManageAttendance", new { courseId });
            }
        }

        // GET: Teacher/EditAttendance/5 (AttendanceId)
        public ActionResult EditAttendance(int? attendanceId)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Teacher")
            {
                TempData["Error"] = "Please log in as a teacher.";
                return RedirectToAction("Login", "Account");
            }

            if (attendanceId == null)
            {
                TempData["Error"] = "Attendance record not specified.";
                return RedirectToAction("Dashboard");
            }

            try
            {
                var rawAttendance = db.ATTENDANCEs
                    .Where(a => a.ATTENDANCE_ID == attendanceId)
                    .Select(a => new
                    {
                        a.ATTENDANCE_ID,
                        a.ENROLLMENT_ID,
                        StudentFirstName = a.ENROLLMENT.STUDENT.USER != null ? a.ENROLLMENT.STUDENT.USER.FIRST_NAME : null,
                        StudentLastName = a.ENROLLMENT.STUDENT.USER != null ? a.ENROLLMENT.STUDENT.USER.LAST_NAME : null,
                        CourseName = a.ENROLLMENT.COURS.COURSE_NAME,
                        a.ATTENDANCE_DATE,
                        a.STATUS
                    })
                    .FirstOrDefault();

                if (rawAttendance == null)
                {
                    TempData["Error"] = "Attendance record not found.";
                    return RedirectToAction("Dashboard");
                }

                var attendance = new StudentAttendanceViewModel
                {
                    ATTENDANCE_ID = rawAttendance.ATTENDANCE_ID,
                    ENROLLMENT_ID = rawAttendance.ENROLLMENT_ID,
                    StudentName = rawAttendance.StudentFirstName != null && rawAttendance.StudentLastName != null
                        ? $"{rawAttendance.StudentFirstName} {rawAttendance.StudentLastName}"
                        : "N/A",
                    CourseName = rawAttendance.CourseName ?? "N/A",
                    AttendanceDate = rawAttendance.ATTENDANCE_DATE,
                    Status = rawAttendance.STATUS
                };

                return View(attendance);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in EditAttendance: {ex.Message}");
                TempData["Error"] = "An error occurred while loading attendance.";
                return RedirectToAction("Dashboard");
            }
        }

        // POST: Teacher/EditAttendance
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAttendance(StudentAttendanceViewModel model)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Teacher")
            {
                TempData["Error"] = "Please log in as a teacher.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var attendance = db.ATTENDANCEs.Find(model.ATTENDANCE_ID);
                if (attendance == null)
                {
                    TempData["Error"] = "Attendance record not found.";
                    return RedirectToAction("Dashboard");
                }

                var courseId = db.ENROLLMENTS
                    .Where(e => e.ENROLLMENT_ID == attendance.ENROLLMENT_ID)
                    .Select(e => e.COURSE_ID)
                    .FirstOrDefault();

                attendance.ATTENDANCE_DATE = model.AttendanceDate;
                attendance.STATUS = model.Status;
                db.SaveChanges();

                TempData["Success"] = "Attendance updated successfully.";
                return RedirectToAction("ManageAttendance", new { courseId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in EditAttendance: {ex.Message}");
                TempData["Error"] = "An error occurred while updating attendance.";
                return View(model);
            }
        }

        // POST: Teacher/DeleteAttendance/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAttendance(int attendanceId)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Teacher")
            {
                TempData["Error"] = "Please log in as a teacher.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var attendance = db.ATTENDANCEs.Find(attendanceId);
                if (attendance == null)
                {
                    TempData["Error"] = "Attendance record not found.";
                    return RedirectToAction("Dashboard");
                }

                var courseId = db.ENROLLMENTS
                    .Where(e => e.ENROLLMENT_ID == attendance.ENROLLMENT_ID)
                    .Select(e => e.COURSE_ID)
                    .FirstOrDefault();

                db.ATTENDANCEs.Remove(attendance);
                db.SaveChanges();

                TempData["Success"] = "Attendance deleted successfully.";
                return RedirectToAction("ManageAttendance", new { courseId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in DeleteAttendance: {ex.Message}");
                TempData["Error"] = "An error occurred while deleting attendance.";
                return RedirectToAction("Dashboard");
            }
        }

        // GET: Teacher/ManageGrades/5 (CourseId)
        public ActionResult ManageGrades(int? courseId)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Teacher")
            {
                TempData["Error"] = "Please log in as a teacher.";
                return RedirectToAction("Login", "Account");
            }

            if (courseId == null)
            {
                TempData["Error"] = "Course not specified.";
                return RedirectToAction("Dashboard");
            }

            int userId = (int)Session["UserId"];
            var teacher = db.TEACHERS.FirstOrDefault(t => t.USER_ID == userId);
            if (teacher == null)
            {
                TempData["Error"] = "Teacher profile not found.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var course = db.COURSES.FirstOrDefault(c => c.COURSE_ID == courseId && c.TEACHER_ID == teacher.TEACHER_ID);
                if (course == null)
                {
                    TempData["Error"] = "Course not found or not assigned to you.";
                    return RedirectToAction("Dashboard");
                }

                var rawGrades = db.ENROLLMENTS
                    .Where(e => e.COURSE_ID == courseId)
                    .SelectMany(e => e.GRADES, (e, g) => new
                    {
                        g.GRADE_ID,
                        e.ENROLLMENT_ID,
                        StudentFirstName = e.STUDENT.USER != null ? e.STUDENT.USER.FIRST_NAME : null,
                        StudentLastName = e.STUDENT.USER != null ? e.STUDENT.USER.LAST_NAME : null,
                        CourseName = e.COURS.COURSE_NAME,
                        g.MARKS,
                        g.GRADE1
                    })
                    .ToList();

                var gradesList = rawGrades.Select(g => new StudentGradeViewModel
                {
                    GRADE_ID = g.GRADE_ID,
                    ENROLLMENT_ID = g.ENROLLMENT_ID,
                    StudentName = g.StudentFirstName != null && g.StudentLastName != null
                        ? $"{g.StudentFirstName} {g.StudentLastName}"
                        : "N/A",
                    CourseName = g.CourseName ?? "N/A",
                    Marks = g.MARKS,
                    Grade = g.GRADE1 ?? "N/A"
                }).ToList();

                ViewBag.CourseId = courseId;
                ViewBag.CourseName = course.COURSE_NAME;
                return View(gradesList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ManageGrades: {ex.Message}");
                TempData["Error"] = "An error occurred while loading grades.";
                return RedirectToAction("Dashboard");
            }
        }

        // GET: Teacher/AddGrade/5 (CourseId)
        public ActionResult AddGrade(int? courseId)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Teacher")
            {
                TempData["Error"] = "Please log in as a teacher.";
                return RedirectToAction("Login", "Account");
            }

            if (courseId == null)
            {
                TempData["Error"] = "Course not specified.";
                return RedirectToAction("Dashboard");
            }

            int userId = (int)Session["UserId"];
            var teacher = db.TEACHERS.FirstOrDefault(t => t.USER_ID == userId);
            if (teacher == null)
            {
                TempData["Error"] = "Teacher profile not found.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var course = db.COURSES.FirstOrDefault(c => c.COURSE_ID == courseId && c.TEACHER_ID == teacher.TEACHER_ID);
                if (course == null)
                {
                    TempData["Error"] = "Course not found or not assigned to you.";
                    return RedirectToAction("Dashboard");
                }

                var rawStudents = db.ENROLLMENTS
                    .Where(e => e.COURSE_ID == courseId)
                    .Select(e => new
                    {
                        e.ENROLLMENT_ID,
                        StudentFirstName = e.STUDENT.USER != null ? e.STUDENT.USER.FIRST_NAME : null,
                        StudentLastName = e.STUDENT.USER != null ? e.STUDENT.USER.LAST_NAME : null,
                        CourseName = course.COURSE_NAME
                    })
                    .ToList();

                var students = rawStudents.Select(e => new StudentGradeViewModel
                {
                    ENROLLMENT_ID = e.ENROLLMENT_ID,
                    StudentName = e.StudentFirstName != null && e.StudentLastName != null
                        ? $"{e.StudentFirstName} {e.StudentLastName}"
                        : "N/A",
                    CourseName = e.CourseName,
                    Marks = null,
                    Grade = "N/A"
                }).ToList();

                ViewBag.CourseId = courseId;
                ViewBag.CourseName = course.COURSE_NAME;
                return View(students);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddGrade: {ex.Message}");
                TempData["Error"] = "An error occurred while loading students.";
                return RedirectToAction("Dashboard");
            }
        }

        // POST: Teacher/AddGrade
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddGrade(int courseId, List<StudentGradeViewModel> grades)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Teacher")
            {
                TempData["Error"] = "Please log in as a teacher.";
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["UserId"];
            var teacher = db.TEACHERS.FirstOrDefault(t => t.USER_ID == userId);
            if (teacher == null)
            {
                TempData["Error"] = "Teacher profile not found.";
                return RedirectToAction("Dashboard");
            }

            try
            {
                var course = db.COURSES.FirstOrDefault(c => c.COURSE_ID == courseId && c.TEACHER_ID == teacher.TEACHER_ID);
                if (course == null)
                {
                    TempData["Error"] = "Course not found or not assigned to you.";
                    return RedirectToAction("Dashboard");
                }

                if (grades == null || !grades.Any())
                {
                    TempData["Error"] = "No grade data provided.";
                    return RedirectToAction("ManageGrades", new { courseId });
                }

                foreach (var grade in grades)
                {
                    if (grade.ENROLLMENT_ID > 0 && grade.Marks.HasValue)
                    {
                        db.GRADES.Add(new GRADE
                        {
                            ENROLLMENT_ID = grade.ENROLLMENT_ID,
                            MARKS = grade.Marks, // Now int?, matches GRADE.MARKS
                            GRADE1 = grade.Grade
                        });
                    }
                }

                db.SaveChanges();
                TempData["Success"] = "Grades recorded successfully.";
                return RedirectToAction("ManageGrades", new { courseId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddGrade: {ex.Message}");
                TempData["Error"] = "An error occurred while saving grades.";
                return RedirectToAction("ManageGrades", new { courseId });
            }
        }

        // GET: Teacher/EditGrade/5 (GradeId)
        public ActionResult EditGrade(int? gradeId)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Teacher")
            {
                TempData["Error"] = "Please log in as a teacher.";
                return RedirectToAction("Login", "Account");
            }

            if (gradeId == null)
            {
                TempData["Error"] = "Grade record not specified.";
                return RedirectToAction("Dashboard");
            }

            try
            {
                var rawGrade = db.GRADES
                    .Where(g => g.GRADE_ID == gradeId)
                    .Select(g => new
                    {
                        g.GRADE_ID,
                        g.ENROLLMENT_ID,
                        StudentFirstName = g.ENROLLMENT.STUDENT.USER != null ? g.ENROLLMENT.STUDENT.USER.FIRST_NAME : null,
                        StudentLastName = g.ENROLLMENT.STUDENT.USER != null ? g.ENROLLMENT.STUDENT.USER.LAST_NAME : null,
                        CourseName = g.ENROLLMENT.COURS.COURSE_NAME,
                        g.MARKS,
                        g.GRADE1
                    })
                    .FirstOrDefault();

                if (rawGrade == null)
                {
                    TempData["Error"] = "Grade record not found.";
                    return RedirectToAction("Dashboard");
                }

                var grade = new StudentGradeViewModel
                {
                    GRADE_ID = rawGrade.GRADE_ID,
                    ENROLLMENT_ID = rawGrade.ENROLLMENT_ID,
                    StudentName = rawGrade.StudentFirstName != null && rawGrade.StudentLastName != null
                        ? $"{rawGrade.StudentFirstName} {rawGrade.StudentLastName}"
                        : "N/A",
                    CourseName = rawGrade.CourseName ?? "N/A",
                    Marks = rawGrade.MARKS,
                    Grade = rawGrade.GRADE1
                };

                return View(grade);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in EditGrade: {ex.Message}");
                TempData["Error"] = "An error occurred while loading grade.";
                return RedirectToAction("Dashboard");
            }
        }

        // POST: Teacher/EditGrade
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGrade(StudentGradeViewModel model)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Teacher")
            {
                TempData["Error"] = "Please log in as a teacher.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var grade = db.GRADES.Find(model.GRADE_ID);
                if (grade == null)
                {
                    TempData["Error"] = "Grade record not found.";
                    return RedirectToAction("Dashboard");
                }

                var courseId = db.ENROLLMENTS
                    .Where(e => e.ENROLLMENT_ID == grade.ENROLLMENT_ID)
                    .Select(e => e.COURSE_ID)
                    .FirstOrDefault();

                grade.MARKS = model.Marks; // Now int?, matches GRADE.MARKS
                grade.GRADE1 = model.Grade;
                db.SaveChanges();

                TempData["Success"] = "Grade updated successfully.";
                return RedirectToAction("ManageGrades", new { courseId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in EditGrade: {ex.Message}");
                TempData["Error"] = "An error occurred while updating grade.";
                return View(model);
            }
        }

        // POST: Teacher/DeleteGrade/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteGrade(int gradeId)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Teacher")
            {
                TempData["Error"] = "Please log in as a teacher.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var grade = db.GRADES.Find(gradeId);
                if (grade == null)
                {
                    TempData["Error"] = "Grade record not found.";
                    return RedirectToAction("Dashboard");
                }

                var courseId = db.ENROLLMENTS
                    .Where(e => e.ENROLLMENT_ID == grade.ENROLLMENT_ID)
                    .Select(e => e.COURSE_ID)
                    .FirstOrDefault();

                db.GRADES.Remove(grade);
                db.SaveChanges();

                TempData["Success"] = "Grade deleted successfully.";
                return RedirectToAction("ManageGrades", new { courseId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in DeleteGrade: {ex.Message}");
                TempData["Error"] = "An error occurred while deleting grade.";
                return RedirectToAction("Dashboard");
            }
        }

        // GET: Teacher/CourseReport/5 (CourseId)
        public ActionResult CourseReport(int? courseId)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Teacher")
            {
                TempData["Error"] = "Please log in as a teacher.";
                return RedirectToAction("Login", "Account");
            }

            if (courseId == null)
            {
                TempData["Error"] = "Course not specified.";
                return RedirectToAction("Dashboard");
            }

            int userId = (int)Session["UserId"];
            var teacher = db.TEACHERS.FirstOrDefault(t => t.USER_ID == userId);
            if (teacher == null)
            {
                TempData["Error"] = "Teacher profile not found.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var course = db.COURSES.FirstOrDefault(c => c.COURSE_ID == courseId && c.TEACHER_ID == teacher.TEACHER_ID);
                if (course == null)
                {
                    TempData["Error"] = "Course not found or not assigned to you.";
                    return RedirectToAction("Dashboard");
                }

                var enrollments = db.ENROLLMENTS
                    .Where(e => e.COURSE_ID == courseId)
                    .ToList();

                var attendanceRecords = enrollments.SelectMany(e => e.ATTENDANCEs).ToList();
                var gradeRecords = enrollments.SelectMany(e => e.GRADES).Where(g => g.MARKS.HasValue).ToList();

                var report = new CourseReportViewModel
                {
                    CourseName = course.COURSE_NAME,
                    TotalStudents = enrollments.Count,
                    AverageAttendance = attendanceRecords.Any()
                        ? attendanceRecords.Count(a => a.STATUS == "P") / (double)attendanceRecords.Count * 100
                        : 0.0,
                    AverageMarks = gradeRecords.Any()
                        ? gradeRecords.Average(g => g.MARKS.Value)
                        : 0.0
                };

                return View(report);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CourseReport: {ex.Message}");
                TempData["Error"] = "An error occurred while generating report.";
                return RedirectToAction("Dashboard");
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}