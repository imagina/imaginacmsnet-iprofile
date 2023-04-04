using Idata.Data.Entities.Iprofile;
using System.Net;
namespace Iprofile.Helpers.Interfaces
{
    public interface IProvider
    {
        public Task<User?> getUserByToken(string token);
        public Task<HttpStatusCode?> logoutUserByToken(string token);
        public Task<User?> RefreshSession(string refresh_token);
    }
}
