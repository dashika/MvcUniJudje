using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcUniJudje.Controllers.WorkStation
{
    public class GroupOfUsersController : Controller
    {
        MvcUniJudje.Models.Login log = new Models.Login();
        static FormCollection formCollection;
        FormCollection FC { get { return formCollection; } set { formCollection = value; } }
      
        //
        // GET: /GroupOfUsers/
         [OutputCache(Location = System.Web.UI.OutputCacheLocation.Any, Duration = 60)]
        public ActionResult Index()
        {
            using (var client = new WebReference.ServiceForAdmin())
            {
                return View(client.GetAllGroups(Session["session_key"].ToString()));
            }
        }

         [HttpPost]
         public ActionResult Index(FormCollection us)
         {
             var st = us.GetKey(0);
             FC = us;
             if (st.CompareTo("Create") != 0)
             {
                 var client = new MvcUniJudje.WebReference.ServiceForAdmin();
                 MvcUniJudje.WebReference.GroupDTO[] GDTO = client.GetAllGroups(Session["session_key"].ToString());
                 int ij = 0;
                 for (int i = 0; i < GDTO.Count(); i++)
                 {
                     var s = FC[(GDTO[i].Id.ToString())].Remove(2);
                     if (s.Equals("tr"))
                     {
                         ij++;
                     }
                 }
                 if (ij == 0)
                 {
                     ModelState.AddModelError("UnSelectGroup", "Не выбраны группы.");
                 }

                 if (ModelState.IsValid)
                 {
                     return RedirectToAction(st);
                 }
                 return View(GDTO);
             }
             return RedirectToAction(st);
         }


         #region Edit
         public ActionResult Edit(int id)
        {
            using (var client = new WebReference.ServiceForAdmin())
            {
                return View(client.GetGroupInfo(Session["session_key"].ToString(),id,true));
            }
         
        }

        [HttpPost]
         public ActionResult Edit(MvcUniJudje.WebReference.GroupDTO group, FormCollection fc)
        {
            if (group.Title == null)
                ModelState.AddModelError("Create", "Введите назание группы.");
            if (ModelState.IsValid)
            {
                using (var client = new WebReference.ServiceForAdmin())
                {
                    MvcUniJudje.WebReference.UserDTO[] GDTO = client.GetAllUserInGroup(Session["session_key"].ToString(), group.Id, true);
                    MvcUniJudje.WebReference.UserDTO[] GDTOout = client.GetAllUserOutGroup(Session["session_key"].ToString(), group.Id, true);      
                    for (int i = 0; i < GDTO.Count(); i++)
                    {
                        var s = fc[(GDTO[i].Id.ToString())].Remove(2);
                        if (s.Equals("tr"))
                        {
                            client.DeleteOutFromGroup(Session["session_key"].ToString(), group.Id, true, GDTO[i].Id, true);
                        }
                    }
                    for (int i = 0; i < GDTOout.Count(); i++)
                    {
                        var s = fc[(GDTOout[i].Id.ToString())].Remove(2);
                        if (s.Equals("tr"))
                        {
                            client.AddInGroup(Session["session_key"].ToString(), group.Id, true, GDTOout[i].Id, true);
                        }
                    }

                    client.WebUpdateGroup(Session["session_key"].ToString(), group.Title, group.Id, true);
                }
                return RedirectToAction("Index");
            }
            return View(group);
        }
#endregion Edit

        #region Delete

        public ActionResult Delete()
        {
            using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
            {
                MvcUniJudje.WebReference.GroupDTO[] GDTO = client.GetAllGroups(Session["session_key"].ToString());
                List<int> checkBox = new List<int>();
                List<WebReference.GroupDTO> group = new List<WebReference.GroupDTO>();
                for (int i = 0; i < GDTO.Count(); i++)
                {
                    var s = FC[(GDTO[i].Id.ToString())].Remove(2);
                    if (s.Equals("tr"))
                    {
                        checkBox.Add(GDTO[i].Id);
                        group.Add((from c in GDTO
                                   where c.Id == GDTO[i].Id
                                   select new WebReference.GroupDTO { Id = c.Id, Title = c.Title }).FirstOrDefault());
                    }
                }
                return View(group);
            }
        }

        [HttpPost]
        public ActionResult Delete(List<WebReference.GroupDTO> group, FormCollection fc)
        {
            var client = new MvcUniJudje.WebReference.ServiceForAdmin();
            foreach (var c in group)
                client.DeleteGroup(Session["session_key"].ToString(), c.Id, true);
            return RedirectToAction("Index", "GroupOfUsers");
        }

#endregion Delete

        #region Create

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(MvcUniJudje.WebReference.GroupDTO group)
        {
            if (group.Title == null)
                ModelState.AddModelError("LoginAdmin", "Введите название группы.");
            if (ModelState.IsValid)
            {
                using (var client = new MvcUniJudje.WebReference.ServiceForAdmin())
                {
                    try
                    {
                        group.Id = client.CreateGroup(Session["session_key"].ToString()).Id;
                        client.WebUpdateGroup(Session["session_key"].ToString(), group.Title, group.Id, true);
                    }
                    catch (Exception ex)
                    {
                                ModelState.AddModelError("Create", ex.Message);
                    }
                    return RedirectToAction("Index");
                }
            }
            return View(group);
        }

        #endregion Create


        #region UserInOutGroup

        public ActionResult EditUsersInGroups()
        {
            return View();
        }

        #endregion UserInOutGroup
    }
}
