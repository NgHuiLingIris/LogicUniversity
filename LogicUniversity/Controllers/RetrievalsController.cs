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

        // GET: Retrievals
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

        // GET: Retrievals/Create
        public ActionResult Create(string PendingDeptRequisition)
        {
            //List<Department> PendingDeptRequisition = (List<Department>)TempData["PendingDeptRequisition"];
            Debug.WriteLine("");
            //retrieve as list
            string[] PendingDeptList = PendingDeptRequisition.Split('*');
            List<Department> dList = new List<Department>();
            foreach(string d in PendingDeptList)
            {
                if (d != "")
                {
                    Department d0 = db.Departments.FirstOrDefault(i => i.DeptName == d);
                    dList.Add(d0);
                }
            }
            //1. Get all the requisitions that is PENDING and for the listed departments
            List<RequisitionDetails> rdListAll = new List<RequisitionDetails>();
            foreach (Department d in dList)
            {
                List<RequisitionDetails> rdList1 = db.RequisitionDetails.Include(s => s.Requisition).ToList();
                List<RequisitionDetails> rdList2 = rdList1.Where(rd => rd.Status == null).Where(rd => rd.Requisition.Department == d.DeptName).ToList();
                //List<Requisition> requisitionByDept = db.Requisition.Where(r => r.Department == d.DeptName).Where(r => r.Status == "PENDING").ToList();
                foreach (RequisitionDetails r in rdList2)
                {
                    rdListAll.Add(r);
                }
            }
            //then we work with this total rList:
            //make a list of itemcode requisition:
            
            //get all the unique products first.
            List<Products> pList = new List<Products>();
            foreach (RequisitionDetails rd in rdListAll)
            {
                Products p = db.Products.FirstOrDefault(s => s.ItemCode == rd.ItemCode);
                if (!pList.Contains(p))
                {
                    pList.Add(p);
                }
            }
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
                foreach(RequisitionDetails d in ByProductRequsitionDetails)
                {
                    TotalNeeded = TotalNeeded + d.Quantity;
                    DeptName.Add(db.Departments.FirstOrDefault(s => s.DeptName == d.Requisition.Department));
                    NeededList.Add(d.Quantity);
                }
                ICR.TotalNeeded = TotalNeeded;
                ICR.DeptName = DeptName;
                ICR.NeededList = NeededList;
                ICRList.Add(ICR);
            }

            //2. This whole list of requsitions is one retrieval
            //3. Each retrieval has a list of retrieval details

            ViewData["ICRList"] = ICRList;
            ViewData["count"] = ICRList.Count();
            //ViewBag.StoreClerkId = new SelectList(db.Employees, "EmployeeId", "EmployeeName");
            return View();
        }

        // POST: Retrievals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "RetrievalId,DateRetrieved,StoreClerkId")] Retrieval retrieval)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Retrievals.Add(retrieval);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.StoreClerkId = new SelectList(db.Employees, "EmployeeId", "EmployeeName", retrieval.StoreClerkId);
        //    return View(retrieval);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection form)
        {
            int count = int.Parse(Request.Form["count"]);

            StockAdjustmentVoucher s = new StockAdjustmentVoucher();
            int id = db.StockAdjustmentVouchers.Count() + 1;
            s.Id = "V" + id;
            s.DateCreated = DateTime.Now;

            List<StockAdjustmentVoucherDetail> sList = new List<StockAdjustmentVoucherDetail>();
            for (int i = 0; i < count; i++)
            {
                string itemcode = Request.Form["ICR[" +i+"].product"];
                int retrievedqty = int.Parse(Request.Form["ICR[" + i + "].retrieved"]); 
                int qtyininventory = int.Parse(Request.Form["ICR[" + i + "].qtyininventory"]);
                if(qtyininventory != retrievedqty)
                {
                    StockAdjustmentVoucherDetail s0 = new StockAdjustmentVoucherDetail();
                    Products p = db.Products.FirstOrDefault(o => o.ItemCode == itemcode);
                    s0.Product = p;
                    s0.ItemCode = itemcode;
                    s0.QuantityAdjusted = retrievedqty - qtyininventory;
                    s0.Status = "Pending";
                    s0.Balance = qtyininventory;
                    sList.Add(s0);
                }
            }
            s.StockAdjustmentVoucherDetails = sList;

            if (sList != null)
            {
                ViewData["s"] = s;
                ViewData["count"] = sList.Count();
                return View("AdjustRetrieval",s);
            }

            return View();
        }

        public ActionResult AdjustRetrieval([Bind(Include = "Id,DateCreated")] StockAdjustmentVoucher stockAdjustmentVoucher, FormCollection form)
        {
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
                    StockAdjustmentVoucherDetail s0 = new StockAdjustmentVoucherDetail();
                    s0.ItemCode = itemcode;
                    s0.QuantityAdjusted = qtyadjusted;
                    s0.Reason = reason;
                    s0.Status = "Pending";
                    s0.Product = db.Products.FirstOrDefault(o => o.Description == itemcode);
                    sList.Add(s0);

                    //StockAdjustmentVoucherDetail s0 = new StockAdjustmentVoucherDetail();
                    //Products p = db.Products.FirstOrDefault(o => o.Description == itemcode);
                    //s0.Product = p;
                    //xs0.ItemCode = itemcode;
                    //s0.QuantityAdjusted = qtyadjusted;
                    //s0.Status = "Pending";
                    ////Products p1 = db.Products.FirstOrDefault(k => k.Description == itemcode);
                    //s0.Balance = s0.Product.Balance;
                    //s0.Reason = reason;
                    //sList.Add(s0);
                }
            }
            
            //C:
            stockAdjustmentVoucher.StockAdjustmentVoucherDetails = sList;
            db.StockAdjustmentVouchers.Add(stockAdjustmentVoucher);
            db.SaveChanges();
            //D:
            Dispose();
            StockAdjustmentVoucher s = new StockAdjustmentVoucher();
            StockAdjustmentVouchersController c = new StockAdjustmentVouchersController();
            c.AllocateAuthorizer(stockAdjustmentVoucher);
            ViewData["count"] = count;
            return View();
        }
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
