using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFServices.ServiceForAdmin.DTO
{
    public class CompilerDTO
    {
       public int Id;
       public String ShortName;
       public String FullName;
       public String CompilerType;
       public String CompilePath;
       public String RunPath;
       public bool Enabled;
       public String FileNameSource;
       public String FileNameTarget;
       public String Note;
       public String Extension;
       public String TestCode;

       
    }
}
