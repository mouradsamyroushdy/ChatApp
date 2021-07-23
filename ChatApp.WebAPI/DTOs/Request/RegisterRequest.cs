using System.ComponentModel.DataAnnotations;

namespace ChatApp.WebAPI.DTOs.Request
{
    public class RegisterRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
