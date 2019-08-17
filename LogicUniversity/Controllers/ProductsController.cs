using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LogicUniversity.Context;
using LogicUniversity.Models;
using LogicUniversity.Services;
using PagedList;
using PagedList.Mvc;

namespace LogicUniversity.Controllers
{
    public class ProductsController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();

        // GET: Products
        public ActionResult Index(string sessionId,string Role,int ?page)
        {

            int empId = (int)Session["empId"];
            ViewData["role"] = db.Employees.Where(r => r.EmployeeId == empId).Select(r => r.Role).SingleOrDefault();
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                return View(db.Products.ToList().ToPagedList(page ?? 1, 5));
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }            
        }

        // GET: Products/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: Products/Create
        public ActionResult Create(string sessionId)
        {
            int empId = (int)Session["empId"];
            ViewData["role"] = db.Employees.Where(r => r.EmployeeId == empId).Select(r => r.Role).SingleOrDefault();
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

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemCode,Category,Description,ReorderLevel,ReorderQty,UOM")] Products products,string sessionId)
        {
            sessionId = Request["sessionId"];
            int empId = (int)Session["empId"];
            ViewData["role"] = db.Employees.Where(r => r.EmployeeId == empId).Select(r => r.Role).SingleOrDefault();
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                if (ModelState.IsValid)
                {
                    db.Products.Add(products);
                    db.SaveChanges();
                    return RedirectToAction("Index",new { sessionId = sessionId });
                }

                return View(products);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: Products/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemCode,Category,Description,ReorderLevel,ReorderQty,UOM")] Products products)
        {
            if (ModelState.IsValid)
            {
                db.Entry(products).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(products);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Products products = db.Products.Find(id);
            db.Products.Remove(products);
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
