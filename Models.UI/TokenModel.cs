using System;
using System.Collections.Generic;

namespace Models.UI
{
    public class TokenModel : BaseResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expires { get; set; }

        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }
        public List<UserRoleModel> Roles { get; set; }
    }
}
