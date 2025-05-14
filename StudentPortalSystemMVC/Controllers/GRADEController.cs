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
    public class GRADEController : Controller
    {
        private StudentPortalSystemEntities db = new StudentPortalSystemEntities();

        // GET: GRADE
        public ActionResult Index()
        {
            var gRADES = db.GRADES.Include(g => g.ENROLLMENT);
            return View(gRADES.ToList());
        }

        // GET: GRADE/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GRADE gRADE = db.GRADES.Find(id);
            if (gRADE == null)
            {
                return HttpNotFound();
            }
            return View(gRADE);
        }

        // GET: GRADE/Create
        public ActionResult Create()
        {
            ViewBag.ENROLLMENT_ID = new SelectList(db.ENROLLMENTS, "ENROLLMENT_ID", "ENROLLMENT_ID");
            return View();
        }

        // POST: GRADE/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GRADE_ID,ENROLLMENT_ID,MARKS,GRADE1")] GRADE gRADE)
        {
            if (ModelState.IsValid)
            {
                db.GRADES.Add(gRADE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ENROLLMENT_ID = new SelectList(db.ENROLLMENTS, "ENROLLMENT_ID", "ENROLLMENT_ID", gRADE.ENROLLMENT_ID);
            return View(gRADE);
        }

        // GET: GRADE/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GRADE gRADE = db.GRADES.Find(id);
            if (gRADE == null)
            {
                return HttpNotFound();
            }
            ViewBag.ENROLLMENT_ID = new SelectList(db.ENROLLMENTS, "ENROLLMENT_ID", "ENROLLMENT_ID", gRADE.ENROLLMENT_ID);
            return View(gRADE);
        }

        // POST: GRADE/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GRADE_ID,ENROLLMENT_ID,MARKS,GRADE1")] GRADE gRADE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gRADE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ENROLLMENT_ID = new SelectList(db.ENROLLMENTS, "ENROLLMENT_ID", "ENROLLMENT_ID", gRADE.ENROLLMENT_ID);
            return View(gRADE);
        }

        // GET: GRADE/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GRADE gRADE = db.GRADES.Find(id);
            if (gRADE == null)
            {
                return HttpNotFound();
            }
            return View(gRADE);
        }

        // POST: GRADE/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GRADE gRADE = db.GRADES.Find(id);
            db.GRADES.Remove(gRADE);
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
