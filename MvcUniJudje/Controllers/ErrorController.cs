using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcUniJudje.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
            Session["flag"] = false;
            Session["name"] = "";
            Session["session_key"] = "";
            Session["id"] = null;
            Session.Clear();
            ModelState.AddModelError("Login", "Сессия была завершена. Выполните вход заново.");
            return RedirectToAction("Login", "Account");
        }

    }
}
