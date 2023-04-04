using Idata.Data;
using Isite.Data.Seeders;

namespace Iprofile.Data.Seeders
{
    public class IprofileSeeder
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<IdataContext>();

                context.Database.EnsureCreated();

                IprofileModuleSeeder.Seed(applicationBuilder);

            }
        }
    }
}
