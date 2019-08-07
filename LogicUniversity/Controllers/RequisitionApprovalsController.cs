using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LogicUniversity.Context;
using LogicUniversity.Models;

namespace LogicUniversity.Controllers
{
    public class RequisitionApprovalsController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();

        // GET: Requisitions
        public ActionResult ViewRequesition()
        {
            var username = Session["UserID"].ToString();
            Employee obj = db.Employees.Where(a => a.Username.Equals(username)).FirstOrDefault();
            return View(db.Requisition.Where(x =>x.ApproverId==obj.EmployeeId && x.Status=="PENDING").ToList());
        }

        //Requesition ALL Deyails for Store Clerk
        public ActionResult SCRequisitionView()
        {
            List<Requisition> reqList = new List<Requisition>();

            reqList = db.Requisition.ToList();

            ViewData["reqList"] = reqList;

            return View();
        }

        public ActionResult TrackRequesition()
        {
            var username = Session["UserID"].ToString();
            Employee obj = db.Employees.Where(a => a.Username.Equals(username)).FirstOrDefault();
            return View(db.Requisition.Where(x => x.EmployeeId == obj.EmployeeId).ToList());
        }

        public ActionResult TrackRequesitionDH()
        {
            var username = Session["UserID"].ToString();
            Employee obj = db.Employees.Where(a => a.Username.Equals(username)).FirstOrDefault();

            //if (req != 0)
            //{
            //    return View(db.Requisition.Where(x => x.RequisitionId == req).ToList());
            //}
            return View(db.Requisition.Where(x => x.ApproverId == obj.EmployeeId).ToList());

            
        }
        // GET: Requisitions/Details/5
        public ActionResult Details(int? id, string status, string Remarks)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<RequisitionDetails> requisitionDetails = db.RequisitionDetails.Where(x => x.RequisitionDetailsId == id).ToList();
            if (id != 0 && status !=null)
            { 
            Requisition requisition = db.Requisition.Where(x => x.RequisitionId == id).FirstOrDefault();

            requisition.Status = status;
            requisition.Remarks = Remarks;

            db.Requisition.AddOrUpdate(requisition);
            db.SaveChanges();

                return View(requisitionDetails);
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

        public ActionResult Search(string fromdate, string todate, string cp, string status)
        {

            DateTime startdate = Convert.ToDateTime(fromdate);
            DateTime enddate = Convert.ToDateTime(todate);

            List<Requisition> searchList = new List<Requisition>();
            searchList = db.Requisition.Where(i => i.Status == status).ToList()
                .Where(j => j.Date >= startdate && j.Date <= enddate).ToList();

            List<RequisitionDetails> searchDetail = new List<RequisitionDetails>();
            searchDetail = db.RequisitionDetails.ToList();

            List<Products> productDetail = new List<Products>();
            productDetail = db.Products.ToList();



            var item = from sl in searchList
                       join sd in searchDetail
                       on sl.RequisitionId equals sd.RequisitionId
                       into combReqList
                       from f in combReqList
                       join p in productDetail
                       on f.ItemCode equals p.ItemCode
                       into retrievalList
                       from rt in retrievalList
                       orderby sl.RequisitionId, rt.ItemCode
                       select new
                       {
                           RequisitionId = sl.RequisitionId,
                           RequisitionDetailId = f.RequisitionDetailsId,
                           StationeryDescription = rt.Description,
                           QuantityInInventory = rt.Balance,
                           Itemcode = rt.ItemCode,
                           QuantityRequested = f.Quantity,
                           Department = sl.Department,
                           Status = sl.Status,
                           ApprovalDate = sl.Date
                       };

          
            List<TempRetrieval> trList = new List<TempRetrieval>();

            foreach (var i in item)
            {
                TempRetrieval tr = new TempRetrieval();
                tr.RequisitionId = i.RequisitionId;
                tr.RequisitionDetailId = i.RequisitionDetailId;
                tr.StationeryDescription = i.StationeryDescription;
                tr.QuantityInInventory = i.QuantityInInventory;
                tr.Itemcode = i.Itemcode;
                tr.QuantityRequested = i.QuantityRequested;
                tr.Department = i.Department;
                tr.Status = i.Status;
                tr.ApprovalDate = i.ApprovalDate;
                trList.Add(tr);
            }
            ViewData["trList"] = trList;
            return View("RetrievalList");

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
