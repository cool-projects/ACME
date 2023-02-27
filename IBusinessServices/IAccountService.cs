using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBusinessServices
{
    public interface IAccountService
    {
        BaseResponse Register(RegisterModel model);
        BaseResponse UpdateUserRole(int userId, int roleId);
        List<RoleModel> GetRoles();
        UserModel Login(UserLoginModel user);
        UserModel GetUserDetailsByUserId(int userId);
    }
}
