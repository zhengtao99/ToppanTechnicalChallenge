using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ApiServer.UniversityModels
{
    public class UpdateUniversityParameterModel
    {
        public string Token { get; set; }

        public string CountryName { get; set; }

        public string UniversityName { get; set; }
        public string Webpages { get; set; }
        public bool IsActive { get; set; }
    }
    public class UpdateUniversityResultsModel
    {
        public string Message { get; set; }
    }
}