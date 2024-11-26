using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibary.Configurations;
using SharedLibary.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> userManager;
        private readonly CustomTokenOptions tokenOptions;

        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOptions> options  )
        {
            this.userManager = userManager;
            tokenOptions = options.Value;
        }

        private string CreateRefreshToken()
        {
            var numberByte = new byte[32];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);
        }

        private IEnumerable<Claim> GetClaims(UserApp userApp, List<String> audiences)// üyelik sistemi token 
        {
            var userList = new List<Claim>{

                new Claim(ClaimTypes.NameIdentifier,userApp.Id),
                new Claim(JwtRegisteredClaimNames.Email,userApp.Email),
                new Claim(ClaimTypes.Name,userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()) // jwt id

            };

            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud,x))); //  foreach de yapılabilir

            return userList;
        }

        private IEnumerable<Claim> GetClaimsByClient(Client client)//  üyelik sistemi gerektirmeyen token
        {
            var claims = new List<Claim>();

            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));

            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()); // jwt id
            new Claim(JwtRegisteredClaimNames.Sub,client.Id.ToString()); // kimin için oluşturuyoruz sub

            return claims;
        }


        public TokenDTO CreateToken(UserApp userApp)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(tokenOptions.AccesTokenExpiration);

            var refreshTokenExpiration = DateTime.Now.AddMinutes(tokenOptions.RefreshTokenExpiration);

            var securityKey = SignService.GetSymemetricSecurityKey(tokenOptions.SecurityKey);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey,SecurityAlgorithms
                .HmacSha256Signature); // tokenımız imzalamak için kullandığımız kod

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: tokenOptions.Issuer, // yayınlayan kim
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims:GetClaims(userApp,tokenOptions.Audience),
                signingCredentials:signingCredentials);

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken);

            var tokenDto = new TokenDTO
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccesTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration,
            };

            return tokenDto;
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(tokenOptions.AccesTokenExpiration);

            var securityKey = SignService.GetSymemetricSecurityKey(tokenOptions.SecurityKey);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms
                .HmacSha256Signature); // tokenımız imzalamak için kullandığımız kod

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: tokenOptions.Issuer, // yayınlayan kim
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaimsByClient(client),
                signingCredentials: signingCredentials);

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken);

            var clientTokenDto = new ClientTokenDto
            {
                AccessToken = token,
                AccesTokenExpiration = accessTokenExpiration,
            };

            return clientTokenDto;
        }
    }
}
