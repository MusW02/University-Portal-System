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
    public class ATTENDANCEController : Controller
    {
        private StudentPortalSystemEntities db = new StudentPortalSystemEntities();

        // GET: ATTENDANCE
        public ActionResult Index()
        {
            var aTTENDANCEs = db.ATTENDANCEs.Include(a => a.ENROLLMENT);
            return View(aTTENDANCEs.ToList());
        }

        // GET: ATTENDANCE/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATTENDANCE aTTENDANCE = db.ATTENDANCEs.Find(id);
            if (aTTENDANCE == null)
            {
                return HttpNotFound();
            }
            return View(aTTENDANCE);
        }

        // GET: ATTENDANCE/Create
        public ActionResult Create()
        {
            ViewBag.ENROLLMENT_ID = new SelectList(db.ENROLLMENTS, "ENROLLMENT_ID", "ENROLLMENT_ID");
            return View();
        }

        // POST: ATTENDANCE/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ATTENDANCE_ID,ATTENDANCE_DATE,STATUS,ENROLLMENT_ID")] ATTENDANCE aTTENDANCE)
        {
            if (ModelState.IsValid)
            {
                db.ATTENDANCEs.Add(aTTENDANCE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ENROLLMENT_ID = new SelectList(db.ENROLLMENTS, "ENROLLMENT_ID", "ENROLLMENT_ID", aTTENDANCE.ENROLLMENT_ID);
            return View(aTTENDANCE);
        }

        // GET: ATTENDANCE/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATTENDANCE aTTENDANCE = db.ATTENDANCEs.Find(id);
            if (aTTENDANCE == null)
            {
                return HttpNotFound();
            }
            ViewBag.ENROLLMENT_ID = new SelectList(db.ENROLLMENTS, "ENROLLMENT_ID", "ENROLLMENT_ID", aTTENDANCE.ENROLLMENT_ID);
            return View(aTTENDANCE);
        }

        // POST: ATTENDANCE/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ATTENDANCE_ID,ATTENDANCE_DATE,STATUS,ENROLLMENT_ID")] ATTENDANCE aTTENDANCE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aTTENDANCE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ENROLLMENT_ID = new SelectList(db.ENROLLMENTS, "ENROLLMENT_ID", "ENROLLMENT_ID", aTTENDANCE.ENROLLMENT_ID);
            return View(aTTENDANCE);
        }

        // GET: ATTENDANCE/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATTENDANCE aTTENDANCE = db.ATTENDANCEs.Find(id);
            if (aTTENDANCE == null)
            {
                return HttpNotFound();
            }
            return View(aTTENDANCE);
        }

        // POST: ATTENDANCE/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ATTENDANCE aTTENDANCE = db.ATTENDANCEs.Find(id);
            db.ATTENDANCEs.Remove(aTTENDANCE);
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
