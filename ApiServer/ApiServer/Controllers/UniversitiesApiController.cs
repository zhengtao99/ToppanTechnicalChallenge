using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using ApiServer.UniversityModels;
using Newtonsoft.Json;

namespace ApiServer.Controllers
{
    public class UniversitiesApiController : ApiController
    {
        [EnableCors(origins: ConfigController.CorsAllowedUrl, headers: "*", methods: "GET")]
        [Route("GET/university")]
        [HttpGet]
        public IHttpActionResult GetUniversityListing([FromUri] UniversitiesListingParameterModel model)
        {
            using (ToppanTechnicalChallengeDBEntities entities = new ToppanTechnicalChallengeDBEntities())
            {
                // Get all active universities
                var filtered = entities.Universities.Where(z => z.IsActive == true);

                // Filtered all not deleted
                filtered = filtered.Where(z => z.DeletedAt == null);

                if (model != null)
                {
                    //Filter by search
                    string searchFilter = model.SearchFilter == null ? "": model.SearchFilter.ToLower();
                    filtered = filtered.Where(z => z.Name.ToLower().Contains(searchFilter));

                    //Filter by country
                    if (model.CountryNameFilter != null)
                    {
                        filtered = filtered.Where(z => z.Name.Contains(model.CountryNameFilter));
                    }
                }

                string token = model?.Token;
                Object authObj = AuthenticationController.ValidateToken(token);

                //Authenticated
                if (authObj.GetType().GetProperty("Error") == null)
                {
                    var authResultsModel = new AuthUniversitiesListingResultsModel();
                    authResultsModel.Results = new List<AuthUniversitiesListingResult>();
                    
                    string username = ((dynamic)authObj).username;
                    var userInactiveUniversities = entities.Users.Where(z => z.Username == username).First()
                                                                .UsersUniversities.Join(entities.Universities, z=> z.UniversityId, y => y.Id, (z, y) => y )
                                                                .Where(z => z.IsActive == false).AsQueryable();
                   
                    AddToAuthUniversitiesListingResults(authResultsModel, filtered);

                    // Display universities by user even when inactive
                    AddToAuthUniversitiesListingResults(authResultsModel, userInactiveUniversities);

                    // Display total number of records
                    authResultsModel.NumofRecords = authResultsModel.Results.Count;

                    // Display bookmark at the top
                    var bookmarkedTop = authResultsModel.Results.OrderByDescending(z => z.IsBookmarked);

                    // Pagination
                    authResultsModel.Results = bookmarkedTop.Skip(model.PageSize * (model.PageNumber - 1))
                                                                .Take(model.PageSize).ToList();
                    return Ok(authResultsModel);
                }

                //Not authenticated
                else
                {
                    var resultsModel = new UniversitiesListingResultsModel();
                    resultsModel.Results = new List<UniversitiesListingResult>();

                    AddToUniversitiesListingResults(resultsModel, filtered);

                    // Display total number of records
                    resultsModel.NumofRecords = resultsModel.Results.Count;

                    // Pagination
                    resultsModel.Results = resultsModel.Results.Skip(model.PageSize * (model.PageNumber - 1))
                                                                .Take(model.PageSize).ToList();
                    return Ok(resultsModel);
                }
            }
        }

        [EnableCors(origins: ConfigController.CorsAllowedUrl, headers: "*", methods: "POST")]
        [Route("POST/university")]
        [HttpPost]
        public IHttpActionResult CreateUniversity([FromUri] CreateUniversityParameterModel model)
        {
            string inputError = null;
            inputError = ValidateUniversityName(model.UniversityName);
            if (inputError != null) return BadRequest(inputError);

            inputError = ValidateWebpages(model.Webpages);
            if (inputError != null) return BadRequest(inputError);

            inputError = ValidateCountryName(model.CountryName);
            if (inputError != null) return BadRequest(inputError);

            CreateUniversityResultsModel result = new CreateUniversityResultsModel();
            if (inputError != null)
            {
                result.Message = inputError;
                return BadRequest(result.Message);
            }
            string token = model?.Token;
            Object authObj = AuthenticationController.ValidateToken(token);

            if (authObj.GetType().GetProperty("Error") != null)
            {
                string error = ((dynamic)authObj).Error;
                return BadRequest(result.Message);
            }

            using (ToppanTechnicalChallengeDBEntities entities = new ToppanTechnicalChallengeDBEntities())
            {
                try
                {
                    string username = ((dynamic)authObj).username;
                    var user = entities.Users.Where(z => z.Username == username).First();
                    var country = entities.Countries.Where(z => z.Name.Contains(model.CountryName)).First();
                    entities.SP_CreateUniversity(user.Id, country.Id, model.Webpages, model.UniversityName, DateTime.UtcNow);
                    result.Message = "Successfully created a university.";
                    return Ok(result.Message);
                }
                catch
                {
                    result.Message = "Unable to create university at the moment, pls try again.";
                    return BadRequest(result.Message);
                }

            }
        }

