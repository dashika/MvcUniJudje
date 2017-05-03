
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcUniJudje.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public DateTime ? Date { get; set; }
        public Boolean Locking { get; set; }
        public DateTime DateRegistration { get; set; }
    }


}