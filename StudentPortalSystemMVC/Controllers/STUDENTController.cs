using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StudentPortalSystemMVC.Models;

namespace StudentPortalSystemMVC.Controllers
{
    public class STUDENTController : Controller
    {
        private StudentPortalSystemEntities db = new StudentPortalSystemEntities();

        // GET: STUDENT/Dashboard
        public ActionResult Dashboard()
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Student")
            {
                TempData["Error"] = "Please log in as a student.";
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["UserId"];
            var student = db.STUDENTS.FirstOrDefault(s => s.USER_ID == userId);
            if (student == null)
            {
                TempData["Error"] = "Student profile not found.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Personal info and department
                ViewBag.Student = db.USERS.FirstOrDefault(u => u.ID == userId) ?? new USER();
                ViewBag.Department = student.DEPARTMENT != null
                    ? db.DEPARTMENTS.FirstOrDefault(d => d.DEP_ID == student.DEPARTMENT.DEP_ID)?.DEP_NAME ?? "N/A"
                    : "N/A";

                // Course registration status
                ViewBag.IsCourseRegistrationOpen = db.CourseRegistrationStatus.FirstOrDefault()?.IsOpen ?? false;

                // Registered courses with teacher details
                var courseData = db.ENROLLMENTS
                    .Where(e => e.STUDENT.ROLL_NO == student.ROLL_NO)
                    .Select(e => new
                    {
                        e.COURSE_ID,
                        CourseName = e.COURS != null ? e.COURS.COURSE_NAME : "N/A",
                        FirstName = e.COURS != null && e.COURS.TEACHER != null && e.COURS.TEACHER.USER != null
                            ? e.COURS.TEACHER.USER.FIRST_NAME : null,
                        LastName = e.COURS != null && e.COURS.TEACHER != null && e.COURS.TEACHER.USER != null
                            ? e.COURS.TEACHER.USER.LAST_NAME : null,
                        Credits = e.COURS != null ? e.COURS.CREDITS : 0
                    })
                    .ToList();

                ViewBag.RegisteredCourses = courseData.Select(c => new CourseViewModel
                {
                    COURSE_ID = c.COURSE_ID,
                    COURSE_NAME = c.CourseName,
                    TeacherName = c.FirstName != null && c.LastName != null ? $"{c.FirstName} {c.LastName}" : "N/A",
                    CREDITS = c.Credits
                }).ToList();

                // Attendance
                ViewBag.Attendance = db.ATTENDANCEs
                    .Where(a => a.ENROLLMENT != null && a.ENROLLMENT.STUDENT.ROLL_NO == student.ROLL_NO)
                    .Select(a => new AttendanceViewModel
                    {
                        COURSE_NAME = a.ENROLLMENT.COURS.COURSE_NAME ?? "N/A",
                        ATTENDANCE_DATE = a.ATTENDANCE_DATE,
                        Status = a.STATUS == "P" ? "Present" : "Absent"
                    })
                    .ToList();

                // Grades
                ViewBag.Grades = db.GRADES
                    .Where(g => g.ENROLLMENT != null && g.ENROLLMENT.STUDENT.ROLL_NO == student.ROLL_NO)
                    .Select(g => new GradeViewModel
                    {
                        COURSE_NAME = g.ENROLLMENT.COURS.COURSE_NAME ?? "N/A",
                        MARKS = g.MARKS,
                        GRADE = g.GRADE1 ?? "N/A"
                    })
                    .ToList();

                // Fees
                var fees = db.FEES
                    .Where(f => f.STUDENT.ROLL_NO == student.ROLL_NO)
                    .Select(f => new FeeViewModel
                    {
                        COURSE_NAME = f.COURS.COURSE_NAME ?? "N/A",
                        AMOUNT = f.AMOUNT,
                        Status = f.FEE_STATUS == true ? "Paid" : "Unpaid"
                    })
                    .ToList();
                ViewBag.Fees = fees;
                ViewBag.TotalFees = fees.Any() ? fees.Sum(f => f.AMOUNT) : 0.0;

                // Transcript
                ViewBag.Transcript = db.TRANSCRIPTs
                    .Where(t => t.STUDENT.ROLL_NO == student.ROLL_NO)
                    .Select(t => new TranscriptViewModel
                    {
                        COURSE_NAME = t.COURS.COURSE_NAME ?? "N/A",
                        MARKS = t.GRADE.MARKS,
                        GRADE = t.GRADE.GRADE1 ?? "N/A",
                        SGPA = t.SGPA ?? 0.0,
                        CGPA = t.CGPA ?? 0.0
                    })
                    .ToList();

                return View();
            }
            catch (Exception ex)
            {
                var errorDetails = ex.Message;
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    errorDetails += $"\nInner Exception: {innerException.Message}";
                    innerException = innerException.InnerException;
                }
                System.Diagnostics.Debug.WriteLine($"Error in Dashboard: {errorDetails}\nStack Trace: {ex.StackTrace}");
                ViewBag.ErrorMessage = "An error occurred while loading the dashboard. Please try again.";
                return View();
            }
        }

