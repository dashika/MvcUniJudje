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
        // получить лог юзеров за последние 2 дня. или 200 последних записей
        public IEnumerable<LogForUserDTO> GetLogForUser(String SessionKey)
        {
            using (var db = new DataBase())
            {
                var minTime = DateTime.Now.AddDays(-2);

                var data = db.LogServiceForUsers.Where(d=>d.DateTime>minTime)
                    .Select(e=> new LogForUserDTO{Id = e.Id, DateTime = e.DateTime, Message = e.Message}).ToList();
                if (data.Count <200)
                {
                    int count = db.LogServiceForUsers.Count()-200;
                    if (count < 0) count = 0;

                    data = db.LogServiceForUsers
                    .Select(e => new LogForUserDTO { Id = e.Id, DateTime = e.DateTime, Message = e.Message }).OrderBy(a=>a.DateTime).Take(200).ToList();
                }
                return data;
            }
        }
    }
}