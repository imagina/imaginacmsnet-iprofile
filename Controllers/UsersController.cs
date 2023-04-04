using Core.Controllers;
using Hangfire;
using Idata.Data;
using Idata.Data.Entities.Iprofile;
using Iprofile.Helpers;
using Iprofile.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IProfile.Controllers
{
    [Authorize]
    [Route("api/profile/v1/Users")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Iprofile")]
    public class UsersController : ControllerBase<User>
    {

        public UsersController(IUsersRepository repositoryBase, IHttpContextAccessor currentContext, IBackgroundJobClient backgroundJobClient) : base(repositoryBase, AuthHelper.AuthUser(currentContext), backgroundJobClient)
        {

        }

    }
}
