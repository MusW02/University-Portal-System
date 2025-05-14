using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StudentPortalSystemMVC.Models;

namespace StudentPortalSystemMVC.Controllers
{
    public class AdminController : Controller
    {
        private StudentPortalSystemEntities db = new StudentPortalSystemEntities();

        // GET: Admin (List Users)
        public ActionResult Index()
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            var USER = db.USERS.Where(u => u.ROLE != "Admin").ToList();
            return View(USER);
        }

        // POST: Approve User
        public ActionResult Approve(int? id)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.USERS.Find(id.Value);
            if (user == null)
                return HttpNotFound();

            if (user.STATUS == "Pending")
            {
                user.STATUS = "Approved";
                db.SaveChanges();

                // Create TEACHER or STUDENT record if approved and department is assigned
                if (user.DepartmentId.HasValue)
                {
                    if (user.ROLE == "Teacher")
                    {
                        var teacher = db.TEACHERS.FirstOrDefault(t => t.USER_ID == user.ID);
                        if (teacher == null)
                        {
                            db.TEACHERS.Add(new TEACHER { USER_ID = user.ID, DEPARTMENT_ID = user.DepartmentId.Value });
                            db.SaveChanges();
                        }
                    }
                    else if (user.ROLE == "Student")
                    {
                        var student = db.STUDENTS.FirstOrDefault(s => s.USER_ID == user.ID);
                        if (student == null)
                        {
                            db.STUDENTS.Add(new STUDENT { USER_ID = user.ID, DEPARTMENT_ID = user.DepartmentId.Value });
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    TempData["Warning"] = $"{user.ROLE} {user.USERNAME} approved but needs a department assigned.";
                }
            }

            return RedirectToAction("Index");
        }

        // GET: Edit User
        public ActionResult Edit(int? id)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.USERS.Find(id.Value);
            if (user == null)
                return HttpNotFound();

            ViewBag.Departments = new SelectList(db.DEPARTMENTS, "DEP_ID", "DEP_NAME", user.DepartmentId);
            return View(user);
        }

        // POST: Edit User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,USERNAME,FIRST_NAME,LAST_NAME,EMAIL,CONTACT,ADDRESS,ROLE,STATUS,DepartmentId")] USER user)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            if (ModelState.IsValid)
            {
                var existingUser = db.USERS.Find(user.ID);
                if (existingUser == null)
                    return HttpNotFound();

                existingUser.USERNAME = user.USERNAME;
                existingUser.FIRST_NAME = user.FIRST_NAME;
                existingUser.LAST_NAME = user.LAST_NAME;
                existingUser.EMAIL = user.EMAIL;
                existingUser.CONTACT = user.CONTACT;
                existingUser.ADDRESS = user.ADDRESS;
                existingUser.ROLE = user.ROLE;
                existingUser.STATUS = user.STATUS;
                existingUser.DepartmentId = user.DepartmentId;

                db.Entry(existingUser).State = EntityState.Modified;
                db.SaveChanges();

                // Create STUDENT or TEACHER record if approved and department is assigned
                if (user.STATUS == "Approved" && user.DepartmentId.HasValue)
                {
                    if (user.ROLE == "Student")
                    {
                        var student = db.STUDENTS.FirstOrDefault(s => s.USER_ID == user.ID);
                        if (student == null)
                        {
                            db.STUDENTS.Add(new STUDENT { USER_ID = user.ID, DEPARTMENT_ID = user.DepartmentId.Value });
                            db.SaveChanges();
                        }
                    }
                    else if (user.ROLE == "Teacher")
                    {
                        var teacher = db.TEACHERS.FirstOrDefault(t => t.USER_ID == user.ID);
                        if (teacher == null)
                        {
                            db.TEACHERS.Add(new TEACHER { USER_ID = user.ID, DEPARTMENT_ID = user.DepartmentId.Value });
                            db.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("Index");
            }

            ViewBag.Departments = new SelectList(db.DEPARTMENTS, "DEP_ID", "DEP_NAME", user.DepartmentId);
            return View(user);
        }

        // GET: User Details
        public ActionResult Details(int? id)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.USERS.Find(id.Value);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // GET: Delete User
        public ActionResult Delete(int? id)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.USERS.Find(id.Value);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // POST: Delete User
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            var user = db.USERS.Find(id);
            if (user == null)
                return HttpNotFound();

            // Remove related STUDENT or TEACHER records
            var student = db.STUDENTS.FirstOrDefault(s => s.USER_ID == id);
            if (student != null)
                db.STUDENTS.Remove(student);

            var teacher = db.TEACHERS.FirstOrDefault(t => t.USER_ID == id);
            if (teacher != null)
                db.TEACHERS.Remove(teacher);

            db.USERS.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Manage Course Registration
        public ActionResult ManageCourseRegistration()
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            var status = db.CourseRegistrationStatus.FirstOrDefault();
            return View(status);
        }

        // POST: Toggle Course Registration
        public ActionResult ToggleCourseRegistration(int? id)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var status = db.CourseRegistrationStatus.Find(id.Value);
            if (status != null)
            {
                status.IsOpen = !status.IsOpen;
                db.SaveChanges();
            }
            return RedirectToAction("ManageCourseRegistration");
        }

