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
using WCFServices.ServiceHelper;

namespace WCFServices.ServiceForAdmin
{

   // [ErrorHandlerBehavior(typeof(ServiceForUserErrorHandler))]
    partial class ServiceForAdmin :IServiceForAdmin
    {
        private static int VersionAPI = 3; // Версия API текущего сервиса
        private static int VersionDB = 2; // Версия структуры БД, под которой может работать этот сервис

        public ServiceForAdmin()
        {
            if (SettingsInDataBase.DataBaseStructureVersion!=VersionDB)
                throw new Exception(String.Format("Сервис не может работать с текущей версией БД. Версия сервиса={1}, Версия БД={0}",
                    SettingsInDataBase.DataBaseStructureVersion,VersionDB));
        }

        // Получить версию библиотеки сервиса
        public int GetVersionAPI()
        {
            return VersionAPI;
        }

        // Проверка доступности БД
        public bool CheckDatabaseExist()
        {
            try
            {
                using (var db = new DataBase())
                    db.Settings.FirstOrDefault();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // проверка сессии. при удачной проверке возвращается Id пользователя, иначке исключение соотв. типа
        private int CheckSession(String SessionKey)
        {
            using (var db = new DataBase())
            {
                Guid guid;
                try
                {
                    guid = Guid.Parse(SessionKey);
                }
                catch (Exception)
                {
                    throw new uniJudgeException("Некорректная сессия", TypeException.InvalidSessionKey);
                }

                var user = db.Admins.Where(u => u.SessionKey == guid).SingleOrDefault();
                if (user == null)
                    throw new uniJudgeException("Некорректная сессия", TypeException.InvalidSessionKey);
                return user.Id;
            }
        }
        /*
        // авторизация пользователя по логину и хешу пароля
        public String LoginUserMd5(String Login, String Password_md5)
        {
            Debug.Info("Call LoginUser Login =" + Login);

            using (var db = new DataBase())
            {
                var user = db.Admins.Where(u => u.Login == Login && String.Compare(u.PasswordMD5, Password_md5, true) == 0).SingleOrDefault();
                if (user == null)
                {
                    throw new uniJudgeException("Пользователь с таким логином и паролем не найден", TypeException.InvalidLoginOrPassword);
                }
                else
                {
                    user.SessionKey = Guid.NewGuid();
                    db.SaveChanges();
                    return user.SessionKey.ToString();
                }
            }
        }*/

        // авторизация пользователя по логину и паролю
        public String LoginUser(String Login, String Password)
        {
            using (var db = new DataBase())
            {
                var user = db.Admins.Where(u => u.Login == Login).SingleOrDefault();
                if (user == null)
                {
                    throw new uniJudgeException("Пользователь с таким логином и паролем не найден", TypeException.InvalidLoginOrPassword);
                }
                else
                {
                    // теперь сверим пароли
                    if (user.Password_SaltAndHash == HashMd5.getMd5Hash(Password + user.PasswordSalt))
                    {
                        if (user.AllowAccessToAdminPanel == false)
                            throw new uniJudgeException("Запрещен доступ к админ панели", TypeException.AccessDenided);

                        user.SessionKey = Guid.NewGuid();
                        db.SaveChanges();
                        return user.SessionKey.ToString();
                    }
                    else
                    {
                        throw new uniJudgeException("Пользователь с таким логином и паролем не найден", TypeException.InvalidLoginOrPassword);
                    }
                }
            }
        }

        public IEnumerable<GroupDTO> GetAllGroups(String SessionKey)
            {
                CheckSession(SessionKey);
                var Groups = from gr  in new DataBase().Groups
                             select new GroupDTO{Id = gr.Id, Title = gr.Title};

                return Groups.ToList(); 
            }


        public DictionaryCollectionDTO GetDictionaries(String SessionKey, bool IncludeContests, bool IncludeTasks, bool IncludeUsers)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                var res = new DictionaryCollectionDTO();
                if (IncludeContests)
                    res.Contests = (from contest in db.Contests
                                    orderby contest.ShortTitle
                                    select new KeyValueDTO { Key = contest.Id, StringValue = contest.ShortTitle }
                                    ).ToList();
                if (IncludeTasks)
                    res.Tasks = (from task in db.Tasks
                                 orderby task.Title
                                 select new KeyValueDTO { Key = task.Id, StringValue = task.Title }
                                 ).ToList();
                if (IncludeUsers)
                    res.Users = (from user in db.Users
                                 orderby user.Name
                                 select new KeyValueDTO { Key = user.Id, StringValue = user.Name }
                                ).ToList();

                return res;
            }
        }

    }
}
