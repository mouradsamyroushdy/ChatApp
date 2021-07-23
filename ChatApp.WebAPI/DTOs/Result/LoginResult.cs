using ChatApp.Database.Entities;

namespace ChatApp.WebAPI.DTOs.Result
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public User User { get; set; }

        public LoginResult()
        {
            Succeeded = false;
        }
    }
}
