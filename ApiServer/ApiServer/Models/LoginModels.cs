using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ApiServer.LoginModels
{
    public class LoginParamaterModel
    {

        public string Username { get; set; }

        public string Password { get; set; }
    }
    public class LoginResultsModel
    {
        public string Message { get; set; }
    }
}