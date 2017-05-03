using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcUniJudje.Controllers.WorkStation
{
    public class AdministratorController : Controller
    {
        MvcUniJudje.Models.Login log = new Models.Login();
        static FormCollection formCollection;
        FormCollection FC { get { return formCollection; } set { formCollection = value; } }
      
        //
        // GET: /Adminisrator/
         [OutputCache(Location = System.Web.UI.OutputCacheLocation.Any, Duration = 60)]
        public ActionResult Index()
        {
            ValidationIn val = new ValidationIn();
            if (val.Validation("Администраторы", Session["session_key"].ToString(), Session["name"].ToString()))
            {
                var client = new MvcUniJudje.WebReference.ServiceForAdmin();
                WebReference.AdminDTO[] list = client.GetAdmins(Session["session_key"].ToString());

                return View(list);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Index(FormCollection us)
        {
            var st = us.GetKey(1);
            FC = us;
            if (st.CompareTo("Create") != 0)
            {
                var client = new MvcUniJudje.WebReference.ServiceForAdmin();
                MvcUniJudje.WebReference.AdminDTO[] ADTO = client.GetAdmins(Session["session_key"].ToString());
                int ij = 0;
                for (int i = 0; i < ADTO.Count(); i++)
                {
                    var s = FC[(ADTO[i].ID.ToString())].Remove(2);
                    if (s.Equals("tr"))
                    {
                        ij++;
                    }
                }
                if (ij == 0)
                {
                    ModelState.AddModelError("UnSelectAdmin", "Не выбраны администраторы.");
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction(st);
                }
                return View(ADTO);
            }
            return RedirectToAction(st);
        }

        #region "edit"
        // GET: /../Edit
        public ActionResult Edit(int id)
        {
            MvcUniJudje.WebReference.AdminDTO admin = new WebReference.AdminDTO();

            using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
            {
                admin = client.GetAdmin(Session["session_key"].ToString(), id, true);
            }
            return View(admin);
        }

        [HttpPost]
        public ActionResult Edit(MvcUniJudje.WebReference.AdminDTO admin)
        {
            if (admin.Login == null)
                ModelState.AddModelError("Create", "Введите логин.");
            if (admin.email == null)
                ModelState.AddModelError("Create", "Введите Email.");
            if (ModelState.IsValid)
            {
                using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
                {
                    try
                    {
                        client.WebUpdateAdmin(Session["session_key"].ToString(), admin, admin.ID, true, admin.NotifyNewRegistrations, true, admin.AllowAccessToAdminPanel, true);
                        }
                    catch (Exception ex)
                    {
                                ModelState.AddModelError("Create", ex.Message);
                    }
                    return RedirectToAction("Index");
                }
            }

            return View(admin);
        }
        #endregion

        #region "create"
        // GET: /../Create

        public ActionResult Create()
        {
            return View();
        }


        //
        // POST: /../Create

        [HttpPost]
        public ActionResult Create(MvcUniJudje.WebReference.AdminDTO admin)
        {
            if (admin.Login == null)
                ModelState.AddModelError("LoginAdmin", "Введите логин.");
            if (admin.email == null)
                ModelState.AddModelError("EmailAdmin", "Введите Email.");
            if (ModelState.IsValid)
            {
                using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
                {
                    try
                    {
                        admin.ID = client.CreateAdmin(Session["session_key"].ToString()).ID;
                        client.WebUpdateAdmin(Session["session_key"].ToString(), admin, admin.ID, true, admin.NotifyNewRegistrations, true, admin.AllowAccessToAdminPanel, true);
                    }
                    catch (Exception ex)
                    {
                        switch (ex.Message)
                        {
                            case "Login already exist":
                                ModelState.AddModelError("submit", "Логин уже существует");
                                break;
                            default:
                                ModelState.AddModelError("Create", ex.Message);
                                throw new Exception(ex.Message, ex);
                        }
                    }
                    return RedirectToAction("Index");
                }
            }

            return View(admin);
        }

        #endregion

        #region "Delete"
        public ActionResult Delete()
        {
            using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
            {
                MvcUniJudje.WebReference.AdminDTO[] ADTO = client.GetAdmins(Session["session_key"].ToString());
                List<int> checkBox = new List<int>();
                List<WebReference.AdminDTO> admit = new List<WebReference.AdminDTO>();
                for (int i = 0; i < ADTO.Count(); i++)
                {
                    var s = FC[(ADTO[i].ID.ToString())].Remove(2);
                    if (s.Equals("tr"))
                    {
                        checkBox.Add(ADTO[i].ID);
                        admit.Add((from c in ADTO
                                  where c.ID == ADTO[i].ID
                                  select new WebReference.AdminDTO { ID = c.ID, Login = c.Login, email = c.email}).FirstOrDefault());
                    }
                }
                return View(admit);
            }
        }

        [HttpPost]
        public ActionResult Delete(List<WebReference.AdminDTO> admin, FormCollection fc)
        {
            var client = new MvcUniJudje.WebReference.ServiceForAdmin();
            int[] id = new int[admin.Count()];
            foreach (var c in admin)
                client.DeleteAdmin(Session["session_key"].ToString(), c.ID, true);
            //  id[i++] = c.ID;
            //  client.DeleteGroupUser(Session["session_key"].ToString(), id);

            return RedirectToAction("Index", "Administrator");
        }

        #endregion "Delete"

        #region "Acess"

        public ActionResult Acess()
        {
            using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
            {
                MvcUniJudje.WebReference.AdminDTO[] ADTO = client.GetAdmins(Session["session_key"].ToString());
                List<int> checkBox = new List<int>();
                List<WebReference.AdminDTO> admit = new List<WebReference.AdminDTO>();
                for (int i = 0; i < ADTO.Count(); i++)
                {
                    var s = FC[(ADTO[i].ID.ToString())].Remove(2);
                    if (s.Equals("tr"))
                    {
                        checkBox.Add(ADTO[i].ID);
                        admit.Add((from c in ADTO
                                   where c.ID == ADTO[i].ID
                                   select new WebReference.AdminDTO { ID = c.ID, Login = c.Login, email=c.email }).FirstOrDefault());
                    }
                }
                return View(admit);
            }
        }

        public ActionResult AcessOne(int id)
        {
            using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
            {
                MvcUniJudje.WebReference.AdminDTO[] ADTO = client.GetAdmins(Session["session_key"].ToString());
                List<WebReference.AdminDTO> admit = new List<WebReference.AdminDTO>();
                      admit.Add( ((from c in ADTO
                                   where c.ID == id
                                   select new WebReference.AdminDTO { ID = c.ID, Login = c.Login, email = c.email }).FirstOrDefault()));
               
                return View("Acess",admit);
            }
        }

        [HttpPost]
        public ActionResult Acess(List<WebReference.AdminDTO> admin, FormCollection fc)
        {
            using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
            {
                foreach (var c in admin)
                {
                    WebReference.AdminDTO ADTO = client.GetAdmin(Session["session_key"].ToString(), c.ID, true);
                    var buttons = client.GetCollectionButtons(Session["session_key"].ToString());
                    Boolean [] acces = new bool[buttons.Count()];
                    int count = 0;
                    foreach (var b in buttons)
                    {
                        var s = fc[b.NameButton.Trim()].Remove(2);

                        acces[count++] = (s.Equals("tr") ? true : false);
                    }
                    client.AccessAdmins(Session["session_key"].ToString(), c.ID, true, acces);
                }
                //foreach (var c in admin)
                //    client.DeleteAdmin(Session["session_key"].ToString(), c.ID, true);
                    return RedirectToAction("Index", "Administrator");
            }
        }

        [HttpPost]
        public ActionResult AcessOne(List<WebReference.AdminDTO> admin, FormCollection fc)
        {
            using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
            {
                foreach (var c in admin)
                {
                    WebReference.AdminDTO ADTO = client.GetAdmin(Session["session_key"].ToString(), c.ID, true);
                    var buttons = client.GetCollectionButtons(Session["session_key"].ToString());
                    Boolean[] acces = new bool[buttons.Count()];
                    int count = 0;
                    foreach (var b in buttons)
                    {
                        var s = fc[b.NameButton.Trim()].Remove(2);

                        acces[count++] = (s.Equals("tr") ? true : false);
                    }
                    client.AccessAdmins(Session["session_key"].ToString(), c.ID, true, acces);
                }
                //foreach (var c in admin)
                //    client.DeleteAdmin(Session["session_key"].ToString(), c.ID, true);
                return RedirectToAction("Index", "Administrator");
            }
        }

        #endregion "Acess"

        #region "Password"
        // GET: /../Edit
        public ActionResult Password(int id)
        {
            return View(id);
        }

        [HttpPost]
        public ActionResult Password(int id,FormCollection fc)
        {
            using (var client = new WebReference.ServiceForAdmin())
            {
                MvcUniJudje.WebReference.AdminDTO ADTO = client.GetAdmin(Session["session_key"].ToString(), id , true);
                var s = fc["TexBoxPass"].Trim();
                ViewBag.Password = client.GetNewPasswordForAdminAndSendByEmail(Session["session_key"].ToString(), ADTO.ID, true, s);

            }
            return View();
        }

        #endregion "Password"
    }
}