        //// GET: Add Course
        //public ActionResult AddCourse()
        //{
        //    if (Session["Role"]?.ToString() != "Admin")
        //        return RedirectToAction("Welcome", "Account");

        //    ViewBag.Departments = new SelectList(db.DEPARTMENTS, "DEP_ID", "DEP_NAME");
        //    ViewBag.Teachers = new SelectList(
        //        db.TEACHERS
        //            .Include(t => t.USER)
        //            .Where(t => t.USER.ROLE == "Teacher" && t.USER.STATUS == "Approved"),
        //        "TEACHER_ID",
        //        "USER.USERNAME"
        //    );
        //    return View();
        //}

        //// POST: Add Course
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddCourse(COURS course)
        //{
        //    if (Session["Role"]?.ToString() != "Admin")
        //        return RedirectToAction("Welcome", "Account");

        //    if (ModelState.IsValid)
        //    {
        //        // Ensure TEACHER_ID exists in TEACHERS table
        //        var teacher = db.TEACHERS.Find(course.TEACHER_ID);
        //        if (teacher == null)
        //        {
        //            ModelState.AddModelError("TEACHER_ID", "Selected teacher is invalid.");
        //        }
        //        else if(!course.DepartmentId.HasValue || db.DEPARTMENTS.Find(course.DepartmentId)==null)
        //        {
        //            ModelState.AddModelError("DepartmentId", "Selected department is invalid");
        //        }
        //        else
        //        {
        //            db.COURSES.Add(course);
        //            db.SaveChanges();
        //            return RedirectToAction("CourseList");
        //        }
        //    }

        //    ViewBag.Departments = new SelectList(db.DEPARTMENTS, "DEP_ID", "DEP_NAME", course.DepartmentId);
        //    ViewBag.Teachers = new SelectList(
        //        db.TEACHERS
        //            .Include(t => t.USER)
        //            .Where(t => t.USER.ROLE == "Teacher" && t.USER.STATUS == "Approved"),
        //        "TEACHER_ID",
        //        "USER.USERNAME",
        //        course.TEACHER_ID
        //    );
        //    return View(course);
        //}

