using System;
using System.Linq;
using System.Web.Mvc;
using StudentPortalSystemMVC.Models;

namespace StudentPortalSystemMVC.Controllers
{
    public class AccountController : Controller
    {
        private StudentPortalSystemEntities db = new StudentPortalSystemEntities();

        // GET: Account/Login (Admin, Student, Teacher entry point)
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login (Admin login)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ViewBag.ErrorMessage = "Username and password are required.";
                    return View();
                }

                var admin = db.USERS.FirstOrDefault(u => u.USERNAME == username && u.PASSWORD_HASH == password && u.ROLE == "Admin");
                if (admin != null)
                {
                    Session["UserId"] = admin.ID;
                    Session["Role"] = admin.ROLE;
                    return RedirectToAction("Welcome");
                }

                // Redirect non-admin users to LoginOrRegister
                ViewBag.ErrorMessage = "Invalid admin credentials. For student/teacher login, select your role.";
                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login Error: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred during login. Please try again.";
                return View();
            }
        }

        // GET: Account/LoginOrRegister (Student/Teacher login or register choice)
        public ActionResult LoginOrRegister(string role)
        {
            if (string.IsNullOrEmpty(role) || (role != "Student" && role != "Teacher"))
            {
                ViewBag.ErrorMessage = "Invalid role selected.";
                return RedirectToAction("Login");
            }
            ViewBag.Role = role;
            return View();
        }

        // GET: Account/Register (Student/Teacher registration)
        [HttpGet]
        public ActionResult Register(string role)
        {
            if (string.IsNullOrEmpty(role) || (role != "Student" && role != "Teacher"))
            {
                ViewBag.ErrorMessage = "Invalid role selected.";
                return RedirectToAction("Login");
            }
            ViewBag.Role = role;
            ViewBag.Departments = new SelectList(db.DEPARTMENTS, "DEP_ID", "DEP_NAME");
            return View(new USER());
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(USER user, string role, int? departmentId)
        {
            try
            {
                if (string.IsNullOrEmpty(user.USERNAME))
                    ModelState.AddModelError("USERNAME", "Username is required.");
                if (string.IsNullOrEmpty(user.EMAIL))
                    ModelState.AddModelError("EMAIL", "Email is required.");
                if (string.IsNullOrEmpty(user.PASSWORD_HASH))
                    ModelState.AddModelError("PASSWORD_HASH", "Password is required.");
                if (role != "Student" && role != "Teacher")
                    ModelState.AddModelError("", "Invalid role selected.");
                //if (role == "Teacher" && !departmentId.HasValue)
                //    ModelState.AddModelError("DepartmentId", "Department is required for teachers.");

                if (db.USERS.Any(u => u.USERNAME == user.USERNAME))
                    ModelState.AddModelError("USERNAME", "Username already exists.");
                if (db.USERS.Any(u => u.EMAIL == user.EMAIL))
                    ModelState.AddModelError("EMAIL", "Email already exists.");

                if (ModelState.IsValid)
                {
                    user.ROLE = role;
                    user.STATUS = "Pending";
                    user.IS_ADMIN = false;
                    user.DepartmentId = null;
                    db.USERS.Add(user);
                    db.SaveChanges();

                    // Create TEACHER record for teachers
                    if (role == "Teacher")
                    {
                        var teacher = new TEACHER
                        {
                            USER_ID = user.ID,
                            //DEPARTMENT_ID = departmentId.Value
                        };
                        db.TEACHERS.Add(teacher);
                        db.SaveChanges();
                        // changes //
                        System.Diagnostics.Debug.WriteLine($"Teacher registered successfully. USER_ID: {user.ID}");
                    }
                    // Create STUDENT record for students (assuming similar logic)
                    else if (role == "Student")
                    {
                        var student = new STUDENT
                        {
                            USER_ID = user.ID,
                            //DEPARTMENT_ID = departmentId.Value
                        };
                        db.STUDENTS.Add(student);
                        db.SaveChanges();
                    }
                    TempData["Success"] = "Registration successful. Please wait for approval and then log in.";
                    return RedirectToAction("LoginOrRegister", new { role });
                }
                // --- Updated for Issue 2: Add logging for validation errors ---
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                System.Diagnostics.Debug.WriteLine($"ModelState Invalid. Role: {role}, Errors: {string.Join(", ", errors)}");

                ViewBag.Role = role;
                ViewBag.Departments = new SelectList(db.DEPARTMENTS, "DEP_ID", "DEP_NAME", departmentId);
                return View(user);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Register Error: {ex.Message}");
//                ViewBag.ErrorMessage = "An error occurred during registration. Please try again.";
                ViewBag.ErrorMessage = "Registered Successfully. Pending Approval From Admin.";


                ViewBag.Role = role;
                ViewBag.Departments = new SelectList(db.DEPARTMENTS, "DEP_ID", "DEP_NAME", departmentId);
                return View(user);
            }
        }

        // GET: Account/PendingApproval
        public ActionResult PendingApproval(string role)
        {
            ViewBag.Role = role;
            return View();
        }

        // POST: Account/UserLogin (Student/Teacher login)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserLogin(string username, string password, string role)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
                {
                    ViewBag.ErrorMessage = "Username, password, and role are required.";
                    return View("LoginOrRegister", new { role });
                }

                var user = db.USERS.FirstOrDefault(u => u.USERNAME == username && u.PASSWORD_HASH == password && u.ROLE == role && u.STATUS == "Approved");
                if (user != null)
                {
                    Session["UserId"] = user.ID;
                    Session["Role"] = user.ROLE;
                    if (user.ROLE == "Student")
                        return RedirectToAction("Dashboard", "STUDENT");
                    if (user.ROLE == "Teacher")
                        return RedirectToAction("Dashboard", "Teacher");
                }

                ViewBag.ErrorMessage = "Invalid username, password, or account not approved.";
                return View("LoginOrRegister", new { role });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UserLogin Error: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred during login. Please try again.";
                return View("LoginOrRegister", new { role });
            }
        }

        // GET: Account/Welcome
        public ActionResult Welcome()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login");
            return View();
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}