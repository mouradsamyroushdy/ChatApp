using System.ComponentModel.DataAnnotations;

namespace ChatApp.WebAPI.DTOs.Request
{
    public class RefreshTokenRequest
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
