using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LogicUniversity.Context;


namespace LogicUniversity.Controllers
{
    public class LoginController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
               var obj = db.Employees.Where(a => a.Username.Equals(username) && a.Password.Equals(password)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserID"] = username.ToString();
                        Session["UserName"] = password.ToString();
                        Session["role"] = obj.Role;
                        Session["empId"] = obj.EmployeeId;

                    switch (Session["role"])
                    {
                        case "DEP_STAFF":
                            return RedirectToAction("StaffDashboard","Login");
                        case "DEP_MNGR":
                            return RedirectToAction("HODDashboard", "Login");
                        case "DEP_REP":
                            return RedirectToAction("SampleView", "Login");
                        case "STORE_CLRK":
                            return RedirectToAction("SCDashboard", "Login");
                        case "STORE_MNGR":
                            return RedirectToAction("SMDashboard", "Login");

                    }
                    return RedirectToAction("Login");
                    
                    }
               
            }
            return View();
        }

        public ActionResult StaffDashboard()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult HODDashboard()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult SMDashboard()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult SampleView()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult SCDashboard()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}
