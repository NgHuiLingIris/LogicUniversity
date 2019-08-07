using LogicUniversity.Context;
using LogicUniversity.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogicUniversity.Controllers
{
    public class RequisitionController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();
      

        // Action Result for Listing/ Searching the values from the Products Table and adding to Cart
        public ActionResult Index(String search,String Id)
        {
            if (Id != null)
            {
                var username = Session["UserID"].ToString();
                Employee obj = db.Employees.Where(a => a.Username.Equals(username)).FirstOrDefault();
                Products p = db.Products.Where(x => x.ItemCode == Id).SingleOrDefault();
                CartItem c = new CartItem();
                c.ItemCode = p.ItemCode;
                c.Category = p.Category;
                c.Description = p.Description;
                c.UOM = p.UOM;
                c.EmployeeId = obj.EmployeeId;

                          
                db.CartItems.AddOrUpdate(c);
                db.SaveChanges();
            }
            if (search !=null)
            {
                return View(db.Products.Where(x => x.Category == search).ToList());
            }
            return View(db.Products.OrderBy(x => x.Category).ToList());

        }

        
        public ActionResult AddtoCart(string id)
        {
            if (Session["UserID"] != null)
            {
                if (id != null)
                {

                    db.CartItems.Remove(db.CartItems.Single(c => c.ItemCode == id));
                    db.SaveChanges();
                }
                var username = Session["UserID"].ToString();
                Employee obj = db.Employees.Where(a => a.Username.Equals(username)).FirstOrDefault();
                List<CartItem> cartItem = db.CartItems.OrderBy(x => x.EmployeeId == obj.EmployeeId).ToList();
                return View(cartItem);
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddtoCart(FormCollection CartItems)
        {
            if (Session["UserID"] != null)
            {
                string item_code = null;
                string item_qty = null;
                if (ModelState.IsValid)
                {
                    var username = Session["UserID"].ToString();
                    Employee obj = db.Employees.Where(a => a.Username.Equals(username)).FirstOrDefault();
                    Requisition request = new Requisition();
                    request.Date = DateTime.Now;
                    request.Status = "PENDING";
                    request.EmployeeId = obj.EmployeeId;
                    request.ApproverId = obj.ApproverId;
                    request.Department = obj.Department.DeptName;

                    db.Requisition.Add(request);
                    db.SaveChanges();

                    RequisitionDetails requestDetails = new RequisitionDetails();

                    foreach (var key in CartItems.AllKeys)
                    {

                        item_code = CartItems["ItemCode"];
                        item_qty = CartItems["Quantity"];

                    }
                    String[] item_code_s = item_code.Split(',');
                    String[] item_qty_s = item_qty.Split(',');

                    for (int i = 0; i < item_code_s.Length; i++)
                    {
                        requestDetails.RequisitionId = request.RequisitionId;
                        requestDetails.ItemCode = item_code_s[i];
                        requestDetails.Quantity = Int32.Parse(item_qty_s[i]);
                        db.RequisitionDetails.Add(requestDetails);
                        db.SaveChanges();
                    }

                    db.CartItems.RemoveRange(db.CartItems.Where(x =>x.EmployeeId==obj.EmployeeId));
                    db.SaveChanges();


                    return RedirectToAction("Index");
                }

                return RedirectToAction("AddtoCart");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        private static DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }

        
    }
}
