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
    public class StockAdjustmentVouchersController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();

        // GET: StockAdjustmentVouchers
        // Index is ordered by descending so that the latest date is always displayed at the top.
        public ActionResult Index(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                var VouchersOrderByDate = from v in db.StockAdjustmentVouchers
                                          orderby v.DateCreated descending
                                          select v;
                return View(VouchersOrderByDate.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: StockAdjustmentVouchers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockAdjustmentVoucher stockAdjustmentVoucher = db.StockAdjustmentVouchers.Find(id);
            if (stockAdjustmentVoucher == null)
            {
                return HttpNotFound();
            }
            return View(stockAdjustmentVoucher);
        }

        // GET: StockAdjustmentVouchers/Create
        /* 
         * Create prepares a voucher id (Auto incremented)
         * It prepares the form by giving the number of rows that match with the number of product types, with an additional buffer of 5.
         * An improvement could be done that allows the user to add the number of rows in the view.
         */
        public ActionResult Create(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                List<Products> ProductList = new List<Products>(db.Products);
                ProductList.Insert(0, new Products());
                //5 is just an estimate on the extras
                int rows = db.Products.Count() + 5;

                StockAdjustmentVoucher s0 = new StockAdjustmentVoucher();
                int id = db.StockAdjustmentVouchers.Count() + 1;
                s0.Id = "V" + id;
                s0.DateCreated = DateTime.Now;

                List<StockAdjustmentVoucherDetail> s0List = new List<StockAdjustmentVoucherDetail>();
                for (int i = 0; i < rows; i++)
                {
                    s0List.Add(new StockAdjustmentVoucherDetail());
                }
                s0.StockAdjustmentVoucherDetails = s0List;
                ViewData["ProductList"] = ProductList;
                ViewData["StockAdjustmentVoucher_Default"] = s0;

                return View(s0);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: StockAdjustmentVouchers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /*
         * A. In the create stock adjustment voucher, we first retrieved all the item codes and the quantity adjusted that was passed from another form, using the form variable.
         * B. Because a single stockadjustmentvoucherdetail object cannot have the same voucherid and itemcode (primary key clashed), we would group the same item codes together.
         * C: Attach the details to the corresponding voucher and save the voucher
         * D: Allocate Authorizer will calculate the total cost of all the past pending adjustment vouchers and decide to allocate it to Supervisor or Manager
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DateCreated")] StockAdjustmentVoucher stockAdjustmentVoucher, FormCollection form,string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;

                if (ModelState.IsValid)
                {
                    //A:
                    //5 is just the buffer given in the Get
                    int rows = db.Products.Count() + 5;

                    List<StockAdjustmentVoucherDetail> s0List = new List<StockAdjustmentVoucherDetail>();

                    for (int i = 0; i < rows; i++)
                    {
                        if (form["StockAdjustmentVoucherDetails[" + i + "].ItemCode"] != "")
                        {
                            string itemcode = form["StockAdjustmentVoucherDetails[" + i + "].ItemCode"];
                            //B:
                            if (s0List.Any(s => s.ItemCode.Contains(itemcode)))
                            {
                                StockAdjustmentVoucherDetail s0 = s0List.Find(s => s.ItemCode == itemcode);
                                s0.QuantityAdjusted = s0.QuantityAdjusted + double.Parse(form["StockAdjustmentVoucherDetails[" + i + "].QuantityAdjusted"]);
                            }
                            else
                            {
                                StockAdjustmentVoucherDetail s0 = new StockAdjustmentVoucherDetail();
                                s0.ItemCode = itemcode;
                                s0.QuantityAdjusted = double.Parse(form["StockAdjustmentVoucherDetails[" + i + "].QuantityAdjusted"]);
                                s0.Reason = form["StockAdjustmentVoucherDetails[" + i + "].Reason"];
                                s0.Status = "Pending";
                                s0.Product = db.Products.Find(s0.ItemCode);
                                s0List.Add(s0);
                            }
                        }
                    }

                    //C:
                    stockAdjustmentVoucher.StockAdjustmentVoucherDetails = s0List;
                    db.StockAdjustmentVouchers.Add(stockAdjustmentVoucher);
                    db.SaveChanges();
                    //D:
                    AllocateAuthorizer(stockAdjustmentVoucher);
                    return RedirectToAction("Index",new { sessionId = sessionId });
                }

                return View(stockAdjustmentVoucher);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        
        // GET: StockAdjustmentVouchers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockAdjustmentVoucher stockAdjustmentVoucher = db.StockAdjustmentVouchers.Find(id);
            if (stockAdjustmentVoucher == null)
            {
                return HttpNotFound();
            }
            return View(stockAdjustmentVoucher);
        }

        // POST: StockAdjustmentVouchers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DateCreated")] StockAdjustmentVoucher stockAdjustmentVoucher)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockAdjustmentVoucher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stockAdjustmentVoucher);
        }

        // GET: StockAdjustmentVouchers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockAdjustmentVoucher stockAdjustmentVoucher = db.StockAdjustmentVouchers.Find(id);
            if (stockAdjustmentVoucher == null)
            {
                return HttpNotFound();
            }
            return View(stockAdjustmentVoucher);
        }

        // POST: StockAdjustmentVouchers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            StockAdjustmentVoucher stockAdjustmentVoucher = db.StockAdjustmentVouchers.Find(id);
            db.StockAdjustmentVouchers.Remove(stockAdjustmentVoucher);
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
        /*
         * AllocateAuthorizer reads the list of details attached to the StockAdjustmentVoucher
         * For each item, it will retrieve all the pending adjustmentvoucher fro the database and sum the quantity adjusted
         * Then it calculates the cost
         * Lastly, it allocates the authorizer on a <250 and >250 cut-off.
         */
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
    }
}
