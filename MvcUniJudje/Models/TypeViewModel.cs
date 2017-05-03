using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcUniJudje.Models
{
    public class TypeViewModel 
    {
            public List<User> List { get; set; }
            public int CurrentPage;
            public int pageSize;
            public double TotalPages;
            public int sortBy;
            public bool isAsc;
            public string Search;
            public int isLastRecord;
            public int Count;

    }
}
