using Core.Exceptions;
using Core.Logger;
using Iprofile.Helpers.Interfaces;
using Newtonsoft.Json;
using System.Dynamic;
using System.Net;
using Newtonsoft.Json.Linq;
using Idata.Data.Entities.Iprofile;

namespace Iprofile.Helpers.AuthProviders
{
    public class AzureProvider : IProvider
    {
        public string? tenantId { get; private set; }

        private readonly string graphURL;

        private readonly string memberOfURL;

        private readonly string logoutURL;

        private readonly string refreshTokenURL;

        public AzureProvider()
        {
            tenantId = Ihelpers.Helpers.ConfigurationHelper.GetConfig<string>("SocialAuth:Microsoft:Tenant-Id");

            if (tenantId == null) throw new Exception("Microsoft Auth Tenant Id not found on app settings file. KeyPath: SocialAuth:Microsoft:Tenant-Id");

            graphURL = "https://graph.microsoft.com/v1.0/me";

            memberOfURL = "https://graph.microsoft.com/v1.0/users/{email}/memberOf";

            logoutURL = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/logout";

            refreshTokenURL = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

        }

        public async Task<User?> getUserByToken(string token)
        {
            User? response = new User();
            try
            {
                CoreLogger.LogMessage($"Begin with authentication for token {token.Substring(0, 15)}..., provider: Azure");

                var httpclient = new HttpClient();

                httpclient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                httpclient.DefaultRequestHeaders.Add("Accept", "application/json");

                var responseAPI = await httpclient.GetAsync(graphURL);

                string responseAPIStr = await responseAPI.Content.ReadAsStringAsync();

                CoreLogger.LogMessage($"Graph API response for token {token.Substring(0, 15)}..., provider: Azure  Status: {responseAPI.StatusCode}", stackTrace: $"responseBody:{responseAPIStr}");

                if (!responseAPI.IsSuccessStatusCode) throw new Exception("Error obtaining user for Azure provider, API is in faulted state");

                dynamic JsonResponse = JsonConvert.DeserializeObject<ExpandoObject>(responseAPIStr.Replace('\"', '\''));

                response.email = JsonResponse.userPrincipalName;

                response.external_id = JsonResponse.id;

                response.first_name = JsonResponse.givenName;

                response.last_name = JsonResponse.surname;

                string? memberNexturl = memberOfURL.Replace("{email}", response.email);

                List<string> memberOfIds = new List<string>();
                do
                {

                    var responseMemberOfAPI = await httpclient.GetAsync(memberNexturl);

                    string responseMemberOfAPIStr = await responseMemberOfAPI.Content.ReadAsStringAsync();

                    if (!responseMemberOfAPI.IsSuccessStatusCode) throw new Exception("Error obtaining user for Azure provider, API is in faulted state");

                    CoreLogger.LogMessage($"Graph API response for memberOf email {response.email}", stackTrace: $"responseBody:{responseMemberOfAPIStr}");

                    JObject memberOfResponse = JObject.Parse(responseMemberOfAPIStr.Replace('\"', '\''));

                    string? groups = (memberOfResponse.SelectToken("value"))?.ToString();

                    if (!string.IsNullOrEmpty(groups))
                    {

                        List<string> groupsIds = memberOfResponse["value"].Select(g => (string)g.SelectToken("id")).ToList();
                        
                        memberOfIds.AddRange(groupsIds);

                        memberNexturl = (memberOfResponse.SelectToken("@odata.nextLink"))?.ToString();
                    }


                } while (!string.IsNullOrEmpty(memberNexturl));

                response.dynamic_parameters.Add("external_groups_id", memberOfIds);

                CoreLogger.LogMessage($"Azure AD User for token {token.Substring(0, 15)}..., provider: Azure has been found, the username is {response.email}");


            }
            catch (Exception ex)
            {
                ExceptionBase.HandleException(ex, $"Error obtaining user for token {token} with Azure AD");
            }
            return response;
        }
       
        public async Task<HttpStatusCode?> logoutUserByToken(string token)
        {
            HttpResponseMessage? responseAPI = new();
            try
            {
                CoreLogger.LogMessage($"Begin with logout for token {token.Substring(0, 15)}..., provider: Azure");

                var httpclient = new HttpClient();

                httpclient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                httpclient.DefaultRequestHeaders.Add("Accept", "application/json");

                responseAPI = await httpclient.GetAsync(logoutURL);

                string responseAPIStr = await responseAPI.Content.ReadAsStringAsync();

                CoreLogger.LogMessage($"oAuth2 API response for token {token.Substring(0, 15)}..., provider: Azure  Status: {responseAPI.StatusCode} responseBody:{responseAPIStr}");

                if (!responseAPI.IsSuccessStatusCode) CoreLogger.LogMessage("Error loggin out user for Azure provider, API is in faulted state");

                return responseAPI.StatusCode;

            }
            catch (Exception ex)
            {
                ExceptionBase.HandleException(ex, $"Error obtaining user for token {token} with Azure AD");
            }
            return responseAPI.StatusCode;
        }


        public async Task<User?> RefreshSession(string? refresh_token)
        {
            var httpclient = new HttpClient();

            //prepare headers, origin is important for request to be procesed
            httpclient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpclient.DefaultRequestHeaders.Add("Origin", "https://developer.mozilla.org");

            string clientId = Ihelpers.Helpers.ConfigurationHelper.GetConfig("AzureAd:ClientId");

            if (string.IsNullOrEmpty(clientId)) throw new Exception("client id configuration for Azure Active Directory not found on appsettings.json!");

            if (string.IsNullOrEmpty(refresh_token)) throw new Exception("refresh_roken is null, unable to refresh the token!");

            string scopes = Ihelpers.Helpers.ConfigurationHelper.GetConfig("AzureAd:Scopes");

            if (string.IsNullOrEmpty(scopes)) throw new Exception("Scopes configuration for Azure Active Directory not found on appsettings.json!");

            //Prepare body of request
            Dictionary<string, string> formData = new();
            formData.Add("grant_type", "refresh_token");
            formData.Add("refresh_token", refresh_token);
            formData.Add("client_id", clientId);

            //each scope MUST be space separated NOT comma separated
            formData.Add("scope", scopes);

            var apiRequest = new HttpRequestMessage(HttpMethod.Post, refreshTokenURL) { Content = new FormUrlEncodedContent(formData) };

            var responseAPI = await httpclient.SendAsync(apiRequest);

            string responseAPIStr = await responseAPI.Content.ReadAsStringAsync();

            if (!responseAPI.IsSuccessStatusCode) throw new Exception("Error refreshing token for Azure provider, API is in faulted state");

            dynamic JsonRespose = JsonConvert.DeserializeObject<ExpandoObject>(responseAPIStr);

            User? socialUser = await getUserByToken(JsonRespose.access_token);

            socialUser.social_data = JsonRespose;

            return socialUser;
        }
    }
}
