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
    public class DisbursementsController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();
        //StockAdjustSerivce stockAdjustSerivce = new StockAdjustSerivce();

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
        public List<Department> splitDString(string deptstring)
        {
            string[] PendingDeptList = deptstring.Split('*');
            List<Department> dList = new List<Department>();
            foreach (string d in PendingDeptList)
            {
                if (d != "")
                {
                    Department d0 = db.Departments.FirstOrDefault(i => i.DeptName == d);
                    dList.Add(d0);
                }
            }
            return dList;
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
        public ActionResult DisplayDisbursement(int RetrievalId,string DeptString)
        {
            List<Department> dList = splitDString(DeptString);
            Retrieval r = db.Retrievals.FirstOrDefault(s => s.RetrievalId == RetrievalId);
            List<RequisitionDetails> rdList = splitString(r.RequisitionString);

            rdList = SaveIncludeAllRD(rdList);
            List<RequisitionDetails> rdListByDept = new List<RequisitionDetails>();
            foreach(Department d in dList)
            {
                var rdDept = rdList.Where(s => s.Requisition.Department == d.DeptName);
                foreach(RequisitionDetails rd1 in rdDept)
                {
                    rdListByDept.Add(rd1);
                }
            }

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
            ViewData["DeptString"] = DeptString;
            ViewData["count"] = ICDList.Count();
            ViewData["ICDList"] = ICDList;
            return View();
        }

        [HttpPost]
        public ActionResult DisplayDisbursement(FormCollection form)
        {
            //create another submit button for the search department string
            int count = int.Parse(Request.Form["count"]);
            string dept = Request.Form["DeptString"];
            List<Department> dList = splitDString(dept);
            //for each dept, got one disbursement and one collection
            StockAdjustmentVoucher s = new StockAdjustmentVoucher();
            List<StockAdjustmentVoucherDetail> sList = new List<StockAdjustmentVoucherDetail>();
            foreach (Department d in dList)
            {
                int id = db.StockAdjustmentVouchers.Count() + 1;
                s.Id = "V" + id;
                s.DateCreated = DateTime.Now;

                Disbursement r = new Disbursement();
                r.DisbursementId = db.Disbursements.Count() + 1;
                r.DateCreated = DateTime.Now;
                r.DateDisbursed = DateTime.Now;

                //Department D = db.Departments.FirstOrDefault(d1 => d1.DeptName == dept);
                CollectionPoint C = db.CollectionPoints.FirstOrDefault(c => c.CollectionPointId == d.CollectionLocationId);
                Employee E = db.Employees.FirstOrDefault(e => e.EmployeeName == d.ContactName);
                r.Representative = E;
                r.CollectionPoint = C;

                List<DisbursementDetail> ddList = new List<DisbursementDetail>();

                
                for (int i = 0; i < count; i++)
                {
                    string itemcode = Request.Form["Disbursement[" + i + "].itemcode"];
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
                if (ddList.Count() != 0)
                {
                    r.DisbursementDetails = ddList;
                    db.Disbursements.Add(r);
                    db.SaveChanges();
                }
                s.StockAdjustmentVoucherDetails = sList;
            }
            
            //CheckRequisitionComplete();
            if (sList.Count() != 0)
            {
                //When there is a discrepancy between the disbursement and the collected
                ViewData["s"] = s;
                ViewData["count"] = sList.Count();
                return View("AdjustDisbursement", s);
            }
            else
            {
                return RedirectToAction("Index", "Disbursements");
            }
            //return View();
            return View();
        }
        [HttpGet]
        public ActionResult Select()
        {
            //select retrieval id and select department
            //need to add status to retrieval to select pending (or can do it by date)
            List<Retrieval> rList = db.Retrievals.ToList();
            List<Department> dList = db.Departments.ToList();
            ViewData["rListCount"] = rList.Count();
            ViewData["dListCount"] = dList.Count();
            ViewData["rList"] = rList;
            ViewData["dList"] = dList;
            return View();
        }
        [HttpPost]
        public ActionResult Select(FormCollection form)
        {

            int rListCount = int.Parse(Request.Form["rListCount"]);
            int dListCount = int.Parse(Request.Form["dListCount"]);
            string test = Request.Form["Retrieval[0]"];
            Debug.WriteLine(test);
            int RetrievalId = 0;
            string DeptString = "";
            for(int i = 0; i < rListCount; i++)
            {
                if (Request.Form["Retrieval[" + i + "]"] != null)
                {
                    RetrievalId = int.Parse(Request.Form["Retrieval[" + i + "]"]);
                    break;
                }
            }
            string dept = Request.Form["Department[0]"];
            Debug.WriteLine(dept);
            string[] deptArray = dept.Split(',');
            for (int i = 0; i < dListCount; i++)
            {
                foreach(string d in deptArray)
                {
                    DeptString = DeptString + "*" + d;
                }
            }

            return RedirectToAction("DisplayDisbursement",new { RetrievalId = RetrievalId , DeptString = DeptString });
        }
        public ActionResult AdjustDisbursement([Bind(Include = "Id,DateCreated")] StockAdjustmentVoucher stockAdjustmentVoucher, FormCollection form)
        {
            int count = int.Parse(Request.Form["count"]);
            List<StockAdjustmentVoucherDetail> sList = new List<StockAdjustmentVoucherDetail>();
            for (int i = 0; i < count; i++)
            {
                string itemcode = Request.Form["ItemCode[" + i + "]"];
                int qtyadjusted = int.Parse(Request.Form["QuantityAdjusted[" + i + "]"]);
                string reason = Request.Form["StockAdjustmentVoucherDetails[" + i + "].Reason"];

                if (sList.Any(a => a.ItemCode.Contains(itemcode)))
                {
                    StockAdjustmentVoucherDetail s0 = sList.Find(b => b.ItemCode == itemcode);
                    s0.QuantityAdjusted = s0.QuantityAdjusted + qtyadjusted;
                }
                else
                {
                    StockAdjustmentVoucherDetail s0 = new StockAdjustmentVoucherDetail();
                    s0.ItemCode = itemcode;
                    s0.QuantityAdjusted = qtyadjusted;
                    s0.Reason = reason;
                    s0.Status = "Pending";
                    s0.Product = db.Products.FirstOrDefault(o => o.Description == itemcode);
                    sList.Add(s0);
                }
            }

            //C:
            stockAdjustmentVoucher.StockAdjustmentVoucherDetails = sList;
            db.StockAdjustmentVouchers.Add(stockAdjustmentVoucher);
            db.SaveChanges();
            //D:
            StockAdjustmentVoucher s = new StockAdjustmentVoucher();
            //StockAdjustmentVouchersController c = new StockAdjustmentVouchersController();
            AllocateAuthorizer(stockAdjustmentVoucher);

            ViewData["count"] = count;
            return View("Index");
        }
        //Retrieval allocate authorizer comes here
        public StockAdjustmentVoucher AllocateAuthorizer(StockAdjustmentVoucher stockAdjustmentVoucher)
        {
            List<StockAdjustmentVoucherDetail> sDetailList = stockAdjustmentVoucher.StockAdjustmentVoucherDetails;

            foreach (StockAdjustmentVoucherDetail d0 in sDetailList)
            {
                string itemcode = d0.ItemCode;
                var sDetailsFromDb0 = from v in db.StockAdjustmentVoucherDetails
                                      where v.ItemCode == itemcode && v.Status == "Pending"
                                      select v;
                //added for retrieval
                var sDetailsFromDb = sDetailsFromDb0.Include(s => s.Product);
                //end retrieval
                List<StockAdjustmentVoucherDetail> sDetailListPerProduct = new List<StockAdjustmentVoucherDetail>();

                foreach (var p in sDetailsFromDb)
                {
                    sDetailListPerProduct.Add(p);
                }
                //Products p1 = db.Products.FirstOrDefault(s => s.ItemCode == itemcode);
                double totalQuantity = sDetailListPerProduct.Sum(x => x.QuantityAdjusted);
                double totalAdjustedCost = Math.Abs(totalQuantity * d0.Product.UnitPrice);

                foreach (StockAdjustmentVoucherDetail s in sDetailListPerProduct)
                {
                    if (totalAdjustedCost < 250)
                    {
                        s.Approver = "Supervisor";
                        //db.Entry(s).State = EntityState.Modified;
                        //db.SaveChanges();
                    }
                    else
                    {
                        s.Approver = "Manager";
                        //db.Entry(s).State = EntityState.Modified;
                        //db.SaveChanges();
                    }
                    s.Balance = s.Product.Balance;
                    s.ApproverRemarks = "NA";
                    db.Entry(stockAdjustmentVoucher).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }
            return stockAdjustmentVoucher;
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
