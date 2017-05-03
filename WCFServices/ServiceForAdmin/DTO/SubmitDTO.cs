using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFServices.ServiceForAdmin.DTO
{
    public class SubmitDTO
    {
        public int Id;
        
        public int? UserId;
        public string UserName;

        public int TaskId;
        public string TaskTitle;
        public string TaskIdInContest;
        public double? TaskCostInContest;

        public int? ContestId;
        public String ContestTitle;

        
        public int VerdictId;
        public string VerdictShortCode;
        public string VerdictDescription;
        public bool VerdictIsFinal;
      
        public int? MaxUsedMemory;
        public int? MaxUsedTime;
        public int? NumberFailTest;

        
        public DateTime DateTimeSend;
        public int? TimeSendInContest;
        public bool SendInContestTime;

        public double SubmitPoints;
        public double SubmitPenalty;
        
        public int CompilerId;
        public string CompilerFullName;
        public string CompilerShortName;

        public string Note;
    }
}
