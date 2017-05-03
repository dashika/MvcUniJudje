using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFServices.ServiceForAdmin.DTO
{
    public class TaskExDTO
    {
        public int Id;
        public String Title;

        //public bool UsePdfStatement;
        public String Statement;
        public bool TaskHasPdfStatement; // если в базе StatementPdf==null - то false, иначе true
        public byte[] StatementPdf;

        public string Note;
        public int TimeLimit_native;
        public int TimeLimit_javavm;
        public int TimeLimit_dotnet;
        public int TimeLimit_custom;
        public int MemoryLimit_native;
        public int MemoryLimit_javavm;
        public int MemoryLimit_dotnet;
        public int MemoryLimit_custom;
        
        public int? OutputLimit;
        public int DefaultOutputLimit;
        
        public int? CodeLimit;
        public int DefaultCodeLimit;

        public String Author;
        public String TaskSource;
        public int TimeSolveProblem;
        public int CountTestToText;
        public bool EnableSend;
        public bool EnableCheck;
        public int? ComplexityId;

        public IEnumerable<TestDTO> Tests;
        public IEnumerable<CheckerDTO> Checkers;
        public IEnumerable<ContestDTO> ContestCollection;
    }
}
