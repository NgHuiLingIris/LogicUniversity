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
        public ActionResult DisplayDisbursement(int RetrievalId,string DepartmentOrAll)
        {
            Retrieval r = db.Retrievals.FirstOrDefault(s => s.RetrievalId == RetrievalId);
            List<RequisitionDetails> rdList = splitString(r.RequisitionString);

            rdList = SaveIncludeAllRD(rdList);
            List<RequisitionDetails> rdListByDept = rdList.Where(s => s.Requisition.Department == DepartmentOrAll).ToList();
            List<ItemCodeDisbursement> ICDList = new List<ItemCodeDisbursement>();
            
            foreach(RequisitionDetails rd2 in rdListByDept)
            {
                if (ICDList.Any(i=>i.Product == rd2.Products))
                {
                    ItemCodeDisbursement ICD = ICDList.FirstOrDefault(i1 => i1.Product == rd2.Products);
                    ICD.Quantity = ICD.Quantity+rd2.Quantity;
                }
                else
                {
                    ItemCodeDisbursement ICD = new ItemCodeDisbursement();
                    ICD.Product = rd2.Products;
                    ICD.Quantity = rd2.Quantity;
                    ICDList.Add(ICD);
                }
            }
            ViewData["DepartmentOrAll"] = DepartmentOrAll;
            ViewData["count"] = ICDList.Count();
            ViewData["ICDList"] = ICDList;
            return View();
        }

        [HttpPost]
        public ActionResult DisplayDisbursement(FormCollection form)
        {
            //create another submit button for the search department string
            int count = int.Parse(Request.Form["count"]);
            

            StockAdjustmentVoucher s = new StockAdjustmentVoucher();
            int id = db.StockAdjustmentVouchers.Count() + 1;
            s.Id = "V" + id;
            s.DateCreated = DateTime.Now;

            Disbursement r = new Disbursement();
            r.DisbursementId = db.Disbursements.Count() + 1;
            r.DateCreated = DateTime.Now;
            r.DateDisbursed = DateTime.Now;
            string dept = Request.Form["DepartmentOrAll"];
            Department D = db.Departments.FirstOrDefault(d => d.DeptName == dept);
            CollectionPoint C = db.CollectionPoints.FirstOrDefault(c => c.CollectionPointId == D.CollectionLocationId);
            Employee E = db.Employees.FirstOrDefault(e => e.EmployeeName == D.ContactName);
            r.Representative = E;
            r.CollectionPoint = C;

            List<DisbursementDetail> ddList = new List<DisbursementDetail>();

            List<StockAdjustmentVoucherDetail> sList = new List<StockAdjustmentVoucherDetail>();
            for (int i = 0; i < count; i++)
            {
                string itemcode = Request.Form["Disbursement["+i+"].itemcode"];
                Products p = db.Products.FirstOrDefault(o => o.ItemCode == itemcode);
                int quantity = int.Parse(Request.Form["Disbursement[" + i + "].quantity"]);
                int collected = int.Parse(Request.Form["Disbursement[" + i + "].collected"]); ;
                
                if (quantity != collected)
                {
                    StockAdjustmentVoucherDetail s0 = new StockAdjustmentVoucherDetail();
                    //Products p = db.Products.FirstOrDefault(o => o.ItemCode == itemcode);
                    s0.Product = p;
                    s0.ItemCode = itemcode;
                    s0.QuantityAdjusted = quantity - collected;
                    s0.Status = "Pending";
                    s0.Balance = p.Balance + s0.QuantityAdjusted;
                    sList.Add(s0);
                }
                else
                {
                    //create new disbursement and save
                    DisbursementDetail dd = new DisbursementDetail();
                    dd.ItemCode = p.ItemCode;
                    dd.QuantityRequested = quantity;
                    dd.QuantityReceived = collected;
                    dd.AdjustmentVoucherId = null;
                    ddList.Add(dd);
                }
            }
            s.StockAdjustmentVoucherDetails = sList;
            //CheckRequisitionComplete();
            if (sList.Count() != 0)
            {
                //When there is a discrepancy between the disbursement and the collected

                //ViewData["s"] = s;
                //ViewData["count"] = sList.Count();
                return View("AdjustRetrieval", s);
            }
            else
            {
                r.DisbursementDetails = ddList;

                db.Disbursements.Add(r);
                db.SaveChanges();
                return RedirectToAction("Index", "Disbursements");
            }
            //return View();
            return View();
        }
        public List<RequisitionDetails> SaveIncludeAllRD(List<RequisitionDetails> rdList)
        {
            foreach (RequisitionDetails rd1 in rdList)
            {
                Requisition rq = db.Requisition.FirstOrDefault(rq1 => rq1.RequisitionId == rd1.RequisitionId);
                rd1.Requisition = rq;
                Products p = db.Products.FirstOrDefault(p1 => p1.ItemCode == rd1.ItemCode);
                rd1.Products = p;
                db.Entry(rd1).State = EntityState.Modified;
                db.SaveChanges();
            }
            return rdList;
        }
        public List<RequisitionDetails> splitString(string RequisitionString)
        {
            string[] PendingRequisitionDetailsList = RequisitionString.Split('*');
            List<RequisitionDetails> rdList = new List<RequisitionDetails>();
            foreach (string rd in PendingRequisitionDetailsList)
            {
                if (rd != "")
                {
                    int int_rd = int.Parse(rd);
                    RequisitionDetails d0 = db.RequisitionDetails.FirstOrDefault(i => i.RequisitionDetailsId == int_rd);
                    rdList.Add(d0);
                }
            }
            return rdList;
        }
    }
}
