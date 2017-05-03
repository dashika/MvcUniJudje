using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace WCFServices.ServiceForAdmin.DTO
{


    public class UserExDTO
    {

      public int Id;
      public String Login;
      public String Name;
      public String Institution;
      public String Phone;
      public String Email;
      public Boolean SendNotifications;
      public Boolean EmailConfirmed;
      public DateTime RegistrationDate;
      public DateTime? LastActivityDateTime;
      public int LoginFailCount;
      public DateTime? BlockedTo;
      public String Note;
      public String SocialNetworks;
      public String ClassCourseGroup;
      public String Address;
        /*
      public Guid CancelNotificationKey;
      public Guid? EmailConfirmKey;
      public Guid? SessionKey;
      public Guid? RestoreKey;
        */
        public IEnumerable<GroupDTO> Groups;
        
        public IEnumerable<int> ContestCollection;

    }
}
