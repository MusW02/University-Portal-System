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
    public class ENROLLMENTController : Controller
    {
        private StudentPortalSystemEntities db = new StudentPortalSystemEntities();

        // GET: ENROLLMENT
        public ActionResult Index()
        {
            var eNROLLMENTS = db.ENROLLMENTS.Include(e => e.COURS).Include(e => e.STUDENT);
            return View(eNROLLMENTS.ToList());
        }

        // GET: ENROLLMENT/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ENROLLMENT eNROLLMENT = db.ENROLLMENTS.Find(id);
            if (eNROLLMENT == null)
            {
                return HttpNotFound();
            }
            return View(eNROLLMENT);
        }

        // GET: ENROLLMENT/Create
        public ActionResult Create()
        {
            ViewBag.COURSE_ID = new SelectList(db.COURSES, "COURSE_ID", "COURSE_NAME");
            ViewBag.STUDENT_ID = new SelectList(db.STUDENTS, "ROLL_NO", "ROLL_NO");
            return View();
        }

        // POST: ENROLLMENT/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ENROLLMENT_ID,STUDENT_ID,COURSE_ID")] ENROLLMENT eNROLLMENT)
        {
            if (ModelState.IsValid)
            {
                db.ENROLLMENTS.Add(eNROLLMENT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.COURSE_ID = new SelectList(db.COURSES, "COURSE_ID", "COURSE_NAME", eNROLLMENT.COURSE_ID);
            ViewBag.STUDENT_ID = new SelectList(db.STUDENTS, "ROLL_NO", "ROLL_NO", eNROLLMENT.STUDENT_ID);
            return View(eNROLLMENT);
        }

        // GET: ENROLLMENT/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ENROLLMENT eNROLLMENT = db.ENROLLMENTS.Find(id);
            if (eNROLLMENT == null)
            {
                return HttpNotFound();
            }
            ViewBag.COURSE_ID = new SelectList(db.COURSES, "COURSE_ID", "COURSE_NAME", eNROLLMENT.COURSE_ID);
            ViewBag.STUDENT_ID = new SelectList(db.STUDENTS, "ROLL_NO", "ROLL_NO", eNROLLMENT.STUDENT_ID);
            return View(eNROLLMENT);
        }

        // POST: ENROLLMENT/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ENROLLMENT_ID,STUDENT_ID,COURSE_ID")] ENROLLMENT eNROLLMENT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eNROLLMENT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.COURSE_ID = new SelectList(db.COURSES, "COURSE_ID", "COURSE_NAME", eNROLLMENT.COURSE_ID);
            ViewBag.STUDENT_ID = new SelectList(db.STUDENTS, "ROLL_NO", "ROLL_NO", eNROLLMENT.STUDENT_ID);
            return View(eNROLLMENT);
        }

        // GET: ENROLLMENT/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ENROLLMENT eNROLLMENT = db.ENROLLMENTS.Find(id);
            if (eNROLLMENT == null)
            {
                return HttpNotFound();
            }
            return View(eNROLLMENT);
        }

        // POST: ENROLLMENT/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ENROLLMENT eNROLLMENT = db.ENROLLMENTS.Find(id);
            db.ENROLLMENTS.Remove(eNROLLMENT);
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
