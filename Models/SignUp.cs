namespace ZPP_Blazor.Models
{
    public class SignUpResult
    {
        public string Message { get; set; }
        public bool Success { get; set; }
    }
    public class SignUpModel
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
    }
}