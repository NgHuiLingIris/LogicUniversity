using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
    public class RequisitionApprovalsController : Controller
    {//this is storeclerk view
        private LogicUniversityContext db = new LogicUniversityContext();

        // GET: Requisitions
        public ActionResult ViewRequisition()
        {
            var username = Session["UserID"].ToString();
            Employee obj = db.Employees.Where(a => a.Username.Equals(username)).FirstOrDefault();
            return View(db.Requisition.Where(x => x.ApproverId == obj.EmployeeId && x.Status == "PENDING" && x.Status == "OUTSTANDING").ToList());
        }

        public ActionResult ViewAllRequisition()
        {
            var username = Session["UserID"].ToString();
            Employee obj = db.Employees.Where(a => a.Username.Equals(username)).FirstOrDefault();
            return View(db.Requisition.Where(x => x.ApproverId == obj.EmployeeId).ToList());
        }

        //Requisition ALL Details for Store Clerk
        [HttpGet]
        public ActionResult SCRequisitionView()
        {
            //
            //should be grouped by department

            List<Requisition> reqListAll = db.Requisition.Include(s => s.RequisitionDetails).Where(s => s.Status == "PENDING").OrderByDescending(s => s.Date).ToList();
            List<Requisition> reqByDept = new List<Requisition>();
            foreach (Requisition r in reqListAll)
            {
                Employee e = db.Employees.FirstOrDefault(a => a.EmployeeId == r.EmployeeId);
                r.Employee = e;
                r.Employee.Department = db.Departments.FirstOrDefault(c=>c.DeptId == e.DeptId);
                r.Department = r.Employee.Department.DeptName;
                string deptName = r.Department;
                //var d0 = db.Departments.FirstOrDefault(a => a.DeptName == deptName);
                //Department d1 = d0;
                //retrieve employee here
                //Employee e = db.Employees.FirstOrDefault(a => a.EmployeeId == r.EmployeeId);
                

                if (!reqByDept.Any(s => s.Department.Contains(r.Department)))
                {
                    reqByDept.Add(r);
                }
            }
            //filter the top

            List<Requisition> reqList = new List<Requisition>();
            foreach (Requisition req in reqByDept)
            {
                reqList.Add(req);
            }
            //reqList = db.Requisition.ToList();
            int count = reqByDept.Count();
            ViewData["count"] = count;
            ViewData["reqList"] = reqList;

            return View(reqList);
        }
        [HttpPost]
        public ActionResult SCRequisitionView(FormCollection form)
        {
            List<Requisition> reqList = new List<Requisition>();
            if (Request.Form["search"] != null)
            {
                string fromdate = Request.Form["fromdate"];
                string cp = Request.Form["cp"];
                string todate = Request.Form["todate"];
                string status = Request.Form["status"];
                //check the search then bring it over to the logic
                //check if this redirect action actually brings back to same list
                reqList = Search(fromdate, todate, cp, status);
            }
            else if (Request.Form["retrieve"] != null)
            {
                List<Requisition> SelectedRequests = new List<Requisition>();
                int count = int.Parse(Request.Form["count"]);
                //for(int i = 0; i<)
                string PendingDeptRequisition = ",";
                for (int i = 0; i < count; i++)
                {
                    if (Request.Form["Requisition[" + i + "].toretrieve"] != null)
                    {
                        string dept = Request.Form["Requisition[" + i + "].toretrieve"];
                        //Department d0 = new Department();
                        //d0 = db.Departments.FirstOrDefault(d => d.DeptName == dept);
                        PendingDeptRequisition = PendingDeptRequisition + dept + ",";
                        //retrieve the requisition id
                        //redirect action to create retrieval

                    }
                    //string checking = Request.Form["Requisition[0].toretrieve"];//on
                    //string checking1 = Request.Form["Requisition[1].toretrieve"];//null
                }

                return RedirectToAction("Create", "Retrievals", new { PendingDeptRequisition = PendingDeptRequisition });

                //Debug.WriteLine("");
            }

            ViewData["reqList"] = reqList;

            return View();
        }

        public ActionResult TrackRequisition(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                var username = Session["UserID"].ToString();
                Employee obj = db.Employees.Where(a => a.Username.Equals(username)).FirstOrDefault();
                return View(db.Requisition.Where(x => x.EmployeeId == obj.EmployeeId).ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }


        // GET: Requisitions/Details/5
        public ActionResult Details(int? id, string status, string Remarks)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<RequisitionDetails> requisitionDetails = db.RequisitionDetails.Where(x => x.RequisitionDetailsId == id).ToList();
            if (id != 0 && status != null)
            {
                Requisition requisition = db.Requisition.Where(x => x.RequisitionId == id).FirstOrDefault();

                requisition.Status = status;
                requisition.Remarks = Remarks;

                db.Requisition.AddOrUpdate(requisition);
                db.SaveChanges();
                EmailService.SendNotification(requisition.EmployeeId, requisition.Status, "Your request is" + requisition.Status);
                return View("ViewRequisition");
            }

            if (requisitionDetails == null)
            {
                return HttpNotFound();
            }
            return View(requisitionDetails);
        }

        public ActionResult TrackDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<RequisitionDetails> requisitionDetails = db.RequisitionDetails.Where(x => x.RequisitionDetailsId == id).ToList();

            if (requisitionDetails == null)
            {
                return HttpNotFound();
            }
            return View(requisitionDetails);
        }

        public List<Requisition> Search(string fromdate, string todate, string cp, string status)
        {
            //PENDING1
            //ignore collection point first. there should be a foreign key that links with department.
            //if status is null, query does not work. all are compulsatory fields
            DateTime startdate = Convert.ToDateTime(fromdate);
            DateTime enddate = Convert.ToDateTime(todate);

            List<Requisition> searchList = new List<Requisition>();
            searchList = db.Requisition.Where(i => i.Status == status)
                .Where(j => j.Date >= startdate && j.Date <= enddate).Include(k => k.RequisitionDetails).ToList();

            return searchList;
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
