using MvcUniJudje.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;
using System.Web.Mvc;
using MvcUniJudje.WebReference;
using System.Threading.Tasks;
using MvcUniJudje;

namespace MvcUniJudje.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        int VersionAPI = 3;
        bool bul = true;

        [HttpGet]
        [AllowAnonymous]
        public ViewResult Login()
        {
            return View();
        }


        /// <summary>
        /// проверка валидатности 
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Login login)
        {
            if (ModelState.IsValid)
            {
                using (var client = new ServiceForAdmin())
                {
                    int b;
                    client.GetVersionAPI(out b, out bul);
                    if (b != VersionAPI)
                    {
                        ModelState.AddModelError("", "Версия клиента" + VersionAPI + " Версия сервиса" + b);
                        return View();
                    }
                    else
                    {
                        try
                        {
                            Session["session_key"] = client.LoginUser(login.LoginName, login.Password);
                            Session["id"] = client.GetCollection(Session["session_key"].ToString(), login.LoginName).ID;
                            Session["name"] = login.LoginName;
                            // Ивезде где тебе нужны эти переменные пишешь  Session["AdminId"] as Int32, Session["session_key"] as String
                        }
                        catch (Exception ex)
                        {
                            switch (ex.Message)
                            {
                                case "InvalidLoginOrPassword":
                                    ModelState.AddModelError("Login", "Неверный логин или пароль.");
                                    break;
                                default:
                                    ModelState.AddModelError("Login", ex.Message);
                                    throw new Exception(ex.Message, ex);
                            }
                            return View();
                        }
                        Session["flag"] = true;
                  
                      return  RedirectToAction("Index", "Home");
                    }
                }
            }
            return View();
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

    }
}
