using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Server.Controllers
{
    public class OAuthController : Controller
    {
        [HttpGet]
        public IActionResult Authorize(
            string client_id,
            string response_type,
            string redirect_uri,
            string scope,
            string state
            )
        {
            var query = new QueryBuilder();
            query.Add("redirect_uri", redirect_uri);
            query.Add("state", state);

            return View(model: query.ToString());
        }

        [HttpPost]
        public IActionResult Authorize(
            string username,
            string client_id,
            string response_type,
            string redirect_uri,
            string scope,
            string state
            )
        {
            const string code = "BABABABABABABAB";
            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", state);
            return Redirect($"{redirect_uri}{query.ToString()}");
        }

        public async Task<IActionResult> Token(
            string grant_type,
            string code,
            string redirect_uri,
            string client_id
            )
        {

            //some mechanism for validating the code

            var claims = new[]
           {
                new Claim(JwtRegisteredClaimNames.Sub,"some_id"),
                new Claim("granny","cookie")
            };
            var secretBytes = Encoding.UTF8.GetBytes(Constans.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;
            var signingCredentials = new SigningCredentials(key, algorithm);
            var token = new JwtSecurityToken(Constans.Issuer, Constans.Audiance, claims, notBefore: DateTime.Now, expires: DateTime.Now.AddHours(1), signingCredentials);
            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var responseObject = new
            {
                access_token,
                token_type = "Bearer",
                raw_claim = "oauthTutorial"
            };

            var responseJson = JsonSerializer.Serialize(responseObject);
            var responseBytes = Encoding.UTF8.GetBytes(responseJson);
            await Response.Body.WriteAsync(responseBytes,0,responseBytes.Length);
            //return Ok(new { access_token = tokenJson });


            return Redirect(redirect_uri);
        }
    }
}