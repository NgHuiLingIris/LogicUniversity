using LogicUniversity.Context;
using LogicUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicUniversity.Services
{
    public class DelegationService
    {
        private LogicUniversityContext db = new LogicUniversityContext();
        public void AddDelegation(Delegation delegation,int empId)
        {
            Employee emp = db.Employees.Find(delegation.EmployeeId);
            emp.Isdelegateded = "Y";
            db.Delegations.Add(delegation);
            db.SaveChanges();
            EmailService.SendNotification(delegation.EmployeeId, "Delegation Appointment reg.", "You are delegated Department Head responsibility from " + delegation.StartDate + " to" + delegation.EndDate);
            EmailService.SendNotification(empId, "Delegation Appointment reg.", delegation.Employee.EmployeeName + " is delegated from " + delegation.StartDate + "to" + delegation.EndDate);
        }

        public List<string> CaptureErrorMsg(Delegation item)
        {
            List<string> details = new List<string>();
            string name = item.Employee.EmployeeName.ToString();
            string startdate = (string)item.StartDate.ToString();
            string enddate = (string)item.EndDate.ToString();
            details.Add(name);
            details.Add(startdate);
            details.Add(enddate);
            return details;
        }

        public void saveDelegationChanges(Delegation delegation,int id)
        {
            var originalDelegation = db.Delegations.Find(delegation.DelegationId);

            string[] delg = { (originalDelegation.StartDate).ToString(),(originalDelegation.EndDate).ToString()};
            originalDelegation.StartDate = delegation.StartDate;
            originalDelegation.EndDate = delegation.EndDate;
            originalDelegation.EmployeeId = delegation.EmployeeId;
            db.SaveChanges();
            EmailService.SendNotification(delegation.EmployeeId, "Change of delegation date","Delegation date has been changed from "+delg[0]+" - "+delg[1]+" to "+delegation.StartDate+" - "+delegation.EndDate);
            EmailService.SendNotification(id, "Change of delegation date", "Delegation date has been changed from " + delg[0] + " - " + delg[1] + " to " + delegation.StartDate + " - " + delegation.EndDate);
        }
    }
}