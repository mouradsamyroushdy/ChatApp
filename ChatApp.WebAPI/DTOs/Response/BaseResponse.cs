using System.Collections.Generic;

namespace ChatApp.WebAPI.DTOs.Response
{
    public abstract class BaseResponse
    {
        public bool Succeded { get; set; }
        public IEnumerable<ResponseError> Errors { get; set; }

        protected BaseResponse(bool succeded = false, IEnumerable<ResponseError> errors = null)
        {
            Succeded = succeded;
            Errors = errors;
        }
    }
}