        [EnableCors(origins: ConfigController.CorsAllowedUrl, headers: "*", methods: "GET")]
        [Route("GET/university/{Id}")]
        [HttpGet]
        public IHttpActionResult GetUniversityById(int id, [FromBody] UniversityByIdParameterModel parameterModel)
        {
            // No validation needed because anyone is able to access this details page.

            using (ToppanTechnicalChallengeDBEntities entities = new ToppanTechnicalChallengeDBEntities())
            {
                // Validate if university exists
                var universityObj = ValidateUniversityExists(id);
                if (universityObj.GetType().GetProperty("Error") != null)
                    return BadRequest(((dynamic)universityObj).Error);

                Object authObj = AuthenticationController.ValidateToken(parameterModel.Token);
                //AAAAAAAAAAAAAAAAAAAAAAAA
                string username = "";
                if (authObj.GetType().GetProperty("Error") == null)
                    username = ((dynamic)authObj).username;
                SP_GetUniversityById_Result university = entities.SP_GetUniversityById(id, username).First();                

                if(!university.IsActive && username != university.Username)
                {
                    return BadRequest("University is inactive.");
                }

                //Authenticated
                if (authObj.GetType().GetProperty("Error") == null)
                {
                    AuthUniversitiesListingResult authResult = new AuthUniversitiesListingResult();
                    authResult.UniversityName = university.Name;
                    authResult.CountryName = university.CountryName;
                    authResult.CreatedAt = TimeDescription((DateTime)university.CreatedAt);
                    authResult.CreatedBy = university.Username;
                    authResult.LastModified = university.LastModified == null ? "" : TimeDescription((DateTime)university.LastModified);
                    authResult.WebPages = university.Webpages?.Split(',');
                    // Display the bookmark and isActive tied to the user
                    authResult.IsBookmarked = university.IsBookmarked;

                    authResult.IsActive = university.IsActive;

                    return Ok(authResult);
                }
                else
                {
                    UniversitiesListingResult result = new UniversitiesListingResult();
                    result.UniversityName = university.Name;
                    result.CountryName = university.CountryName;
                    result.CreatedAt = TimeDescription((DateTime)university.CreatedAt);
                    result.CreatedBy = university.Username;
                    result.LastModified = university.LastModified == null ? "" : TimeDescription((DateTime)university.LastModified);
                    result.WebPages = university.Webpages?.Split(',');

                    return Ok(result);
                }
            }
        }

        [EnableCors(origins: ConfigController.CorsAllowedUrl, headers: "*", methods: "PUT")]
        [Route("PUT/university/{Id}")]
        [HttpPut]
        public IHttpActionResult UpdateUniversity(int id, [FromBody] UpdateUniversityParameterModel parameterModel)
        {
            // If there is no parameter 
            if (parameterModel == null)
                return BadRequest(ValidateUniversityName(null));

            if (parameterModel.UniversityName == null)
                return BadRequest(ValidateUniversityName(null));

            if (parameterModel.Webpages == null)
                return BadRequest(ValidateWebpages(null));

            if (parameterModel.CountryName == null)
                return BadRequest(ValidateCountryName(null));

            // Validate Authenticated User
            var authObj = AuthenticationController.ValidateToken(parameterModel.Token);
            if (authObj.GetType().GetProperty("Error") != null)
            {
                string error = ((dynamic)authObj).Error;
                return BadRequest(error);
            }
            using(ToppanTechnicalChallengeDBEntities entities = new ToppanTechnicalChallengeDBEntities())
            {
                // Validate if university exists
                var universityObj = ValidateUniversityExists(id);
                if (universityObj.GetType().GetProperty("Error") != null)
                    return BadRequest(((dynamic)universityObj).Error);

                // Verify if user is the creator of that university
                string username = ((dynamic)authObj).username;
                var user = entities.Users.Where(z => z.Username == username).FirstOrDefault();
                var creatorId = entities.Universities.Where(z => z.Id == id).First().UsersUniversity.UserId;
                University university = entities.Universities.Where(z => z.Id == id).FirstOrDefault();
                if (user.Id != creatorId)
                {
                    return BadRequest(string.Format("User is not an editor for the university. ({0})", university.Name));
                }

                university.Name = parameterModel.UniversityName;
                university.CountryId = entities.Countries.Where(z => z.Name.Contains(parameterModel.CountryName)).FirstOrDefault().Id;
                university.Webpages = parameterModel.Webpages;
                university.IsActive = parameterModel.IsActive;
                university.LastModified = DateTime.UtcNow;
                entities.SaveChanges();

                UpdateUniversityResultsModel resultModel = new UpdateUniversityResultsModel();
                resultModel.Message = string.Format("Successfully updated the university. ({0})", university.Name);
                return Ok(resultModel.Message);
            }
        }

