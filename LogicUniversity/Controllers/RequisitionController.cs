﻿using LogicUniversity.Context;
using LogicUniversity.Models;
using LogicUniversity.Services;
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
            ViewBag.Message = TempData["message"];
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
                ViewBag.Message = "Items added to cart";
                return View(cartItem);
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddtoCart(FormCollection CartItems)
        {
            if (Session["UserID"] != null)
            {
                var username = (string)Session["UserID"];

                if (DepartmentRequestService.CartSubmission(username, CartItems))
                {
                    TempData["message"] = "Requistion Successful";
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
