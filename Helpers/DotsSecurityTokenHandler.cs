using DotsApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DotsApi.Helpers
{
    public sealed class DotsSecurityTokenHandler : IDotsSecurityTokenHandler
    {
        private SymmetricSecurityKey _key;
        private SigningCredentials _signingCredentials;
        private JwtSecurityTokenHandler _tokenHandler;
        private DateTime _expires;

        public DotsSecurityTokenHandler(IOptions<AppSettings> appSettings)
        {
            _key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.Value.Secret));
            _signingCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);
            _tokenHandler = new JwtSecurityTokenHandler();
            _expires = DateTime.UtcNow.AddDays(1);
        }

        public string CreateToken(User user)
        {
            IEnumerable<Claim> claims = new List<Claim> 
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id)
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = _expires,
                SigningCredentials = _signingCredentials
            };
            SecurityToken token = _tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = _tokenHandler.WriteToken(token);

            return tokenString;
        }

        public SecurityToken ValidateToken(string token) //never used yet, added just in case
        {
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateLifetime = true,
                ValidateIssuer = false,
                ValidateAudience = false
            };

            _tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            return validatedToken;
        }
    }
}
