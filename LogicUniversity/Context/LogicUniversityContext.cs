using LogicUniversity.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LogicUniversity.Context
{
    public class LogicUniversityContext:DbContext
    {
        public LogicUniversityContext() : base("Server=LAPTOP-MH5LDLUL; Database=LogicUniversity; Integrated Security = True")
        {
            Database.SetInitializer(new InventoryDBInitializer<LogicUniversityContext>());
        }
        //public class InventoryDBInitializer<T> : DropCreateDatabaseAlways<LogicUniversityContext>

        public class InventoryDBInitializer<T> : DropCreateDatabaseIfModelChanges<LogicUniversityContext>
        {
            protected override void Seed(LogicUniversityContext context)
            {

                base.Seed(context);
            }
        }

        public DbSet<Products> Products { get; set; }
        public DbSet<Requisition> Requisition { get; set; }        
        public DbSet<RequisitionDetails> RequisitionDetails { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<CollectionPoint> CollectionPoints { get; set; }
        public DbSet<Delegation> Delegations { get; set; }
        public DbSet<StockAdjustmentVoucher> StockAdjustmentVouchers { get; set; }
        public StockAdjustmentVoucher StockAdjustmentVoucher { get; set; }
        public DbSet<StockAdjustmentVoucherDetail> StockAdjustmentVoucherDetails { get; set; }
        public StockAdjustmentVoucherDetail StockAdjustmentVoucherDetail { get; set; }
        public Supplier Supplier { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public StockCard StockCard { get; set; }
        public DbSet<StockCard> StockCards { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public Retrieval Retrieval { get; set; }
        public DbSet<Retrieval> Retrievals { get; set; }
        public RetrievalDetail RetrievalDetail { get; set; }
        public DbSet<RetrievalDetail> RetrievalDetails { get; set; }
        public Disbursement Disbursement { get; set; }
        public DbSet<Disbursement> Disbursements { get; set; }
        public DisbursementDetail DisbursementDetail { get; set; }
        public DbSet<DisbursementDetail> DisbursementDetails { get; set; }


    }
}
