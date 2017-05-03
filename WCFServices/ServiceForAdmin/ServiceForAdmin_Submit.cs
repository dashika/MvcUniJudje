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
    public partial class ServiceForAdmin : IServiceForAdmin
    {
       
        public IEnumerable<SubmitDTO> GetSubmits(String SessionKey, int? FilterContestId, int? FilterUserId, int? FilterTaskId, int CountOnPage, int PageNum)
        {
            CheckSession(SessionKey);


            using (var db = new DataBase())
            {
                var submits = (from s in db.Submits
                               orderby s.DateTimeSend descending
                               select s).AsQueryable();


                if (FilterContestId != null) submits = submits.Where(s => s.ContestId == FilterContestId);
                if (FilterUserId != null) submits = submits.Where(s => s.UserId == FilterUserId);
                if (FilterTaskId != null) submits = submits.Where(s => s.TaskId == FilterTaskId);


                submits = submits.Skip((PageNum - 1) * CountOnPage); // пропустим те которые были на предыдущих страницах
                submits = submits.Take(CountOnPage); // возьмем только то количество, сколько нужно на страницу

                var res = (from s in submits
                           select new SubmitDTO
                           {
                               Id = s.Id,
                               Note = s.Note,
                               CompilerId = s.CompilerId,
                               CompilerShortName = s.Compiler.ShortName,
                               CompilerFullName = s.Compiler.FullName,
                               ContestId = s.ContestId,
                               ContestTitle = s.Contest.ShortTitle,

                               DateTimeSend = s.DateTimeSend,
                               TimeSendInContest = s.TimeSendInContest,
                               MaxUsedMemory = s.MaxUsedMemory,
                               MaxUsedTime = s.MaxUsedTime,
                               NumberFailTest = s.NumberFailTest,
                               //SendInContestTime = (s.Contest != null && s.Contest.DateEnd.HasValue && s.Contest.DateEnd >= s.DateTimeSend), 
                               SubmitPoints = s.SubmitPoints,
                               SubmitPenalty = s.SubmitPenalty,
                               TaskId = s.TaskId,
                               //TaskIdInContest = s.MM_ContestTask.TaskIdInContest, //т.к. не реализовано. зарезервировано
                               //TaskCostInContest = s.MM_ContestTask.Cost,//т.к. не реализовано. зарезервировано
                               TaskTitle = s.Task.Title,

                               UserId = s.UserId,
                               UserName = s.User.Name,
                               VerdictId = s.VerdictId,
                               VerdictIsFinal = s.Verdict.IsFinal,
                               VerdictShortCode = s.Verdict.ShortCode,
                               VerdictDescription = s.Verdict.Description
                           }
                       ).ToList();

                var MM_ContestTask_cashe = db.MM_ContestTask.ToList();

      
                foreach (var e in res)
                {
                    var tmp = MM_ContestTask_cashe.Where(q => q.TaskId == e.TaskId && q.ContestId == e.ContestId).SingleOrDefault();
                    if (tmp == null) continue;
                    e.TaskIdInContest = tmp.TaskIdInContest;
                    e.TaskCostInContest = tmp.Cost;

                }

                return res;

            }
        }


        public IEnumerable<SubmitDTO> WebGetSubmits(String SessionKey)
        {
            CheckSession(SessionKey);


            using (var db = new DataBase())
            {
                var submits = (from s in db.Submits
                               orderby s.DateTimeSend descending
                               select s).AsQueryable();

                var res = (from s in submits
                           select new SubmitDTO
                           {
                               Id = s.Id,
                               Note = s.Note,
                               CompilerId = s.CompilerId,
                               CompilerShortName = s.Compiler.ShortName,
                               CompilerFullName = s.Compiler.FullName,
                               ContestId = s.ContestId,
                               ContestTitle = s.Contest.ShortTitle,

                               DateTimeSend = s.DateTimeSend,
                               TimeSendInContest = s.TimeSendInContest,
                               MaxUsedMemory = s.MaxUsedMemory,
                               MaxUsedTime = s.MaxUsedTime,
                               NumberFailTest = s.NumberFailTest,
                               //SendInContestTime = (s.Contest != null && s.Contest.DateEnd.HasValue && s.Contest.DateEnd >= s.DateTimeSend), 
                               SubmitPoints = s.SubmitPoints,
                               SubmitPenalty = s.SubmitPenalty,
                               TaskId = s.TaskId,
                               //TaskIdInContest = s.MM_ContestTask.TaskIdInContest, //т.к. не реализовано. зарезервировано
                               //TaskCostInContest = s.MM_ContestTask.Cost,//т.к. не реализовано. зарезервировано
                               TaskTitle = s.Task.Title,

                               UserId = s.UserId,
                               UserName = s.User.Name,
                               VerdictId = s.VerdictId,
                               VerdictIsFinal = s.Verdict.IsFinal,
                               VerdictShortCode = s.Verdict.ShortCode,
                               VerdictDescription = s.Verdict.Description
                           }
                       ).ToList();

                var MM_ContestTask_cashe = db.MM_ContestTask.ToList();


                foreach (var e in res)
                {
                    var tmp = MM_ContestTask_cashe.Where(q => q.TaskId == e.TaskId && q.ContestId == e.ContestId).SingleOrDefault();
                    if (tmp == null) continue;
                    e.TaskIdInContest = tmp.TaskIdInContest;
                    e.TaskCostInContest = tmp.Cost;

                }

                return res;

            }
        }



        // получить обновляемую(обновляемую при перепроверке) информацию о сабмитах
        public IEnumerable<SubmitDTO> GetSubmitUpdate(String SessionKey, int[] SubmitIdCollection)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {

                var submits = (from s in db.Submits
                               where SubmitIdCollection.Contains(s.Id)
                               select s).AsQueryable();

                var res = (from s in submits
                           select new SubmitDTO
                           {
                               Id = s.Id,

                               MaxUsedMemory = s.MaxUsedMemory,
                               MaxUsedTime = s.MaxUsedTime,
                               NumberFailTest = s.NumberFailTest,

                               SubmitPoints = s.SubmitPoints,
                               SubmitPenalty = s.SubmitPenalty,

                               VerdictId = s.VerdictId,
                               VerdictIsFinal = s.Verdict.IsFinal,
                               VerdictShortCode = s.Verdict.ShortCode,
                               VerdictDescription = s.Verdict.Description
                           }
                   ).ToList();

                return res;
            }
        }

        public String SubmitShowCode(String SessionKey, int SubmitId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                return db.Submits.Where(s => s.Id == SubmitId).Single().Code;
            }
        }

        public String SubmitShowAdminLog(String SessionKey, int SubmitId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                return db.Submits.Where(s => s.Id == SubmitId).Single().VerdictLogAdmin;
            }
        }

        public String SubmitShowUserLog(String SessionKey, int SubmitId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                return db.Submits.Where(s => s.Id == SubmitId).Single().VerdictLogUser;
            }
        }

        public bool RejudgeSubmit(String SessionKey, IEnumerable<int> SubmitIdCollection)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var s = db.Submits.Where(s2 => SubmitIdCollection.Contains(s2.Id)).ToList();
                if (s.Count != SubmitIdCollection.Count())
                    throw new Exception("Не удалось найти на сервере все сабмиты из переданной коллекции");
                    
                /*
                Добавим проверку уникальности
                */
                /*var d = new DiffMatchPatch.diff_match_patch();
                var collection = new List<dynamic>();
                foreach(var submit in db.Submits.Where(c=>c.TaskId==s.TaskId && c.DateTimeSend<s.DateTimeSend && c.UserId!=s.UserId))
                {
                    if (submit.Code.Length < 50) 
                        continue;

                    var res = d.diff_main(submit.Code, s.Code);
                    // вычислим то, сколько кода было взято из сабмита
                    int a=0,b=0;
                 

                    foreach (var t in res)
                    {
                        switch( t.operation)
                        {
                            case DiffMatchPatch.Operation.EQUAL:
                                a+=t.text.Length;
                                b+=t.text.Length;
                                break;
                            case DiffMatchPatch.Operation.DELETE:
                                b+=t.text.Length;
                                break;
                        }
                    }
                    collection.Add(new { SubmitId = submit.Id, proc = 100.0*a / b });
                }

                if (collection.Count > 0)
                {
                    collection.Sort((a, b) =>Math.Sign(b.proc - a.proc));


                    var sb = new StringBuilder();
                    foreach (var w in collection)
                        sb.Append("Сравнение с сабмитом Id=").Append(w.SubmitId).Append(", результат=").Append(Math.Round(w.proc, 1)).Append("%\n");

                    s.DiffMax = (float)(collection[0]).proc;
                    s.DiffLog = sb.ToString();
                    s.VerdictLogAdmin = s.DiffLog;
                }
                */
                foreach (var sub in s)
                {
                    sub.VerdictId = 0;// это вердикт "IQ"
                    sub.DateCheck = null;
                    sub.MaxUsedMemory = null;
                    sub.MaxUsedTime = null;
                    sub.NumberFailTest = null;
                    sub.SubmitPenalty = 0;
                    sub.SubmitPoints = 0;
                }
                if (db.SaveChanges() == SubmitIdCollection.Count())
                {
                    ServiceForChecker.ServiceForChecker.HasNewSubmit();
                    return true;
                }
                else
                    return false;
            }
        }

        public bool DeleteSubmit(String SessionKey,IEnumerable<int> SubmitIdCollection)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var s = db.Submits.Where(s2 => SubmitIdCollection.Contains(s2.Id)).ToList();
                if (s.Count != SubmitIdCollection.Count())
                    throw new Exception("Не удалось найти на сервере все сабмиты из переданной коллекции");

                foreach (var sub in s)
                {
                    db.Submits.DeleteObject(sub);
                }

                if (db.SaveChanges() == SubmitIdCollection.Count())
                    return true;
                else
                    return false;
            }
            
        }

    }
}
