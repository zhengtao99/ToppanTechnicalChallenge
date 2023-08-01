using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ApiServer.UniversityModels
{
    public class DeleteUniversityParameterModel
    {
        public string Token { get; set; }
    }
    public class DeleteUniversityResultsModel
    {
        public string Message { get; set; }
    }
}