        [EnableCors(origins: ConfigController.CorsAllowedUrl, headers: "*", methods: "DELETE")]
        [Route("DELETE/university/{Id}")]
        [HttpDelete]
        public IHttpActionResult DeleteUniversity(int id, [FromBody] DeleteUniversityParameterModel parameterModel)
        {
            // If there is no token parameter
            Object authObj = null;
            if (parameterModel == null)
            {
                authObj = AuthenticationController.ValidateToken(null);
                return BadRequest(((dynamic)authObj).Error);
            }

            // Validate Authenticated User
            authObj = AuthenticationController.ValidateToken(parameterModel.Token);
            if (authObj.GetType().GetProperty("Error") != null)
            {
                string error = ((dynamic)authObj).Error;
                return BadRequest(error);
            }

            // Validate if university exists
            var universityObj = ValidateUniversityExists(id);
            if (universityObj.GetType().GetProperty("Error") != null)
                return BadRequest(((dynamic)universityObj).Error);

            // Verify if user is the creator of that university
            using (ToppanTechnicalChallengeDBEntities entities = new ToppanTechnicalChallengeDBEntities())
            {
                

                string username = ((dynamic)authObj).username;
                var user = entities.Users.Where(z => z.Username == username).FirstOrDefault();
                var university = entities.Universities.Where(z => z.Id == id).First();
                var creatorId = university.UsersUniversity.UserId;
                if (user.Id != creatorId)
                {
                    return BadRequest(string.Format("User is not an editor for the university. ({0})", university.Name));
                }

                university.DeletedAt = DateTime.UtcNow;
                entities.SaveChanges();

                DeleteUniversityResultsModel resultModel = new DeleteUniversityResultsModel();
                resultModel.Message = string.Format("Successfully deleted the university. ({0})", university.Name);
                return Ok(resultModel.Message);
            }
        }


        [EnableCors(origins: ConfigController.CorsAllowedUrl, headers: "*", methods: "POST")]
        [Route("POST/university/bookmark/{Id}")]
        [HttpPost]
        public IHttpActionResult CreateUniversityBookmark(int id, [FromBody] CreateUniversityBookmarkParameterModel parameterModel)
        {
            UpdateUniversityResultsModel resultModel = new UpdateUniversityResultsModel();

            // If there is no parameter 
            if(parameterModel == null)
                return BadRequest(ValidateUniversityBookmark(null));

            if (parameterModel.BookmarkDescription == null)
                return BadRequest(ValidateUniversityBookmark(null));

            // Returns error if no token found
            var authObj = AuthenticationController.ValidateToken(parameterModel.Token);
            if (authObj.GetType().GetProperty("Error") != null)
            {
                string error = ((dynamic)authObj).Error;
                return BadRequest(error);
            }

            using (ToppanTechnicalChallengeDBEntities entities = new ToppanTechnicalChallengeDBEntities())
            {
                // Validate if university exists
                var universityObj = ValidateUniversityExists(id);
                if (universityObj.GetType().GetProperty("Error") != null)
                    return BadRequest(((dynamic)universityObj).Error);

                // Verify if user is the creator of that university
                string username = ((dynamic)authObj).username;
                var user = entities.Users.Where(z => z.Username == username).FirstOrDefault();
                University university = entities.Universities.Where(z => z.Id == id).First();
                var creatorId = university.UsersUniversity.UserId;
               
                if (user.Id != creatorId)
                {
                    return BadRequest(string.Format("User is not an editor for the university. ({0})", university.Name));
                }

                var universityBookmark = entities.UniversityBookmarks.Where(z => z.UniversityId == university.Id).FirstOrDefault();

                // if university doesnt have bookmark
                if (universityBookmark == null)
                {
                    universityBookmark = new UniversityBookmark();
                    universityBookmark.UniversityId = university.Id;
                    
                    entities.UniversityBookmarks.Add(universityBookmark);
                }
                
                universityBookmark.Description = parameterModel.BookmarkDescription;
                
                university.IsBookmarked = true;

                entities.SaveChanges();

                resultModel.Message = string.Format("Successfully bookmarked the university. ({0})", university.Name);
                return Ok(resultModel.Message);
            }
        }



