using System.Collections.Generic;
namespace ZPP_Blazor.Models
{
    public class LoginUser
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
    public class SignInResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public JsonWebToken Token { get; set; }
    }

    public class JsonWebToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public long Expires { get; set; }
        public string Role { get; set; }
        public IDictionary<string, string> Claims { get; set; }
    }
}