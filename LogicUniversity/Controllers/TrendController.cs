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
            List<Products> products = db.Products.ToList();
            List<DataPoint> dataPoints = new List<DataPoint>();
            

            var result = from order in orderDetails
                         group order by order.Products.Category into prodCat
                         select new
                         {
                             Category = prodCat.Key,
                             Quantity = prodCat.Sum(x => x.Quantity),
                         };

            foreach(var x  in result )
            {
                dataPoints.Add(new DataPoint(x.Category, x.Quantity));

            }

            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            return View();
        }

        //public ActionResult DepartmentTrend()
        //{

        //    List<OrderDetail> orderDetails = db.OrderDetails.ToList();
        //    List<Products> products = db.Products.ToList();
        //    List<DataPoint> dataPoints = new List<DataPoint>();


        //    var result = from order in orderDetails
        //                 group order by order.Products into prodCat
        //                 select new
        //                 {
        //                     Category = prodCat.Key,
        //                     Quantity = prodCat.Sum(x => x.Quantity),
        //                 };

        //    foreach (var x in result)
        //    {
        //        dataPoints.Add(new DataPoint(x.Category, x.Quantity));

        //    }

        //    ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
        //    return View();
        //}
    }
}