        private void AddToUniversitiesListingResults(dynamic model, IQueryable<University> filtered)
        {
            foreach (var university in filtered)
            {
                UniversitiesListingResult result = new UniversitiesListingResult();
                result.UniversityName = university.Name;
                result.CountryName = university.Country.Name;
                result.CreatedAt = TimeDescription(university.CreatedAt);
                result.CreatedBy = university.UsersUniversity.User.Username;
                result.LastModified = university.LastModified == null ? "" : TimeDescription((DateTime)university.LastModified);
                result.WebPages = university.Webpages?.Split(',');

                model.Results.Add(result);
            }
        }

        private void AddToAuthUniversitiesListingResults(AuthUniversitiesListingResultsModel model, IQueryable<University> filtered)
        {
            foreach (var university in filtered)
            {
                AuthUniversitiesListingResult authResult = new AuthUniversitiesListingResult();
                authResult.UniversityName = university.Name;
                authResult.CountryName = university.Country.Name;
                authResult.CreatedAt = TimeDescription(university.CreatedAt);
                authResult.CreatedBy = university.UsersUniversity.User.Username;
                authResult.LastModified = university.LastModified == null ? "" : TimeDescription((DateTime)university.LastModified);
                authResult.WebPages = university.Webpages?.Split(',');

                // Display the bookmark and isActive tied to the user
                authResult.IsBookmarked = university.IsBookmarked;
                authResult.IsActive = university.IsActive;
                model.Results.Add(authResult);
            }
        }

        private Object ValidateUniversityExists(int id)
        {
            using (ToppanTechnicalChallengeDBEntities entities = new ToppanTechnicalChallengeDBEntities())
            {
                var university = entities.Universities.Where(z => z.Id == id).FirstOrDefault();
                if (university == null)
                {
                    return new { Error = "The university id doesnt exist." };
                }
                if(university.DeletedAt != null)
                {
                    return new { Error = "The university has been deleted." };
                }
                return university;
            }
            
        }

        private string ValidateUniversityBookmark(string name)
        {
            if (name == null)
            {
                return "Please enter the bookmark description of the university.";
            }
            return null;
        }

        private string ValidateUniversityName(string name)
        {
            if (name == null)
            {
                return "Please enter the name of the university.";
            }
            return null;
        }
        private string ValidateWebpages(string name)
        {
            if (name == null)
            {
                return "Please enter the webpages of the university.";
            }
            return null;
        }
        private string ValidateCountryName(string name)
        {
            if (name == null)
            {
                return "Please enter the country of the university";
            }

            using (ToppanTechnicalChallengeDBEntities entities = new ToppanTechnicalChallengeDBEntities())
            {
                var country = entities.Countries.Where(z => z.Name.Contains(name)).FirstOrDefault();
                if (country == null)
                {
                    return "Unable to find the country";
                }
            }
            return null;
        }

        private string TimeDescription(DateTime targetDate)
        {

            TimeSpan ts = DateTime.Now.ToUniversalTime() - targetDate;


            if (ts.Minutes < 1)
            {
                return string.Format(ts.Seconds + " seconds ago");
            }
            if (ts.Hours < 1)
            {
                return string.Format(ts.Minutes + " mins ago");
            }
            if (ts.Days < 1)
            {
                return string.Format(ts.Hours + " hours ago");
            }

            return string.Format(ts.Days + " days ago");
        }
    }
}
