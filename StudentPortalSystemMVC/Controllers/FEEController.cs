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
    public class FEEController : Controller
    {
        private StudentPortalSystemEntities db = new StudentPortalSystemEntities();

        // GET: FEE
        public ActionResult Index()
        {
            var fEES = db.FEES.Include(f => f.COURS).Include(f => f.STUDENT);
            return View(fEES.ToList());
        }

        // GET: FEE/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FEE fEE = db.FEES.Find(id);
            if (fEE == null)
            {
                return HttpNotFound();
            }
            return View(fEE);
        }

        // GET: FEE/Create
        public ActionResult Create()
        {
            ViewBag.COURSE_ID = new SelectList(db.COURSES, "COURSE_ID", "COURSE_NAME");
            ViewBag.STUDENT_ID = new SelectList(db.STUDENTS, "ROLL_NO", "ROLL_NO");
            return View();
        }

        // POST: FEE/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FEE_ID,STUDENT_ID,COURSE_ID,AMOUNT,FEE_STATUS")] FEE fEE)
        {
            if (ModelState.IsValid)
            {
                db.FEES.Add(fEE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.COURSE_ID = new SelectList(db.COURSES, "COURSE_ID", "COURSE_NAME", fEE.COURSE_ID);
            ViewBag.STUDENT_ID = new SelectList(db.STUDENTS, "ROLL_NO", "ROLL_NO", fEE.STUDENT_ID);
            return View(fEE);
        }

        // GET: FEE/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FEE fEE = db.FEES.Find(id);
            if (fEE == null)
            {
                return HttpNotFound();
            }
            ViewBag.COURSE_ID = new SelectList(db.COURSES, "COURSE_ID", "COURSE_NAME", fEE.COURSE_ID);
            ViewBag.STUDENT_ID = new SelectList(db.STUDENTS, "ROLL_NO", "ROLL_NO", fEE.STUDENT_ID);
            return View(fEE);
        }

        // POST: FEE/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FEE_ID,STUDENT_ID,COURSE_ID,AMOUNT,FEE_STATUS")] FEE fEE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fEE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.COURSE_ID = new SelectList(db.COURSES, "COURSE_ID", "COURSE_NAME", fEE.COURSE_ID);
            ViewBag.STUDENT_ID = new SelectList(db.STUDENTS, "ROLL_NO", "ROLL_NO", fEE.STUDENT_ID);
            return View(fEE);
        }

        // GET: FEE/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FEE fEE = db.FEES.Find(id);
            if (fEE == null)
            {
                return HttpNotFound();
            }
            return View(fEE);
        }

        // POST: FEE/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FEE fEE = db.FEES.Find(id);
            db.FEES.Remove(fEE);
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
