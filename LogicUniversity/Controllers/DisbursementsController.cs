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

        // GET: Disbursements
        public ActionResult Index()
        {
            var disbursements = db.Disbursements.Include(d => d.CollectionPoint).Include(d => d.Representative);
            List<Disbursement> dList = disbursements.ToList();
            ViewData["dList"] = dList;
            return View(dList);
        }
        public List<Department> splitDString(string deptstring)
        {
            string[] PendingDeptList = deptstring.Split(',');
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
        public List<RequisitionDetails> GetRequisitionDetailsBFromDList(List<Department> dList, List<RequisitionDetails> rdList)
        {
            List<RequisitionDetails> rdListByDept = new List<RequisitionDetails>();
            foreach (Department d in dList)
            {
                var rdDept = rdList.Where(s => s.Requisition.Department == d.DeptName);
                foreach (RequisitionDetails rd1 in rdDept)
                {
                    rdListByDept.Add(rd1);
                }
            }
            return rdListByDept;
        }
        public List<RequisitionDetails> SplitRDString(string RequisitionDetailsString, List<RequisitionDetails> rdList)
        {
            string[] RequisitionDetailsArray = RequisitionDetailsString.Split(',');
            foreach (string Rdstring in RequisitionDetailsArray)
            {
                if (Rdstring != "")
                {
                    int RdId = int.Parse(Rdstring);
                    RequisitionDetails Rd = db.RequisitionDetails.FirstOrDefault(s => s.RequisitionDetailsId == RdId);
                    rdList.Add(Rd);
                }
            }
            return rdList;
        }
        [HttpGet]
        public ActionResult DisplayDisbursement(string RequisitionDetailsString, string DeptString)
        {
            
            List<Department> dListAll = db.Departments.ToList();
            List<CollectionPoint> CpListAll = db.CollectionPoints.ToList();
            List<Department> dList = new List<Department>();
            if (DeptString == "All")
            {
                dList = dListAll;
            }
            else
            {
                dList = splitDString(DeptString);
            }
            List<RequisitionDetails> rdList = new List<RequisitionDetails>();
            if (RequisitionDetailsString == "All")
            {
                foreach (Department d in dList)
                {
                    //rd that are retrieved and from the dept - all disbursement string (have not been disbursed)
                    string dept = d.DeptName;
                    List<Requisition> RequisitionsThatAreFromDept = db.Requisition.Where(s => s.Department == dept).ToList();
                    
                    foreach (Requisition r in RequisitionsThatAreFromDept)
                    {
                        List<RequisitionDetails> AllRdByDept = db.RequisitionDetails.Where(rd => rd.RequisitionId == r.RequisitionId).Where(rd1 => rd1.Status == "Retrieved").ToList();
                        foreach (RequisitionDetails rd2 in AllRdByDept)
                        {
                            rdList.Add(rd2);
                        }
                    }
                }
            }
            else
            {
                rdList = SplitRDString(RequisitionDetailsString, rdList);
            }
            //--start here: List<RequisitionDetails> RDThatHaveBeenDisbursed
            string RequisitionDetailThatHasBeenDisbursed = "";
            foreach (Disbursement d in db.Disbursements)
            {
                RequisitionDetailThatHasBeenDisbursed = RequisitionDetailThatHasBeenDisbursed + "," + d.Status;
            }
            string[] RequisitionDetailThatHasBeenDisbursedArray = RequisitionDetailThatHasBeenDisbursed.Split(',');
            List<RequisitionDetails> rdListRemove = new List<RequisitionDetails>();
            foreach (RequisitionDetails rd3 in rdList)
            {
                //if it is not in the RequisitionDetailThatHasBeenDisbursed
                if (RequisitionDetailThatHasBeenDisbursedArray.Contains(rd3.RequisitionDetailsId.ToString()))
                {
                    rdListRemove.Add(rd3);
                }

            }
            foreach(RequisitionDetails ToRemove in rdListRemove)
            {
                rdList.Remove(ToRemove);
            }
            //---end here
            rdList = SaveIncludeAllRD(rdList);
            List<RequisitionDetails> rdListByDept = GetRequisitionDetailsBFromDList(dList, rdList);

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
            ViewData["CpListAll"] = CpListAll;
            ViewData["DeptString"] = DeptString;
            ViewData["count"] = ICDList.Count();
            ViewData["ICDList"] = ICDList;
            ViewData["RequisitionDetailsString"] = RequisitionDetailsString;
            ViewData["dListAll"] = dListAll;
            return View();
        }
        public StockAdjustmentVoucher PrepareVoucher()
        {
            StockAdjustmentVoucher s = new StockAdjustmentVoucher();
            int id = db.StockAdjustmentVouchers.Count() + 1;
            s.Id = "V" + id;
            s.DateCreated = DateTime.Now;
            return s;
        }
        public Disbursement PrepareAndIncludeAllDisbursement(Disbursement d0, Department d)
        {
            d0.DisbursementId = db.Disbursements.Count() + 1;
            d0.DateCreated = DateTime.Now;
            d0.DateDisbursed = DateTime.Now;

            CollectionPoint C = db.CollectionPoints.FirstOrDefault(c => c.CollectionPointId == d.CollectionLocationId);
            Employee E = db.Employees.FirstOrDefault(e => e.EmployeeName == d.ContactName);
            d0.Representative = E;
            d0.CollectionPoint = C;

            return d0;
        }
        public List<StockAdjustmentVoucherDetail> AddVoucherDetailToVoucherDetailList(List<StockAdjustmentVoucherDetail> sList, string itemcode, int quantityadjusted)
        {
            StockAdjustmentVoucherDetail s0 = new StockAdjustmentVoucherDetail();
            s0.Product = db.Products.FirstOrDefault(p => p.ItemCode == itemcode);
            s0.ItemCode = itemcode;
            s0.QuantityAdjusted = quantityadjusted;
            s0.Status = "Pending";
            s0.Balance = s0.Product.Balance + s0.QuantityAdjusted;
            sList.Add(s0);
            return sList;
        }
        public List<DisbursementDetail> AddDisbursementDetailToDDList(List<DisbursementDetail> ddList, string itemcode, int quantity, int collected)
        {
            DisbursementDetail dd = new DisbursementDetail();
            dd.ItemCode = itemcode;
            dd.QuantityRequested = quantity;
            dd.QuantityReceived = collected;
            dd.AdjustmentVoucherId = null;
            ddList.Add(dd);
            return ddList;
        }
        [HttpPost]
        public ActionResult DisplayDisbursement(FormCollection form)
        {
            
            string RequisitionDetailsString = Request.Form["RequisitionDetailsString"];
            //create another submit button for the search department string
            if (Request.Form["SearchDept"] != null)
            {
                string deptname = Request.Form["SearchDeptName"];
                return RedirectToAction("DisplayDisbursement", new { RequisitionDetailsString = RequisitionDetailsString, DeptString = deptname });
            }
            if (Request.Form["SearchCP"] != null)
            {
                string CPId = Request.Form["SearchCPId"];
                List<Department> dListByCp = db.Departments.Where(d => d.CollectionLocationId == CPId).ToList();
                string deptname = "";
                foreach (Department d in dListByCp)
                {
                    deptname = deptname + "," + d.DeptName;
                }
                return RedirectToAction("DisplayDisbursement", new { RequisitionDetailsString = RequisitionDetailsString, DeptString = deptname });
            }
            int count = int.Parse(Request.Form["count"]);
            string dept = Request.Form["DeptString"];
            List<Department> dList = splitDString(dept);
            StockAdjustmentVoucher s = PrepareVoucher();

            List<StockAdjustmentVoucherDetail> sList = new List<StockAdjustmentVoucherDetail>();
            foreach (Department d in dList)
            {
                Disbursement disbursement = new Disbursement();
                disbursement = PrepareAndIncludeAllDisbursement(disbursement, d);
                List<DisbursementDetail> ddList = new List<DisbursementDetail>();

                
                for (int i = 0; i < count; i++)
                {
                    string itemcode = Request.Form["Disbursement[" + i + "].itemcode"];
                    Products p = db.Products.FirstOrDefault(o => o.ItemCode == itemcode);
                    int quantity = int.Parse(Request.Form["Disbursement[" + i + "].quantity"]);
                    int collected = int.Parse(Request.Form["Disbursement[" + i + "].collected"]);

                    if (quantity != collected)
                    {
                        //if any of the voucher in sList itemcode == itemcode
                        if(sList.Any(sdv=>sdv.ItemCode == itemcode))
                        {
                            StockAdjustmentVoucherDetail sd1 = sList.FirstOrDefault(sdv1 => sdv1.ItemCode == itemcode);
                            //sd1.QuantityAdjusted = sd1.QuantityAdjusted;
                        }
                        else
                        {
                            sList = AddVoucherDetailToVoucherDetailList(sList, itemcode, quantity - collected);
                        }
                    }
                    ddList = AddDisbursementDetailToDDList(ddList, itemcode, quantity, collected);
                }
                if (ddList.Count() != 0)
                {
                    disbursement.Status = RequisitionDetailsString;
                    disbursement.DisbursementDetails = ddList;
                    db.Disbursements.Add(disbursement);
                    db.SaveChanges();
                }
                s.StockAdjustmentVoucherDetails = sList;
            }
            
            if (sList.Count() != 0)
            {
                ViewData["s"] = s;
                ViewData["count"] = sList.Count();
                return View("AdjustDisbursement", s);
            }
            else
            {
                return RedirectToAction("Index", "Disbursements");
            }
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
                    s0.Reason = "DISBURSEMENT: " + reason;
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
            AllocateAuthorizer(stockAdjustmentVoucher);

            ViewData["count"] = count;
            return RedirectToAction("Index");
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
                double totalQuantity = sDetailListPerProduct.Sum(x => x.QuantityAdjusted);
                double totalAdjustedCost = Math.Abs(totalQuantity * d0.Product.UnitPrice);

                foreach (StockAdjustmentVoucherDetail s in sDetailListPerProduct)
                {
                    if (totalAdjustedCost < 250)
                    {
                        s.Approver = "Supervisor";
                    }
                    else
                    {
                        s.Approver = "Manager";
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
            string[] PendingRequisitionDetailsList = RequisitionString.Split(',');
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

        //-------------------------UNUSED METHODS------------------------
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
    }
}
