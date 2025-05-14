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
    public class TRANSCRIPTController : Controller
    {
        private StudentPortalSystemEntities db = new StudentPortalSystemEntities();

        // GET: TRANSCRIPT
        public ActionResult Index()
        {
            var tRANSCRIPTs = db.TRANSCRIPTs.Include(t => t.COURS).Include(t => t.GRADE).Include(t => t.STUDENT);
            return View(tRANSCRIPTs.ToList());
        }

        // GET: TRANSCRIPT/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRANSCRIPT tRANSCRIPT = db.TRANSCRIPTs.Find(id);
            if (tRANSCRIPT == null)
            {
                return HttpNotFound();
            }
            return View(tRANSCRIPT);
        }

        // GET: TRANSCRIPT/Create
        public ActionResult Create()
        {
            ViewBag.COURSE_ID = new SelectList(db.COURSES, "COURSE_ID", "COURSE_NAME");
            ViewBag.GRADE_ID = new SelectList(db.GRADES, "GRADE_ID", "GRADE1");
            ViewBag.STUDENT_ID = new SelectList(db.STUDENTS, "ROLL_NO", "ROLL_NO");
            return View();
        }

        // POST: TRANSCRIPT/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TRANSCRIPT_ID,COURSE_ID,STUDENT_ID,GRADE_ID,SGPA,CGPA")] TRANSCRIPT tRANSCRIPT)
        {
            if (ModelState.IsValid)
            {
                db.TRANSCRIPTs.Add(tRANSCRIPT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.COURSE_ID = new SelectList(db.COURSES, "COURSE_ID", "COURSE_NAME", tRANSCRIPT.COURSE_ID);
            ViewBag.GRADE_ID = new SelectList(db.GRADES, "GRADE_ID", "GRADE1", tRANSCRIPT.GRADE_ID);
            ViewBag.STUDENT_ID = new SelectList(db.STUDENTS, "ROLL_NO", "ROLL_NO", tRANSCRIPT.STUDENT_ID);
            return View(tRANSCRIPT);
        }

        // GET: TRANSCRIPT/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRANSCRIPT tRANSCRIPT = db.TRANSCRIPTs.Find(id);
            if (tRANSCRIPT == null)
            {
                return HttpNotFound();
            }
            ViewBag.COURSE_ID = new SelectList(db.COURSES, "COURSE_ID", "COURSE_NAME", tRANSCRIPT.COURSE_ID);
            ViewBag.GRADE_ID = new SelectList(db.GRADES, "GRADE_ID", "GRADE1", tRANSCRIPT.GRADE_ID);
            ViewBag.STUDENT_ID = new SelectList(db.STUDENTS, "ROLL_NO", "ROLL_NO", tRANSCRIPT.STUDENT_ID);
            return View(tRANSCRIPT);
        }

        // POST: TRANSCRIPT/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TRANSCRIPT_ID,COURSE_ID,STUDENT_ID,GRADE_ID,SGPA,CGPA")] TRANSCRIPT tRANSCRIPT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tRANSCRIPT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.COURSE_ID = new SelectList(db.COURSES, "COURSE_ID", "COURSE_NAME", tRANSCRIPT.COURSE_ID);
            ViewBag.GRADE_ID = new SelectList(db.GRADES, "GRADE_ID", "GRADE1", tRANSCRIPT.GRADE_ID);
            ViewBag.STUDENT_ID = new SelectList(db.STUDENTS, "ROLL_NO", "ROLL_NO", tRANSCRIPT.STUDENT_ID);
            return View(tRANSCRIPT);
        }

        // GET: TRANSCRIPT/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRANSCRIPT tRANSCRIPT = db.TRANSCRIPTs.Find(id);
            if (tRANSCRIPT == null)
            {
                return HttpNotFound();
            }
            return View(tRANSCRIPT);
        }

        // POST: TRANSCRIPT/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TRANSCRIPT tRANSCRIPT = db.TRANSCRIPTs.Find(id);
            db.TRANSCRIPTs.Remove(tRANSCRIPT);
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
