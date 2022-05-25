using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using capstoneDotNet.Interfaces;
using capstoneDotNet.Models;
using Microsoft.IdentityModel.Tokens;

namespace capstoneDotNet.Services
{
	public class TokenService: ITokenService
	{
        private readonly IConfiguration _configuration;

        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
        }

        public string CreateToken(UserDetails userdata)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userdata.email)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

