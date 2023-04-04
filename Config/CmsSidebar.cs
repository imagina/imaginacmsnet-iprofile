namespace Iprofile.Config
{
    public static class CmsSidebar
    {

        private static string cmsSidebar = @"{
            'admin':[{
            'title':'iprofile.cms.sidebar.adminGroup',
            'icon':'fas fa-users',
            'children':[
                'iprofile_cms_admin_userIndex',
                'iprofile_cms_admin_userDepartments',
                'iprofile_cms_admin_userRoles',
                'iprofile_cms_admin_directory'
            ]}
            ],
            'panel':['iprofile_cms_main_userProfile']
        }";

        public static string GetCmsSidebar()
        {
            return cmsSidebar;
        }
    }
}
