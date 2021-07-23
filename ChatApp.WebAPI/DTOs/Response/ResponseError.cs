namespace ChatApp.WebAPI.DTOs.Response
{
    public sealed class ResponseError
    {
        public string Code { get; }
        public string Description { get; }

        public ResponseError(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
