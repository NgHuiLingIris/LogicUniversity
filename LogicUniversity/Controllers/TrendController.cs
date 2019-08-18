using LogicUniversity.Context;
using LogicUniversity.Models;
using LogicUniversity.Services;
using Microsoft.Ajax.Utilities;
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
        public ActionResult Index(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                int empId = (int)Session["empId"];
                ViewData["Role"] = db.Employees.Where(r => r.EmployeeId == empId).Select(r => r.Role).SingleOrDefault();
                ViewData["sessionId"] = sessionId;
                List<OrderDetail> orderDetails = db.OrderDetails.ToList();
                List<RequisitionDetails> requisitionDetails = db.RequisitionDetails.ToList();
                //List<Products> products = db.Products.ToList();
                List<DataPoint> dataPoints_1 = new List<DataPoint>();
                List<DataPoint> dataPoints_2 = new List<DataPoint>();
                List<DataPoint> dataPoints_3 = new List<DataPoint>();

                // Trend Logic - Category based Orders
                var result_1 = from order in orderDetails
                                   //where order.Order.DateDelivery.Month==7
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
            else
            {
                return RedirectToAction("Login", "Login");
            }           
        }


        // Inventory Trend based on Category
        public ActionResult InventoryStatusTrend(string myDropDownList,string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                int empId = (int)Session["empId"];
                ViewData["Role"] = db.Employees.Where(r => r.EmployeeId == empId).Select(r => r.Role).SingleOrDefault();
                ViewData["sessionId"] = sessionId;
                List<Products> products = db.Products.ToList();

                List<DataPoint> dataPoints_1 = new List<DataPoint>();
                List<DataPoint> dataPoints_2 = new List<DataPoint>();


                // Trend Logic - Category based Inventory Status
                //if (value != null)
                //{
                var result_1 = products.Where(s => s.Category == myDropDownList).ToList();
                //}
                ////else
                //    result_1 = products.Where(s => s.Category == "Clip").ToList();

                foreach (var x in result_1)
                {
                    dataPoints_1.Add(new DataPoint(x.Description, (x.Balance - x.ReorderLevel)));

                }

                foreach (var x in result_1)
                {
                    dataPoints_2.Add(new DataPoint(x.Description, x.ReorderLevel));

                }

                var category = db.Products.DistinctBy(x => x.Category).ToList();



                //ViewData["dListAll"] = products;
                //ViewBag.Category = new SelectList( category, "Category");

                ViewBag.DataPoints_1 = JsonConvert.SerializeObject(dataPoints_1);
                ViewBag.DataPoints_2 = JsonConvert.SerializeObject(dataPoints_2);
                ViewData["sessionId"] = sessionId;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }          
        }

        public ActionResult TwoMonthsCompare(string sessionId)
        {
            int empId = (int)Session["empId"];
            ViewData["Role"] = db.Employees.Where(r => r.EmployeeId == empId).Select(r => r.Role).SingleOrDefault();
            List<OrderDetail> orderDetails = db.OrderDetails.ToList();
            List<RequisitionDetails> requisitionDetails = db.RequisitionDetails.ToList();
            //List<Products> products = db.Products.ToList();
            List<DataPoint> dataPoints_1 = new List<DataPoint>();
            List<DataPoint> dataPoints_2 = new List<DataPoint>();
            List<DataPoint> dataPoints_3 = new List<DataPoint>();
            List<DataPoint> dataPoints_4 = new List<DataPoint>();
            List<DataPoint> dataPoints_5 = new List<DataPoint>();

            // Trend Logic - Category based Orders
            var result_1 = from order in orderDetails
                           where order.Order.DateDelivery.Month == 7
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

            var result_2 = from order in orderDetails
                           where order.Order.DateDelivery.Month == 6
                           group order by order.Products.Category into prodCat
                          

                           select new
                           {
                               Category = prodCat.Key,
                               Quantity = prodCat.Sum(x => x.Quantity),
                           };

            foreach (var x in result_2)
            {
                dataPoints_2.Add(new DataPoint(x.Category, x.Quantity));

            }

           

            // Trend Logic - Supplier based Orders
            var result_3 = from order in orderDetails
                               //where order.Products != null
                           group order by order.Products.Supplier.SupplierName into prodCat
                           select new
                           {
                               Category = prodCat.Key,
                               Quantity = prodCat.Sum(x => x.Quantity),
                           };

            foreach (var x in result_3)
            {
                dataPoints_3.Add(new DataPoint(x.Category, x.Quantity));

            }

            // Trend Logic - Departement based Requisition
            var result_4 = from request in requisitionDetails
                           where request.Requisition.Date.Month==6
                           group request by request.Requisition.Employee.Department.DeptName into prodCat
                           select new
                           {
                               Category = prodCat.Key,
                               Quantity = prodCat.Sum(x => x.Quantity),
                           };

            foreach (var x in result_4)
            {
                dataPoints_4.Add(new DataPoint(x.Category, x.Quantity));

            }

            var result_5 = from request in requisitionDetails
                           where request.Requisition.Date.Month==7
                           group request by request.Requisition.Employee.Department.DeptName into prodCat
                           select new
                           {
                               Category = prodCat.Key,
                               Quantity = prodCat.Sum(x => x.Quantity),
                           };

            foreach (var x in result_5)
            {
                dataPoints_5.Add(new DataPoint(x.Category, x.Quantity));

            }
            

            ViewBag.DataPoints_1 = JsonConvert.SerializeObject(dataPoints_1.OrderBy(x => x.Label).ToList());
            ViewBag.DataPoints_2 = JsonConvert.SerializeObject(dataPoints_2.OrderBy(x => x.Label).ToList());
            ViewBag.DataPoints_3 = JsonConvert.SerializeObject(dataPoints_3);
            ViewBag.DataPoints_4 = JsonConvert.SerializeObject(dataPoints_4.OrderBy(x => x.Label).ToList());
            ViewBag.DataPoints_5 = JsonConvert.SerializeObject(dataPoints_5.OrderBy(x => x.Label).ToList());
            return View();
        }
    }
}