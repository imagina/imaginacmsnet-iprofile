using Microsoft.AspNetCore.Mvc;
using profile.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace profile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        //Profile perfil = new Profile();
        Profile profile = new Profile();
        public ProfileController()
        {
            profile.data = new List<Data>() {
                new Data {
                    id = 1,
                    firstName = "Imagina",
                    lastName = "Colombia",
                    fullName = "Imagina Colombia",
                    isActivated = 1,
                    email = "soporte@imaginacolombia.com",
                    permissions = new List<Permission>() { },
                    createdAt = DateTime.Parse("2021-11-25 16:24:29"),
                    updatedAt = DateTime.Parse("2022 -03-01 17:54:20"),
                    lastLoginDate = DateTime.Parse("2022-03-01 17:54:20"),
                    smallImage = @"https:\/\/josenet.ozonohosting.com\/modules\/iprofile\/img\/default.jpg",
                    mediumImage = @"https:\/\/josenet.ozonohosting.com\/modules\/iprofile\/img\/default.jpg",
                    mainImage = @"https:\/\/josenet.ozonohosting.com\/modules\/iprofile\/img\/default.jpg",
                    contacts = new Contacts { name = "contacts", value = new List<ContactsValue>(){ } }
                    ,
                    socialNetworks = new SocialNetworks { name = "socialNetworks", value = new List<SocialNetworksValue>(){ } }
                    ,
                    departments = new List<Departments>() {
                                new Departments { id = 1, title = "Users", partenId = 0, INTERNAL = 0, settings = new List<DepartmentsSettings>(){ },createdAt = DateTime.Parse("2021-11-25 16:25:01"), updatedAt = DateTime.Parse("2021-11-25 16:25:01") }
                    },
                    settings = new DataSettings { assignedRoles = new List<DataSettingsAssignedRoles>(){ }, assignedDepartments = new List<DataSettingsAssignedDepartments>(){ } }
                    ,
                    fields = new List<Fields>()
                    {

                    },
                    roles = new List<Roles>()
                    {
                        new Roles { id = 1,
                                    name = "Super Admin",
                                    slug = "super-admin",
                                    permissions = new RolesPermissions
                                    {
                                        core_sidebar_group = true,
                                        dashboard_index = true,
                                        dashboard_update = true
                                    },
                                    createdAt = DateTime.Parse("2021-11-25 16:22:39"),
                                    updatedAt = DateTime.Parse("2022-02-14 12:24:10"),
                                    settings = new RolesSettings{},
                                    form = null,
                                    formId = null,
                                   },
                        new Roles { id = 3,
                                    name = "Editor",
                                    slug = "editor",
                                    permissions = new RolesPermissions
                                    {
                                        profile_api_login = true,
                                        profile_user_index = true,
                                        iblog_categories_manage = true
                                    },
                                    settings = new RolesSettings{},
                                    form = null,
                                    formId = null,
                                   },
                        new Roles { id = 4,
                                    name = "Author",
                                    slug = "author",
                                    permissions = new RolesPermissions
                                    {
                                        profile_api_login = true,
                                        profile_user_index = true,
                                        iblog_categories_manage = true
                                    },
                                    settings = new RolesSettings{},
                                    form = null,
                                    formId = null,
                                   }
                    },
                    allPermissions = new AllPermissions{
                        core_sidebar_group= true,
                         dashboard_index= true,
                         dashboard_update= true
                    },
                    allSettings = new AllSettings
                    {
                        assignedRoles = new List<AssignedRoles>(){},
                        assignedDepartments = new List<AssignedDepartments>(){},
                    }

                }
            };

            profile.meta = new Meta()
            {
                page = new Page()
                {
                    total = 1,
                    lastPage = 1,
                    perPage = 10,
                    currentPage = 1
                }
            };

        }

        /*
        // GET: api/<ProfileController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        */
        // GET api/<ProfileController>/5
        [HttpGet("{id}")]
        public Profile Get()
        {

                return profile;
          

        }

        /*
        // POST api/<ProfileController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ProfileController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProfileController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        */

    }
}
