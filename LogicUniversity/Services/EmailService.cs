using LogicUniversity.Context;
using LogicUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace LogicUniversity.Services
{
    public class EmailService
    {
        private static LogicUniversityContext db = new LogicUniversityContext();
        public static Employee GetUser(int empid)
        {
            Employee temp = db.Employees.Where(x => x.EmployeeId == empid).FirstOrDefault();
            return temp;
        }

        public static bool SendNotification(int empid, string sub, string msg)
        {
            Employee obj = EmailService.GetUser(empid);

            //Configuring webMail class to send emails  
            //gmail smtp server  
            WebMail.SmtpServer = "smtp.gmail.com";
            //gmail port to send emails  
            WebMail.SmtpPort = 587;
            WebMail.SmtpUseDefaultCredentials = true;
            //sending emails with secure protocol  
            WebMail.EnableSsl = true;
            //EmailId used to send emails from application  
            WebMail.UserName = "logicuniversity.t6@gmail.com";
            WebMail.Password = "LogicUniv123";

            //Sender email address.  
            WebMail.From = "logicuniversity.t6@gmail.com";

            //Send email  
            WebMail.Send(to: obj.Email, subject: sub, body: msg, isBodyHtml: true);


            return true;
        }

      
    }
}