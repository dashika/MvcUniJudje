using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFServices.ServiceForAdmin.DTO
{
    public class ContestExDTO
    {
        public int Id;
        public string Title;
        public string ShortTitle;
        
        public string Note;
        public int ContestTypeId;
        public int? MainContestId_IfVirtual;

        public DateTime DateStart;
        public DateTime? DateEnd;
        public DateTime? DateFrozen;
        public DateTime? DateUnfrozen;
        public bool InvididualDateStart;
      
        public bool Enabled;
        public bool AllowPractice;
        public bool AllowShowAllBoard;
        public bool AllowShowNumberFailTest;
        public bool AllowShowComplexityTask;
        public bool AllowShowSubjectTask;
        public bool AllowShowVirtualUser;
        public bool VirtualUserOutOfCompetition;
        public bool AllowShowTaskForGuest;
        public bool AllowShowBoardForGuest;

        public bool AllowShowOnlyActiveUserInBoard;
        public bool AllowShowTimeInBoard;
        public bool AllowShowPointsInBoard;

        public int? AutoRegisterToGroupId;

        public bool AllowShowTaskAuthor;
        public bool AllowShowTaskSource;
        public bool AllowSwowSubmitsInBoardWhenFrozenTime;

        public string WellcomeText;
        public string SponsorText;

        public IEnumerable<TaskInContestDTO> Tasks;
        public IEnumerable<GroupInContestDTO> Groups;
        public IEnumerable<CompilerDTO> Compilers;
    }

   

}
