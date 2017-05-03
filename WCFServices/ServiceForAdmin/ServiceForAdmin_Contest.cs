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
    public partial class ServiceForAdmin
    {
        public IEnumerable<ContestDTO> GetContests(String SessionKey)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                return (from c in db.Contests
                        select new ContestDTO
                        {
                            Id = c.Id,
                            ShortTitle = c.ShortTitle,
                            Enabled = c.Enabled,
                            MainContestId_IfVirtual = c.MainContestId_IfVirtual,
                            Note = c.Note
                        }).ToList();
            }
        }


        // Возвращается информация о контесте за исключением:
        // Приветственного сообщения
        // Спонсорского текста
        public ContestExDTO GetContest(String SessionKey, int ContestId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var c = db.Contests.Where(e=>e.Id==ContestId).Single();

                var contest = new ContestExDTO
                                {
                                    Id = c.Id,
                                    Title = c.Title,
                                    ShortTitle = c.ShortTitle,
                                    Enabled = c.Enabled,
                                    MainContestId_IfVirtual = c.MainContestId_IfVirtual,
                                    Note = c.Note,
                                    AllowPractice = c.AllowPractice,
                                    AllowShowAllBoard = c.AllowShowAllBoard,
                                    AllowShowBoardForGuest = c.AllowShowBoardForGuest,
                                    AllowShowComplexityTask = c.AllowShowComplexityTask,
                                    AllowShowNumberFailTest = c.AllowShowNumberFailTest,
                                    AllowShowSubjectTask = c.AllowShowSubjectTask,
                                    AllowShowTaskForGuest = c.AllowShowTaskForGuest,
                                    AllowShowVirtualUser = c.AllowShowVirtualUser,
                                    VirtualUserOutOfCompetition = c.VirtualUserOutOfCompetition,

                                    AutoRegisterToGroupId = c.AutoRegisterToGroupId,
                                    ContestTypeId = c.ContestTypeId,
                                    DateEnd = c.DateEnd,
                                    DateFrozen = c.DateFrozen,
                                    DateStart = c.DateStart,
                                    DateUnfrozen = c.DateUnfrozen,
                                    InvididualDateStart = c.InvididualDateStart,
                                    AllowShowOnlyActiveUserInBoard = c.AllowShowOnlyActiveUserInBoard,
                                    AllowShowPointsInBoard = c.AllowShowPointsInBoard,
                                    AllowShowTimeInBoard = c.AllowShowTimeInBoard,
                                    AllowShowTaskAuthor = c.AllowShowTaskAuthor, 
                                    AllowShowTaskSource = c.AllowShowTaskSource,
                                    AllowSwowSubmitsInBoardWhenFrozenTime = c.AllowSwowSubmitsInBoardWhenFrozenTime,
                                    //WellcomeText = c.WellcomeText
                                    //SponsorText = c.SponsorText
                                };

                contest.Tasks = (from d in c.MM_ContestTask
                                select new TaskInContestDTO
                                {
                                    Id = d.TaskId,
                                    Title = d.Task.Title,
                                    CostInContest = d.Cost,
                                    TaskIdInContest = d.TaskIdInContest
                                }).ToList();

                contest.Compilers = (from d in c.MM_ContestCompiler
                                     select new CompilerDTO
                                     {
                                         Id = d.CompilerId,
                                         Note = d.Compiler.Note,
                                         ShortName = d.Compiler.ShortName,
                                         FullName = d.Compiler.FullName
                                     }).ToList();

                contest.Groups = (from d in c.MM_ContestGroup
                                     select new GroupInContestDTO
                                     {
                                         Id = d.GroupId,
                                         Title = d.Group.Title,
                                         GroupType = WCFServices.GetGroupTypeForContest.Get(d.GroupType)
                                     }).ToList();


                return contest;
            }
        }

        // обновить информацию о контесте(кроме списка задач, компиляторов, групп, приветственного сообщения, и  спонсорского текста)
        public void UpdateContest(String SessionKey, ContestExDTO Contest)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                var _contest = db.Contests.Where(c => c.Id == Contest.Id).Single();
                _contest.Title = Contest.Title;
                _contest.ShortTitle = Contest.ShortTitle;
                _contest.Enabled = Contest.Enabled;
                _contest.MainContestId_IfVirtual = Contest.MainContestId_IfVirtual;
                _contest.Note = Contest.Note;
                _contest.AllowPractice = Contest.AllowPractice;
                _contest.AllowShowAllBoard = Contest.AllowShowAllBoard;
                _contest.AllowShowBoardForGuest = Contest.AllowShowBoardForGuest;

                if (Contest.AllowShowAllBoard==false && Contest.AllowShowBoardForGuest == true)
                    throw new Exception("Неверная конфигурация. Нельзя личный борд делать доступным гостям");

                _contest.AllowShowComplexityTask = Contest.AllowShowComplexityTask;
                _contest.AllowShowNumberFailTest = Contest.AllowShowNumberFailTest;
                _contest.AllowShowSubjectTask = Contest.AllowShowSubjectTask;
                _contest.AllowShowTaskForGuest = Contest.AllowShowTaskForGuest;
                _contest.AllowShowVirtualUser = Contest.AllowShowVirtualUser;
                _contest.VirtualUserOutOfCompetition = Contest.VirtualUserOutOfCompetition;

                _contest.AutoRegisterToGroupId = Contest.AutoRegisterToGroupId;
                if (Contest.AutoRegisterToGroupId.HasValue)
                {
                    var cnt = db.MM_ContestGroup.Where(a=> a.ContestId==Contest.Id && a.GroupId==Contest.AutoRegisterToGroupId.Value).Count();
                    if (cnt == 0)
                        throw new Exception("Нельзя назначить авто регистрацию в группу, которая не принадлежит контесту");
                    if (cnt >1)
                        throw new Exception(String.Format("В таблице MM_ContestGroup существует больше одной записи для ContestId={0} GroupId={1}",
                            Contest.Id,Contest.AutoRegisterToGroupId.Value));
                }

                _contest.ContestTypeId = Contest.ContestTypeId;
                _contest.DateEnd = Contest.DateEnd;
                _contest.DateFrozen = Contest.DateFrozen;
                _contest.DateStart = Contest.DateStart;
                _contest.DateUnfrozen = Contest.DateUnfrozen;

                if (_contest.DateEnd.HasValue && _contest.DateEnd< _contest.DateStart)
                    throw new Exception("Дата окончания контеста должна быть позже даты начала");
                if (_contest.DateFrozen.HasValue==false && _contest.DateUnfrozen.HasValue==true)
                    throw new Exception("Если есть дата разморозки - то должна быть указана и дата заморозки");
                if (_contest.DateFrozen.HasValue && _contest.DateUnfrozen.HasValue &&  _contest.DateFrozen > _contest.DateUnfrozen)
                    throw new Exception("Дата разморозки должна быть позже даты заморозки");
                if (_contest.DateFrozen.HasValue && _contest.DateEnd.HasValue && _contest.DateFrozen > _contest.DateEnd)
                    throw new Exception("Дата заморозки должна быть во время контеста");
                
                _contest.InvididualDateStart = Contest.InvididualDateStart;
                
                _contest.AllowShowOnlyActiveUserInBoard = Contest.AllowShowOnlyActiveUserInBoard;
                _contest.AllowShowPointsInBoard = Contest.AllowShowPointsInBoard;
                _contest.AllowShowTimeInBoard = Contest.AllowShowTimeInBoard;

                _contest.AllowSwowSubmitsInBoardWhenFrozenTime = Contest.AllowSwowSubmitsInBoardWhenFrozenTime;
                _contest.AllowShowTaskSource = Contest.AllowShowTaskSource;
                _contest.AllowShowTaskAuthor = Contest.AllowShowTaskAuthor;
                db.SaveChanges();
            }
            return;
        }

        public void UpdateGroupsForContest(String SessionKey, int ContestId, IEnumerable<GroupInContestDTO> newGroupCollecton)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                foreach (var e in db.MM_ContestGroup.Where(u => u.ContestId == ContestId))
                    db.DeleteObject(e);

                foreach (var e in newGroupCollecton)
                    db.MM_ContestGroup.AddObject(new MM_ContestGroup {ContestId = ContestId, GroupId = e.Id, GroupType =  (int)e.GroupType });

                db.SaveChanges();
            }
        }

        public void UpdateCompilersForContest(String SessionKey, int ContestId, IEnumerable<int> newCompilerIdCollecton)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                foreach (var e in db.MM_ContestCompiler.Where(u => u.ContestId == ContestId))
                    db.DeleteObject(e);

                foreach (var e in newCompilerIdCollecton)
                    db.MM_ContestCompiler.AddObject(new MM_ContestCompiler { ContestId = ContestId, CompilerId = e });

                db.SaveChanges();
            }
        }

        public void UpdateTasksForContest(String SessionKey, int ContestId, IEnumerable<TaskInContestDTO> newTasksCollecton)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                foreach (var e in db.MM_ContestTask.Where(u => u.ContestId == ContestId))
                    db.DeleteObject(e);

                foreach (var e in newTasksCollecton)
                    db.MM_ContestTask.AddObject(new MM_ContestTask { ContestId = ContestId, TaskId=e.Id, Cost = e.CostInContest, TaskIdInContest = e.TaskIdInContest });

                db.SaveChanges();
            }
        }

        public ContestExDTO CreateContest(String SessionKey)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                var contest = new Contest();

                contest.Title = "ContestTitle";
                contest.ShortTitle = "Contest.ShortTitle";
                contest.Enabled = true;
                contest.MainContestId_IfVirtual = null;
                contest.Note = "";
                contest.AllowPractice = false;
                contest.AllowShowAllBoard = true;
                contest.AllowShowBoardForGuest = false;

                contest.AllowShowComplexityTask = false;
                contest.AllowShowNumberFailTest = true;
                contest.AllowShowSubjectTask = false;
                contest.AllowShowTaskForGuest = false;
                contest.AllowShowVirtualUser = true;
                contest.VirtualUserOutOfCompetition = true;
                contest.AllowShowTaskAuthor = false;
                contest.AllowShowTaskSource = false;
                contest.AllowSwowSubmitsInBoardWhenFrozenTime = false;
                contest.WellcomeText = "Добро пожаловать";
                contest.SponsorText = "";
                
                contest.AutoRegisterToGroupId = null;
                
      
                contest.InvididualDateStart = false;

                contest.AllowShowOnlyActiveUserInBoard = true;
                contest.AllowShowPointsInBoard = false;
                contest.AllowShowTimeInBoard = false;

                contest.DateStart = DateTime.Now;
                contest.ContestTypeId = db.ContestTypes.First().Id;

                
                db.Contests.AddObject(contest);
                db.SaveChanges();

                return GetContest(SessionKey, contest.Id);
            }
        }

        public void DeleteContest(String SessionKey, int ContestId)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                var contest = db.Contests.Where(c=>c.Id == ContestId).Single();

                if (contest.Submits.Count >0)
                    throw new Exception(string.Format("В этом контесте {0} сабмитов. Удаление невозможно",contest.Submits.Count));

                db.Contests.DeleteObject(contest);
                
                db.SaveChanges();
            }
        }

        public String GetWellcomeText(String SessionKey, int ContestId)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                return db.Contests.Where(c => c.Id == ContestId).Single().WellcomeText;
            }
        }
        
        public void UpdateWellcomeText(String SessionKey, int ContestId, String WellcomeText)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                var contest = db.Contests.Where(c => c.Id == ContestId).Single();
                contest.WellcomeText = WellcomeText;
                db.SaveChanges();
            }
        }

        public String GetSponsorText(String SessionKey, int ContestId)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                return db.Contests.Where(c => c.Id == ContestId).Single().SponsorText;
            }
        }

        public void UpdateSponsorText(String SessionKey, int ContestId, String SponsorText)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                var contest = db.Contests.Where(c => c.Id == ContestId).Single();
                contest.SponsorText = SponsorText;
                db.SaveChanges();
            }
        }
    }
}
