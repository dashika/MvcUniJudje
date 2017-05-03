using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFServices.ServiceForAdmin.DTO
{
    public class SettingDTO
    {
        public int Id;
        public string KeyName;
        public string Description;
        public int? ValueInt;
        public double? ValueReal;
        public DateTime? ValueDateTime;
        public Guid? ValueGuid;
        public string ValueString;
        public bool? ValueBool;
    }
}
