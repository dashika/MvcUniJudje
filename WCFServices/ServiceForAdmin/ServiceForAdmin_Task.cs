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
        // получить список задач
        public IEnumerable<TaskDTO> GetTasks(String SessionKey)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var tmp = from c in db.Tasks
                          select new TaskDTO
                          {
                              Id = c.Id,
                              Title = c.Title,
                              EnableSend = c.EnableSend,
                              EnableCheck = c.EnableCheck,
                              Note = c.Note,
                              ContestCollection = from t in c.MM_ContestTask
                                                  select new ContestDTO
                                                  {
                                                        Id = t.ContestId,
                                                        ShortTitle = t.Contest.ShortTitle
                                                  }

                          };
                return tmp.ToList();
            }
        }


        // получить список сложностей задач
        public IEnumerable<TaskComlexity> GetTasksComlexity(String SessionKey)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                return (from c in db.ComplexityTasks
                        select new TaskComlexity
                        {
                            ID = c.Id,
                            Title = c.Title
                        }).ToList();
            }
        }

        public IEnumerable<TaskComlexity> GetTasksSubject(String SessionKey)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                return (from c in db.Subjects
                        select new TaskComlexity
                        {
                            ID = c.Id,
                            Title = c.Title
                        }).ToList();
            }
        }


        // получить задачу
        public TaskExDTO GetTask(String SessionKey, int TaskId, bool IncludeStatement)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var tmp = from c in db.Tasks
                          where c.Id == TaskId
                          select new TaskExDTO
                          {
                              Id = c.Id,
                              Title = c.Title,
                              EnableSend = c.EnableSend,
                              EnableCheck = c.EnableCheck,
                              Author = c.Author,

                              Note = c.Note,
                              ComplexityId = c.ComplexityId,
                              CountTestToText = c.CountTestToText,
                              CodeLimit = c.CodeLimit,
                              DefaultCodeLimit = WCFServices.ServiceHelper.SettingsInDataBase.DefautlCodeLimit,
                              OutputLimit = c.OutputLimit,
                              DefaultOutputLimit = WCFServices.ServiceHelper.SettingsInDataBase.DefaultOutputLimit,

                              TaskSource = c.TaskSource,
                              TimeSolveProblem = c.TimeSolveProblem,

                              MemoryLimit_custom = c.MemoryLimit_custom,
                              MemoryLimit_dotnet = c.MemoryLimit_dotnet,
                              MemoryLimit_javavm = c.MemoryLimit_javavm,
                              MemoryLimit_native = c.MemoryLimit_native,

                              TimeLimit_javavm = c.TimeLimit_javavm,
                              TimeLimit_custom = c.TimeLimit_custom,
                              TimeLimit_native = c.TimeLimit_native,
                              TimeLimit_dotnet = c.TimeLimit_dotnet,
                              
                              Statement = IncludeStatement?(c.Statement):(""),
                              StatementPdf = IncludeStatement ? (c.StatementPdf) : (null),
                              TaskHasPdfStatement = (c.StatementPdf!=null),
                              //UsePdfStatement = IncludeStatement ? (c.UsePdfStatement) : (false),
                              
                              Tests = from t in c.Tests
                                      select new TestDTO
                                      {
                                          Id = t.Id,
                                          Enabled = t.Enabled,
                                          Number = t.Number
                                      },
                              Checkers = from t in c.Checkers
                                      select new CheckerDTO
                                      {
                                          Id = t.Id,
                                          Enabled = t.Enabled
                                      },

                              ContestCollection = from t in c.MM_ContestTask
                                                  select new ContestDTO
                                                  {
                                                        Id = t.ContestId,
                                                        ShortTitle = t.Contest.ShortTitle
                                                  }
                          };
                
                return tmp.Single();
            }
        }


        // получить текст задачи
        public String GetTaskStatement(String SessionKey, int TaskId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                return db.Tasks.Where(t => t.Id == TaskId).Single().Statement;
            }
        }

        // обновить текст задачи
        public void UpdateTaskStatement(String SessionKey, int TaskId, String Statement)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var task = db.Tasks.Where(t => t.Id == TaskId).Single();
                task.Statement = Statement;

                db.SaveChanges();
            }
        }

        // обновить информацию о задаче(кроме текста, тестов и чекеров)
        public void UpdateTask(String SessionKey, TaskExDTO Task, bool UpdateStatement)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                UpdateTaskHelper(db, Task, UpdateStatement);

                db.SaveChanges();
            }
            return;
        }

        // внутренний вспомагательный метод для обновления задачи
        private void UpdateTaskHelper(DataBase db, TaskExDTO Task, bool UpdateStatement)
        {
            var task = db.Tasks.Where(t => t.Id == Task.Id).Single();
            task.Title = Task.Title;
            task.Note = Task.Note;
            task.TimeLimit_native = Task.TimeLimit_native;
            task.TimeLimit_javavm = Task.TimeLimit_javavm;
            task.TimeLimit_dotnet = Task.TimeLimit_dotnet;
            task.TimeLimit_custom = Task.TimeLimit_custom;
            task.MemoryLimit_native = Task.MemoryLimit_native;
            task.MemoryLimit_javavm = Task.MemoryLimit_javavm;
            task.MemoryLimit_dotnet = Task.MemoryLimit_dotnet;
            task.MemoryLimit_custom = Task.MemoryLimit_custom;
            task.OutputLimit = Task.OutputLimit;
            task.CodeLimit = Task.CodeLimit;
            task.Author = Task.Author;
            task.TaskSource = Task.TaskSource;
            task.TimeSolveProblem = Task.TimeSolveProblem;
            task.CountTestToText = Task.CountTestToText;
            task.EnableSend = Task.EnableSend;
            task.EnableCheck = Task.EnableCheck;
            if (UpdateStatement)
            {
                //task.UsePdfStatement = Task.UsePdfStatement;
                task.StatementPdf = Task.StatementPdf;
                task.Statement = Task.Statement;
            }
        }


        // создает новую задачу и возвращает ее
        public TaskExDTO CreateTask(String SessionKey)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                var task = new Task();

                // установим все значения по умолчанию
                task.Title = "new Task";
                task.TimeLimit_native = 2000;
                task.TimeLimit_javavm = 2000;
                task.TimeLimit_dotnet = 2000;
                task.TimeLimit_custom = 2000;
                task.MemoryLimit_native=64000;
                task.MemoryLimit_javavm=64000;
                task.MemoryLimit_dotnet=64000;
                task.MemoryLimit_custom=64000;
                task.OutputLimit  = null;
                task.CodeLimit = null;
                task.TimeSolveProblem = 60;
                task.CountTestToText = 2;
                task.EnableCheck = false;
                task.EnableSend = false;

                db.Tasks.AddObject(task);
                db.SaveChanges();


                MM_TaskSubject MMT = new MM_TaskSubject();
                MMT.TaskId = task.Id;
                MMT.SubjectId = 1;
                db.MM_TaskSubject.AddObject(MMT);
                db.SaveChanges();
                
                return GetTask(SessionKey,task.Id,false);
            }
        }

        /// <summary>
        /// получить ИД темы задачи
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="ID">ИД задачи</param>
        /// <returns> ИД темы задачи</returns>
        public int GetSubject(String SessionKey, int ID)
        {    
            CheckSession(SessionKey);

              using (var db = new DataBase())
              {
                  return (from c in db.MM_TaskSubject
                          where c.TaskId == ID
                          select c.SubjectId).SingleOrDefault();
                    }
        }

        // удалить задачу
        public void DeleteTask(String SessionKey, int TaskId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var task = db.Tasks.Where(t => t.Id == TaskId).Single();
                
                db.DeleteObject(task);
                db.SaveChanges();
                return ;
            }
        }

        // обновить задачу
        public void UpdateTaskEx(String SessionKey, TaskExDTO Task, IEnumerable<TestDTO> Tests, bool AppendTests, 
            bool UpdateTaskStatement, IEnumerable<CheckerDTO> Checkers, bool AppendCheckers)
        {
            
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                UpdateTaskHelper(db, Task, UpdateTaskStatement); // обновим задачу

                // теперь приступим к тестам
                if (Tests != null && Tests.Count() > 0)
                {
                    if (AppendTests == false) // если не добавляем тесты - значит удалим существующий набор
                        foreach (var test in db.Tests.Where(t => t.TaskId == Task.Id))
                            db.DeleteObject(test);


                    foreach (var test in Tests)
                        db.Tests.AddObject(new Test
                        {
                            TaskId = Task.Id,
                            InputData = test.InputData,
                            PatternData = test.PatternData,
                            Number = test.Number,
                            Multitest = test.Multitest,
                            Enabled = test.Enabled
                        });


                }

                // и не забудем про чекеры.
                // НЕ РЕАЛИЗОВАНО!!!!!

                db.SaveChanges();
            }
        }

        public void UpdateSubjectComlexity(String SessionKey,int IDTask, int subject, int Comlexity)
        {
              CheckSession(SessionKey);

              using (var db = new DataBase())
              {

                  MM_TaskSubject MMT = (from c in db.MM_TaskSubject
                               where c.TaskId == IDTask
                                         select c).SingleOrDefault<MM_TaskSubject>();

                  if (MMT == null)
                  {
                      MMT = new MM_TaskSubject()
                      {
                          TaskId = IDTask,
                          SubjectId = subject
                      };
                      db.MM_TaskSubject.AddObject(MMT);
                  }
                  MMT.SubjectId = subject;

                  Task task = (from cc in db.Tasks
                                 where cc.Id == IDTask
                                 select cc).SingleOrDefault<Task>();
                  task.ComplexityId = Comlexity;

                      db.SaveChanges();

              }
        }

        // обновить pdf версию задачи
        public void UpdateTaskPdfStatement(String SessionKey, int TaskId, byte[] Statement)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var task = db.Tasks.Where(t => t.Id == TaskId).Single();

                task.StatementPdf = Statement;
                db.SaveChanges();
                return;
            }
        }

        // удалить pdf версию задачи

        public void DeleteTaskPdfStatement(String SessionKey, int TaskId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var task = db.Tasks.Where(t => t.Id == TaskId).Single();

                task.StatementPdf = null;
                db.SaveChanges();
                return;
            }
        }

        // удалить pdf версию задачи
        public byte[] DownloadTaskPdfStatement(String SessionKey, int TaskId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var task = db.Tasks.Where(t => t.Id == TaskId).Single();

                return task.StatementPdf;
            }
        }


        /// <summary>
        /// добавление новой темы
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="title">навание</param>
        public void AddNewSubject(String SessionKey, String title)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                Subject sub = new Subject()
                {
                    Title = title
                };

                db.Subjects.AddObject(sub);
                db.SaveChanges();
            }
        }


        /// <summary>
        /// добавление сложности
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="title"></param>
        public void AddNewComlexity(String SessionKey, String title)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                ComplexityTask com = new ComplexityTask()
                {
                    Title = title
                };

                db.ComplexityTasks.AddObject(com);
                db.SaveChanges();
            }
        }


        /// <summary>
        /// удаление темы
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="IDS">ИД темы</param>
        public void DeleteSubject(String SessionKey, Int32 IDS)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                // удаляем все связи в MM_TaskSubject
                // AccessButtons - просто по параметрам подходил
              IEnumerable <AccessButtons> mm = (from c in db.MM_TaskSubject
                                     where c.SubjectId == IDS
                                     select new AccessButtons{ID = c.SubjectId}).ToList();

                foreach (var sub in mm)
                {
                    MM_TaskSubject tas = (from c in db.MM_TaskSubject
                                          where c.SubjectId == sub.ID
                                          select c).SingleOrDefault<MM_TaskSubject>();
                    db.MM_TaskSubject.DeleteObject(tas);
                    db.SaveChanges();
                };

                Subject  subj = (from c in db.Subjects
                                      where c.Id == IDS
                                      select c).SingleOrDefault<Subject>();

                db.Subjects.DeleteObject(subj);
                db.SaveChanges();
            }
        }


        /// <summary>
        /// удаление сложности
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="IDD">ИД сложности</param>
        public void DeleteComlexity(String SessionKey, Int32 IDD)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                Task task = (from c in db.Tasks
                             where c.ComplexityId == IDD
                             select c).SingleOrDefault<Task>();
                if (task != null)
                {
                    task.ComplexityId = null;
                    db.SaveChanges();
                }
                ComplexityTask subj = (from c in db.ComplexityTasks
                                       where c.Id == IDD
                                       select c).SingleOrDefault<ComplexityTask>();

                db.ComplexityTasks.DeleteObject(subj);
                db.SaveChanges();
            }
        }


        /// <summary>
        ///  Обновление темы
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="IDS">ИД темы</param>
        /// <param name="title">название</param>
        public void UpdateNewSubject(String SessionKey, Int32 IDS, String title)
        {
            CheckSession(SessionKey);

               using (var db = new DataBase())
            {
                Subject sub = (from c in db.Subjects
                               where c.Id == IDS
                               select c).SingleOrDefault<Subject>();

                sub.Title = title;
                try
                {
                    db.SaveChanges();
                }
                catch (OptimisticConcurrencyException ex)
                {
                    db.Refresh(refreshMode: RefreshMode.ClientWins,
                         collection: db.Subjects);
                    db.SaveChanges();
                    throw new Exception("Не удалось сохранить изменения. Ошибка :" + ex.Message);
                }            
               }
        }


        /// <summary>
        /// Обновление сложности
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="IDD">ИД сложности</param>
        /// <param name="title">название</param>
        public void UpdateNewComlexity(String SessionKey, Int32 IDD, String title)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                ComplexityTask com = (from c in db.ComplexityTasks
                               where c.Id == IDD
                               select c).SingleOrDefault<ComplexityTask>();

                com.Title = title;
                try
                {
                    db.SaveChanges();
                }
                catch (OptimisticConcurrencyException ex)
                {
                    db.Refresh(refreshMode: RefreshMode.ClientWins,
                         collection: db.ComplexityTasks);
                    db.SaveChanges();
                    throw new Exception("Не удалось сохранить изменения. Ошибка :" + ex.Message);
                }
            }
        }


        /// <summary>
        /// получить ИД темы
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="title">название</param>
        /// <returns>ИД темы</returns>
        public int GetIDSubject(String SessionKey, String title)
        {
             CheckSession(SessionKey);

             using (var db = new DataBase())
             {
                 return (from c in db.Subjects
                         where c.Title.CompareTo(title) == 0
                         select c.Id).SingleOrDefault();
             }
        }


        /// <summary>
        /// получить ИД сложности
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="title">название</param>
        /// <returns> ИД сложности</returns>
        public int GetIDComlexity(String SessionKey,String title)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                return (from c in db.ComplexityTasks
                        where c.Title.CompareTo(title) == 0
                        select c.Id).SingleOrDefault();
            }
        }


        /// <summary>
        /// получить название сложности
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="ID">ИД сложности</param>
        /// <returns>название сложности</returns>
        public String GetTitleComlexity(String SessionKey,Int32 ID)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                return (from c in db.ComplexityTasks
                        where c.Id == ID
                        select c.Title).SingleOrDefault();
            }
        }


        /// <summary>
        /// Получить название темы
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="ID">ИД темы</param>
        /// <returns>название темы</returns>
        public String GetTitleSubject(String SessionKey, Int32 ID)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                return (from c in db.Subjects
                        where c.Id == ID
                        select c.Title).SingleOrDefault();
            }
        }
    }
}
