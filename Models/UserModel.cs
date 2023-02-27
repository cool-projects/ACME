using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UserModel : BaseResponse
    {
        public string Name { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        public List<UserRoleModel> UserRoles { get; set; }
    }
}
