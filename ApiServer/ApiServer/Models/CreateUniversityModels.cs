using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ApiServer.UniversityModels
{
    public class CreateUniversityParameterModel
    {
       
        public string Token { get; set; }

        public string CountryName { get; set; }

        public string UniversityName { get; set; }
        public string Webpages { get; set; }
    }
    public class CreateUniversityResultsModel
    {
        public string Message { get; set; }
    }
}