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
    public class COURSController : Controller
    {
        private StudentPortalSystemEntities db = new StudentPortalSystemEntities();

        // GET: COURS
        public ActionResult Index()
        {
            var cOURSES = db.COURSES.Include(c => c.TEACHER);
            return View(cOURSES.ToList());
        }

        // GET: COURS/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            COURS cOURS = db.COURSES.Find(id);
            if (cOURS == null)
            {
                return HttpNotFound();
            }
            return View(cOURS);
        }

        // GET: COURS/Create
        public ActionResult Create()
        {
            ViewBag.TEACHER_ID = new SelectList(db.TEACHERS, "TEACHER_ID", "TEACHER_ID");
            return View();
        }

        // POST: COURS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "COURSE_ID,COURSE_NAME,CREDITS,TEACHER_ID,AMOUNT_PER_COURSE")] COURS cOURS)
        {
            if (ModelState.IsValid)
            {
                db.COURSES.Add(cOURS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TEACHER_ID = new SelectList(db.TEACHERS, "TEACHER_ID", "TEACHER_ID", cOURS.TEACHER_ID);
            return View(cOURS);
        }

        // GET: COURS/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            COURS cOURS = db.COURSES.Find(id);
            if (cOURS == null)
            {
                return HttpNotFound();
            }
            ViewBag.TEACHER_ID = new SelectList(db.TEACHERS, "TEACHER_ID", "TEACHER_ID", cOURS.TEACHER_ID);
            return View(cOURS);
        }

        // POST: COURS/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "COURSE_ID,COURSE_NAME,CREDITS,TEACHER_ID,AMOUNT_PER_COURSE")] COURS cOURS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cOURS).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TEACHER_ID = new SelectList(db.TEACHERS, "TEACHER_ID", "TEACHER_ID", cOURS.TEACHER_ID);
            return View(cOURS);
        }

        // GET: COURS/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            COURS cOURS = db.COURSES.Find(id);
            if (cOURS == null)
            {
                return HttpNotFound();
            }
            return View(cOURS);
        }

        // POST: COURS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            COURS cOURS = db.COURSES.Find(id);
            db.COURSES.Remove(cOURS);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);}

            
    
    }
}
