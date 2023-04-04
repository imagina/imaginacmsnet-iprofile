
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Iprofile.Helpers
{
    public class AuthorizeBase : AuthorizeAttribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasClaim = context.HttpContext.Items.ContainsKey("custom");

            context.HttpContext.Items.Add("custom", "coyeeeeeee");



        }


    }
}
