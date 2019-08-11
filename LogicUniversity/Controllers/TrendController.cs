using LogicUniversity.Context;
using LogicUniversity.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogicUniversity.Controllers
{
    public class TrendController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();
        // GET: Trend
        public ActionResult Index()
        {

            List<OrderDetail> orderDetails = db.OrderDetails.ToList();
            List<RequisitionDetails> requisitionDetails = db.RequisitionDetails.ToList();
            //List<Products> products = db.Products.ToList();
            List<DataPoint> dataPoints_1 = new List<DataPoint>();
            List<DataPoint> dataPoints_2 = new List<DataPoint>();
            List<DataPoint> dataPoints_3 = new List<DataPoint>();

            // Trend Logic - Category based Orders
            var result_1 = from order in orderDetails
                           //where order.Products != null
                           group order by order.Products.Category into prodCat
                           
                           select new
                           {
                               Category = prodCat.Key,
                               Quantity = prodCat.Sum(x => x.Quantity),
                           };

            foreach (var x in result_1)
            {
                dataPoints_1.Add(new DataPoint(x.Category, x.Quantity));

            }

            // Trend Logic - Supplier based Orders
            var result_2 = from order in orderDetails
                           //where order.Products != null
                           group order by order.Products.Supplier.SupplierName into prodCat
                           select new
                           {
                               Category = prodCat.Key,
                               Quantity = prodCat.Sum(x => x.Quantity),
                           };

            foreach (var x in result_2)
            {
                dataPoints_2.Add(new DataPoint(x.Category, x.Quantity));

            }

            
            // Trend Logic - Departement based Requisition
            var result_3 = from request in requisitionDetails
                           group request by request.Requisition.Employee.Department.DeptName into prodCat
                           select new
                           {
                               Category = prodCat.Key,
                               Quantity = prodCat.Sum(x => x.Quantity),
                           };

            foreach (var x in result_3)
            {
                dataPoints_3.Add(new DataPoint(x.Category, x.Quantity));

            }

            ViewBag.DataPoints_1 = JsonConvert.SerializeObject(dataPoints_1);
            ViewBag.DataPoints_2 = JsonConvert.SerializeObject(dataPoints_2);
            ViewBag.DataPoints_3 = JsonConvert.SerializeObject(dataPoints_3);
            return View();
        }

        public ActionResult DepartmentTrend()
        {
            List<RequisitionDetails> requisitionDetails = db.RequisitionDetails.ToList();

            List<DataPoint> dataPoints_1 = new List<DataPoint>();
            List<DataPoint> dataPoints_2 = new List<DataPoint>();

            // Trend Logic - Departement based Requisition
            var result_1 = from request in requisitionDetails
                           group request by request.Requisition.Employee.Department.DeptName into prodCat
                           select new
                           {
                               Category = prodCat.Key,
                               Quantity = prodCat.Sum(x => x.Quantity),
                           };

            foreach (var x in result_1)
            {
                dataPoints_1.Add(new DataPoint(x.Category, x.Quantity));

            }
            ViewBag.DataPoints_1 = JsonConvert.SerializeObject(dataPoints_1);

            
            return View();
        }
    }
}