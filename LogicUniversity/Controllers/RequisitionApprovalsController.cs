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
        public ActionResult ViewRequisition(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                var username = Session["UserID"].ToString();
                Employee obj = db.Employees.Where(a => a.Username.Equals(username)).FirstOrDefault();
                return View(db.Requisition.Where(x => x.ApproverId == obj.EmployeeId && x.Status == "PENDING").ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult ViewAllRequisition(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                var username = Session["UserID"].ToString();
                Employee obj = db.Employees.Where(a => a.Username.Equals(username)).FirstOrDefault();
                return View(db.Requisition.Where(x => x.ApproverId == obj.EmployeeId).ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        //Requisition ALL Details for Store Clerk
        [HttpGet]
        public ActionResult SCRequisitionView(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                //------------------Prepare SCRequisitionViewPage-------
                List<CollectionPoint> CPList = db.CollectionPoints.ToList();
                CPList.Insert(0, new CollectionPoint());
                List<string> StatusList = new List<string> { "", "PENDING", "OUTSTANDING" };
                ViewData["CPList"] = CPList;
                ViewData["StatusList"] = StatusList;
                //---------------------END HERE----------------------
                List<Requisition> reqListAll = db.Requisition.Include(s => s.RequisitionDetails).Where(s => s.Status == "PENDING" || s.Status == "OUTSTANDING").OrderByDescending(s => s.Date).ToList();
                List<Requisition> reqByDept = new List<Requisition>();
                foreach (Requisition r in reqListAll)
                {
                    Employee e = db.Employees.FirstOrDefault(a => a.EmployeeId == r.EmployeeId);
                    r.Employee = e;
                    r.Employee.Department = db.Departments.FirstOrDefault(c => c.DeptId == e.DeptId);
                    r.Department = r.Employee.Department.DeptName;
                    string deptName = r.Department;

                    if (!reqByDept.Any(s => s.Department.Contains(r.Department)))
                    {
                        reqByDept.Add(r);
                    }
                }

                List<Requisition> reqList = new List<Requisition>();
                foreach (Requisition req in reqByDept)
                {
                    reqList.Add(req);
                }
                ViewData["count"] = reqList.Count();
                ViewData["reqList"] = reqList;
                return View(reqList);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        [HttpPost]
        public ActionResult SCRequisitionView(FormCollection form,string sessionId)
        {
            List<Requisition> reqList = new List<Requisition>();
            if (Request.Form["search"] != null)
            {
                string fromdate = Request.Form["fromdate"];
                string cp = Request.Form["SelectedCP"];
                string todate = Request.Form["todate"];
                string status = Request.Form["status"];
                //check the search then bring it over to the logic
                //check if this redirect action actually brings back to same list
                reqList = Search(fromdate, todate, cp, status);

                ViewData["count"] = reqList.Count();
                ViewData["reqList"] = reqList;
                //------------------Prepare SCRequisitionViewPage-------
                List<CollectionPoint> CPList = db.CollectionPoints.ToList();
                CPList.Insert(0, new CollectionPoint());
                List<string> StatusList = new List<string> { "", "PENDING", "OUTSTANDING" };
                ViewData["CPList"] = CPList;
                ViewData["StatusList"] = StatusList;
                //---------------------END HERE----------------------
                return View(reqList);
            }
            else if (Request.Form["retrieve"] != null)
            {
                List<Requisition> SelectedRequests = new List<Requisition>();
                int count = int.Parse(Request.Form["count"]);
                string PendingDeptRequisition = ",";
                for (int i = 0; i < count; i++)
                {
                    if (Request.Form["Requisition[" + i + "].toretrieve"] != null)
                    {
                        string dept = Request.Form["Requisition[" + i + "].toretrieve"];
                        PendingDeptRequisition = PendingDeptRequisition + dept + ",";

                    }
                }

                return RedirectToAction("Create", "Retrievals", new { PendingDeptRequisition = PendingDeptRequisition });
                
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
                var req = db.Requisition.Where(x => x.EmployeeId == obj.EmployeeId).ToList();
                return View(db.Requisition.Where(x => x.EmployeeId == obj.EmployeeId).ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }


        // GET: Requisitions/Details/5
        public ActionResult Details(int? id, string status, string Remarks, string sessionId)
        {
            ViewData["sessionId"] = sessionId;
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
                return RedirectToAction("ViewRequisition",new{sessionId=sessionId});
            }

            if (requisitionDetails == null)
            {
                return HttpNotFound();
            }
            return View(requisitionDetails);
        }

        public ActionResult TrackDetails(int? id, string sessionId)
        {
            ViewData["sessionId"] = sessionId;
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
            //put search list at the back
            //string SearchCriteria = "";
            List<Requisition> SearchList = new List<Requisition>();
            List<Requisition> DateList = new List<Requisition>();
            List<Requisition> CPList = new List<Requisition>();
            List<Requisition> StatusList = new List<Requisition>();
            if (cp != null)
            {
                List<Department> DepartmentInCp = db.Departments.Where(d => d.CollectionLocationId == cp).ToList();
                foreach(Department d1 in DepartmentInCp)
                {
                    List<Requisition> searchListByDept = db.Requisition.Where(r => r.Department == d1.DeptName).ToList();
                    foreach(Requisition r1 in searchListByDept)
                    {
                        CPList.Add(r1);
                    }
                }
                SearchList = AddOrMerge(SearchList, CPList);
                //SearchCriteria = SearchCriteria + "CP";
            }
            if(fromdate!="" && todate!= "")
            {
                DateTime startdate = Convert.ToDateTime(fromdate);
                DateTime enddate = Convert.ToDateTime(todate);
                DateList = db.Requisition.Where(j => j.Date >= startdate && j.Date <= enddate).ToList();
                SearchList = AddOrMerge(SearchList, DateList);
                //SearchCriteria = SearchCriteria + "DATE";
            }
            if (status != "")
            {
                StatusList = db.Requisition.Where(r1 => r1.Status == status).ToList();
                SearchList = AddOrMerge(SearchList, StatusList);
                //SearchCriteria = SearchCriteria + "STATUS";
            }
            //if (SearchCriteria.Contains("STATUS")&& SearchCriteria.Contains("CP"))
            //{
            //    searchList = StatusList.Intersect(CPList).ToList();
            //}
            return SearchList;
        }
        public List<Requisition> AddOrMerge(List<Requisition> SearchList, List<Requisition> MergingList)
        {
            if (SearchList.Count() == 0)//nothing inside
            {
                foreach (Requisition r in MergingList)
                {
                    SearchList.Add(r);
                }
            }
            else
            {
                SearchList = SearchList.Intersect(MergingList).ToList();
            }
            return SearchList;
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
