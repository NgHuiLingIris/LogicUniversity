using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LogicUniversity.Context;
using LogicUniversity.Models;
using LogicUniversity.Services;

namespace LogicUniversity.Controllers
{
    public class StockCardsController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();

        // GET: StockCards
        public ActionResult Index(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                var stockCards = db.StockCards.Include(s => s.product);
                return View(stockCards.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: StockCards/Details/5
        public ActionResult Details(string id,string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                //edited since change schema 31/7/19
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Products product = db.Products.Find(id);
                //retrieve all the stock adjustment voucher detail by product item code here
                List<StockAdjustmentVoucherDetail> StockAdjustmentVoucherList = new List<StockAdjustmentVoucherDetail>(db.StockAdjustmentVoucherDetails);
                List<StockAdjustmentVoucherDetail> FilterStockAdjustmentVoucher = new List<StockAdjustmentVoucherDetail>();
                var RetrieveStockAdjustmentVoucher = StockAdjustmentVoucherList.Where(s => s.ItemCode.Contains(id) && s.Status.Contains("Approved"));

                foreach (var Voucher in RetrieveStockAdjustmentVoucher)
                {
                    Voucher.StockAdjustmentVoucher = db.StockAdjustmentVouchers.Find(Voucher.StockAdjustmentVoucherId);
                    FilterStockAdjustmentVoucher.Add(Voucher);
                }

                List<StockCard> StockCardList = new List<StockCard>(db.StockCards);
                List<StockCard> FilterStockCard = new List<StockCard>();
                var RetrieveStockCard = StockCardList.Where(s => s.ItemCode.Contains(id));

                foreach (var StockCard in RetrieveStockCard)
                    FilterStockCard.Add(StockCard);

                ViewData["product"] = product;
                ViewData["stockadjustmentvoucherlist"] = FilterStockAdjustmentVoucher;
                ViewData["stockcardlist"] = FilterStockCard;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: StockCards/Create
        public ActionResult Create(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                ViewBag.ItemCode = new SelectList(db.Products, "ItemCode", "ItemCode");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: StockCards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemCode,BinNo")] StockCard stockCard,string sessionId)
        {
            sessionId = Request["sessionId"];
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                if (ModelState.IsValid)
                {
                    db.StockCards.Add(stockCard);
                    db.SaveChanges();
                    return RedirectToAction("Index",new { sessionId = sessionId });
                }

                ViewBag.ItemCode = new SelectList(db.Products, "ItemCode", "Category", stockCard.ItemCode);
                return View(stockCard);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: StockCards/Edit/5
        public ActionResult Edit(string id,string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                StockCard stockCard = db.StockCards.Find(id);
                if (stockCard == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ItemCode = new SelectList(db.Products, "ItemCode", "Category", stockCard.ItemCode);
                return View(stockCard);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: StockCards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemCode,BinNo")] StockCard stockCard,string sessionId)
        {
            sessionId = Request["sessionId"];
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                if (ModelState.IsValid)
                {
                    db.Entry(stockCard).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.ItemCode = new SelectList(db.Products, "ItemCode", "Category", stockCard.ItemCode);
                return View(stockCard);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: StockCards/Delete/5
        public ActionResult Delete(string id,string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                StockCard stockCard = db.StockCards.Find(id);
                if (stockCard == null)
                {
                    return HttpNotFound();
                }
                return View(stockCard);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: StockCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id,string sessionId)
        {
            sessionId = Request["sessionId"];
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                StockCard stockCard = db.StockCards.Find(id);
                db.StockCards.Remove(stockCard);
                db.SaveChanges();
                return RedirectToAction("Index",new { sessionId = sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
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
