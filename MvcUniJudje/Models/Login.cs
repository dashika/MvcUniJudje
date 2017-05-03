using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcUniJudje.Models
{
    public  class Login
    {
       
        private  string login;

        [Required(ErrorMessage = "Пожалуйста, введите логин")]
        public String LoginName { set { login = value; } get { return login; } }
        [Required(ErrorMessage = "Пожалуйста, введите пароль")]
        public String Password { set; get; }
    }
}