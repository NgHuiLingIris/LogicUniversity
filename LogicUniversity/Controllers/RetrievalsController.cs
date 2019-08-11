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
using LogicUniversity.Controllers;

namespace LogicUniversity.Controllers
{
    public class RetrievalsController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();
        public List<Department> splitString(string deptstring)
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
        public List<RequisitionDetails> RetrieveRequisitionDetailsByDepartment(List<RequisitionDetails> rdListAll, List<Department> dList, List<RequisitionDetails> InputRDList)
        {
            
            IncludeSaveAllRequisitionDetails(InputRDList);
            foreach (Department d in dList)
            {
                List<RequisitionDetails> rdList2 = InputRDList.Where(rd => rd.Status == null).Where(rd => rd.Requisition.Department == d.DeptName).ToList();
                //List<Requisition> requisitionByDept = db.Requisition.Where(r => r.Department == d.DeptName).Where(r => r.Status == "PENDING").ToList();
                foreach (RequisitionDetails r in rdList2)
                {
                    rdListAll.Add(r);
                }
            }
            return rdListAll;
        }
        public List<Products> GetProductListFromRequisitionDetails(List<RequisitionDetails> rdListAll)
        {
            List<Products> pList = new List<Products>();
            foreach (RequisitionDetails rd in rdListAll)
            {
                Products p = db.Products.FirstOrDefault(s => s.ItemCode == rd.ItemCode);
                if (!pList.Contains(p))
                {
                    pList.Add(p);
                }
            }
            return pList;
        }
        public List<ItemCodeRequisition> ICRListPerProduct(List<RequisitionDetails> rdListAll)
        {
            List<Products> pList = GetProductListFromRequisitionDetails(rdListAll);
            List<ItemCodeRequisition> ICRList = new List<ItemCodeRequisition>();
            foreach (Products p in pList)
            {
                ItemCodeRequisition ICR = new ItemCodeRequisition();
                ICR.Product = p;
                ICR.QtyInInventory = ICR.Product.Balance;
                List<RequisitionDetails> ByProductRequsitionDetails = rdListAll.Where(x => x.ItemCode == ICR.Product.ItemCode).ToList();
                List<Department> DeptName = new List<Department>();
                double TotalNeeded = 0;
                List<double> NeededList = new List<double>();
                foreach (RequisitionDetails d in ByProductRequsitionDetails)
                {
                    Department d0 = db.Departments.FirstOrDefault(s => s.DeptName == d.Requisition.Department);
                    if (DeptName.Any(s => s == d0))
                    {
                        //get position of department in DeptName list
                        int idx = DeptName.IndexOf(d0);
                        TotalNeeded = TotalNeeded + d.Quantity;
                        NeededList[idx] = NeededList[idx] + d.Quantity;
                    }
                    else
                    {
                        TotalNeeded = TotalNeeded + d.Quantity;
                        DeptName.Add(d0);
                        NeededList.Add(d.Quantity);
                    }

                }
                ICR.TotalNeeded = TotalNeeded;
                ICR.DeptName = DeptName;
                ICR.NeededList = NeededList;
                ICRList.Add(ICR);
            }
            return ICRList;
        }
        // GET: Retrievals/Create
        public ActionResult Create(string PendingDeptRequisition)
        {
            List<Department> dList = splitString(PendingDeptRequisition);
            List<RequisitionDetails> rdListAll = new List<RequisitionDetails>();
            List<RequisitionDetails> rdList1 = db.RequisitionDetails.Include(s => s.Requisition).ToList();
            rdListAll = RetrieveRequisitionDetailsByDepartment(rdListAll, dList,rdList1);
            List<ItemCodeRequisition> ICRList = ICRListPerProduct(rdListAll);

            ViewData["ICRList"] = ICRList;
            ViewData["count"] = ICRList.Count();
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
        public Retrieval PrepareRetrieval()
        {
            Retrieval r = new Retrieval();
            r.RetrievalId = db.Retrievals.Count() + 1;
            r.DateRetrieved = DateTime.Now;

            return r;
        }
        public List<StockAdjustmentVoucherDetail> AddVoucherDetailToVoucherDetailList(List<StockAdjustmentVoucherDetail> sList, string itemcode, int quantityadjusted)
        {
            StockAdjustmentVoucherDetail s0 = new StockAdjustmentVoucherDetail();
            s0.Product = db.Products.FirstOrDefault(p=>p.ItemCode == itemcode);
            s0.ItemCode = itemcode;
            s0.QuantityAdjusted = quantityadjusted;
            s0.Status = "Pending";
            s0.Balance = s0.Product.Balance+quantityadjusted;
            sList.Add(s0);
            return sList;
        }
        public List<RetrievalDetail> AddRetrievalDetailToRdList(List<RetrievalDetail> rdList,string itemcode,int retrievedqty,int TotalNeeded)
        {
            //AddRetrievalDetailToRdList
            RetrievalDetail rd = new RetrievalDetail();
            rd.ItemCode = itemcode;
            rd.QuantityRetrieved = retrievedqty;
            rd.QuantityNeeded = TotalNeeded;
            rd.AdjustmentVoucherId = null;
            rdList.Add(rd);
            return rdList;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection form)
        {
            int count = int.Parse(Request.Form["count"]);
            string DeptString = "";
            StockAdjustmentVoucher s = PrepareVoucher();
            Retrieval r = PrepareRetrieval();
            List<RetrievalDetail> rdList = new List<RetrievalDetail>();
            //Requisition SplitRequisition = new Requisition();
            //List<RequisitionDetails> SplitRDList = new List<RequisitionDetails>();
            List<StockAdjustmentVoucherDetail> sList = new List<StockAdjustmentVoucherDetail>();
            for (int i = 0; i < count; i++)
            {
                string itemdesc = Request.Form["ICR[" +i+"].product"];
                Products p = db.Products.FirstOrDefault(o => o.Description == itemdesc);
                string itemcode = p.ItemCode;
                int retrievedqty = int.Parse(Request.Form["ICR[" + i + "].retrieved"]); 
                int qtyininventory = int.Parse(Request.Form["ICR[" + i + "].qtyininventory"]);
                int TotalNeeded = int.Parse(Request.Form["ICR[" + i + "].TotalNeeded"]);
                string dept = Request.Form["ICR[" + i + "].Dept"];
                List<Department> dList = splitString(dept);
                DeptString = dept;
                List<RequisitionDetails> requisitiondetaillist = new List<RequisitionDetails>();
                List<RequisitionDetails> requisitiondetaillist1 = db.RequisitionDetails.Where(a => a.ItemCode == itemcode).Where(b => b.Status != "Retrieved").Include(c => c.Requisition).ToList();
                requisitiondetaillist = RetrieveRequisitionDetailsByDepartment(requisitiondetaillist, dList, requisitiondetaillist1);
                requisitiondetaillist = IncludeSaveAllRequisitionDetails(requisitiondetaillist);
                //the requisition detail list is already by department and itemcode
                if (TotalNeeded != retrievedqty)
                    {
                    sList = AddVoucherDetailToVoucherDetailList(sList, itemcode, retrievedqty - qtyininventory);
                    requisitiondetaillist = requisitiondetaillist.Where(q => q.ItemCode == itemcode).OrderBy(w=>w.Requisition.Date).ToList();
                    //go down each list

                    for(int j = 0; j< requisitiondetaillist.Count(); j++)
                    {
                        if (retrievedqty >= requisitiondetaillist[j].Quantity)
                        {
                            retrievedqty = retrievedqty - requisitiondetaillist[j].Quantity;
                        }
                        else
                        {
                            int newquantity = requisitiondetaillist[j].Quantity - retrievedqty;
                            requisitiondetaillist[j].Quantity = retrievedqty;
                            
                            RequisitionDetails newRD = new RequisitionDetails();
                            newRD.Quantity = newquantity;
                            newRD.ItemCode = itemcode;
                            newRD.RequisitionId = requisitiondetaillist[j].RequisitionId;

                            db.Entry(newRD).State = EntityState.Added;
                            db.Entry(requisitiondetaillist[j]).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                    }
                }
                r = CreateRequisitionString(r, requisitiondetaillist);
                rdList = AddRetrievalDetailToRdList(rdList, p.ItemCode, retrievedqty, TotalNeeded);
            }
            s.StockAdjustmentVoucherDetails = sList;
            CheckRequisitionComplete();
            if (sList.Count() != 0)
            {

                r = SaveRetrieval(r, rdList);
                ViewData["s"] = s;
                ViewData["count"] = sList.Count();
                ViewData["RetrievalId"] = r.RetrievalId;
                ViewData["DeptString"] = DeptString;
                //can bring dept string here
                return View("AdjustRetrieval",s);
            }
            else
            {
                r = SaveRetrieval(r, rdList);
                return RedirectToAction("DisplayDisbursement", "Disbursements", new {RetrievalId = r.RetrievalId, DeptString = DeptString });
            }

        }
        public Retrieval SaveRetrieval(Retrieval r, List<RetrievalDetail> rdList)
        {
            r.RetrievalDetails = rdList;
            db.Retrievals.Add(r);
            db.SaveChanges();
            return r;
        }
        public Retrieval CreateRequisitionString(Retrieval r,List<RequisitionDetails> requisitiondetaillist)
        {
            foreach (RequisitionDetails c in requisitiondetaillist)
            {
                c.Status = "Retrieved";
                r.RequisitionString = r.RequisitionString + "*" + c.RequisitionDetailsId;
                //will work on method to check if item is retrieved here, if not put 'PENDING'
                //c.Requisition.Status = "COMPLETE";
                db.Entry(c).State = EntityState.Modified;
                db.SaveChanges();
            }
            return r;
        }
        public ActionResult AdjustRetrieval([Bind(Include = "Id,DateCreated")] StockAdjustmentVoucher stockAdjustmentVoucher, FormCollection form)
        {
            string DeptString = Request.Form["DeptString"];
            int RetrievalId = int.Parse(Request.Form["RetrievalId"]);
            int count = int.Parse(Request.Form["count"]);
            List<StockAdjustmentVoucherDetail> sList = new List<StockAdjustmentVoucherDetail>();
            for (int i = 0; i< count; i++)
            {
                string itemcode = Request.Form["ItemCode["+i+"]"];
                int qtyadjusted = int.Parse(Request.Form["QuantityAdjusted["+i+"]"]);
                string reason = Request.Form["StockAdjustmentVoucherDetails["+i+"].Reason"];
                
                if (sList.Any(a => a.ItemCode.Contains(itemcode)))
                {
                    StockAdjustmentVoucherDetail s0 = sList.Find(b => b.ItemCode == itemcode);
                    s0.QuantityAdjusted = s0.QuantityAdjusted + qtyadjusted;
                }
                else
                {
                    sList = AddVoucherDetailToVoucherDetailList(sList, itemcode, qtyadjusted);
                }
            }
            //create new outstanding retrieval for partial fulfillment here;
            //C:
            stockAdjustmentVoucher.StockAdjustmentVoucherDetails = sList;
            db.StockAdjustmentVouchers.Add(stockAdjustmentVoucher);
            db.SaveChanges();
            //D:
            StockAdjustmentVoucher s = new StockAdjustmentVoucher();
            //StockAdjustmentVouchersController c = new StockAdjustmentVouchersController();
            AllocateAuthorizer(stockAdjustmentVoucher);

            ViewData["count"] = count;
            return RedirectToAction("DisplayDisbursement", "Disbursements", new { RetrievalId = RetrievalId, DeptString = DeptString });
        }
        public void CheckRequisitionComplete()
        {
            List<Requisition> rList = db.Requisition.Where(s => s.Status == "PENDING").ToList();
            foreach(Requisition r in rList)
            {
                List<RequisitionDetails> rdList = db.RequisitionDetails.Where(s => s.RequisitionId == r.RequisitionId).ToList();
                if (rdList.All(s=>s.Status == "Retrieved"))
                {
                    r.Status = "COMPLETE";
                    db.Entry(r).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        public List<RequisitionDetails> IncludeSaveAllRequisitionDetails(List<RequisitionDetails> requisitiondetaillist1)
        {
            foreach (RequisitionDetails d in requisitiondetaillist1)
            {
                Requisition r1 = db.Requisition.FirstOrDefault(d2 => d2.RequisitionId == d.RequisitionId);
                Employee e = db.Employees.FirstOrDefault(x => x.EmployeeId == r1.EmployeeId);
                Department d1 = db.Departments.FirstOrDefault(f => f.DeptId == e.DeptId);
                d.Requisition = r1;
                r1.Department = d1.DeptName;
                db.Entry(r1).State = EntityState.Modified;
                db.SaveChanges();
                //to factorise
                //entity state saved and modified
            }
            return requisitiondetaillist1;
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
                var sDetailsFromDb = sDetailsFromDb0.Include(s => s.Product);
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

        //Default controller methods----------------------------------------------------------------------------------------------------------
        // GET: Retrievals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Retrieval retrieval = db.Retrievals.Find(id);
            if (retrieval == null)
            {
                return HttpNotFound();
            }
            ViewBag.StoreClerkId = new SelectList(db.Employees, "EmployeeId", "EmployeeName", retrieval.StoreClerkId);
            return View(retrieval);
        }
        

        //GET: Retrievals
        public ActionResult Index()
        {
            var retrievals = db.Retrievals.Include(r => r.StoreClerk);
            return View(retrievals.ToList());
        }

        // GET: Retrievals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Retrieval retrieval = db.Retrievals.Find(id);
            if (retrieval == null)
            {
                return HttpNotFound();
            }
            return View(retrieval);
        }
        // POST: Retrievals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RetrievalId,DateRetrieved,StoreClerkId")] Retrieval retrieval)
        {
            if (ModelState.IsValid)
            {
                db.Entry(retrieval).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StoreClerkId = new SelectList(db.Employees, "EmployeeId", "EmployeeName", retrieval.StoreClerkId);
            return View(retrieval);
        }

        // GET: Retrievals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Retrieval retrieval = db.Retrievals.Find(id);
            if (retrieval == null)
            {
                return HttpNotFound();
            }
            return View(retrieval);
        }

        // POST: Retrievals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Retrieval retrieval = db.Retrievals.Find(id);
            db.Retrievals.Remove(retrieval);
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
