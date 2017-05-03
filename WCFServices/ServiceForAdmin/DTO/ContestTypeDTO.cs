using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFServices.ServiceForAdmin.DTO
{
    public class ContestTypeDTO
    {
        public int Id;
        public string Title;
        public string Note;

        public string FormulaForPoints;         // in extended
        public string FormulaForPenaltyPoints;  // in extended
        public string AvailableVerdicts;    // in extended
        public string NoFailVerdicts;       // in extended
        public bool CheckFirstFail;         // in extended
        public bool ChooseMaximumSubmit;    // in extended
        public string Rulles;               // in extended
    }
}
