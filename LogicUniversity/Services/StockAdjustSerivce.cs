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

namespace LogicUniversity.Services
{
    public class StockAdjustSerivce
    {
        private static LogicUniversityContext db = new LogicUniversityContext();

        public StockAdjustmentVoucher AllocateAuthorizer(StockAdjustmentVoucher stockAdjustmentVoucher)
        {
            List<StockAdjustmentVoucherDetail> sDetailList = stockAdjustmentVoucher.StockAdjustmentVoucherDetails;

            foreach (StockAdjustmentVoucherDetail d0 in sDetailList)
            {
                string itemcode = d0.ItemCode;
                Products prod = db.Products.FirstOrDefault(p => p.ItemCode == itemcode);
                d0.Product = prod;
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