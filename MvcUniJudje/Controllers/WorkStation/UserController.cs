using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcUniJudje.WebReference;
using MvcUniJudje.Models;


namespace MvcUniJudje.Controllers.WorkStation
{
    public class UserController : Controller
    {
        /// <summary>
        /// проверка по ключу пока в заморозке 
        /// </summary>
        MvcUniJudje.Models.Login log = new MvcUniJudje.Models.Login();
        static  FormCollection formCollection;
        FormCollection FC { get { return formCollection; } set { formCollection = value; } }
        //
        // GET: /User/

            //   [OutputCache(Location = System.Web.UI.OutputCacheLocation.Any, Duration = 60)]
        public ActionResult Index()
        {
            ValidationIn val = new ValidationIn();
            if (val.Validation("Пользователи", Session["session_key"].ToString(), Session["name"].ToString()))
            {
                var client = new MvcUniJudje.WebReference.ServiceForAdmin();
                MvcUniJudje.WebReference.UserDTO[] UDTO = client.GetUsers(Session["session_key"].ToString());

                return View(Enumerable.Range(0, UDTO.Length).Select(i => new User { ID = UDTO[i].Id, Name = UDTO[i].Name, Login = UDTO[i].Login, Locking = (UDTO[i].BlockedTo != null ? true : false), Date = UDTO[i].BlockedTo, DateRegistration = UDTO[i].DateRegistration }));
            }
            return RedirectToAction("Index", "Home");
        }

        [ChildActionOnly]
        public ActionResult GroupUserInfo()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Index(FormCollection us)
        {
            var st = us.GetKey(1);
            FC = us;
            if (st.CompareTo("Create") != 0)
            {
                var client = new MvcUniJudje.WebReference.ServiceForAdmin();
                MvcUniJudje.WebReference.UserDTO[] UDTO = client.GetUsers(Session["session_key"].ToString());
                int ij = 0;
                for (int i = 0, j =0; i < UDTO.Count() & j<100; i++, j++)
                {
                    var s = FC[(UDTO[i].Id.ToString())].Remove(2);
                    if (s.Equals("tr"))
                    {
                        ij++;
                    }
                }
                if (ij == 0)
                {
                    ModelState.AddModelError("Create", "Не выбраны пользователи.");
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction(st);
                }
                return View(Enumerable.Range(0, UDTO.Length).Select(i => new User { ID = UDTO[i].Id, Name = UDTO[i].Name, Login = UDTO[i].Login, Locking = (UDTO[i].BlockedTo != null ? true : false), Date = UDTO[i].BlockedTo, DateRegistration = UDTO[i].DateRegistration }));
            }
            return RedirectToAction(st);
        }

        #region "create"
        // GET: /../Create

        public ActionResult Create()
        {
            return View();
        }

  
        //
        // POST: /../Create

