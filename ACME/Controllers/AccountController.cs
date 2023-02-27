using Helpers;
using IBusinessServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AcmeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _token;
        private readonly ILoggingHelper _logging;
        public AccountController(IAccountService accountService, ILoggingHelper loggingHelper, ITokenService token)
        {
            _accountService = accountService;
            _logging = loggingHelper;
            _token = token;
        }

        [AllowAnonymous]
        [HttpPost("token")]
        public ActionResult<BaseResponse> Login(UserLoginModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(user);
            }

            var result = _accountService.Login(user);
            if (result is null || !result.Success)
                return new BaseResponse()
                {
                    Success = false,
                    Errors = result.Errors
                };

            var tokenResponse = _token.GenerateToken(result);

            return Ok(tokenResponse);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("refreshtoken")]
        public ActionResult<TokenModel> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var requestId = _logging.RequestLogger(JsonSerializer.Serialize(refreshTokenRequest), null, RequestTypes.RefreshToken, 0);
            if (!ModelState.IsValid)
            {
                _logging.RequestLogger(null, JsonSerializer.Serialize(ModelState), RequestTypes.RefreshToken, requestId);
                return BadRequest(refreshTokenRequest);
            }

            var validateRefreshTokenResponse = _token.ValidateRefreshToken(refreshTokenRequest);

            if (!validateRefreshTokenResponse.Success)
            {
                _logging.RequestLogger(null, JsonSerializer.Serialize(validateRefreshTokenResponse), RequestTypes.RefreshToken, requestId);
                return BadRequest(validateRefreshTokenResponse);
                //return UnprocessableEntity(validateRefreshTokenResponse);
            }
            else
            {
                var user = _accountService.GetUserDetailsByUserId(refreshTokenRequest.UserId);
                if (user == null)
                {
                    _logging.RequestLogger(null, JsonSerializer.Serialize(validateRefreshTokenResponse), RequestTypes.RefreshToken, requestId);
                    return BadRequest("User not found.");
                }

                var tokenResponse = _token.GenerateToken(user);
                _logging.RequestLogger(null, JsonSerializer.Serialize(tokenResponse), RequestTypes.RefreshToken, requestId);
                return Ok(tokenResponse);
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult<BaseResponse> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var result = _accountService.Register(model);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("getroles")]
        public ActionResult<List<RoleModel>> GetRoles()
        {
            var result = _accountService.GetRoles();

            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("updaterole/{employeeid}/{roleid}")]
        public ActionResult<BaseResponse> UpdateRole(int employeeid, int roleid)
        {
            var result = _accountService.UpdateUserRole(employeeid, roleid);

            return Ok(result);
        }
    }
}
