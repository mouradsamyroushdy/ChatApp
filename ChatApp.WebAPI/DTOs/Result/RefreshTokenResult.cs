namespace ChatApp.WebAPI.DTOs.Result
{
    public class RefreshTokenResult
    {
        public bool Succeeded { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
