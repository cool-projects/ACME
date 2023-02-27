using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Helpers
{
    public class TokenHelper : ITokenHelper
    {
        private readonly IConfiguration _configuration;
        public TokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public TokenModel GenerateAccessToken(UserModel user)
        {
            var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Sid, Convert.ToString(user.UserId)),
                    new  Claim(ClaimTypes.Name, user.UserName),
                };
            if (user.UserRoles.Count() > 0)
                foreach (var role in user.UserRoles)
                    claims.Add(new Claim(ClaimTypes.Role, role.Role.RoleName));

            var token = new JwtSecurityToken
                (
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                //expires: DateTime.Now.AddMinutes(_configuration.GetValue<int>("TokenExpiryMinutes")),
                expires: DateTime.Now.AddMinutes(30),
                notBefore: DateTime.Now,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                    SecurityAlgorithms.HmacSha256)
                );

            var tokenModel = new TokenModel()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = token.ValidTo,
                Username = user.UserName,
                FullName = $"{user.Name}",
                Email = user.Email,
                UserId = user.UserId,
                Roles = user.UserRoles
            };

            return tokenModel;
        }
        public string GenerateRefreshToken()
        {
            var secureRandomBytes = new byte[32];

            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(secureRandomBytes);

            var refreshToken = Convert.ToBase64String(secureRandomBytes);
            return refreshToken;
        }
    }
}