using Core.Entities;
using Core.Exceptions;
using Idata.Data;
using Idata.Data.Entities.Iprofile;
using Ihelpers.Helpers;
using Ihelpers.Interfaces;
using Ihelpers.Middleware.TokenManager.Interfaces;
using Iprofile.Helpers;
using Iprofile.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Iprofile.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        public IdataContext _dataContext { get; }

        public IConfiguration _configuration;

        private const string controllerName = "Auth";

        private readonly ICacheBase _cache;

        private readonly ITokenManager _tokenManager;
        public AuthRepository(IdataContext dataContext, IConfiguration config, ICacheBase cache, ITokenManager tokenManager)
        {
            //Dependency injection of dataContext and logger
            _configuration = config;

            _dataContext = dataContext;

            _cache = cache;

            _tokenManager = tokenManager;
        }

        public async Task<Dictionary<string, dynamic>> AuthAD(string requestUsername, string requestPassword)
        {

            //prepare the response
            Dictionary<string, dynamic> response = new Dictionary<string, dynamic>();
            try
            {

                ConfigurationHelper config = ConfigurationHelper.ReadFromJsonFile("appsettings.json");

                var appConfig = config.PublicClientApplicationOptions;

                var app = PublicClientApplicationBuilder.CreateWithApplicationOptions(appConfig).Build();

                var httpClient = new HttpClient();

                MyInformation myInformation = new MyInformation(app, httpClient, config.MicrosoftGraphBaseEndpoint);

                //Perform login to azure AD
                var authenticationResult = await myInformation.AdquireTokenFromUserNameAndPasswordAsync(requestUsername, requestPassword);


                if (authenticationResult != null)
                {
                    MyInformation.DisplaySignedInAccount(authenticationResult.Account);

                    Console.WriteLine(authenticationResult.ClaimsPrincipal.ToString());

                    string accessToken = authenticationResult.AccessToken;

                    var userAD = await myInformation.CallWebApiAndDisplayResultAsync(myInformation.WebApiUrlMe, accessToken, "Me");

                    userAD.Add("email", requestUsername);

                    userAD.Add("password", requestPassword);

                    response.Add("User", await populateUser(userAD));

                }
                else
                {
                    throw new ExceptionBase($"User not found on azure active directory", 404);
                }
            }
            catch (ExceptionBase ex)
            {
                ExceptionBase.HandleException(ex, ex.Message);

            }
            return response;
            // string strPassword = "C@rgoAGI!";
            //username = "admin-dev01@allianceground.com";


        }

        public async Task<Dictionary<string, dynamic>> AuthNoAD(string requestUsername, string requestPassword)
        {


            Dictionary<string, dynamic> response = new Dictionary<string, dynamic>();
            try
            {

                var encriptedPass = await EncryptionHelper.Encrypt(requestPassword);

                var authenticationResult = await _dataContext.Users.Where(user => user.email == requestUsername && user.password == encriptedPass).FirstOrDefaultAsync();


                if (authenticationResult != null)
                {


                    var userAD = new JObject();


                    authenticationResult.password = requestPassword;

                    userAD.Add("email", requestUsername);

                    userAD.Add("password", requestPassword);

                    response.Add("User", authenticationResult);



                }
                else
                {
                    throw new ExceptionBase($"User not found", 404);
                }
            }
            catch (ExceptionBase ex)
            {
                ExceptionBase.HandleException(ex, ex.Message);

            }
            return response;
            // string strPassword = "C@rgoAGI!";
            //username = "admin-dev01@allianceground.com";


        }

        public async Task<Dictionary<string, dynamic>> SocialAuth(string token, string provider)
        {


            Dictionary<string, dynamic> response = new Dictionary<string, dynamic>();
            try
            {

                User? providerResponse = await AuthHelper.GetUserByToken(token, provider);

                response.Add("User", providerResponse);


            }
            catch (ExceptionBase ex)
            {
                ExceptionBase.HandleException(ex, ex.Message);

            }
            return response;



        }

        public async Task<Dictionary<string, dynamic>> SocialLogout(string token, string provider)
        {


            Dictionary<string, dynamic> response = new Dictionary<string, dynamic>();
            try
            {

                var providerResponse = await AuthHelper.LogoutUserByToken(token, provider);

                response.Add("data", providerResponse);


            }
            catch (ExceptionBase ex)
            {
                ExceptionBase.HandleException(ex, ex.Message);

            }
            return response;



        }

        public async Task<Dictionary<string, dynamic>> RefreshSession(string token, string provider)
        {


            Dictionary<string, dynamic> response = new Dictionary<string, dynamic>();
            try
            {

                var providerResponse = await AuthHelper.RefreshSession(token, provider);

                response.Add("User", providerResponse);



            }
            catch (ExceptionBase ex)
            {
                ExceptionBase.HandleException(ex, ex.Message);

            }
            return response;



        }

        public async Task<User> populateUser(JObject wichObject)
        {
            User user = new User();

            try
            {
                user.email = JObjectHelper.GetJObjectValue(wichObject, "email").ToString();

                user.password = JObjectHelper.GetJObjectValue(wichObject, "password").ToString();

                user.first_name = JObjectHelper.GetJObjectValue(wichObject, "givenName").ToString();

                user.last_name = JObjectHelper.GetJObjectValue(wichObject, "surname").ToString();

                user.external_guid = Guid.Parse(JObjectHelper.GetJObjectValue(wichObject, "id").ToString());
            }
            catch (Exception ex)
            {

                ExceptionBase.HandleException(ex, "error parsing azure user into user");
            }


            return user;
        }

        public async Task<Dictionary<string, dynamic>> JWTAppend(User authenticatedUser)
        {
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();

            try
            {
                double expiresInTime = 0;

                try { expiresInTime = authenticatedUser.social_data.expires_in != null ? authenticatedUser.social_data.expires_in : null; } catch { }

                var timeToExpire = expiresInTime > 0 ? DateTime.UtcNow.AddMinutes(expiresInTime) : DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:Duration"]));

                var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", authenticatedUser.id.ToString()),
                        new Claim("expiresIn", timeToExpire.ToString()),
                        new Claim("ExternalId", authenticatedUser.external_id != null ? authenticatedUser.external_id: string.Empty),
                        //new Claim("UserData", JsonConvert.SerializeObject(authenticatedUser, Formatting.None, new JsonSerializerSettings()
                        //{
                        //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        //})),
                        new Claim("DisplayName", authenticatedUser.first_name +authenticatedUser.last_name ),
                        new Claim("UserName", authenticatedUser.email),
                        new Claim("Email", authenticatedUser.email)
                    };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: timeToExpire,
                    signingCredentials: signIn);

                result.Add("userToken", "Bearer " + new JwtSecurityTokenHandler().WriteToken(token));

                result.Add("expiresIn", timeToExpire);


            }
            catch (Exception ex)
            {
                ExceptionBase.HandleException(ex, "error token configuration into JWT ");

            }
            //create claims details based on the user information

            return result;
        }

        public async Task<Dictionary<string, dynamic>> JWTLogout()
        {

            Dictionary<string, dynamic> response = new Dictionary<string, dynamic>();
            try
            {

                //Get the token and put it in the black list
                await _tokenManager.DeactivateCurrentAsync();

                //then return OK
                response.Add("data", System.Net.HttpStatusCode.OK);

                //Token validation middleware will take care of rest

            }
            catch (ExceptionBase ex)
            {
                ExceptionBase.HandleException(ex, ex.Message);

            }
            return response;
        }
    }
}
