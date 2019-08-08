using LogicUniversity.Context;
using LogicUniversity.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogicUniversity.Controllers
{
    public class ReportController : Controller
    {
        LogicUniversityContext db = new LogicUniversityContext();
        // GET: Report
        public ActionResult Index()
        {

            List <Order> order = new List<Order>();
            order = db.Orders.ToList();




            List<DataPoint> dataPoints = new List<DataPoint>();

            dataPoints.Add(new DataPoint("USA", 121));
            dataPoints.Add(new DataPoint("Great Britain", 67));
            dataPoints.Add(new DataPoint("China", 70));
            dataPoints.Add(new DataPoint("Russia", 56));
            dataPoints.Add(new DataPoint("Germany", 42));
            dataPoints.Add(new DataPoint("Japan", 41));
            dataPoints.Add(new DataPoint("France", 42));
            dataPoints.Add(new DataPoint("South Korea", 21));

            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            return View();
        }
    }
}