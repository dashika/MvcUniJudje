using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace WCFServices.ServiceForAdmin.DTO
{
  
    public class UserDTO
    {

        public int Id;
        public String Name;
        public String Login;
        public String Note;
        public DateTime? BlockedTo;
        public DateTime DateRegistration;

        public IEnumerable<int> ContestCollection;
    }
}
