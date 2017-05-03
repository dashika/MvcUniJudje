using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcUniJudje.Controllers.WorkStation
{
    public class SubmitController : Controller
    {

        MvcUniJudje.Models.Login log = new MvcUniJudje.Models.Login();

        //
        // GET: /Submit/
         [OutputCache(Location = System.Web.UI.OutputCacheLocation.Any, Duration = 60)]
        public ActionResult Index()
        {
            ValidationIn val = new ValidationIn();
            if (val.Validation("Сабмиты", Session["session_key"].ToString(), Session["name"].ToString()))
            {
                var client = new MvcUniJudje.WebReference.ServiceForAdmin();
                MvcUniJudje.WebReference.SubmitDTO[] SDTO = client.WebGetSubmits(Session["session_key"].ToString());

                return View(SDTO);
            }
            return RedirectToAction("Index", "Home");
        }

    }
}
