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
using LogicUniversity.Services;

namespace LogicUniversity.Controllers
{
    public class SuppliersController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();

        // GET: Suppliers
        [HttpGet]
        public ActionResult Index(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                List<Supplier> SupplierList = new List<Supplier>(db.Suppliers);
                ViewData["SupplierList"] = SupplierList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost]
        public ActionResult Index(string SupplierId, String SearchString,string sessionId)
        {
            sessionId = Request["sessionId"];
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                if (Request.Form["Edit"] != null)
                {
                    Debug.WriteLine("Edit");
                    Debug.WriteLine(SupplierId);
                    return RedirectToAction("Edit", "Suppliers", new { id = SupplierId ,sessionId=sessionId});
                }
                if (Request.Form["Create"] != null)
                {
                    return RedirectToAction("Create", "Suppliers",new { sessionId = sessionId });
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
                    return RedirectToAction("Delete", "Suppliers", new { id = SupplierId,sessionId=sessionId });
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
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
        public ActionResult Create(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SupplierId,SupplierName,ContactName,Phone,Fax,Address,GSTRegistrationNo")] Supplier supplier,string sessionId)
        {
            sessionId = Request["sessionId"];
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                if (ModelState.IsValid)
                {
                    db.Suppliers.Add(supplier);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { sessionId = sessionId });
                }

                return View(supplier);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(String id,string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
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
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SupplierId,SupplierName,ContactName,Phone,Fax,Address,GSTRegistrationNo")] Supplier supplier,string sessionId)
        {
            sessionId = Request["sessionId"];
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                if (ModelState.IsValid)
                {
                    db.Entry(supplier).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index",new { sessionId = sessionId });
                }
                return View(supplier);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: Suppliers/Delete/5
        public ActionResult Delete(string id,string sessionId)
        {
            //Debug.WriteLine("Delete 1");
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
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
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id,string sessionId)
        {
            sessionId = Request["sessionId"];
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                //Debug.WriteLine("Delete 2");
                Supplier supplier = db.Suppliers.Find(id);
                db.Suppliers.Remove(supplier);
                db.SaveChanges();
                return RedirectToAction("Index",new { sessionId=sessionId});
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
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
