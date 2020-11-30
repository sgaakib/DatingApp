using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenInterface
    {
        private readonly SymmetricSecurityKey key;
        public TokenService(IConfiguration config)
        {
            key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId,user.userName)
            };
            var certs = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
            var tokendiscription= new SecurityTokenDescriptor{
                Subject=new ClaimsIdentity(claims),
                Expires=DateTime.Now.AddDays(7),
                SigningCredentials=certs
                
            } ;
            var tokenhander= new JwtSecurityTokenHandler();
            var token = tokenhander.CreateToken(tokendiscription);
            return tokenhander.WriteToken(token);
        }
    }

}