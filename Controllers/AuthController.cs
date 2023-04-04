using Core;
using Core.Exceptions;
using Core.Logger;
using Core.Transformers;
using Idata.Data;
using Idata.Data.Entities.Iprofile;
using Ihelpers.Helpers;
using Ihelpers.Middleware.TokenManager.Interfaces;
using Iprofile.Helpers;
using Iprofile.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Security.Claims;
namespace IProfile.Controllers
{
    [Route("api/profile/v1/auth/")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Iprofile")]
    public class AuthController : ControllerBase
    {
        private readonly IUsersRepository userRepository;

        private readonly IAuthRepository authRepository;

        private readonly IHttpContextAccessor _currentContext;

        private readonly ITokenManager _tokenManager;

        private readonly IdataContext dbContext;
        public AuthController(IUsersRepository _userRepository, IAuthRepository _authRepository, IdataContext dbContext, IHttpContextAccessor currentContext, ITokenManager tokenManager)
        {
            this.userRepository = _userRepository;

            this.authRepository = _authRepository;

            this.dbContext = dbContext;

            _currentContext = currentContext;

            _tokenManager = tokenManager;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me([FromQuery] UrlRequestBase? urlRequestBase)
        {

            int status = 200;

            Dictionary<string, object?> dicReturn = new();

            dynamic response;


            var accessToken = Request.Headers[HeaderNames.Authorization];

            var identity = HttpContext.User.Identity as ClaimsIdentity;

            var claims = identity.Claims;

            try
            {
                //parser
                await urlRequestBase.Parse(this);

                urlRequestBase.currentContextUser = AuthHelper.AuthUser(_currentContext);
                //Begin ME request response construction
                var userId = await JWTHelper.getJWTTokenClaimAsync(accessToken, "UserId");

                urlRequestBase.criteria = userId;

                //TODO cuando se cree la tabla organizations se debe quitar ya esta línea
                urlRequestBase.include = urlRequestBase.include.Replace("organizations.", "");
                urlRequestBase.include = urlRequestBase.include.Replace("organizations", "");

                //repository invoke
                userRepository.Initialize(dbContext);
                //TODO from where to get the user id??? caching maybe? 
                response = await userRepository.GetItem(urlRequestBase);

                //transformer
                dicReturn.Add("userData", await TransformerBase.TransformItem(response));



            }
            catch (ExceptionBase ex)
            {
                return StatusCode(ex.CodeResult, ex.CreateResponseFromException());
            }
            //reponse
            return StatusCode(status, await ResponseBase.Response(dicReturn));
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] UrlRequestBase? urlRequestBase, [FromBody] AuthLoginRequest authRequest)
        {
            int status = 200;

            Dictionary<string, dynamic?> response = new();

            var hasClaim = HttpContext.Items.ContainsKey("custom");
            try
            {
                string? entityType = "Department";
                var nameOf = nameof(entityType);

                await urlRequestBase.Parse(this);

                Dictionary<string, dynamic> userADResponse;
                //await bodyRequestBase.parse();

                var EnableAzureADAuthentication = Ihelpers.Helpers.ConfigurationHelper.GetConfig<bool>("DefaultConfigs:EnableAzureADAuthentication");

                userADResponse = EnableAzureADAuthentication == true ? await authRepository.AuthAD(authRequest.username, authRequest.password) :
                    await authRepository.AuthNoAD(authRequest.username, authRequest.password); ;


                if (userADResponse != null)
                {
                    User? loggedInData;

                    loggedInData = EnableAzureADAuthentication == true ? await userRepository.LoginAD((User)userADResponse["User"]) : await userRepository.LoginNoAD(authRequest.username, authRequest.password);


                    var JWTAuth = await authRepository.JWTAppend(loggedInData);

                    foreach (var item in JWTAuth)
                    {
                        userADResponse.Add(item.Key, item.Value);

                    }
                    userADResponse.Add("userData", await TransformerBase.TransformItem(loggedInData));

                    userADResponse.Remove("User");


                    response = userADResponse;

                    Task.Factory.StartNew(() => CoreLogger.LogMessage($"{loggedInData.email} has logged in", userId: loggedInData.id));



                }
                else
                {
                    throw new ExceptionBase("Unhautorized user", 403);
                }




            }
            catch (ExceptionBase ex)
            {
                return StatusCode(ex.CodeResult, ex.CreateResponseFromException());
            }

            return StatusCode(status, await ResponseBase.Response(response));
        }


        [HttpPost("social/{provider}")]
        public async Task<IActionResult> Social(string provider, [FromBody] BodyRequestBase authRequest)
        {
            int status = 200;

            Dictionary<string, dynamic?> response = new();

            var hasClaim = HttpContext.Items.ContainsKey("custom");

            try
            {
                string? entityType = "Department";

                var nameOf = nameof(entityType);

                await authRequest.Parse();

                dynamic? attributes = JsonConvert.DeserializeObject<ExpandoObject>(authRequest._attributes);

                string? token = String.Empty;


                try { token = attributes != null ? attributes.socialData.refresh_token : null; } catch { }



                if (string.IsNullOrEmpty(token) && provider != "local") throw new ExceptionBase($"Token cannot be null for provider {provider}", 400);

                Dictionary<string, dynamic> userADResponse;
                //await bodyRequestBase.parse();

                var EnableAzureADAuthentication = Ihelpers.Helpers.ConfigurationHelper.GetConfig<bool>("DefaultConfigs:EnableAzureADAuthentication");

                ///Social Auth goes here, replacing lines below
                userADResponse = await authRepository.RefreshSession(token, provider);


                if (userADResponse != null)
                {
                    User? loggedInData;

                    loggedInData = await userRepository.LoginAD((User)userADResponse["User"]);


                    //sync JWT duration with social duration
                    double expiresInTime = 0;

                    try { expiresInTime = loggedInData.social_data != null ? loggedInData.social_data.expires_in : null; } catch { }


                    if (expiresInTime > 0) { Ihelpers.Extensions.ConfigContainer.cache.CreateValue<dynamic>(loggedInData.external_id, loggedInData.social_data, Convert.ToDouble(loggedInData.social_data.expires_in)); }
                    else
                    { Ihelpers.Extensions.ConfigContainer.cache.CreateValue<dynamic>(loggedInData.external_id, loggedInData.social_data, Convert.ToDouble(Ihelpers.Helpers.ConfigurationHelper.GetConfig("Jwt:Duration"))); }



                    var JWTAuth = await authRepository.JWTAppend(loggedInData);

                    foreach (var item in JWTAuth)
                    {
                        userADResponse.Add(item.Key, item.Value);

                    }
                    userADResponse.Add("userData", await TransformerBase.TransformItem(loggedInData));

                    userADResponse.Remove("User");


                    response = userADResponse;

                    Task.Factory.StartNew(() => CoreLogger.LogMessage($"{loggedInData.email} has logged in", userId: loggedInData.id));



                }
                else
                {
                    throw new ExceptionBase("Unhautorized user", 403);
                }


            }
            catch (ExceptionBase ex)
            {
                return StatusCode(ex.CodeResult, ex.CreateResponseFromException());
            }

            return StatusCode(status, await ResponseBase.Response(response));
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout([FromQuery] UrlRequestBase authRequest)
        {
            int status = 200;

            Dictionary<string, dynamic?> response = new();

            Dictionary<string, dynamic> logoutResponse;

            var hasClaim = HttpContext.Items.ContainsKey("custom");
            try
            {

                await authRequest.Parse();

                var authorizationHeader = _currentContext.HttpContext.Request.Headers.Authorization;


                //get wich user is loggin out for logs historic
                var logginOutUser = AuthHelper.AuthUser(_currentContext);

                //get this token from cache


                string token = ((dynamic)Ihelpers.Extensions.ConfigContainer.cache.GetValue(logginOutUser.external_id)).access_token;


                string? provider = JObjectHelper.GetJObjectValue<string?>(JObject.Parse(authRequest.setting), "authProvider");


                if (provider is null) throw new ExceptionBase("Provider is null", 400);

                if (token is null && provider != "local") throw new ExceptionBase("Token is null", 400);


                if (token.Contains("Bearer"))
                {
                    token = token.Split(" ").Last().Trim();
                }





                if (provider != "local")
                {
                    ///Social Auth goes here, replacing lines below
                    logoutResponse = await authRepository.SocialLogout(token, provider);

                }
                //logout by blacklisting token
                logoutResponse = await authRepository.JWTLogout();



                if (logoutResponse["data"] == System.Net.HttpStatusCode.OK)
                {

                    string? userId = JWTHelper.getJWTTokenClaim(authorizationHeader, "userId");
                    Task.Factory.StartNew(() => CoreLogger.LogMessage($"{logginOutUser.email} has logged out", userId: logginOutUser.id));

                }
                else
                {
                    throw new ExceptionBase("Exception encountered trying to loggin out user", logoutResponse["data"]);
                }


            }
            catch (ExceptionBase ex)
            {
                return StatusCode(ex.CodeResult, ex.CreateResponseFromException());
            }

            return StatusCode(status, await ResponseBase.Response(response));
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromQuery] UrlRequestBase urlRequestBase)
        {
            int status = 200;

            Dictionary<string, dynamic?> response = new();

            var hasClaim = HttpContext.Items.ContainsKey("custom");
            try
            {
                string? entityType = "Department";


                await urlRequestBase.Parse(this);

                //get the context User
                var refreshingTokenUser = AuthHelper.AuthUser(_currentContext);


                //get provider from settings
                string? provider = JObjectHelper.GetJObjectValue<string?>(JObject.Parse(urlRequestBase.setting), "authProvider");

                //try get the social data related to the user
                dynamic socialData = Ihelpers.Extensions.ConfigContainer.cache.GetValue(refreshingTokenUser.external_id);

                if (socialData is null && provider != "local") throw new ExceptionBase("cacheData related to user is null", 400);

                string? token = string.Empty;

                try { token = socialData != null ? socialData.refresh_token : null; } catch { }

                if (provider is null) throw new ExceptionBase("Provider is null", 400);

                if (string.IsNullOrEmpty(token) && provider != "local") throw new ExceptionBase($"Token cannot be null for provider {provider}", 400);

                //var EnableAzureADAuthentication = Core.Helpers.ConfigurationHelper.GetConfig<bool>("DefaultConfigs:EnableAzureADAuthentication");

                Dictionary<string, dynamic> userADResponse = new();

                if (provider != "local")
                {
                    userADResponse = await authRepository.RefreshSession(token, provider);
                }
                else
                {
                    userADResponse.Add("User", refreshingTokenUser);
                }

                if (userADResponse != null)
                {
                    User? loggedInData;

                    loggedInData = await userRepository.LoginAD((User)userADResponse["User"]);

                    //sincronice JWT duration with social duration
                    double? expiresInTime = 0;

                    try { expiresInTime = socialData != null ? Convert.ToDouble(socialData.expires_in) : null; } catch { }

                    loggedInData.social_data.expiresIn = expiresInTime;

                    var JWTAuth = await authRepository.JWTAppend(loggedInData);

                    foreach (var item in JWTAuth)
                    {
                        userADResponse.Add(item.Key, item.Value);

                    }
                    userADResponse.Add("userData", await TransformerBase.TransformItem(loggedInData));

                    userADResponse.Remove("User");


                    response = userADResponse;

                    //blacklist current token
                    await _tokenManager.DeactivateCurrentAsync();

                    Task.Factory.StartNew(() => CoreLogger.LogMessage($"{loggedInData.email} has refreshed session", userId: loggedInData.id));



                }
                else
                {
                    throw new ExceptionBase("Unhautorized user", 403);
                }




            }
            catch (ExceptionBase ex)
            {
                return StatusCode(ex.CodeResult, ex.CreateResponseFromException());
            }

            return StatusCode(status, await ResponseBase.Response(response));
        }


    }
}
