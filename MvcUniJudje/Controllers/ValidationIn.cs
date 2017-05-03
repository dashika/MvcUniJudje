using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcUniJudje.Controllers
{
    public class ValidationIn
    {
        public bool Validation(string name, string sessionKey, string adminName)
        {
            int id_b = 0;
            switch(name)
            {
                case "Пользователи": id_b = 1; break;
                case "Компиляторы" : id_b = 2; break;
                case "Сабмиты" : id_b = 3; break;
                case "Задачи" : id_b = 4; break;
                case "Контесты": id_b = 5; break;
                case "Администраторы": id_b = 6; break;
                case "Настройки": id_b = 7; break;
                case "Группы пользователей": id_b = 10; break;
            }
            using(var client = new WebReference.ServiceForAdmin())
            {
                WebReference.AdminDTO id_adm = client.GetCollection(sessionKey, adminName);

                var acces = client.GetCollectionAccess(sessionKey, id_adm.ID, true);
                foreach (var c in acces)
                {
                    if (c.ID_Button == id_b) return c.YesNoAccess;
                }
            }

            return false;
        }
    }
}