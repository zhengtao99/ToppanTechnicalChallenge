using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ApiServer.Controllers
{
    public class AuthenticationController
    {
        // Generate token
        public static string GenerateJwtToken(string username, List<string> roles)
        {
            var claims = new List<Claim>
            {

                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, username)
            };
            roles.ForEach(role =>
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            });


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"])));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(Convert.ToDouble(Convert.ToString(ConfigurationManager.AppSettings["config:JwtExpireDays"])));
            var token = new JwtSecurityToken(
                    Convert.ToString(ConfigurationManager.AppSettings["config:JwtIssuer"]),
                    Convert.ToString(ConfigurationManager.AppSettings["config:JwtAudience"]),
                    claims,
                    notBefore: DateTime.UtcNow,
                    expires: expires,
                    signingCredentials: creds
             );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }






        // Validate the token
        public static object ValidateToken(string token)
        {
            string error = "Unauthorized user.";
            if (token == null)
            {
                return new { Error = error};
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"]));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var jti = jwtToken.Claims.First(claim => claim.Type == "jti").Value;
                var username = jwtToken.Claims.First(sub => sub.Type == "sub").Value;

                // return user id from JWT token if validation successful return userName;
                return new { username = username };
            }
            catch
            {
                // return null if validation fails
                return new { Error = error };
            }               
            
        }
       
    }
}