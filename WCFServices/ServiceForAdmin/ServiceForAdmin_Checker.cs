using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DataModel;
using WCFServices.ServiceForAdmin;
using WCFServices.ServiceForAdmin.DTO;

namespace WCFServices.ServiceForAdmin
{
    public partial class ServiceForAdmin : IServiceForAdmin
    {
        // получить чекер
        public CheckerDTO GetChecker(String SessionKey, int CheckerId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var ch = db.Checkers.Where(t => t.Id == CheckerId).Single();
                return new CheckerDTO
                {
                    Id = ch.Id,
                    Code = ch.Code,
                    Note = ch.Note,
                    CompilerId = ch.CompilerId,
                    Enabled = ch.Enabled
                };
            }
        }

        // создать чекер
        public CheckerDTO CreateChecker(String SessionKey, int TaskId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var ch = new Checker();
                var task = db.Tasks.Where(t => t.Id == TaskId).Single();
                
                ch.Code = "";
                ch.Note = "";
                ch.CompilerId = db.Compilers.First().Id;
                ch.Enabled = false;
                ch.TaskId = TaskId;

                db.Checkers.AddObject(ch);
                db.SaveChanges();

                return GetChecker(SessionKey, ch.Id);
            }
        }

        // обновить чекер
        public void UpdateChecker(String SessionKey, CheckerDTO Checker)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var ch = db.Checkers.Where(t => t.Id == Checker.Id).Single();
                ch.Code = Checker.Code;
                ch.Note = Checker.Note;
                ch.CompilerId = Checker.CompilerId;
                ch.Enabled = Checker.Enabled;
                
                db.SaveChanges();
            }

        }

        // удалить чекер
        public void DeleteChecker(String SessionKey, int CheckerId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var ch = db.Checkers.Where(t => t.Id == CheckerId).Single();

                db.DeleteObject(ch);
                db.SaveChanges();
                return;
            }
        }

    }
}
