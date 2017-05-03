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


namespace WCFServices.ServiceForAdmin
{
    partial class ServiceForAdmin : IServiceForAdmin
    {
        
        // Оптимизировать подгрузку контестов!!
        public IEnumerable<UserDTO> GetUsers(String SessionKey)
        {
            CheckSession(SessionKey);
   
            using (var db = new DataBase())
            {
                var UserContestDictionary = new Dictionary<int, List<int>>();
                foreach (var user in db.Users)
                {
                    var colection = new List<int>();
                    foreach (var group in user.MM_UserGroup.Select(s => s.Group))
                        colection.AddRange(group.MM_ContestGroup.Select(s => s.ContestId));

                    UserContestDictionary.Add(user.Id, colection.Distinct().ToList());
                }

                // Выберем информацию о пользователях
                var ee = (from user in db.Users
                         select new UserDTO {
                             Id = user.Id, 
                             Name = user.Name,
                             BlockedTo = user.BlockedTo, 
                             Note = user.Note,
                             Login = user.Login,
                             DateRegistration = user.RegistrationDate
                         }
                         ).ToList();
                
                // Дополним ее информацией о принадлежности к контестам
                foreach (var user in ee)
                {
                    List<int> ret;
                    if (UserContestDictionary.TryGetValue(user.Id, out ret))
                        user.ContestCollection = ret;
                    else
                        user.ContestCollection = new List<int>();
                }

                return ee;
            }
        }

        // Оптимизировать подгрузку контестов!!
        public UserExDTO GetUser(String SessionKey, int UserId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {

                var user = db.Users.Where(u => u.Id == UserId).Single();

                var group = from user_gr in user.MM_UserGroup
                            join gr in db.Groups on user_gr.GroupId equals gr.Id
                            select new GroupDTO { Id = gr.Id, Title = gr.Title };

                // Подготовим коллекцию контестов
                var colection = new List<int>();
                foreach (var gr in user.MM_UserGroup.Select(s => s.Group))
                    colection.AddRange(gr.MM_ContestGroup.Select(s => s.ContestId));
                colection = colection.Distinct().ToList();
                

                return new UserExDTO
                {
                    Id = user.Id,
                    Login = user.Login,
                    Name = user.Name,
                    Institution = user.Institution,
                    Phone = user.Phone,
                    Email = user.Email,
                    SendNotifications = user.SendNotifications,
                    EmailConfirmed = user.EmailConfirmed,
                    RegistrationDate = user.RegistrationDate,
                    LastActivityDateTime = user.LastActivityDateTime,
                    LoginFailCount = user.LoginFailCount,
                    BlockedTo = user.BlockedTo,
                    Note = user.Note,
                    SocialNetworks = user.SocialNetworks,
                    Address = user.Address,
                    ClassCourseGroup = user.ClassCourseGroup,

                    Groups = group.ToList(),

                    ContestCollection = colection

                };
            }
        }

