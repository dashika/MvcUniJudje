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
        /// Получения всех участников группы
        /// </summary>
        /// <param name="SessionKey">ключ сессии</param>
        /// <param name="ID">ИД группы</param>
        /// <returns></returns>
        public IEnumerable<UserDTO> GetAllUserInGroup(String SessionKey, Int32 ID)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                IEnumerable<UserDTO> UDTO = (from c in db.MM_UserGroup
                                             where c.GroupId == ID
                                             select new UserDTO { Id = c.UserId }).ToList();

                UserDTO[] users = new UserDTO[UDTO.Count()];
                int i = 0;
                foreach (var str in UDTO)
                {
                    users[i++] = (from c in db.Users
                                  where c.Id == str.Id
                                  select new UserDTO { Id = c.Id, Name = c.Name }).SingleOrDefault<UserDTO>();
                }

                return users;
            }
        }


        /// <summary>
        /// проверяем если в ИД(Юзера) и ИД(MM_UserGroup) совпадают то пропускаем
        /// </summary>
        /// <param name="user">ID users</param>
        /// <param name="MM_">MM_UserGroup </param>
        /// <returns></returns>
        bool IfIn(int user, IEnumerable<UserDTO> MM_)
        {
            bool pp = true;
            foreach (var tt in MM_)
                if (user == tt.Id) pp = false;
            return pp;
        }

        /// <summary>
        /// Полечение НЕ участников группы
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="ID">ИД группы</param>
        /// <returns></returns>
        public IEnumerable<UserDTO> GetAllUserOutGroup(String SessionKey, Int32 ID)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                //  получаем коллекцию  принадлежащую группе
                IEnumerable<UserDTO> UDTO = (from c in db.MM_UserGroup
                                             where c.GroupId == ID
                                             select new UserDTO { Id = c.UserId }).ToList();
                //  получаем коллекцию всех юзеров
                IEnumerable<UserDTO> Users = (from c in db.Users
                                              select new UserDTO { Id = c.Id, Name = c.Name }).ToList();

                //  для создания списка юзеров не входящих в группу
                List<UserDTO> users = new List<UserDTO>();
                //  int i = 0;


                // проходимся по всему списку юзеров
                foreach (var str in Users)
                {
                    // проверяем если в ИД(Юзера) и ИД(MM_UserGroup) совпадают то пропускаем
                    if (IfIn(str.Id, UDTO))
                    {
                        //  users[i++] = str;
                        users.Add(str);
                    }
                }
                return users;
            }
        }


        /// <summary>
        /// Получение информации о группе
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public GroupDTO GetGroupInfo(String SessionKey, Int32 ID)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                return (from c in db.Groups
                        where c.Id == ID
                        select new GroupDTO { Id = c.Id, Title = c.Title }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Получение контестов группы
        /// </summary>
        /// <param name="SessionKey">код сессии</param>
        /// <param name="Id">ИД группы</param>
        /// <returns>список контестов</returns>
        public IEnumerable<String>  GetContestsInGroup(String SessionKey, Int32 Id)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                List<MM_ContestGroup> MMCG = (from c in db.MM_ContestGroup
                                        where c.GroupId == Id
                                        select c).ToList();
                List<String> ListContests = new List<String>();

                foreach (var mm in MMCG)
                {
                  ListContests.Add(  (from c in db.Contests
                                      where c.Id == mm.ContestId
                                      select c.Title).SingleOrDefault());
                }

                return ListContests;
            }
        }

        /// <summary>
        /// Создание новой группы
        /// </summary>
        /// <param name="SessionKey">ключ сессии</param>
        /// <returns></returns>
        public GroupDTO CreateGroup(String SessionKey)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var group = new Group();

                string NewName;

                do
                    NewName = "Group" + (new Random().Next()).ToString();
                while (db.Groups.Where(u => u.Title == NewName).Count() > 0);

                group.Title = NewName;

                db.Groups.AddObject(group);
                db.SaveChanges();

                return GetGroupInfo(SessionKey, group.Id);
            }
        }


        /// <summary>
        /// Сохранение изменений
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="Group">структура данных группы для изменения</param>
        public void UpdateGroup(String SessionKey, GroupDTO Group)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                Group group = (from c in db.Groups
                               where c.Id == Group.Id
                               select c).SingleOrDefault<Group>();

                group.Title = Group.Title;

                try
                {
                    db.SaveChanges();
                }
                catch (OptimisticConcurrencyException ex)
                {
                    db.Refresh(refreshMode: RefreshMode.ClientWins,
                         collection: db.Groups);
                    db.SaveChanges();
                    throw new Exception("Не удалось сохранить изменения. Ошибка :" + ex.Message);
                }


            }
        }


        public void WebUpdateGroup(String SessionKey, String GroupTitle, int id)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                Group group = (from c in db.Groups
                               where c.Id == id
                               select c).SingleOrDefault<Group>();

                group.Title = GroupTitle;

                try
                {
                    db.SaveChanges();
                }
                catch (OptimisticConcurrencyException ex)
                {
                    db.Refresh(refreshMode: RefreshMode.ClientWins,
                         collection: db.Groups);
                    db.SaveChanges();
                    throw new Exception("Не удалось сохранить изменения. Ошибка :" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Удаление группы
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="GroupID">идентификатор группы</param>
        public void DeleteGroup(String SessionKey, int GroupID)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                IEnumerable<MM_UserGroup> mm = (from c in db.MM_UserGroup
                                                where c.GroupId == GroupID
                                                select c).ToList();

                foreach (var idb in mm)
                {
                    MM_UserGroup mm_ = (from c in db.MM_UserGroup
                                        where c.GroupId == GroupID && c.UserId == idb.UserId
                                        select c).SingleOrDefault<MM_UserGroup>();
                    db.MM_UserGroup.DeleteObject(mm_);
                    db.SaveChanges();
                }


                Group grop = (from c in db.Groups
                              where c.Id == GroupID
                              select c).SingleOrDefault<Group>();
                db.Groups.DeleteObject(grop);
                db.SaveChanges();

                if (db.Groups.Where(u => u.Id == GroupID).SingleOrDefault() != null)
                    throw new Exception("Не удалось удалить ");
            }
        }

        /// <summary>
        /// Добавление участника в группу
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="GroupID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public UserDTO AddInGroup(String SessionKey, int GroupID, int UserID)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var group = new MM_UserGroup();

                group.UserId = UserID;
                group.GroupId = GroupID;

                db.MM_UserGroup.AddObject(group);
                db.SaveChanges();

                return (from c in db.Users
                        where c.Id == UserID
                        select new UserDTO { Id = c.Id, Name = c.Name }).SingleOrDefault();
            }
        }


        /// <summary>
        /// Извлечение участника из группы
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="GroupID"></param>
        /// <param name="UserID"></param>
        public void DeleteOutFromGroup(String SessionKey, int GroupID, int UserID)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                MM_UserGroup mm = (from c in db.MM_UserGroup
                                   where c.GroupId == GroupID && c.UserId == UserID
                                   select c).SingleOrDefault<MM_UserGroup>();
                db.MM_UserGroup.DeleteObject(mm);
                db.SaveChanges();
            }


        }
    }
}