        // get course
        [HttpGet]
        public ActionResult RegisterCourse()
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Student")
            {
                TempData["Message"] = "Please log in as a student.";
                return RedirectToAction("Login", "Account");
            }

            if (!db.CourseRegistrationStatus.FirstOrDefault()?.IsOpen ?? false)
            {
                TempData["Message"] = "Course registration is closed.";
                return RedirectToAction("Dashboard");
            }

            int userId = (int)Session["UserId"];
            var student = db.STUDENTS.FirstOrDefault(s => s.USER_ID == userId);
            if (student == null)
            {
                TempData["Message"] = "Student not found.";
                return RedirectToAction("Dashboard");
            }

            try
            {
                var enrolledCourseIds = db.ENROLLMENTS
                    .Where(e => e.STUDENT.ROLL_NO == student.ROLL_NO)
                    .Select(e => e.COURSE_ID)
                    .ToList();

                var availableCourses = db.COURSES
                    .Where(c => !enrolledCourseIds.Contains(c.COURSE_ID) && c.DepartmentId == student.DEPARTMENT.DEP_ID)
                    .Select(c => new SelectListItem
                    {
                        Value = c.COURSE_ID.ToString(),
                        Text = c.COURSE_NAME + " (" + c.CREDITS + " Credits)"
                    })
                    .ToList();

                ViewBag.Courses = availableCourses;

                // Log for debugging
                System.Diagnostics.Debug.WriteLine($"Available Courses Count: {availableCourses.Count}, Student ROLL_NO: {student.ROLL_NO}, DepartmentId: {student.DEPARTMENT.DEP_ID}");

                if (!availableCourses.Any())
                {
                    TempData["Message"] = "No courses available for registration in your department.";
                }

                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RegisterCourse: {ex.Message}");
                TempData["Message"] = "An error occurred while loading courses. Please try again.";
                return RedirectToAction("Dashboard");
            }
        }

        // POST: Student/RegisterCourse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterCourse(int courseId)
        {
            if (Session["UserId"] == null || Session["Role"].ToString() != "Student")
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["UserId"];
            var student = db.STUDENTS.FirstOrDefault(s => s.USER_ID == userId);
            if (student == null)
            {
                TempData["Message"] = "Student not found.";
                return RedirectToAction("Dashboard");
            }

            // Verify the course exists
            var course = db.COURSES.Find(courseId);
            if (course == null)
            {
                TempData["Message"] = "Course not found.";
                return RedirectToAction("Dashboard");
            }

            // Check if the student is already enrolled in the course
            if (db.ENROLLMENTS.Any(e => e.STUDENT_ID == student.ROLL_NO && e.COURSE_ID == courseId))
            {
                TempData["Message"] = "You are already enrolled in this course.";
                return RedirectToAction("Dashboard");
            }

            // Check if a fee already exists for this student and course
            if (db.FEES.Any(f => f.STUDENT_ID == student.ROLL_NO && f.COURSE_ID == courseId))
            {
                TempData["Message"] = "A fee record already exists for this course.";
                return RedirectToAction("Dashboard");
            }

            try
            {
                // Create enrollment
                var enrollment = new ENROLLMENT
                {
                    STUDENT_ID = student.ROLL_NO,
                    COURSE_ID = courseId
                };
                db.ENROLLMENTS.Add(enrollment);

                // Generate fee
                var feeAmount = course.AMOUNT_PER_COURSE > 0 ? course.AMOUNT_PER_COURSE : 100.0; // Default to 100 if 0
                var fee = new FEE
                {
                    STUDENT_ID = student.ROLL_NO,
                    COURSE_ID = courseId,
                    AMOUNT = feeAmount,
                    FEE_STATUS = false
                };
                db.FEES.Add(fee);

                int rowsAffected = db.SaveChanges();
                if (rowsAffected > 0)
                {
                    TempData["Message"] = "Course registered successfully!";
                }
                else
                {
                    TempData["Message"] = "Course registration failed: No changes were saved.";
                }
            }
            catch (Exception ex)
            {
                // Log detailed exception information
                var innerException = ex.InnerException;
                var errorDetails = ex.Message;
                while (innerException != null)
                {
                    errorDetails += $"\nInner Exception: {innerException.Message}";
                    innerException = innerException.InnerException;
                }
                TempData["Message"] = $"Error registering course: {errorDetails}";
                System.Diagnostics.Debug.WriteLine($"Error in RegisterCourse: {errorDetails}");
            }

            return RedirectToAction("Dashboard");
        }

        // GET: STUDENT
        public ActionResult Index()
        {
            var sTUDENTS = db.STUDENTS.Include(s => s.DEPARTMENT).Include(s => s.USER);
            return View(sTUDENTS.ToList());
        }

        // GET: STUDENT/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STUDENT sTUDENT = db.STUDENTS.Find(id);
            if (sTUDENT == null)
            {
                return HttpNotFound();
            }
            return View(sTUDENT);
        }

        // GET: STUDENT/Create
        public ActionResult Create()
        {
            ViewBag.DEPARTMENT_ID = new SelectList(db.DEPARTMENTS, "DEP_ID", "DEP_NAME");
            ViewBag.USER_ID = new SelectList(db.USERS, "ID", "EMAIL");
            return View();
        }

        // POST: STUDENT/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ROLL_NO,USER_ID,DEPARTMENT_ID")] STUDENT sTUDENT)
        {
            if (ModelState.IsValid)
            {
                db.STUDENTS.Add(sTUDENT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DEPARTMENT_ID = new SelectList(db.DEPARTMENTS, "DEP_ID", "DEP_NAME", sTUDENT.DEPARTMENT_ID);
            ViewBag.USER_ID = new SelectList(db.USERS, "ID", "EMAIL", sTUDENT.USER_ID);
            return View(sTUDENT);
        }

        // GET: STUDENT/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STUDENT sTUDENT = db.STUDENTS.Find(id);
            if (sTUDENT == null)
            {
                return HttpNotFound();
            }
            ViewBag.DEPARTMENT_ID = new SelectList(db.DEPARTMENTS, "DEP_ID", "DEP_NAME", sTUDENT.DEPARTMENT_ID);
            ViewBag.USER_ID = new SelectList(db.USERS, "ID", "EMAIL", sTUDENT.USER_ID);
            return View(sTUDENT);
        }

        // POST: STUDENT/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ROLL_NO,USER_ID,DEPARTMENT_ID")] STUDENT sTUDENT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sTUDENT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DEPARTMENT_ID = new SelectList(db.DEPARTMENTS, "DEP_ID", "DEP_NAME", sTUDENT.DEPARTMENT_ID);
            ViewBag.USER_ID = new SelectList(db.USERS, "ID", "EMAIL", sTUDENT.USER_ID);
            return View(sTUDENT);
        }

        // GET: STUDENT/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STUDENT sTUDENT = db.STUDENTS.Find(id);
            if (sTUDENT == null)
            {
                return HttpNotFound();
            }
            return View(sTUDENT);
        }

        // POST: STUDENT/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            STUDENT sTUDENT = db.STUDENTS.Find(id);
            db.STUDENTS.Remove(sTUDENT);
            db.SaveChanges();
            return RedirectToAction("Index");
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