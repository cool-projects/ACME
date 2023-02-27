using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.UI
{
    public class UserRoleModel
    {
        public int UserRoleId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int RoleId { get; set; }
        public int SubscriptionStatusId { get; set; }
        public RoleModel Role { get; set; }

    }
}
