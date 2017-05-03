using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;

using DataModel;

using WCFServices.Exceptions;

using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

using WCFServices.ServiceForAdmin.DTO;
using System.Data;
using System.Data.Objects;



namespace WCFServices.ServiceForAdmin
{
    partial class ServiceForAdmin : IServiceForAdmin
    {


        /// <summary>
        /// Получить всех админов
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <returns>Список всех админов</returns>
        public IEnumerable<AdminDTO> GetAdmins(String SessionKey)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                return (from c in db.Admins
                        select new AdminDTO { ID = c.Id, Login = c.Login, AllowAccessToAdminPanel = c.AllowAccessToAdminPanel, email = c.Email, NotifyNewRegistrations = c.NotifyNewRegistrations }).ToList();
            }
        }


        /// <summary>
        /// Получить опреденный список в зависимости от ID
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="str">Логин</param>
        /// <returns></returns>
        public AdminDTO GetCollection(String SessionKey, string str)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                return (from c in db.Admins
                        where str.StartsWith(c.Login)
                        select new AdminDTO
                        {
                            ID = c.Id,
                            Login = c.Login,
                            AllowAccessToAdminPanel = c.AllowAccessToAdminPanel,
                            email = c.Email,
                            NotifyNewRegistrations = c.NotifyNewRegistrations
                        }).SingleOrDefault();
            }
        }
        public AdminDTO GetAdmin(String SessionKey, int id)
        {
            using (var db = new DataBase())
            {
                return (from c in db.Admins
                        where c.Id == id
                        select new AdminDTO
                        {
                            ID = c.Id,
                            Login = c.Login,
                            AllowAccessToAdminPanel = c.AllowAccessToAdminPanel,
                            email = c.Email,
                            NotifyNewRegistrations = c.NotifyNewRegistrations
                        }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Получить коллекцию допусков
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="ID">идентификатор администратора</param>
        /// <returns></returns>
        public IEnumerable<AccessToPanel> GetCollectionAccess(String SessionKey, int ID)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                return (from c in db.AccesToes
                        where c.ID_Admin == ID
                        select new AccessToPanel
                        {
                            ID = c.ID_Access,
                            ID_Admin = c.ID_Admin,
                            ID_Button = c.ID_Button,
                            YesNoAccess = c.YesNoAccess
                        }).ToList();
            }
        }


        /// <summary>
        /// Получить коллекцию кнопок
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <returns></returns>
        public IEnumerable<AccessButtons> GetCollectionButtons(String SessionKey)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                return (from c in db.DictionaryButtons
                        select new AccessButtons
                        {
                            ID = c.ID_Button,
                            NameButton = c.Name
                        }).ToList();
            }
        }

        /// <summary>
        /// Генерация нового пароля
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="AdminId"></param>
        /// <returns></returns>
        public String GenerateNewAdminPassword(String SessionKey, int AdminId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var admin = db.Admins.Where(u => u.Id == AdminId).Single();
                var newPass = Guid.NewGuid().ToString().Substring(0, 13); // новый парль - это Guid в котором были взять первые 13 символов

                admin.PasswordSalt = Guid.NewGuid().ToString();
                admin.Password_SaltAndHash = HashMd5.getMd5Hash(newPass + admin.PasswordSalt);

                db.SaveChanges();

                return newPass;
            }
        }

        public String CreateNewPasswordUseble(String SessionKey, int AdminId, string pass)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var admin = db.Admins.Where(u => u.Id == AdminId).Single();
                var newPass =  pass.Length >= 6 ? pass : Guid.NewGuid().ToString().Substring(0, 13); // новый парль - это Guid в котором были взять первые 13 символов

                admin.PasswordSalt = Guid.NewGuid().ToString();
                admin.Password_SaltAndHash = HashMd5.getMd5Hash(newPass + admin.PasswordSalt);

                db.SaveChanges();

                return newPass;
            }
        }

        /// <summary>
        /// Добавление/удаление доступа к панелям
        /// </summary>
        /// <param name="SessionKey">номер сессии</param>
        /// <param name="AdminId">идентификатор администратора</param>
        /// <param name="access">переданные поля доступа</param>
        public void AccessAdmins(String SessionKey, Int32 AdminId, Boolean[] access)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                ///проверочку на пустоту не помешало бы

                IEnumerable<AccessToPanel> DButton = (from c in db.AccesToes
                                                      where c.ID_Admin == AdminId
                                                      select new AccessToPanel { ID_Button = c.ID_Button }).ToList();

                Int32 i = 0;

                foreach (var idb in DButton)
                {
                    AccesTo acces = (from c in db.AccesToes
                                     where c.ID_Admin == AdminId && c.ID_Button == idb.ID_Button
                                     select c).SingleOrDefault<AccesTo>();
                    acces.YesNoAccess = access[i++];

                    // Сохранить изменения
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        db.Refresh(refreshMode: RefreshMode.ClientWins,
                             collection: db.AccesToes);
                        db.SaveChanges();
                        throw new Exception("Не удалось сохранить изменения. Ошибка :" + ex.Message);
                    }
                }
            }
        }


        /// <summary>
        /// Создание нового администратора
        /// </summary>
        /// <param name="SessionKey">ключ сессии</param>
        /// <returns></returns>
        public AdminDTO CreateAdmin(String SessionKey)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var admin = new Admin();

                string NewLogin;

                do
                    NewLogin = "Login" + (new Random().Next()).ToString();
                while (db.Admins.Where(u => u.Login == NewLogin).Count() > 0);

                admin.Login = NewLogin;
                admin.PasswordSalt = Guid.NewGuid().ToString();
                admin.Password_SaltAndHash = WCFServices.HashMd5.getMd5Hash(NewLogin + admin.PasswordSalt);
                admin.Email = "Не указан";
                admin.AllowAccessToAdminPanel = false;
                admin.NotifyNewRegistrations = false;

                db.Admins.AddObject(admin);
                db.SaveChanges();

                IEnumerable<AccessButtons> DButton = (from c in db.DictionaryButtons
                                                      select new AccessButtons { ID = c.ID_Button }).ToList();

                foreach (var idb in DButton)
                {
                    AccesTo acces = new AccesTo();
                    acces.ID_Admin = admin.Id;
                    acces.ID_Button = idb.ID;
                    acces.YesNoAccess = true;
                    db.AccesToes.AddObject(acces);
                    db.SaveChanges();
                }

                return GetCollection(SessionKey, admin.Login);
            }
        }


        /// <summary>
        /// Сохранение изменений
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="admin">структура данных администратора для изменения</param>
        public void UpdateAdmin(String SessionKey, AdminDTO admin)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                Admin UAdmin = (from c in db.Admins
                                where c.Id == admin.ID
                                select c).SingleOrDefault<Admin>();

                UAdmin.Login = admin.Login;
                UAdmin.Email = admin.email;
                UAdmin.AllowAccessToAdminPanel = admin.AllowAccessToAdminPanel;
                UAdmin.NotifyNewRegistrations = admin.NotifyNewRegistrations;

                // Сохранить изменения
                try
                {
                    db.SaveChanges();
                }
                catch (OptimisticConcurrencyException ex)
                {
                    db.Refresh(refreshMode: RefreshMode.ClientWins,
                         collection: db.Admins);
                    db.SaveChanges();
                    throw new Exception("Не удалось сохранить изменения. Ошибка :" + ex.Message);
                }
            }
        }

       public void WebUpdateAdmin(String SessionKey, AdminDTO Admin, int id, bool Notify, bool acess)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                Admin UAdmin = (from c in db.Admins
                                where c.Id == id
                                select c).SingleOrDefault<Admin>();

                UAdmin.Login = Admin.Login;
                UAdmin.Email = Admin.email;
                UAdmin.AllowAccessToAdminPanel = acess;
                UAdmin.NotifyNewRegistrations = Notify;

                // Сохранить изменения
                try
                {
                    db.SaveChanges();
                }
                catch (OptimisticConcurrencyException ex)
                {
                    db.Refresh(refreshMode: RefreshMode.ClientWins,
                         collection: db.Admins);
                    db.SaveChanges();
                    throw new Exception("Не удалось сохранить изменения. Ошибка :" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Удаление администратора
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="AdminId">идентификатор администратора</param>
        public void DeleteAdmin(String SessionKey, int AdminId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                IEnumerable<AccessToPanel> DButton = (from c in db.AccesToes
                                                      where c.ID_Admin == AdminId
                                                      select new AccessToPanel { ID_Button = c.ID_Button }).ToList();

                foreach (var idb in DButton)
                {
                    AccesTo acces = (from c in db.AccesToes
                                     where c.ID_Admin == AdminId && c.ID_Button == idb.ID_Button
                                     select c).SingleOrDefault<AccesTo>();
                    db.AccesToes.DeleteObject(acces);
                    db.SaveChanges();
                }

                Admin admin = (from c in db.Admins
                               where c.Id == AdminId
                               select c).SingleOrDefault<Admin>();
                db.Admins.DeleteObject(admin);
                db.SaveChanges();

                if (db.Admins.Where(u => u.Id == AdminId).SingleOrDefault() != null)
                    throw new Exception("Не удалось удалить пользователя");
            }
        }

        // отправить новый пароль на электронною почту 
        public String GetNewPasswordForAdminAndSendByEmail(String SessionKey, int id, string password)
        {
            CheckSession(SessionKey);
            String pass = this.CreateNewPasswordUseble(SessionKey, id, password);
            ServiceHelper.MailManager.PassByEmail(id, pass);
            return pass;
        }


    }
}
