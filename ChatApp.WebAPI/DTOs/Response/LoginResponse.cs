namespace ChatApp.WebAPI.DTOs.Response
{
    public class LoginResponse : BaseResponse
    {
        public string UserName { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
