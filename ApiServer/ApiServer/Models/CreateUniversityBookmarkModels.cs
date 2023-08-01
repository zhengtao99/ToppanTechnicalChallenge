using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ApiServer.UniversityModels
{
    public class CreateUniversityBookmarkParameterModel
    {
        public string Token { get; set; }

        public string BookmarkDescription { get; set; }
    }
    public class CreateUniversityBookmarkResultsModel
    {
        public string Message { get; set; }
    }
}