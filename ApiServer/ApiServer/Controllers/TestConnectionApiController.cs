using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiServer.Controllers
{
    public class TestConnectionApiController : ApiController
    {
        [EnableCors(origins: ConfigController.CorsAllowedUrl, headers: "*", methods: "Get")]
        [Route("GET/TestConnection")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Json(new { Success = "Successfully connected to the api server." });
        }
    }
}
