using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Web.Mvc;

namespace LogicUniversity.Services
{
    public class Sessions
    {
        public static Hashtable userSessions = new Hashtable();

        public static bool IsValidSession(string sessionId)
        {
            HttpContext context = HttpContext.Current;
            return (context.Session["UserID"] != null &&
                Sessions.userSessions[context.Session["UserID"]] != null &&
                Sessions.userSessions[context.Session["UserID"]].ToString() == sessionId);
        }
    }
}