using Core.Events.Interfaces;
using Idata.Data;
using Iprofile.Events.Handlers;
using Iprofile.Repositories;
using Iprofile.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;

namespace Iprofile
{
    public static class IprofileServiceProvider
    {


        public static WebApplicationBuilder? Boot(WebApplicationBuilder? builder)
        {
            //TODO Implement controllerBase to avoid basic crud redundant code
            builder.Services.AddControllers().ConfigureApplicationPartManager(o =>
            {

                o.ApplicationParts.Add(new AssemblyPart(typeof(IprofileServiceProvider).Assembly));
            });

            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped(typeof(IUsersRepository), typeof(UsersRepository));

            //builder.Services.AddScoped<IUsersRepository, UsersRepository>();
            builder.Services.AddScoped<IRolesRepository, RolesRepository>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<IServiceProvider>(sp => sp);

            //builder.Services.AddTransient(typeof(IRepositoryBase<EntityBase>), typeof(RepositoryBase<EntityBase>));
            //System.Web.RequestContext.Configure((IHttpContextAccessor)builder.Services.Where(serv => serv is IHttpContextAccessor).First());

            builder.Services.AddScoped(typeof(IEventHandlerBase), typeof(URlRequestParseHandler));
            return builder;

        }


    }
}
