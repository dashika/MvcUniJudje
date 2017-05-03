using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFServices.ServiceForAdmin.DTO
{
    public class TaskDTO
    {
        public int Id;
        public String Title;
        public String Note;
        public bool EnableSend;
        public bool EnableCheck;

        public IEnumerable<ContestDTO> ContestCollection;
    };
}
