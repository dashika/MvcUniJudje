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
        public IEnumerable<ContestTypeDTO> GetContestTypes(String SessionKey)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                return (from s in db.ContestTypes
                        select new ContestTypeDTO
                        {
                            Id = s.Id,
                            Title = s.Title,
                            Note = s.Note
                        }).ToList();
            }

        }
    }
}
