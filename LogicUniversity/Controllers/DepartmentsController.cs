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

namespace LogicUniversity.Controllers
{
    public class DepartmentsController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();

        // GET: Departments
        public ActionResult Index(string sessionId,string role)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                ViewData["role"] = role;
                return View(db.Departments.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: Departments/Details/5
        public ActionResult Details(int? id,string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Department department = db.Departments.Find(id);
                if (department == null)
                {
                    return HttpNotFound();
                }
                return View(department);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: Departments/Create
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

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DeptId,DeptName,CollectionLocationId,ContactName,TelephoneNo,Fax")] Department department,string sessionId)
        {
            if (sessionId == null)
            {
                sessionId = Request["sessionId"];
            }
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                if (ModelState.IsValid)
                {
                    db.Departments.Add(department);
                    db.SaveChanges();
                    return RedirectToAction("Index",new { sessionId = sessionId, role = "StoreClerk" });
                }

                return View(department);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id,string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Department department = db.Departments.Find(id);
                if (department == null)
                {
                    return HttpNotFound();
                }
                return View(department);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeptId,DeptName,CollectionLocationId,ContactName,TelephoneNo,Fax")] Department department,string sessionId)
        {
            if (sessionId == null)
            {
                sessionId = Request["sessionId"];
            }
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                if (ModelState.IsValid)
                {
                    db.Entry(department).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index",new { sessionId = sessionId , role = "StoreClerk" });
                }
                return View(department);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id,string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Department department = db.Departments.Find(id);
                if (department == null)
                {
                    return HttpNotFound();
                }
                return View(department);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id,string sessionId)
        {
            if (sessionId == null)
            {
                sessionId = Request["sessionId"];
            }
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                Department department = db.Departments.Find(id);
                db.Departments.Remove(department);
                db.SaveChanges();
                return RedirectToAction("Index",new { sessionId = sessionId, role = "StoreClerk" });
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
