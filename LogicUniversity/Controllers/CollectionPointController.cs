using LogicUniversity.Context;
using LogicUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogicUniversity.Controllers
{
    public class CollectionPointController : Controller
    {


        private LogicUniversityContext db = new LogicUniversityContext();

        public ActionResult ManageCollection()
        {
            return View();
        }



        //Selecting the new collection point
        public ActionResult UpdateCollectionPoint()
        {
            int empId = (int)Session["empId"];
            var deptId = db.Employees.Where(r => r.EmployeeId == empId).Select(r => r.Department.DeptId).SingleOrDefault();

            //extracting the collection point details of the department whom the employee belongs to
            var collectionPoint = db.Departments.Where(r => r.DeptId == deptId).Select(r => r.CollectionLocationId).SingleOrDefault();
            var cp = db.CollectionPoints.ToList();

            //gets the current collection point name
            var currentCollectionPoint = db.CollectionPoints.Where(r => r.CollectionPointId.Equals(collectionPoint)).Select(r => r.LocationName).SingleOrDefault();


            ViewData["currentCollectionPoint"] = currentCollectionPoint;
            ViewData["list"] = cp;
            ViewData["deptid"] = deptId;
            return View(cp);
        }



        //Updates the database Department 
        [HttpPost]
        public ActionResult SaveChangedCollectionPoint(string selection, int deptid)
        {
            Department cp = db.Departments.Where(d => d.DeptId == deptid).FirstOrDefault();
            //set to the new collectionpoint
            cp.CollectionLocationId = selection.ToString();
            db.SaveChanges();
            return RedirectToAction("Display", "CollectionPoint");
        }




        public ActionResult UpdateRepresentative()
        {
            int empId = (int)Session["empId"];
            var deptId = db.Employees.Where(r => r.EmployeeId == empId).Select(r => r.Department.DeptId).SingleOrDefault();

            //extracting the dep rep details of the department whom the employee belongs to
            var currentRepresentative = db.Employees.Where(r => r.DeptId == deptId && r.Role == "DEP_REP").Select(x => x.EmployeeName).SingleOrDefault();

            ViewData["representative"] = currentRepresentative;

            //Only taking employees from the database,thereby excluding HOD and others
            var departmentRepresentatives = db.Employees.Where(r => r.DeptId == deptId && r.Role == "DEP_STAFF");

            ViewBag.EmployeeId = new SelectList(departmentRepresentatives, "EmployeeId", "EmployeeName");

            ViewData["deptid"] = deptId;
            return View(departmentRepresentatives.ToList());
        }



        //Saving to database Employee
        [HttpPost]
        public ActionResult SaveChangedDepartmentrepresentative(Employee emp, string name)
        {
            int empId = (int)Session["empId"];
            var deptId = db.Employees.Where(r => r.EmployeeId == empId).Select(r => r.Department.DeptId).SingleOrDefault();
            var rep = db.Employees.Where(r => r.DeptId == deptId && r.Role == "DEP_REP").SingleOrDefault();
            if (ModelState.IsValid)
            {
                rep.Role = "DEP_STAFF";
                Employee newrep = db.Employees.Find(emp.EmployeeId);
                newrep.Role = "DEP_REP";
                db.SaveChanges();
                return RedirectToAction("Display", "CollectionPoint");
            }


            return RedirectToAction("UpdateRepresentative", "CollectionPoint");


        }

        public ActionResult Display()
        {

            int empId = (int)Session["empId"];
            var deptId = db.Employees.Where(r => r.EmployeeId == empId).Select(r => r.Department.DeptId).SingleOrDefault();
            var currentRepresentative = db.Employees.Where(r => r.DeptId == deptId && r.Role == "DEP_REP").Select(x => x.EmployeeName).SingleOrDefault();
            var collectionPoint = db.Departments.Where(r => r.DeptId == deptId).Select(r => r.CollectionLocationId).SingleOrDefault();
            var currentCollectionPoint = db.CollectionPoints.Where(r => r.CollectionPointId.Equals(collectionPoint)).Select(r => r.LocationName).SingleOrDefault();
            ViewData["rep"] = currentRepresentative;
            ViewData["cp"] = currentCollectionPoint;
            return View();
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