        [HttpPost]
        public ActionResult Create(MvcUniJudje.WebReference.UserExDTO user)
        {

            if (user.Name == null)
                ModelState.AddModelError("Create", "Введите ФИО.");
            if (user.Login == null)
                ModelState.AddModelError("Create", "Введите логин.");
            if (user.Email == null)
                ModelState.AddModelError("Create", "Введите Email.");
            if (ModelState.IsValid)
            {
                using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
                {
                    try
                    {
                        client.CreateOneUser(Session["session_key"].ToString(), user);
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

            return View(user);
        }

        #endregion

        #region "edit"
        // GET: /../Edit
        public ActionResult Edit(int id)
        {
            MvcUniJudje.WebReference.UserExDTO user = new UserExDTO();

            using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
            {
               user = client.GetUser(Session["session_key"].ToString(), id, true);
            }
                return View(user);
        }

        [HttpPost]
        public ActionResult Edit(MvcUniJudje.WebReference.UserExDTO user)
        {
            if (user.Name == null)
                ModelState.AddModelError("Create", "Введите ФИО.");
            if (user.Login == null)
                ModelState.AddModelError("Create", "Введите логин.");
            if (user.Email == null)
                ModelState.AddModelError("Create", "Введите Email.");
            if (ModelState.IsValid)
            {
                using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
                {
                    try
                    {
                        client.WebUpdateUser(Session["session_key"].ToString(), user, user.Id, true, user.LoginFailCount,true, user.SendNotifications, true, user.EmailConfirmed, true);
                    }
                    catch (Exception ex)
                    {
                        switch (ex.Message)
                        {
                            case "Login already exist":
                                ModelState.AddModelError("Create", "Логин уже существует");
                                break;
                            default:
                                ModelState.AddModelError("Create", ex.Message);
                                throw new Exception(ex.Message, ex);
                        }
                    }
                    return RedirectToAction("Index");
                }
            }

            return View(user);
        }
         
        public ActionResult SendConfirm(int id)
        {
            using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
            {
                client.ValidateUserEmail(Session["session_key"].ToString(), id, true);
                ModelState.AddModelError("Create", "Запрос отправлен");
            }
            MvcUniJudje.WebReference.UserExDTO user = new UserExDTO();

            using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
            {
                user = client.GetUser(Session["session_key"].ToString(), id, true);
            }
            return View("Edit",user);
        }

        #endregion

        #region "Password"
        public ActionResult Password(int Id, string name)
        {
            return View(new Pass { id = Id, Key = "", Name = name });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Password(Pass pass, FormCollection us)
        {
            if (us.GetKey(1) == "Show")
            {
                if (pass.Key == "" || pass.Key == null)
                    ModelState.AddModelError("Password", "Введите ключ");
                if (ModelState.IsValid)
                {
                    var client = new MvcUniJudje.WebReference.ServiceForAdmin();
                    MvcUniJudje.Models.Login log = new MvcUniJudje.Models.Login();
                    if (client.GetUserPassword(Session["session_key"].ToString(), pass.id, true, pass.Key).IsNormalized())
                        ModelState.AddModelError("Password", "Некорректный ключ");
                    else
                        ViewBag.Password = client.GetUserPassword(Session["session_key"].ToString(), pass.id, true, pass.Key);
                }
            }
            else
                if (us.GetKey(0) == "NewPass")
            {
             var client = new MvcUniJudje.WebReference.ServiceForAdmin();
                    MvcUniJudje.Models.Login log = new MvcUniJudje.Models.Login();
                    ViewBag.NewPassword = client.GenerateNewUserPassword(Session["session_key"].ToString(), pass.id, true);
            }
                else
                    if (us.GetKey(1) == "NewPassSave")
                    {
                        var client = new MvcUniJudje.WebReference.ServiceForAdmin();
                        MvcUniJudje.Models.Login log = new MvcUniJudje.Models.Login();
                        var s = us["passwo"].Trim();
                        if (s.Length >= 6)
                        {
                            client.SendPasswordOnEmail(Session["session_key"].ToString(), s, pass.id, true);
                            return RedirectToAction("Index");
                        }
                        ViewBag.NewPassword = s;
                        ModelState.AddModelError("Password", "Пароль должен быть не менее 6 символов");
                    }
            return View(pass);
        }



        #endregion "password"

        #region "(Un)BlockTo"
        public ActionResult BlockTo()
        {
           var client = new MvcUniJudje.WebReference.ServiceForAdmin();
           MvcUniJudje.WebReference.UserDTO[] UDTO = client.GetUsers(Session["session_key"].ToString());
           List<int> checkBox = new List<int>();
           List<User> user = new List<User>();
           for (int i=0, j =0; i < UDTO.Count() & j < 100; i++,j++)
           {
           var s= FC[(UDTO[i].Id.ToString())].Remove(2);
           if (s.Equals("tr"))
           {
               checkBox.Add(UDTO[i].Id);
               user.Add((from c in UDTO
                         where c.Id == UDTO[i].Id
                         select new User { ID = c.Id, Login = c.Login, DateRegistration = c.DateRegistration, Name = c.Name }).FirstOrDefault());
           }
        }

           return View(user);
        }

        public ActionResult UnBlockTo()
        {
            var client = new MvcUniJudje.WebReference.ServiceForAdmin();
            MvcUniJudje.WebReference.UserDTO[] UDTO = client.GetUsers(Session["session_key"].ToString());
            List<int> checkBox = new List<int>();
            List<User> user = new List<User>();
            for (int i = 0, j = 0; i < UDTO.Count() & j < 100; i++, j++)
            {
                var s = FC[(UDTO[i].Id.ToString())].Remove(2);
                if (s.Equals("tr"))
                {
                    checkBox.Add(UDTO[i].Id);
                    user.Add((from c in UDTO
                              where c.Id == UDTO[i].Id
                              select new User { ID = c.Id, Login = c.Login, DateRegistration = c.DateRegistration, Name = c.Name, Date = c.BlockedTo }).FirstOrDefault());
                }
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult UnBlockTo(List<User> user, FormCollection fc)
        {
            var client = new MvcUniJudje.WebReference.ServiceForAdmin();
            int[] id = new int[user.Count()];
            int i = 0;
            foreach (var c in user)
                id[i++] = c.ID;
            client.UnBlockGroupUser(Session["session_key"].ToString(), id);

            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        public ActionResult BlockTo(List<User> user, FormCollection fc)
        {
            var client = new MvcUniJudje.WebReference.ServiceForAdmin();
            DateTime date = new DateTime();
            try
            {
                date = Convert.ToDateTime(fc["DateRegist"]); //string.Format("dd MMM yyyy", s);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("date", "Выберите дату, до которой блокировать пользователя !");
                return View(user);
            }
            if (DateTime.Now > date)
            {
                ModelState.AddModelError("date", "Дата должна быть позднее текущей !");
                return View(user);
            }
            int [] id = new int[user.Count()];
            int i = 0;
            foreach (var c in user)
                id[i++] = c.ID;

           client.BlockGroupUser(Session["session_key"].ToString(), id, date, true);

                return RedirectToAction("Index", "User");
        }

        #endregion

        #region "Delete"
        public ActionResult Delete()
        {
            var client = new MvcUniJudje.WebReference.ServiceForAdmin();
            MvcUniJudje.WebReference.UserDTO[] UDTO = client.GetUsers(Session["session_key"].ToString());
            List<int> checkBox = new List<int>();
            List<User> user = new List<User>();
            for (int i = 0, j = 0; i < UDTO.Count() & j < 100; i++, j++)
            {
                var s = FC[(UDTO[i].Id.ToString())].Remove(2);
                if (s.Equals("tr"))
                {
                    checkBox.Add(UDTO[i].Id);
                    user.Add((from c in UDTO
                              where c.Id == UDTO[i].Id
                              select new User { ID = c.Id, Login = c.Login, DateRegistration = c.DateRegistration, Name = c.Name }).FirstOrDefault());
                }
            }
                return View(user);
        }

        [HttpPost]
        public ActionResult Delete(List<User> user, FormCollection fc)
        {
            var client = new MvcUniJudje.WebReference.ServiceForAdmin();
            int[] id = new int[user.Count()];
            int i = 0;
            foreach (var c in user)
                client.DeleteUser(Session["session_key"].ToString(), c.ID, true);
              //  id[i++] = c.ID;
          //  client.DeleteGroupUser(Session["session_key"].ToString(), id);

            return RedirectToAction("Index", "User");
        }

        #endregion "Delete"


        #region "Group"

        public ActionResult Group()
        {
            using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
            {
                MvcUniJudje.WebReference.UserDTO[] UDTO = client.GetUsers(Session["session_key"].ToString());
                List<int> checkBox = new List<int>();
                List<WebReference.UserDTO> user = new List<WebReference.UserDTO>();
                for (int i = 0, j = 0; i < UDTO.Count() & j < 100; i++, j++)
                {
                    var s = FC[(UDTO[i].Id.ToString())].Remove(2);
                    if (s.Equals("tr"))
                    {
                        checkBox.Add(UDTO[i].Id);
                        user.Add((from c in UDTO
                                  where c.Id == UDTO[i].Id
                                  select new WebReference.UserDTO { Id = c.Id, Login = c.Login, DateRegistration = c.DateRegistration, Name = c.Name }).FirstOrDefault());
                    }
                    
                }
                return View(user);
            }
        }

        #endregion "Group"

        
    }
}
