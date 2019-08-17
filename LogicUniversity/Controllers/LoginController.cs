using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LogicUniversity.Context;
using LogicUniversity.Models;
using LogicUniversity.Services;

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
                Employee obj = db.Employees.Where(a => a.Username == username && a.Password == password).FirstOrDefault();
                if (obj != null)
                {
                    string uname = username.ToString();
                    Session["USER"] = obj;
                    Session["UserID"] = uname;
                    Session["UserName"] = password.ToString();
                    Session["role"] = obj.Role;
                    Session["empId"] = obj.EmployeeId;

                    string pass = password.ToString();

                    string sessionId = Guid.NewGuid().ToString();
                    Sessions.userSessions.Add(uname, sessionId);

                    Debug.WriteLine(sessionId);
                    Session["id"] = sessionId;
                    ViewData["sessionId"] = sessionId;
                    switch (Session["role"])
                    {
                        case "DEP_STAFF":
                            return RedirectToAction("StaffDashboard", "Login", new { sessionId = sessionId });
                        case "DEP_MNGR":
                            return RedirectToAction("HODDashboard", "Login", new { sessionId = sessionId });
                        case "DEP_REP":
                            return RedirectToAction("SampleView", "Login", new { sessionId = sessionId });
                        case "STORE_CLRK":
                            return RedirectToAction("SCDashboard", "Login", new { sessionId = sessionId });
                        case "STORE_MNGR":
                            return RedirectToAction("SMDashboard", "Login", new { sessionId = sessionId });
                        case "STORE_SUP":
                            return RedirectToAction("SSDashboard", "Login", new { sessionId = sessionId });
                    }

                    return RedirectToAction("Login");
                }

            }
            return View();
        }

        public ActionResult StaffDashboard(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult HODDashboard(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult SMDashboard(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult SampleView(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult SCDashboard(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }


        public ActionResult SSDashboard(string sessionId)
        {
            if (Sessions.IsValidSession(sessionId))
            {
                ViewData["sessionId"] = sessionId;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult Logout()
        {
            var username = Sessions.userSessions.Keys.OfType<String>().FirstOrDefault(s => Sessions.userSessions[s] == Session["id"]);
            //Debug.WriteLine("username is: " + username);
            Session["id"] = null;
            Sessions.userSessions.Remove(username);

            //Debug.WriteLine("hashtable size is: " + Sessions.userSessions.Count);
            //Debug.WriteLine("sessionid is: " + Sessions.userSessions[username]);
            return RedirectToAction("Login", "Login");
        }
    }
}
