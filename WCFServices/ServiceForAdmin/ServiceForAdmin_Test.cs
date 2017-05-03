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

        // получить тест
        public TestDTO GetTest(String SessionKey, int TestId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var t = db.Tests.Where(q => q.Id == TestId).Single();

                return new TestDTO
                {
                    Id = t.Id,
                    Enabled = t.Enabled,
                    InputData = t.InputData,
                    Multitest = t.Multitest,
                    Number = t.Number,
                    PatternData = t.PatternData
                };
            }
        }

        // создать тест
        public TestDTO CreateTest(String SessionKey, int TaskId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var test = new Test();
                var task = db.Tasks.Where(t => t.Id == TaskId).Single();

                // установим все значения по умолчанию
                test.Enabled = false;
                test.InputData = "";
                test.PatternData = "";
                test.Multitest = false;
                test.Number = 0;
                test.TaskId = TaskId;
                
                db.Tests.AddObject(test);
                db.SaveChanges();

                return GetTest(SessionKey,test.Id);
            }
        }

        // обновить тест
        public void UpdateTest(String SessionKey, TestDTO Test)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var t = db.Tests.Where(q => q.Id == Test.Id).Single();

                t.InputData = Test.InputData;
                t.PatternData = Test.PatternData;
                t.Multitest = Test.Multitest;
                t.Number = Test.Number;
                t.Enabled = Test.Enabled;

                db.SaveChanges();
            }
        }

        // удалить тест
        public void DeleteTest(String SessionKey, int TestId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var test = db.Tests.Where(t => t.Id == TestId).Single();

                db.DeleteObject(test);
                db.SaveChanges();
                return;
            }
        }
    }
}
