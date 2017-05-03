using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFServices.ServiceForAdmin.DTO
{
    public class AdminDTO
    {
        public Int32 ID;
        public String Login;
        public Boolean AllowAccessToAdminPanel;
        public String email;
        public Boolean NotifyNewRegistrations;
    };
}
