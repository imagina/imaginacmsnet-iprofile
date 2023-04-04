namespace Iprofile.Helpers
{
    public class AuthLoginRequest
    {
        public string? username { get; set; }
        public string? password { get; set; }
    }

    public class SocialAuthLoginRequest
    {
        public string? token { get; set; }
        public string? provider { get; set; }
    }
}
