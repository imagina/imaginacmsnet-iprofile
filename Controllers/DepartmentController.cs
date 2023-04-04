using Core.Controllers;
using Idata.Data;
using Idata.Data.Entities.Iprofile;
using Iprofile.Helpers;
using Iprofile.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Iprofile.Controllers
{
    [Authorize]
    [Route("api/profile/v1/departments")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Iprofile")]
    public class DepartmentController : ControllerBase<Department>
    {

        public DepartmentController(IDepartmentRepository repositoryBase, IHttpContextAccessor currentContext) : base(repositoryBase, AuthHelper.AuthUser(currentContext))
        {

        }
    }
}
