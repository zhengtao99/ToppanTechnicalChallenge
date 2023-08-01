using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using System.IdentityModel.Tokens.Jwt;
using ApiServer.LoginModels;
namespace ApiServer.Controllers
{
    public class LoginApiController : ApiController
    {
        [EnableCors(origins: ConfigController.CorsAllowedUrl, headers: "*", methods: "POST")]
        [Route("POST/Login")]
        [HttpPost]
        public IHttpActionResult Login([FromUri] LoginParamaterModel model)
        {
            using (ToppanTechnicalChallengeDBEntities entity = new ToppanTechnicalChallengeDBEntities())
            {
                string hashedPassword = EncryptionController.HashPassword(model.Password);
                var user = entity.Users.Where(z => z.Username == model.Username && z.Password == hashedPassword).FirstOrDefault();
                if(user!=null)
                {
                    var roles = new string[] { "user" };
                    var jwtSecurityToken = AuthenticationController.GenerateJwtToken(user.Username, roles.ToList());
                    var validUsername = AuthenticationController.ValidateToken(jwtSecurityToken);
                    return Ok(new { Success = 
                                     new { Token = jwtSecurityToken } });
                }
                LoginResultsModel loginResultModel = new LoginResultsModel();
                loginResultModel.Message = "Invalid username or password";
                return BadRequest(loginResultModel.Message);
            }
        }


      
    }
}
