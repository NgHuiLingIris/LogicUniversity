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
    public class SuppliersController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();

        // GET: Suppliers
        [HttpGet]
        public ActionResult Index()
        {
            List<Supplier> SupplierList = new List<Supplier>(db.Suppliers);
            ViewData["SupplierList"] = SupplierList;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string SupplierId, String SearchString)
        {
            if (Request.Form["Edit"] != null)
            {
                Debug.WriteLine("Edit");
                Debug.WriteLine(SupplierId);
                return RedirectToAction("Edit", "Suppliers", new { id = SupplierId });
            }
            if (Request.Form["Create"] != null)
            {
                return RedirectToAction("Create", "Suppliers");
            }
            if (Request.Form["Search"] != null)
            {
                List<Supplier> SupplierListAll = new List<Supplier>(db.Suppliers);
                List<Supplier> SupplierList = new List<Supplier>();
                var FilterSuppliers = SupplierListAll.Where(s => s.SupplierId.Contains(SearchString));
               
                
                foreach (var Supplier in FilterSuppliers)
                    SupplierList.Add(Supplier);

                ViewData["SupplierList"] = SupplierList;
                return View();
            }
            else if (Request.Form["Delete"] != null)
            {
                Debug.WriteLine("Delete");
                Debug.WriteLine(SupplierId);
                return RedirectToAction("Delete", "Suppliers", new { id = SupplierId });
            }
            return View();
        }

        // GET: Suppliers/Details/5
        public ActionResult Details(string id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // GET: Suppliers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SupplierId,SupplierName,ContactName,Phone,Fax,Address,GSTRegistrationNo")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                db.Suppliers.Add(supplier);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(String id)
        {
            //string id = Request["SupplierId"];
            Debug.WriteLine(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SupplierId,SupplierName,ContactName,Phone,Fax,Address,GSTRegistrationNo")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public ActionResult Delete(string id)
        {
            Debug.WriteLine("Delete 1");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Debug.WriteLine("Delete 2");
            Supplier supplier = db.Suppliers.Find(id);
            db.Suppliers.Remove(supplier);
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
