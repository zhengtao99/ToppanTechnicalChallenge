using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ApiServer.UniversityModels
{
    public class UniversitiesListingParameterModel
    {
        public string Token { get; set; }
        public string SearchFilter { get; set; } = string.Empty;

        public string CountryNameFilter { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }

    public class UniversitiesListingResultsModel
    {
        public List<UniversitiesListingResult> Results { get; set; }
        public int  NumofRecords { get; set; }
        
    }
    public class AuthUniversitiesListingResultsModel
    {
        public List<AuthUniversitiesListingResult> Results { get; set; }
        public int NumofRecords { get; set; }

    }

    public class AuthUniversitiesListingResult : UniversitiesListingResult
    {
        public bool IsBookmarked { get; set; }
        public bool IsActive { get; set; }
    }

    public class UniversitiesListingResult
    {
        public string UniversityName { get; set; }
        public string CountryName { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string LastModified { get; set; }
        public string[] WebPages { get; set; }
     
    }

 
}