using Core;
using Idata.Data;
using Idata.Data.Entities.Iprofile;
using Idata.Data.Entities.Iprofile;
using Ihelpers.Helpers;
using Iprofile.Helpers.AuthProviders;
using Iprofile.Helpers.Interfaces;
using Iprofile.Repositories;
using TimeZoneConverter;

namespace Iprofile.Helpers
{
    public static class AuthHelper
    {
        public enum providers
        {
            azure,
            google,
            facebook
        }

        public static User? AuthUser(IHttpContextAccessor _httpContextAccessor)
        {

            string token = _httpContextAccessor.HttpContext.Request.Headers.Authorization;

            UrlRequestBase urlRequestBase = new UrlRequestBase();

            var userIdstr = JWTHelper.getJWTTokenClaim(token, "UserId");

            if (userIdstr != null)
            {
                urlRequestBase.criteria = userIdstr;

                urlRequestBase.include = "roles,departments";

                urlRequestBase.setActionMessage($"System request recognition of {typeof(User).Name} with id: {userIdstr}");

                using (var db = new IdataContext())
                {
                    var userRepo = new UsersRepository(null);

                    var userTask = userRepo.GetItem(urlRequestBase);

                    userTask.Wait();

                    User? user = userTask.Result;

                    user.Initialize();

                    if (user.timezone != null)
                    {
                        user.timezone = getTimezoneOffset(user.timezone);

                    }
                    else
                    {
                        user.timezone = "00:00";
                    }

                    return (user);

                }
            }
            else
            {
                return null;
            }

        }

        public static User? AuthUser(string? token)
        {


            UrlRequestBase urlRequestBase = new UrlRequestBase();

            var userIdstr = JWTHelper.getJWTTokenClaim(token, "UserId");

            if (userIdstr != null)
            {
                urlRequestBase.criteria = userIdstr;

                urlRequestBase.include = "roles,departments";

                urlRequestBase.setActionMessage($"System request recognition of {typeof(User).Name} with id: {userIdstr}");

                using (var db = new IdataContext())
                {
                    var userRepo = new UsersRepository(null);

                    var userTask = userRepo.GetItem(urlRequestBase);

                    userTask.Wait();

                    User? user = userTask.Result;

                    user.Initialize();

                    //from America/Colombia to -05:00
                    if (user.timezone != null)
                    {
                        user.timezone = getTimezoneOffset(user.timezone);

                    }
                    else
                    {
                        user.timezone = "00:00";
                    }

                    return (user);

                }
            }
            else
            {
                return null;
            }

        }

        public static string getTimezoneOffset(string? timezoneSufix)
        {
            return Ihelpers.Helpers.TimezoneHelper.getTimezoneOffset(timezoneSufix);

        }

        public static async Task<User?> GetUserByToken(string token, string provider)
        {
            IProvider authProvider = GetAuthProvider(provider);

            User? user = await authProvider.getUserByToken(token);

            return user;
        }

        public static async Task<User?> RefreshSession(string token, string provider)
        {
            IProvider authProvider = GetAuthProvider(provider);

            User? user = await authProvider.RefreshSession(token);

            return user;
        }

        public static async Task<System.Net.HttpStatusCode?> LogoutUserByToken(string token, string provider)
        {
            IProvider authProvider = GetAuthProvider(provider);

            return await authProvider.logoutUserByToken(token);

        }

        public static IProvider GetAuthProvider(string provider)
        {
            Enum.TryParse<providers>(provider, out var prov);

            switch (prov)
            {
                case providers.azure:
                    return new AzureProvider();
                default:
                    throw new Exception($"Invalid Auth provider name {provider}");

            }

        }

    }

}
