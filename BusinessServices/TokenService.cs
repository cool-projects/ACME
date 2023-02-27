using Acme.Data;
using Helpers;
using IBusinessServices;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessServices
{
    public class TokenService : ITokenService
    {
        private readonly ITokenHelper _tokenHelper;
        private readonly AcmeDBContext db;

        public TokenService(ITokenHelper tokenHelper, AcmeDBContext dataContext)
        {
            _tokenHelper = tokenHelper;
            db = dataContext;
        }
        public TokenModel GenerateToken(UserModel user)
        {

            var accessToken = _tokenHelper.GenerateAccessToken(user);
            var refreshToken = _tokenHelper.GenerateRefreshToken();
            accessToken.RefreshToken = refreshToken;
            accessToken.Success = true;

            var salt = PasswordHelper.GetSecureSalt();

            var refreshTokenHashed = PasswordHelper.HashUsingPbkdf2(refreshToken, salt);

            var refreshTokenRecords = db.RefreshTokens.Where(x => x.UserId == user.UserId).ToList();

            if (refreshTokenRecords.Count > 0) //update
            {
                var refreshTokenRecord = refreshTokenRecords.FirstOrDefault();
                refreshTokenRecord.ExpiryDate = DateTime.Now.AddDays(30);
                refreshTokenRecord.CreatedDate = DateTime.Now;
                refreshTokenRecord.UserId = user.UserId;
                refreshTokenRecord.TokenHash = refreshTokenHashed;
                refreshTokenRecord.TokenSalt = Convert.ToBase64String(salt);

                db.SaveChanges();
            }
            else
            {
                var newRefreshToken = new RefreshToken() // add new
                {
                    ExpiryDate = DateTime.Now.AddDays(30),
                    CreatedDate = DateTime.Now,
                    UserId = user.UserId,
                    TokenHash = refreshTokenHashed,
                    TokenSalt = Convert.ToBase64String(salt)
                };

                db.RefreshTokens.Add(newRefreshToken);
                db.SaveChanges();
            }

            return accessToken;
        }

        public bool RemoveRefreshToken(RefreshToken refreshToken)
        {

            if (refreshToken == null)
            {
                return false;
            }

            if (refreshToken != null)
            {
                db.Entry(refreshToken).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                db.SaveChanges();
            }

            return false;
        }

        public ValidateRefreshTokenResponse ValidateRefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var refreshToken = db.RefreshTokens.Where(o => o.UserId == refreshTokenRequest.UserId).FirstOrDefault();

            var response = new ValidateRefreshTokenResponse();
            if (refreshToken == null)
            {
                response.Success = false;
                response.Errors = new List<string>() { "Invalid session or user is already logged out" };
                response.ResponseCode = 1;
                return response;
            }

            var refreshTokenToValidateHash = PasswordHelper.HashUsingPbkdf2(refreshTokenRequest.RefreshToken, Convert.FromBase64String(refreshToken.TokenSalt));

            if (refreshToken.TokenHash != refreshTokenToValidateHash)
            {
                response.Success = false;
                response.Errors = new List<string>() { "Invalid refresh token" };
                response.ResponseCode = 2;
                return response;
            }

            if (refreshToken.ExpiryDate < DateTime.Now)
            {
                response.Success = false;
                response.Errors = new List<string>() { "Refresh token has expired" };
                response.ResponseCode = 3;
                return response;
            }

            response.Success = true;
            response.UserId = refreshToken.UserId;

            return response;
        }
    }
}
