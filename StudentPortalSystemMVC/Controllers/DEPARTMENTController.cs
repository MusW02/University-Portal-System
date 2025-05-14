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
    public class DEPARTMENTController : Controller
    {
        private StudentPortalSystemEntities db = new StudentPortalSystemEntities();

        // GET: DEPARTMENT
        public ActionResult Index()
        {
            return View(db.DEPARTMENTS.ToList());
        }

        // GET: DEPARTMENT/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEPARTMENT dEPARTMENT = db.DEPARTMENTS.Find(id);
            if (dEPARTMENT == null)
            {
                return HttpNotFound();
            }
            return View(dEPARTMENT);
        }

        // GET: DEPARTMENT/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DEPARTMENT/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DEP_ID,DEP_NAME")] DEPARTMENT dEPARTMENT)
        {
            if (ModelState.IsValid)
            {
                db.DEPARTMENTS.Add(dEPARTMENT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dEPARTMENT);
        }

        // GET: DEPARTMENT/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEPARTMENT dEPARTMENT = db.DEPARTMENTS.Find(id);
            if (dEPARTMENT == null)
            {
                return HttpNotFound();
            }
            return View(dEPARTMENT);
        }

        // POST: DEPARTMENT/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DEP_ID,DEP_NAME")] DEPARTMENT dEPARTMENT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dEPARTMENT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dEPARTMENT);
        }

        // GET: DEPARTMENT/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEPARTMENT dEPARTMENT = db.DEPARTMENTS.Find(id);
            if (dEPARTMENT == null)
            {
                return HttpNotFound();
            }
            return View(dEPARTMENT);
        }

        // POST: DEPARTMENT/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DEPARTMENT dEPARTMENT = db.DEPARTMENTS.Find(id);
            db.DEPARTMENTS.Remove(dEPARTMENT);
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
