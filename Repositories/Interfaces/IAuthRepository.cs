using Idata.Data.Entities.Iprofile;

namespace Iprofile.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        public Task<Dictionary<string, dynamic>> AuthAD(string requestUsername, string requestPassword);

        public Task<Dictionary<string, dynamic>> AuthNoAD(string requestUsername, string requestPassword);
        public Task<Dictionary<string, dynamic>> JWTAppend(User wichUser);


        public Task<Dictionary<string, dynamic>> SocialAuth(string? token, string provider);
        public Task<Dictionary<string, dynamic>> SocialLogout(string? token, string provider);
        public Task<Dictionary<string, dynamic>> RefreshSession(string? token, string provider);
        public Task<Dictionary<string, dynamic>> JWTLogout();
    }
}