        // GET: Admin/AddCourse
        public ActionResult AddCourse()
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            try
            {
                ViewBag.Departments = new SelectList(db.DEPARTMENTS, "DEP_ID", "DEP_NAME");
                ViewBag.Teachers = new SelectList(
                    db.TEACHERS
                        .Include(t => t.USER)
                        .Where(t => t.USER.ROLE == "Teacher" && t.USER.STATUS == "Approved")
                        .Select(t => new
                        {
                            t.TEACHER_ID,
                            FullName = t.USER.FIRST_NAME + " " + (t.USER.LAST_NAME ?? "")
                        }),
                    "TEACHER_ID",
                    "FullName"
                );
                return View(new COURS());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddCourse GET: {ex.Message}");
                TempData["Error"] = "An error occurred while loading the add course page.";
                return RedirectToAction("CourseList");
            }
        }

        // POST: Admin/AddCourse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCourse(COURS course)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            try
            {
                if (ModelState.IsValid)
                {
                    // Ensure TEACHER_ID exists in TEACHERS table
                    var teacher = db.TEACHERS.Find(course.TEACHER_ID);
                    if (teacher == null)
                    {
                        ModelState.AddModelError("TEACHER_ID", "Selected teacher is invalid.");
                    }
                    else if (!course.DepartmentId.HasValue || db.DEPARTMENTS.Find(course.DepartmentId) == null)
                    {
                        ModelState.AddModelError("DepartmentId", "Selected department is invalid.");
                    }
                    else
                    {
                        db.COURSES.Add(course);
                        db.SaveChanges();
                        TempData["Success"] = "Course added successfully.";
                        return RedirectToAction("AddCourse"); // Redirect to GET to reset the form
                    }
                }

                // Repopulate dropdowns if validation fails
                ViewBag.Departments = new SelectList(db.DEPARTMENTS, "DEP_ID", "DEP_NAME", course.DepartmentId);
                ViewBag.Teachers = new SelectList(
                    db.TEACHERS
                        .Include(t => t.USER)
                        .Where(t => t.USER.ROLE == "Teacher" && t.USER.STATUS == "Approved")
                        .Select(t => new
                        {
                            t.TEACHER_ID,
                            FullName = t.USER.FIRST_NAME + " " + (t.USER.LAST_NAME ?? "")
                        }),
                    "TEACHER_ID",
                    "FullName",
                    course.TEACHER_ID
                );

                TempData["Error"] = "Please correct the errors and try again.";
                return View(course);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddCourse POST: {ex.Message}");
                TempData["Error"] = "An error occurred while adding the course.";

                // Repopulate dropdowns in case of an error
                ViewBag.Departments = new SelectList(db.DEPARTMENTS, "DEP_ID", "DEP_NAME", course.DepartmentId);
                ViewBag.Teachers = new SelectList(
                    db.TEACHERS
                        .Include(t => t.USER)
                        .Where(t => t.USER.ROLE == "Teacher" && t.USER.STATUS == "Approved")
                        .Select(t => new
                        {
                            t.TEACHER_ID,
                            FullName = t.USER.FIRST_NAME + " " + (t.USER.LAST_NAME ?? "")
                        }),
                    "TEACHER_ID",
                    "FullName",
                    course.TEACHER_ID
                );

                return View(course);
            }
        }

        // GET: Course List
        public ActionResult CourseList()
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            var courses = db.COURSES.Include(c => c.TEACHER).ToList();
            return View(courses);
        }

        // GET: Department List
        public ActionResult DepartmentList()
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            var departments = db.DEPARTMENTS.ToList();
            return View(departments);
        }

        // GET: Add Department
        public ActionResult AddDepartment()
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            return View(new DEPARTMENT());
        }

        // POST: Add Department
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDepartment(DEPARTMENT department)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            if (ModelState.IsValid)
            {
                db.DEPARTMENTS.Add(department);
                db.SaveChanges();
                return RedirectToAction("DepartmentList");
            }
            return View(department);
        }

        // GET: Edit Department
        public ActionResult EditDepartment(int? id)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var department = db.DEPARTMENTS.Find(id.Value);
            if (department == null)
                return HttpNotFound();

            return View(department);
        }

        // POST: Edit Department
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDepartment(DEPARTMENT department)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DepartmentList");
            }
            return View(department);
        }

        // GET: Delete Department
        public ActionResult DeleteDepartment(int? id)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var department = db.DEPARTMENTS.Find(id.Value);
            if (department == null)
                return HttpNotFound();

            return View(department);
        }

        // POST: Delete Department
        [HttpPost, ActionName("DeleteDepartment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDepartmentConfirmed(int id)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            var department = db.DEPARTMENTS.Find(id);
            if (department == null)
                return HttpNotFound();

            db.DEPARTMENTS.Remove(department);
            db.SaveChanges();
            return RedirectToAction("DepartmentList");
        }

        // GET: Assign Department
        public ActionResult AssignDepartment(int? id)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.USERS.Find(id.Value);
            if (user == null)
                return HttpNotFound();

            ViewBag.Departments = new SelectList(db.DEPARTMENTS, "DEP_ID", "DEP_NAME");
            return View(user);
        }

        // POST: Assign Department
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignDepartment(USER user)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Welcome", "Account");

            var existingUser = db.USERS.Find(user.ID);
            if (existingUser == null)
                return HttpNotFound();

            existingUser.DepartmentId = user.DepartmentId;
            db.SaveChanges();

            // Create TEACHER or STUDENT record if approved and department assigned
            if (existingUser.STATUS == "Approved" && existingUser.DepartmentId.HasValue)
            {
                if (existingUser.ROLE == "Teacher")
                {
                    var teacher = db.TEACHERS.FirstOrDefault(t => t.USER_ID == existingUser.ID);
                    if (teacher == null)
                    {
                        db.TEACHERS.Add(new TEACHER { USER_ID = existingUser.ID, DEPARTMENT_ID = existingUser.DepartmentId.Value });
                        db.SaveChanges();
                    }
                }
                else if (existingUser.ROLE == "Student")
                {
                    var student = db.STUDENTS.FirstOrDefault(s => s.USER_ID == existingUser.ID);
                    if (student == null)
                    {
                        db.STUDENTS.Add(new STUDENT { USER_ID = existingUser.ID, DEPARTMENT_ID = existingUser.DepartmentId.Value });
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}