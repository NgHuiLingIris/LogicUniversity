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
using PagedList;

namespace LogicUniversity.Controllers
{
    public class ChargeBackDepartmentController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();

        public ActionResult ChargeBackDepartment()
        {

            List<String> dep = db.Departments.Select(r => r.DeptName).ToList();
            //ViewData["DepartmentName"] = dep;
            ViewData["DepartmentName"] = db.Departments.ToList();
            var Dept_Name = (db.Departments.Select(r => r.DeptName).First()).ToString();
            List<string> months = (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames).ToList();
            ViewData["months"] = months;
            ViewData["deptName"] = Dept_Name;


            return View();
        }


        // GET: Chargeback
        public ActionResult ShowChargeBackDepartment(string Dept_Name, string yearselected, string monthselected)
        {
            List<String> dep = db.Departments.Select(r => r.DeptName).ToList();
            ViewData["DepartmentName"] = dep;

            List<string> months = (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames).ToList();
            ViewData["months"] = months;

            int month;
            string year = yearselected;
            if (Dept_Name == "")
            { Dept_Name = (db.Departments.Select(r => r.DeptName).First()).ToString(); }

            if (monthselected == "")
            {
                month = DateTime.Now.Month;
            }
            else
            {
                month = (months.IndexOf(monthselected)) + 1;
            }
            if (yearselected == "")
            {
                year = DateTime.Now.Year.ToString();
                //year = "1894";

            }


            List<Products> products = db.Products.ToList();
            List<DisbursementDetail> list = new List<DisbursementDetail>();
            var disbursements = (db.Disbursements.Include(d => d.CollectionPoint).Include(d=>d.Representative).Where(d => d.DateDisbursed.Month == month && d.DateDisbursed.Year.ToString() == year && d.Representative.Department.DeptName == Dept_Name && d.Status == "Delivered")).ToList();
            foreach (Disbursement d in disbursements)
            {
                List<DisbursementDetail> disbursementlist = (db.DisbursementDetails.Include(dd => dd.StockAdjustmentVoucher).Where(dd => dd.DisbursementId == d.DisbursementId)).ToList();
                foreach (DisbursementDetail item in disbursementlist)
                {
                    list.Add(item);
                }

            }

            //Combining disbursementdetails and product details for viewing and class Charge is 
            //created for this purpose
            var dlist = (from d in list
                         join p in products on d.ItemCode equals p.ItemCode
                         select new Charge
                         {
                             disburementDetail = d,
                             product = p
                         });

            //returning the combined result


            return View(dlist);
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
