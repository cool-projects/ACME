using System.ComponentModel.DataAnnotations;

namespace Models.UI
{
    public class UserLogin
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