        public String GetUserPassword(String SessionKey, int UserId, String key)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                return WCFServices.ServiceHelper.Crypt.Decrypt(db.Users.Where(u => u.Id == UserId).Single().PasswordEncrypted,key);
            }
        }

        // Сгенерировать новый пароль для пользователя и вернуть его
        public String GenerateNewUserPassword(String SessionKey, int UserId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var user = db.Users.Where(u => u.Id == UserId).Single();
                var newPass = Guid.NewGuid().ToString().Substring(0, 13); // новый парль - это Guid в котором были взять первые 13 символов
                
                user.PasswordEncrypted  = WCFServices.ServiceHelper.Crypt.Encrypt(newPass,Properties.Settings.Default.KeyForPasswordEncrypt);
                user.PasswordSalt = Guid.NewGuid().ToString();
                user.Password_SaltAndHash = HashMd5.getMd5Hash(newPass + user.PasswordSalt);

                db.SaveChanges();

                return newPass;
            }
        }

        public String CreateNewUserPassword(String SessionKey, int UserId, String pass)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var user = db.Users.Where(u => u.Id == UserId).Single();
                var newPass = pass.Length >= 6 ? pass : Guid.NewGuid().ToString().Substring(0, 13); // новый парль - это Guid в котором были взять первые 13 символов

                user.PasswordEncrypted = WCFServices.ServiceHelper.Crypt.Encrypt(newPass, Properties.Settings.Default.KeyForPasswordEncrypt);
                user.PasswordSalt = Guid.NewGuid().ToString();
                user.Password_SaltAndHash = HashMd5.getMd5Hash(newPass + user.PasswordSalt);

                db.SaveChanges();

                return newPass;
            }
        }

        public void SendPasswordOnEmail(String SessionKey, String pass, int id)
        {
            CheckSession(SessionKey);
            pass = CreateNewUserPassword(SessionKey, id, pass);
            ServiceHelper.MailManager.UsersPassByEmail(id, pass);
        }

        // создать пользователя
        public UserExDTO CreateUser(String SessionKey)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var user = new User();

                // сгенерируем новый логин, которого нет в базе
                string NewLogin;
                do
                    NewLogin = "Login" + (new Random().Next()).ToString();
                while (db.Users.Where(u => u.Login == NewLogin).Count() > 0);

                user.Login = NewLogin;
                //user.Password = NewLogin;
                user.PasswordSalt = Guid.NewGuid().ToString();
                user.Password_SaltAndHash = WCFServices.HashMd5.getMd5Hash(NewLogin + user.PasswordSalt);

                user.PasswordEncrypted = WCFServices.ServiceHelper.Crypt.Encrypt(NewLogin, Properties.Settings.Default.KeyForPasswordEncrypt);

                user.Name = "Имя";
                user.Institution = "Не указано";
                user.Phone = "Не указан";
                user.Email = NewLogin + "@c.com";
                user.SendNotifications = false;
                user.CancelNotificationKey = Guid.NewGuid();
                user.EmailConfirmed = false;
                user.EmailConfirmKey = Guid.NewGuid();
                user.Address = "Не указан";
                user.RegistrationDate = DateTime.Now;
                user.LoginFailCount = 0;

                db.Users.AddObject(user);
                db.SaveChanges();

                return GetUser(SessionKey, user.Id);
            }
        }

        // создать пользователя
        public void CreateOneUser(String SessionKey, UserExDTO Data)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                var user = new User();

                if (user.Login != Data.Login)
                {
                    if (db.Users.Where(u => u.Login == Data.Login).Count() > 0)
                        throw new Exception("Login already exist");
                }
                
                user.Login = Data.Login;
                //user.Password = NewLogin;
                user.PasswordSalt = Guid.NewGuid().ToString();
                user.Password_SaltAndHash = WCFServices.HashMd5.getMd5Hash(Data.Login + user.PasswordSalt);
                user.PasswordEncrypted = WCFServices.ServiceHelper.Crypt.Encrypt(Data.Login, Properties.Settings.Default.KeyForPasswordEncrypt);

                user.Name = Data.Name;
                user.Institution =  Data.Institution==null ? "Не указано" : Data.Institution;
                user.Phone = Data.Phone == null ? "Не указано" : Data.Phone; 
                user.Email = Data.Email;
                user.SendNotifications = Data.SendNotifications == null ? false : Data.SendNotifications; 
                user.CancelNotificationKey = Guid.NewGuid();
                user.EmailConfirmed = false;
                user.EmailConfirmKey = Guid.NewGuid();
                user.Address = Data.Address == null ? "Не указано" : Data.Address;
                user.RegistrationDate = DateTime.Now;
                user.LoginFailCount = 0;
                user.SocialNetworks = Data.SocialNetworks;
                user.ClassCourseGroup = Data.ClassCourseGroup == null ? "Не указано" : Data.ClassCourseGroup;

                db.Users.AddObject(user);
                db.SaveChanges();

            }
        }

        // обновить основные данные пользователя(все кроме пароля)
        public void UpdateUser(String SessionKey, UserExDTO Data)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var user = db.Users.Where(u => u.Id == Data.Id).Single();

                user.Name = Data.Name;
                if (user.Login != Data.Login)
                {
                    if (db.Users.Where(u => u.Login == Data.Login).Count() > 0)
                        throw new Exception("Login already exist");
                    user.Login = Data.Login;
                }

                user.Institution = Data.Institution == null ? "Не указано." : Data.Institution;

                if (user.Email != Data.Email)
                {
                    var emails = db.Users.Select(u => u.Email.Trim().ToLower()).ToList();
                    if (emails.Contains(Data.Email.Trim().ToLower()))
                        throw new uniJudgeException("Email already exist", TypeException.EmailIsAlredyExist);

                    user.Email = Data.Email;
                }



                user.EmailConfirmed = Data.EmailConfirmed;
                user.Phone = Data.Phone == null ? "Не указано." : Data.Phone;

                user.SendNotifications = Data.SendNotifications;
                user.RegistrationDate = Data.RegistrationDate;

                user.Note = Data.Note == null ? "" : Data.Note;

                user.LoginFailCount = Data.LoginFailCount == null ? 0 : Data.LoginFailCount;
                 user.BlockedTo = Data.BlockedTo;

                user.Address = Data.Address == null ? "Не указано" : Data.Address;
                user.ClassCourseGroup = Data.ClassCourseGroup == null ? "Не указано" : Data.ClassCourseGroup;
                db.SaveChanges();
            }
        }

        // обновить основные данные пользователя(все кроме пароля)
        public void WebUpdateUser(String SessionKey, UserExDTO Data, int id, int failCount, bool sentConf, bool emailConf)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var user = db.Users.Where(u => u.Id == id).Single();

                user.Name = Data.Name;
                if (user.Login != Data.Login)
                {
                    if (db.Users.Where(u=>u.Login == Data.Login).Count()>0)
                        throw new Exception("Login already exist");
                    user.Login = Data.Login;
                }

                if (user.Email != Data.Email)
                {
                    var emails = db.Users.Select(u => u.Email.Trim().ToLower()).ToList();
                    if (emails.Contains(Data.Email.Trim().ToLower()))
                        throw new uniJudgeException("Email already exist", TypeException.EmailIsAlredyExist);

                    user.Email = Data.Email;
                }

                user.Phone = Data.Phone==null?"Не указано.":Data.Phone;
                user.Note = Data.Note==null ? "" : Data.Note;
                user.Address = Data.Address==null? "Не указано" : Data.Address;
                user.ClassCourseGroup = Data.ClassCourseGroup == null ? "Не указано" : Data.ClassCourseGroup;
                user.Institution = Data.Institution == null ? "Не указано." : Data.Institution;
                user.SocialNetworks = Data.SocialNetworks == null ? "Не указано." : Data.SocialNetworks;

                user.SendNotifications = sentConf;
                user.EmailConfirmed = emailConf;
                user.LoginFailCount = failCount == null ? 0 : failCount;

                db.SaveChanges();
            }
        }


        public void DeleteUser(String SessionKey, int UserId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var user = db.Users.Where(u => u.Id == UserId).Single();
                db.Users.DeleteObject(user);
                db.SaveChanges();

                if (db.Users.Where(u => u.Id == UserId).SingleOrDefault() != null)
                    throw new Exception("Не удалось удалить пользователя");
            }

        }


        public void UpdateGroupsForUser(String SessionKey, int UserId, IEnumerable<int> newGroupIdCollecton)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                foreach (var e in db.MM_UserGroup.Where(u => u.UserId == UserId))
                    db.DeleteObject(e);

                foreach (var e in newGroupIdCollecton)
                    db.MM_UserGroup.AddObject(new MM_UserGroup { UserId = UserId, GroupId = e });

                db.SaveChanges();
            }
        }

        // Запросить валидацию email
        public void ValidateUserEmail(String SessionKey, int UserId)
        {
            CheckSession(SessionKey);
            ServiceHelper.MailManager.ValidateUserEmail(UserId, false);
        }

        // Запросить восстановление пароля через email
        public void ChangePasswordUsingUserEmail(String SessionKey, int UserId)
        {
            throw new Exception("Not implement");
        }

       // заблокировать группу пользователей
        public void BlockGroupUser(String SessionKey, int [] id, DateTime date)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                for (int i = 0; i < id.Length; i++)
                {
                    int ID = id[i];
                    var user = db.Users.Where(u => u.Id == ID).Single();
                    user.BlockedTo = date;
                }
                db.SaveChanges();
            }

        }

        // разблокировать группу пользователей
        public void UnBlockGroupUser(String SessionKey, int[] id)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                for (int i = 0; i < id.Length; i++)
                {
                    int ID = id[i];
                    var user = db.Users.Where(u => u.Id == ID).Single();
                    user.BlockedTo = null;
                }
                db.SaveChanges();
            }

        }

        // удалить группу пользователей
        public void DeleteGroupUser(String SessionKey, int[] id)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                for (int i = 0; i < id.Length; i++)
                {
                    int ID = id[i];
                    var user = db.Users.Where(u => u.Id == ID).Single();
                    db.Users.DeleteObject(user);
                    db.SaveChanges();

                    if (db.Users.Where(u => u.Id == ID).SingleOrDefault() != null)
                        throw new Exception("Не удалось удалить пользователя, по индексу"+ID);
                }
            }

        }

    }
}
