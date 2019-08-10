using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LogicUniversity.Context;
using LogicUniversity.Models;

namespace LogicUniversity.Controllers
{
    public class DisbursementsController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();

        // GET: Disbursements
        public ActionResult Index()
        {
            var disbursements = db.Disbursements.Include(d => d.CollectionPoint).Include(d => d.Representative);
            return View(disbursements.ToList());
        }

        // GET: Disbursements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Disbursement disbursement = db.Disbursements.Find(id);
            if (disbursement == null)
            {
                return HttpNotFound();
            }
            return View(disbursement);
        }

        // GET: Disbursements/Create
        public ActionResult Create(int RetrievalId)
        {
            ViewBag.CollectionPointId = new SelectList(db.CollectionPoints, "CollectionPointId", "LocationName");
            ViewBag.RepresentativeId = new SelectList(db.Employees, "EmployeeId", "EmployeeName");
            return View();
        }

        // POST: Disbursements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DisbursementId,RepresentativeId,DateCreated,DateDisbursed,Status,CollectionPointId")] Disbursement disbursement)
        {
            if (ModelState.IsValid)
            {
                db.Disbursements.Add(disbursement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CollectionPointId = new SelectList(db.CollectionPoints, "CollectionPointId", "LocationName", disbursement.CollectionPointId);
            ViewBag.RepresentativeId = new SelectList(db.Employees, "EmployeeId", "EmployeeName", disbursement.RepresentativeId);
            return View(disbursement);
        }

        // GET: Disbursements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Disbursement disbursement = db.Disbursements.Find(id);
            if (disbursement == null)
            {
                return HttpNotFound();
            }
            ViewBag.CollectionPointId = new SelectList(db.CollectionPoints, "CollectionPointId", "LocationName", disbursement.CollectionPointId);
            ViewBag.RepresentativeId = new SelectList(db.Employees, "EmployeeId", "EmployeeName", disbursement.RepresentativeId);
            return View(disbursement);
        }

        // POST: Disbursements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DisbursementId,RepresentativeId,DateCreated,DateDisbursed,Status,CollectionPointId")] Disbursement disbursement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(disbursement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CollectionPointId = new SelectList(db.CollectionPoints, "CollectionPointId", "LocationName", disbursement.CollectionPointId);
            ViewBag.RepresentativeId = new SelectList(db.Employees, "EmployeeId", "EmployeeName", disbursement.RepresentativeId);
            return View(disbursement);
        }

        // GET: Disbursements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Disbursement disbursement = db.Disbursements.Find(id);
            if (disbursement == null)
            {
                return HttpNotFound();
            }
            return View(disbursement);
        }

        // POST: Disbursements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Disbursement disbursement = db.Disbursements.Find(id);
            db.Disbursements.Remove(disbursement);
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
        [HttpGet]
        public ActionResult DisplayDisbursement(string RetrievalId)
        {
            //split to a list of retrieval
            //among the list of retrievals have a compiled list of retrievaldetails
            //each retrieval should have a list of requisition details.
            Debug.WriteLine(RetrievalId);
            return View();
        }
    }
}
