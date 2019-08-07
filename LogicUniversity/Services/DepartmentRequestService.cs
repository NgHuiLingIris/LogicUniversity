using LogicUniversity.Context;
using LogicUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogicUniversity.Services
{
    public class DepartmentRequestService
    {
        private static LogicUniversityContext db = new LogicUniversityContext();
        public static Employee GetUser(String Username)
        {
            Employee temp = db.Employees.Where(x => x.Username == Username).FirstOrDefault();
            return temp;
        }


        public static bool CartSubmission(String UserName,FormCollection CartItems)
        {
            string item_code = null;
            string item_qty = null;
            var username = UserName;
            Employee obj = DepartmentRequestService.GetUser(UserName);
                Requisition request = new Requisition();
                request.Date = DateTime.Now;
                request.Status = "PENDING";
                request.EmployeeId = obj.EmployeeId;
                request.ApproverId = obj.ApproverId;
                request.Department = obj.Department.DeptName;

                db.Requisition.Add(request);
                db.SaveChanges();
            EmailService.SendNotification(obj.ApproverId, "Waiting for Approval", "You have received an requisition. Waiting for Approval");
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

                db.CartItems.RemoveRange(db.CartItems.Where(x => x.EmployeeId == obj.EmployeeId));
                db.SaveChanges();
            return true;
            }

    }
}