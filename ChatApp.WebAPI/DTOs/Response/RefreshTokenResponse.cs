﻿namespace ChatApp.WebAPI.DTOs.Response
{
    public class RefreshTokenResponse : BaseResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
