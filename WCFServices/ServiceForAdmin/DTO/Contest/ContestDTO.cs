using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFServices.ServiceForAdmin.DTO
{
    public class ContestDTO
    {
        public int Id;
        public string ShortTitle;
        public string Note;
        public int? MainContestId_IfVirtual;

        public bool Enabled;
    }
}
