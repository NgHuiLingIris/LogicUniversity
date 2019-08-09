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

namespace LogicUniversity.Controllers
{
    public class DelegationsController : Controller
    {
        private LogicUniversityContext db = new LogicUniversityContext();

        // GET: Delegations

        public ActionResult ManageDelegation()
        {
            return View();
        }


        public ActionResult ViewDelegation()
        {
            int empId = (int)Session["empId"];
            var deptId = db.Employees.Where(r => r.EmployeeId == empId).Select(r => r.Department.DeptId).SingleOrDefault();
            var delegations = db.Delegations.Include(d => d.Employee).Where(d=>d.Employee.DeptId==deptId);
            return View(delegations.ToList());
        }

        // GET: Delegations/Details/5
        public ActionResult GetDelegationDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delegation delegation = db.Delegations.Find(id);
            if (delegation == null)
            {
                return HttpNotFound();
            }
            return View(delegation);
        }

        // GET: Delegations/Create
        public ActionResult AppointDelegation()
        {
            int empId = (int)Session["empId"];
            var deptId = db.Employees.Where(r => r.EmployeeId == empId).Select(r => r.Department.DeptId).SingleOrDefault();
            ViewBag.EmployeeId = new SelectList(db.Employees.Where(d => d.Role == "DEP_STAFF" && d.DeptId == deptId), "EmployeeId", "EmployeeName");
            ViewData["list"] = null;
            return View();
        }

      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AppointDelegation([Bind(Include = "DelegationId,EmployeeId,StartDate,EndDate")] Delegation delegation)
        {

            if (ModelState.IsValid)
            {
                int empId = (int)Session["empId"];
                var deptId = db.Employees.Where(r => r.EmployeeId == empId).Select(r => r.Department.DeptId).SingleOrDefault();

                int value = DateTime.Compare(delegation.StartDate, delegation.EndDate);
                if (value > 0 || delegation.StartDate < DateTime.Now)
                {
                    return RedirectToAction("AppointDelegation", "Delegations");
                }
                else
                {
                    List<string> msglist = new List<string>();
                    var case1 = db.Delegations.Include(d => d.Employee).Where(d => d.StartDate >= delegation.StartDate && d.EndDate > delegation.EndDate && d.Employee.DeptId == deptId).ToList();
                    var case2 = db.Delegations.Include(d => d.Employee).Where(d => d.StartDate < delegation.StartDate && d.EndDate <= delegation.EndDate && d.Employee.DeptId == deptId).ToList();
                    var case3 = db.Delegations.Include(d => d.Employee).Where(d => d.StartDate >= delegation.StartDate && d.EndDate <= delegation.EndDate && d.Employee.DeptId == deptId).ToList();
                    var case4 = db.Delegations.Include(d => d.Employee).Where(d => d.StartDate < delegation.StartDate && d.EndDate > delegation.EndDate && d.Employee.DeptId == deptId).ToList();
                    ViewBag.EmployeeId = new SelectList(db.Employees.Where(d => d.Role == "DEP_STAFF" && d.DeptId == deptId), "EmployeeId", "EmployeeName");
                    if (case1.Count == 0 && case2.Count == 0 && case3.Count == 0 && case4.Count == 0)
                    {
                        Employee emp = db.Employees.Find(delegation.EmployeeId);
                        emp.Isdelegateded = "Y";
                        db.Delegations.Add(delegation);
                        db.SaveChanges();
                        //MailMessage Message = default(MailMessage);
                        //SmtpClient Client = default(SmtpClient);
                        //Message = new MailMessage();
                        //Message.From = new MailAddress(" logicuniversity.t6@gmail.com ");//give your From address
                        //Message.To.Add(new MailAddress("rnair.reshma31@gmail.com"));// give TO address
                        //Message.Subject = "Delegation Assigned";
                        //Message.Body = "Testing";
                        //Client = new SmtpClient();
                        //Client.Host = "LAPTOP-97C2OE7M";// give your host name
                        //Client.Port = 50519; // give your port number
                        //Client.Send(Message);
                        //Message.Dispose();

                        return RedirectToAction("ViewDelegation");
                    }
                    else
                    {

                        if (case3.Count != 0)
                        {

                            foreach (var item in case3)
                            {
                                string name = item.Employee.EmployeeName.ToString();
                                string startdate = (string)item.StartDate.ToString();
                                string enddate = (string)item.EndDate.ToString();
                                msglist.Add(name + " is already deleagated from " + startdate + " to" + enddate);
                            }
                            ViewData["list"] = msglist;
                            return View("AppointDelegation");
                        }

                        else if (case4.Count != 0)
                        {

                            foreach (var item in case4)
                            {
                                string name = item.Employee.EmployeeName.ToString();
                                string startdate = (string)item.StartDate.ToString();
                                string enddate = (string)item.EndDate.ToString();
                                msglist.Add(name + " is already deleagated from " + startdate + " to" + enddate);
                            }
                            ViewData["list"] = msglist;
                            return View("AppointDelegation");
                        }
                  
                        else if (case1.Count != 0)
                        {
                            foreach (var item in case1)
                            {
                                if (item.StartDate > delegation.EndDate)
                                {
                                    Employee emp = db.Employees.Find(delegation.EmployeeId);
                                    emp.Isdelegateded = "Y";
                                    db.Delegations.Add(delegation);
                                    db.SaveChanges();
                                    return RedirectToAction("ViewDelegation");
                                }
                                else
                                {
                                   
                                    string name = item.Employee.EmployeeName.ToString();
                                    string startdate = (string)item.StartDate.ToString();
                                    string enddate = (string)item.EndDate.ToString();
                                    msglist.Add(name + " is already deleagated from " + startdate + " to" + enddate);
                                }

                            }
                            ViewData["list"] = msglist;
                            return View("AppointDelegation");
                        }

                        else if (case2.Count != 0)
                        {

                            foreach (var item in case2)
                            {
                                if (item.EndDate < delegation.StartDate)
                                {
                                    Employee emp = db.Employees.Find(delegation.EmployeeId);
                                    emp.Isdelegateded = "Y";
                                    db.Delegations.Add(delegation);
                                    db.SaveChanges();
                                    return RedirectToAction("ViewDelegation");
                                }
                                else
                                {
                                    string name = item.Employee.EmployeeName.ToString();
                                    string startdate = (string)item.StartDate.ToString();
                                    string enddate = (string)item.EndDate.ToString();
                                    msglist.Add(name + " is already deleagated from " + startdate + " to" + enddate + " ");

                                }
                            }
                            ViewData["list"] = msglist;
                            return View("AppointDelegation");


                        }
                       
                    }



                }

            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeName", delegation.EmployeeId);
            return View(delegation);
        }



        // GET: Delegations/Edit/5
        public ActionResult EditDelegation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delegation delegation = db.Delegations.Find(id);
            if (delegation == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeName", delegation.EmployeeId);
            return View(delegation);
        }

        // POST: Delegations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDelegation([Bind(Include = "DelegationId,EmployeeId,StartDate,EndDate")] Delegation delegation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(delegation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewDelegation");
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeName", delegation.EmployeeId);
            return View(delegation);
        }

        // GET: Delegations/Delete/5
        public ActionResult CancelDelegation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delegation delegation = db.Delegations.Find(id);
            if (delegation == null)
            {
                return HttpNotFound();
            }
            return View(delegation);
        }

        // POST: Delegations/Delete/5
        [HttpPost, ActionName("cancelDelegation")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Delegation delegation = db.Delegations.Find(id);
            Employee emp = db.Employees.Find(delegation.EmployeeId);
            var delegated = db.Delegations.Include(d => d.Employee).Where(d => d.DelegationId != id).ToList();
            foreach (var item in delegated)
            {
                if (item.EmployeeId == delegation.EmployeeId)
                { emp.Isdelegateded = "Y"; }

                else
                {
                    emp.Isdelegateded = "N";
                }
            }

            db.Delegations.Remove(delegation);
            db.SaveChanges();
            return RedirectToAction("Index");
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
