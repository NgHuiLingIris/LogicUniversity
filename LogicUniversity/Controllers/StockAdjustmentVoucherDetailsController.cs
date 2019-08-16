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

namespace LogicUniversity.Controllers //Written By Iris
{
    public class StockAdjustmentVoucherDetailsController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();

        // GET: StockAdjustmentVoucherDetails
        public ActionResult Index(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                var stockAdjustmentVoucherDetails = db.StockAdjustmentVoucherDetails.Include(s => s.Product).Include(s => s.StockAdjustmentVoucher);
                return View(stockAdjustmentVoucherDetails.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: StockAdjustmentVoucherDetails/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockAdjustmentVoucherDetail stockAdjustmentVoucherDetail = db.StockAdjustmentVoucherDetails.Find(id);
            if (stockAdjustmentVoucherDetail == null)
            {
                return HttpNotFound();
            }
            return View(stockAdjustmentVoucherDetail);
        }

        // GET: StockAdjustmentVoucherDetails/Create
        public ActionResult Create(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                ViewBag.ItemCode = new SelectList(db.Products, "ItemCode", "Category");
                ViewBag.StockAdjustmentVoucherId = new SelectList(db.StockAdjustmentVouchers, "Id", "Id");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: StockAdjustmentVoucherDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockAdjustmentVoucherId,ItemCode,Reason,QuantityAdjusted,Status,ApproverRemarks,Balance,Approver")] StockAdjustmentVoucherDetail stockAdjustmentVoucherDetail,string sessionId)
        {
            if (sessionId == null)
            {
                sessionId = Request["sessionId"];
            }
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                if (ModelState.IsValid)
                {
                    db.StockAdjustmentVoucherDetails.Add(stockAdjustmentVoucherDetail);
                    db.SaveChanges();
                    return RedirectToAction("Index",new { sessionId = sessionId });
                }

                ViewBag.ItemCode = new SelectList(db.Products, "ItemCode", "Category", stockAdjustmentVoucherDetail.ItemCode);
                ViewBag.StockAdjustmentVoucherId = new SelectList(db.StockAdjustmentVouchers, "Id", "Id", stockAdjustmentVoucherDetail.StockAdjustmentVoucherId);
                return View(stockAdjustmentVoucherDetail);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: StockAdjustmentVoucherDetails/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockAdjustmentVoucherDetail stockAdjustmentVoucherDetail = db.StockAdjustmentVoucherDetails.Find(id);
            if (stockAdjustmentVoucherDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemCode = new SelectList(db.Products, "ItemCode", "Category", stockAdjustmentVoucherDetail.ItemCode);
            ViewBag.StockAdjustmentVoucherId = new SelectList(db.StockAdjustmentVouchers, "Id", "Id", stockAdjustmentVoucherDetail.StockAdjustmentVoucherId);
            return View(stockAdjustmentVoucherDetail);
        }

        // POST: StockAdjustmentVoucherDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockAdjustmentVoucherId,ItemCode,Reason,QuantityAdjusted,Status,ApproverRemarks,Balance,Approver")] StockAdjustmentVoucherDetail stockAdjustmentVoucherDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockAdjustmentVoucherDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ItemCode = new SelectList(db.Products, "ItemCode", "Category", stockAdjustmentVoucherDetail.ItemCode);
            ViewBag.StockAdjustmentVoucherId = new SelectList(db.StockAdjustmentVouchers, "Id", "Id", stockAdjustmentVoucherDetail.StockAdjustmentVoucherId);
            return View(stockAdjustmentVoucherDetail);
        }

        // GET: StockAdjustmentVoucherDetails/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockAdjustmentVoucherDetail stockAdjustmentVoucherDetail = db.StockAdjustmentVoucherDetails.Find(id);
            if (stockAdjustmentVoucherDetail == null)
            {
                return HttpNotFound();
            }
            return View(stockAdjustmentVoucherDetail);
        }

        // POST: StockAdjustmentVoucherDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            StockAdjustmentVoucherDetail stockAdjustmentVoucherDetail = db.StockAdjustmentVoucherDetails.Find(id);
            db.StockAdjustmentVoucherDetails.Remove(stockAdjustmentVoucherDetail);
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

        // GET: StockAdjustmentVoucherDetails/FilterView/V1
        public ActionResult FilterView(string voucherId,string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                var detailById = from v in db.StockAdjustmentVoucherDetails
                                 where v.StockAdjustmentVoucherId == voucherId
                                 select v;

                var detailByIdIncluded = detailById.Include(s => s.Product).Include(s => s.StockAdjustmentVoucher);

                List<StockAdjustmentVoucherDetail> sDetailList = detailByIdIncluded.ToList();

                ViewData["sDetailList"] = sDetailList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        /*
        *ApproverView
	        This method collects the Role and the SearchString.
		        string Role: To split the view between the supervisor and the manager
		        string SearchString: To enable the user to query and search for the products
		        Method RetrieveApproverList: To get the list of the items under the approver requests and order in Pending, Approved, Rejected
	        Flow
		        After preparing the collective list for the authorizer, the three lists: 1. PendingItemList, 2. StatusList and 3. DatetimeList are prepared according to the latest date at the top.
		        These three lists will each display the view needed.
		        An improvement could be done to allow the use of one list with multiple object types.
         */
        [HttpGet]
        public ActionResult ApproverView(string Role, string SearchString,string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;

                List<StockAdjustmentVoucherDetail> VoucherDetailList = RetrieveApproverList(Role);

                if (SearchString != null)
                {
                    var SearchResult = VoucherDetailList.Where(x => x.Product.Description.Contains(SearchString));
                    VoucherDetailList = SearchResult.ToList();
                }
                List<Products> PendingItemList = new List<Products>();
                List<string> StatusList = new List<string>();
                List<DateTime> Datetimelist = new List<DateTime>();

                foreach (StockAdjustmentVoucherDetail p in VoucherDetailList)
                {
                    if (!PendingItemList.Contains(p.Product))
                    {
                        PendingItemList.Add(p.Product);
                        StatusList.Add(p.Status);
                        Datetimelist.Add(p.StockAdjustmentVoucher.DateCreated);
                    }

                }
                ViewData["PendingItemList"] = PendingItemList;
                ViewData["StatusList"] = StatusList;
                ViewData["Datetimelist"] = Datetimelist;
                ViewData["Role"] = Role;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        /*
         * ApproverAction
	        Before coming to this Action Method, the user would have selected the Pending item codes for approver.
	        This method retrieve the selected item code and calculate the total cost incurred in all pending items.
	        Improvement: StockAdjustmentVoucherDetails could be pass as a string from the view to this controller.
            */
        [HttpGet]
        public ActionResult ApprovalAction(string ItemCode, string Role,string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                var VoucherDetail = from v in db.StockAdjustmentVoucherDetails
                                    where v.ItemCode == ItemCode && v.Status == "Pending"
                                    select v;
                List<StockAdjustmentVoucherDetail> DetailList = VoucherDetail.Include(s => s.Product).Include(s => s.StockAdjustmentVoucher).ToList();
                double TotalAmount = 0;
                foreach (StockAdjustmentVoucherDetail d in DetailList)
                {
                    TotalAmount += d.QuantityAdjusted * d.Product.UnitPrice;
                }
                ViewData["Role"] = Role;
                ViewData["ItemCodeString"] = ItemCode;
                ViewData["DetailList"] = DetailList;
                ViewData["TotalAmount"] = TotalAmount;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        /*
         In the Post Method of ApproverAction,
	        Before this, the approver would have written the remarks and determine whether to approve or reject.
	        Hence, from the view, there are two buttons.
	        Improvement: The details of stock adjustment vouchers could be pass as a string instead of querying again.
	        A list of the vouchers is retrieved.
	        DateCreated is listed in ascending order. This is an important business logic because the quantity adjusted is deducted from the balance chronologically.
	        There is FinalProductBalance at the end which is always updated for each VoucherDetailList and finally becomes the product balance.
	        Both Manager and Supervisor access will have the same logic
             */
        [HttpPost]
        public ActionResult ApprovalAction(FormCollection form, string sessionId)
        {
            if (sessionId == null)
            {
                sessionId = Request["sessionId"];
            }
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                string Remarks = form["ApproverRemarks"];
                string ItemCodeString = form["ItemCodeString"];
                string Role = form["Role"];

                var VoucherDetail = from v in db.StockAdjustmentVoucherDetails
                                    where v.ItemCode == ItemCodeString && v.Status == "Pending"
                                    select v;
                var VoucherDetailInclude = VoucherDetail.Include(s => s.Product).Include(s => s.StockAdjustmentVoucher);
                var VoucherDetailListOrderDate = from v in VoucherDetailInclude
                                                 orderby v.StockAdjustmentVoucher.DateCreated ascending
                                                 select v;
                List<StockAdjustmentVoucherDetail> VoucherDetailList = VoucherDetailListOrderDate.ToList();

                double FinalProductBalance = 0;
                foreach (StockAdjustmentVoucherDetail v in VoucherDetailList)
                {

                    v.ApproverRemarks = Remarks;

                    if (Request.Form["Reject"] != null)
                    {
                        v.Status = "Rejected";
                    }
                    else if (Request.Form["Approve"] != null)
                    {
                        double NewBalance = v.Product.Balance + v.QuantityAdjusted;
                        v.Balance = NewBalance;
                        v.Product.Balance = NewBalance;
                        FinalProductBalance = NewBalance;
                        v.Status = "Approved";
                    }
                    db.Entry(v).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("ApproverView", new { Role = Role ,sessionId=sessionId});
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        /*
         The RetrieveApproveList is a sub-method called by the preceding methods and do not return any view.
            This method allows the factorizing of Supervisor or Manager view.
            DateCreated is ordered by Descending so that the returned list can display the latest date
            Improvement: The Pending, Approved and Rejected status could be ordered by a linq query.
             */
        public List<StockAdjustmentVoucherDetail> RetrieveApproverList (string ApproverRole)
        {
            List<Products> ProductList = db.Products.ToList();
            var VoucherDetail = from v in db.StockAdjustmentVoucherDetails
                                where v.Approver == ApproverRole
                                select v;
            var VoucherDetailInclude = VoucherDetail.Include(s => s.Product).Include(s => s.StockAdjustmentVoucher);
            var VoucherDetailListOrderDate = from v in VoucherDetailInclude
                                             orderby v.StockAdjustmentVoucher.DateCreated descending
                                             select v;
            var VoucherDetailListOrderPending = from v in VoucherDetailListOrderDate
                                                where v.Status == "Pending"
                                                select v;
            var VoucherDetailListOrderApproved = from v in VoucherDetailListOrderDate
                                                 where v.Status == "Approved"
                                                 select v;
            var VoucherDetailListOrderRejected = from v in VoucherDetailListOrderDate
                                                 where v.Status == "Rejected"
                                                 select v;
            List<StockAdjustmentVoucherDetail> VoucherDetailList = VoucherDetailListOrderPending.Concat(VoucherDetailListOrderApproved).Concat(VoucherDetailListOrderRejected).ToList();

            return VoucherDetailList;
        }
    }
}
