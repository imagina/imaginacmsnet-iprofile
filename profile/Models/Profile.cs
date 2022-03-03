namespace profile.Models
{
    public class Profile
    {
        public List<Data> data { get; set; }
        public Meta meta { get; set; }
    }

    public class Data
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
        public int isActivated { get; set; }
        public string email { get; set; }
        public List<Permission> permissions { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime lastLoginDate { get; set; }
        public string smallImage { get; set; }
        public string mediumImage { get; set; }
        public string mainImage { get; set; }
        public Contacts contacts { get; set; } //
        public SocialNetworks? socialNetworks { get; set; } //
        public List<Departments> departments { get; set; }
        public DataSettings settings { get; set; } //
        public List<Fields> fields { get; set; }
        public List<Roles> roles { get; set; }
        public AllPermissions allPermissions { get; set; }
        public AllSettings allSettings { get; set; }



    }

    public class Permission
    {
    }

    public class Contacts
    {
        public string name { get; set; }
        public List<ContactsValue>? value { get; set; }
    }

    public class ContactsValue
    {

    }

    public class SocialNetworks
    {
        public string name { get; set; }

        public List<SocialNetworksValue>? value { get; set; }
    }

    public class SocialNetworksValue
    {

    }

    public class Departments
    {
        public int id { get; set; }
        public string title { get; set; }
        public int partenId { get; set; }
        public int INTERNAL { get; set; }//Palabra Reservada
        public List<DepartmentsSettings>? settings { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

    }
    public class DepartmentsSettings
    {

    }

    public class DataSettings
    {
        public List<DataSettingsAssignedRoles>? assignedRoles { get; set; }
        public List<DataSettingsAssignedDepartments>? assignedDepartments { get; set; }
    }

    public class DataSettingsAssignedRoles
    {

    }
    public class DataSettingsAssignedDepartments
    {

    }

    public class Fields
    {

    }

    public class Roles
    {
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public RolesPermissions permissions { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public RolesSettings settings { get; set; }
        public string? form { get; set; }
        public int? formId { get; set; }


    }

    public class RolesPermissions
    {
        public bool core_sidebar_group { get; set; }
        public bool dashboard_index { get; set; }
        public bool dashboard_update { get; set; }

        /// Muchos Mas
        /// 
        public bool profile_api_login { get; set; }
        public bool profile_user_index { get; set; }
        public bool iblog_categories_manage { get; set; }

    }

    public class RolesSettings
    {

    }

    public class AllPermissions
    {
        public bool core_sidebar_group { get; set; }
        public bool dashboard_index { get; set; }
        public bool dashboard_update { get; set; }
        //Muchos MAS!!
    }

    public class AllSettings
    {
        public List<AssignedRoles> assignedRoles { get; set; }
        public List<AssignedDepartments> assignedDepartments { get; set; }
    }

    public class AssignedRoles
    {

    }
    public class AssignedDepartments
    {

    }

    public class Meta
    {
        public Page page { get; set; }
    }

    public class Page
    {
        public int total { get; set; }
        public int lastPage { get; set; }
        public int perPage { get; set; }
        public int currentPage { get; set; }

    }
}
