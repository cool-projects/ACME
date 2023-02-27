using Models;

namespace IBusinessServices
{
    public interface ITokenService
    {
        TokenModel GenerateToken(UserModel user);
        ValidateRefreshTokenResponse ValidateRefreshToken(RefreshTokenRequest refreshTokenRequest);
    }
}