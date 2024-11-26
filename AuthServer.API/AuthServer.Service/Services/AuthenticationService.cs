using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> clients;
        private readonly ITokenService tokenService;
        private readonly UserManager<UserApp> userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> userRefreshTokenService;

        public AuthenticationService(IOptions<List<Client>> optionsClient, ITokenService tokenService,UserManager<UserApp>
            userManager,IUnitOfWork unitOfWork,IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            this.tokenService = tokenService;
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
            this.userRefreshTokenService = userRefreshTokenService;
            clients = optionsClient.Value;
        }




        public async Task<Response<TokenDTO>> CreateTokenAsync(LoginDto loginDto)
        {
            if(loginDto == null) throw new ArgumentNullException(nameof(loginDto));

            var user = await userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return Response<TokenDTO>.Fail("Email veya Password yanlış.", 400, true);

            if ( !await userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Response<TokenDTO>.Fail("Email veya Password yanlış.", 400, true);
            }

            var token = tokenService.CreateToken(user);

            var userRefreshToken = await userRefreshTokenService.Where(x=> x.UserId== user.Id).SingleOrDefaultAsync();

            if(userRefreshToken == null)
            {
                await userRefreshTokenService.AddAsync(new UserRefreshToken
                {
                    UserId = user.Id,
                    Code = token.RefreshToken,
                    Expiration = token.RefreshTokenExpiration
                });
            }
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }

            await unitOfWork.SaveAsync();

            return Response<TokenDTO>.Success(token, 200);


        }

        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var client = clients.SingleOrDefault(x => x.Id == clientLoginDto.ClientId &&
            x.Secret == clientLoginDto.ClientSecret);

            if(client == null)
            {
                return Response<ClientTokenDto>.Fail("ClientId veya Secret Not Found",404,true);
            }

            var token = tokenService.CreateTokenByClient(client);

            return Response<ClientTokenDto>.Success(token, 200);
        }

        public async Task<Response<TokenDTO>> CreateTokenByRefreshToken(string refreshToken)
        {
            var existRefreshToken = await userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();

            if(existRefreshToken == null)
            {
                return Response<TokenDTO>.Fail("RefreshToken Not Found", 404, true);
            }

            var user = await userManager.FindByIdAsync(existRefreshToken.UserId);

            if(user == null) 
            {
                return Response<TokenDTO>.Fail("UserId Not Found", 404, true);
            }

            var tokenDto = tokenService.CreateToken(user);

            existRefreshToken.Code = tokenDto.RefreshToken;
            existRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;

            await unitOfWork.SaveAsync();

            return Response<TokenDTO>.Success(tokenDto, 200);
        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
          var ExistRefreshToken = await userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
          if(ExistRefreshToken == null) 
            {
                return Response<NoDataDto>.Fail("RefreshToken Not Found", 404, true);
            }

            userRefreshTokenService.Remove(ExistRefreshToken);
            await unitOfWork.SaveAsync();

            return Response<NoDataDto>.Success(200);
        }
    }
}
