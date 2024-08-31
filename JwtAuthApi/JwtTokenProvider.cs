using JwtAuthApi.Db;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

using System.Security.Claims;
using System.Text;

namespace JwtAuthApi
{
    internal class JwtTokenProvider(IConfiguration configuration)
    {
        public string GetToken(User user)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:secret"]));
            var signature = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new("CustomClaim", "CustomValue")
            };

            var subject = new ClaimsIdentity(claims);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                Issuer = configuration["jwt:iss"],
                Audience = configuration["jwt:aud"],
                SigningCredentials = signature,
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("jwt:expirationMinutes")),
            };

            var handler = new JsonWebTokenHandler();

            var token = handler.CreateToken(descriptor);

            return token;
        }
    }
}
