
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MvcUnijudjes.Controllers
{
    public class HomeController : Controller
    {

        MvcUniJudje.Models.Login log = new MvcUniJudje.Models.Login();
        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (!(Convert.ToBoolean( Session["flag"] )))
                return RedirectToAction("Login", "Account");
            return View();
        }


        public ActionResult LogOff(MvcUniJudje.Models.Login log)
        {
            Session["flag"] = false;
            Session["name"] = "";
            Session["session_key"] = "";
            Session["id"] = null;
          //  return View();
         return RedirectToAction("Index", "Home");
        }

        public ActionResult SendEmail()
        {
            return View();
        }
    }
}
