using AutoMapper;
using Core;
using Core.Events.Interfaces;
using Core.Exceptions;
using Core.Logger;
using Core.Repositories;
using Core.Transformers;
using Idata.Data;
using Idata.Data.Entities.Iprofile;
using Ihelpers.Helpers;
using Iprofile.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using Newtonsoft.Json.Linq;
using System.Linq.Dynamic.Core;

namespace Iprofile.Repositories
{
    public class UsersRepository : RepositoryBase<User>, IUsersRepository
    {
        IHttpContextAccessor _httpContext;

        private const string controllerName = "Users";
        public UsersRepository(
            IHttpContextAccessor httpContext,
            IEventHandlerBase<User> eventHandler)
        {
            //Dependency injection of dataContext 
            _httpContext = httpContext;


            _eventHandler = eventHandler;

        }

        public UsersRepository(IHttpContextAccessor httpContext) 
        {

            _httpContext = httpContext;


        }

        public async Task<User?> LoginAD(User? userToFind)
        {

            Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transaction = null;

            try
            {

                var userFromDB = await _dataContext.Users.Include("roles").Include("departments").Where(obj => obj.email == userToFind.email).Include("roles").FirstOrDefaultAsync();


                if (userFromDB == null)
                {
                    Task.Factory.StartNew(() => CoreLogger.LogMessage($"No user found with email {userToFind.email} a new user with email {userToFind.email} will becreated"));
                    //configure default password
                    userToFind.password = "7jAtv2RVr+4zxUHIm2ihB8aqkmZ7cyHMSyLx6Gm8DIE=";

                    userToFind.external_id = userToFind.external_id;

                    _dataContext.Users.Add(userToFind);

                    await _dataContext.SaveChangesAsync(CancellationToken.None);

                    List<Role> rolesToAssing = new List<Role>();

                    dynamic? rolesID;

                    userToFind.dynamic_parameters.TryGetValue("external_groups_id", out rolesID);

                    if (rolesID != null)
                    {

                        var explicitList = rolesID as List<string?>;


                        userToFind.roles = await _dataContext.Roles.Where(role => explicitList.Contains(role.external_id)).ToListAsync();

                        Task.Factory.StartNew(() => CoreLogger.LogMessage($"Role sync result: appended {(userToFind.roles != null ? userToFind.roles.Count() : 0)} from DB"));
                    }

                    if (userToFind.roles.Count() == 0)
                    {

                        var defaultRoleConfig = ConfigurationHelper.GetConfig("DefaultConfigs:DefaultRole");

                        if (defaultRoleConfig == null) throw new ExceptionBase($"Default role not found in configuration", 404);

                        Task.Factory.StartNew(() => CoreLogger.LogMessage($"New user with email {userToFind.email} doesn't have role assigned, default role : {defaultRoleConfig} will be mapped"));

                        var defaultRole = await _dataContext.Roles.Where(role => role.slug == defaultRoleConfig).FirstOrDefaultAsync();

                        if (defaultRole == null) throw new ExceptionBase($"Default role not found in database", 404);

                        userToFind.roles.Add(defaultRole);

                        await _dataContext.SaveChangesAsync(CancellationToken.None);

                        userFromDB = userToFind;

                    }
                    else
                    {
                        await _dataContext.SaveChangesAsync(CancellationToken.None);
                        userFromDB = userToFind;
                    }

                }
                else
                {
                    Task.Factory.StartNew(() => CoreLogger.LogMessage($"User with email {userToFind.email} found on database"));


                    userFromDB.external_id = userFromDB.external_id == null ? userToFind.external_id : userFromDB.external_id;

                    await _dataContext.SaveChangesAsync(CancellationToken.None);

                    List<Role> rolesToAssing = new List<Role>();

                    dynamic? rolesID;

                    userToFind.dynamic_parameters.TryGetValue("external_groups_id", out rolesID);

                    if (rolesID != null)
                    {
                        userFromDB.roles.Clear();

                        var explicitList = rolesID as List<string?>;

                        userFromDB.roles = await _dataContext.Roles.Where(role => explicitList.Contains(role.external_id)).ToListAsync();

                        Task.Factory.StartNew(() => CoreLogger.LogMessage($"Role sync result: appended {(userToFind.roles != null ? userToFind.roles.Count() : 0)} to user with email : {userToFind.email}from DB"));

                        await _dataContext.SaveChangesAsync(CancellationToken.None);
                    }

                    if (userFromDB.roles.Count() == 0)
                    {
                        var defaultRoleConfig = ConfigurationHelper.GetConfig("DefaultConfigs:DefaultRole");

                        if (defaultRoleConfig == null) throw new ExceptionBase($"Default role not found in configuration", 404);
                        Task.Factory.StartNew(() => CoreLogger.LogMessage($"New user with email {userToFind.email} doesn't have role assigned, default role : {defaultRoleConfig} will be mapped"));

                        var defaultRole = await _dataContext.Roles.Where(role => role.slug == defaultRoleConfig).FirstOrDefaultAsync();

                        if (defaultRole == null) throw new ExceptionBase($"Default role not found in database", 404);

                        userFromDB.roles.Add(defaultRole);

                        await _dataContext.SaveChangesAsync(CancellationToken.None);

                    }

                    userFromDB.social_data = userToFind.social_data;

                }


                userFromDB.Initialize();

                return userFromDB;


            }
            catch (Exception ex)
            {
                ExceptionBase.HandleException(ex, $"Error logging user {controllerName}", null, transaction);

            }
            return null;

        }

        public async Task<User?> LoginNoAD(string? username, string? password, Guid? guid = null)
        {
            Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transaction = null;

            try
            {
                transaction = await _dataContext.Database.BeginTransactionAsync();

                User? model = new();

                Dictionary<string, dynamic?> userDic = new Dictionary<string, dynamic?>();

                userDic.Add("email", username);

                userDic.Add("password", password);

                User? userToFind = await TransformerBase.ToClass<User>(userDic);

                var userFromDB = await _dataContext.Users.Include("roles").Include("departments").Where(obj => obj.email == userToFind.email && obj.password == userToFind.password).FirstOrDefaultAsync();

                if (userFromDB == null)
                {
                    userToFind.external_guid = guid;

                    _dataContext.Users.Add(userToFind);

                    await _dataContext.SaveChangesAsync(CancellationToken.None);


                    var defaultRoleConfig = ConfigurationHelper.GetConfig("DefaultConfigs:DefaultRole");

                    if (defaultRoleConfig != null)
                    {
                        var addedUser = await _dataContext.Users.Where(obj => obj.email == userToFind.email && obj.password == userToFind.password).FirstOrDefaultAsync();

                        var defaultRole = await _dataContext.Roles.Where(role => role.name == defaultRoleConfig).FirstOrDefaultAsync();

                        if (defaultRole != null)
                        {
                            addedUser.roles.Add(defaultRole);

                            await _dataContext.SaveChangesAsync(CancellationToken.None);
                        }
                        else
                        {
                            throw new ExceptionBase($"Default role not found in database ", 404);
                        }

                        userFromDB = addedUser;
                    }
                    else
                    {
                        throw new ExceptionBase($"Default role not found in configuration", 404);
                    }


                }



                await transaction.CommitAsync();

                userFromDB.Initialize();

                return userFromDB;

            }
            catch (Exception ex)
            {
                ExceptionBase.HandleException(ex, $"Error logging user {controllerName}", null, transaction);

            }
            return null;
        }

        public override void CustomFilters(ref IQueryable<User> query, ref UrlRequestBase? requestBase)
        {
            if (requestBase.include != null)
            {
                requestBase.include = requestBase.include.Replace(",settings", "");
                requestBase.include = requestBase.include.Replace("settings", "");
                requestBase.include = requestBase.include.Replace("settings,", "");
            }

        }
        public override async Task SyncRelations(object? input, dynamic relations, dynamic dataContext)
        {
            //First clear current relations 
            var model = (User?)input;

            //List for storing relations given in front.
            List<long?> relationIds;

            //new dbContext is necesary for avoid strick id checking in previous context
            //Already implements IDisposable so when finish invoke gets disposed
            using (var db = new IdataContext())
            {
                ////get the model again
                var internalModel = await db.Users.Where(u => u.id == model.id).FirstOrDefaultAsync();

                //Asign relations
                if (relations.ContainsKey("departments"))
                {
                    //clear all current relations
                    model.departments.Clear();

                    await dataContext.SaveChangesAsync(CancellationToken.None);

                    relationIds = relations["departments"];

                    internalModel.departments = db.Departments.Where(dep => relationIds.Contains(dep.id)).ToList();
                }

                if (relations.ContainsKey("roles"))
                {
                    model.roles.Clear();

                    await dataContext.SaveChangesAsync(CancellationToken.None);

                    relationIds = relations["roles"];

                    internalModel.roles = db.Roles.Where(dep => relationIds.Contains(dep.id)).ToList();
                }
                //save the changes to the model
                await db.SaveChangesAsync(CancellationToken.None);

            }



        }

        public override void BeforeUpdate(ref User? common, ref UrlRequestBase? requestBase, ref BodyRequestBase? bodyRequestBase)
        {

            var token = requestBase.getCurrentContext().Request.Headers.Authorization;
            JObject options = JObject.Parse(common.options);
            string? stationAssigned = options.SelectToken("stationAssigned")?.ToString();


            //security
            bool isSameUser = common.id == requestBase.currentContextUser.id;
            dynamic? relations = common.dynamic_parameters.ContainsKey("relations") ? common.dynamic_parameters["relations"] : null;
    
            //List for storing relations given in front.
            List<long?>? relationIds;

            long? userToEditId = common.id;
            //avoid dynamic lambda exception
            var currentUserToEditValues = _dataContext.Users.Include("roles").Include("departments").Where(us => us.id == userToEditId).FirstOrDefault();

            //An user that doesn't have profile.users.edit-others permission can ONLY edit itself
            //Throw unhautorized exception if is trying to edit another user
            if (!requestBase.currentContextUser.HasAccess("profile.user.edit-others") && !isSameUser)
            {
                 throw new ExceptionBase($"{requestBase.currentContextUser.email} has tried to: edit user {common.id} without proper permissions", 403, _userId: requestBase.currentContextUser.id);
            }
            //An user that doesn't have profile.role.manage permission cannot edit any role
            //Throw unhautorized exception if is trying to edit roles
            //get previousRoles and compare with new ones comming from request
            if (!requestBase.currentContextUser.HasAccess("profile.role.manage")) 
            {
                relationIds = relations.ContainsKey("roles") ? relations["roles"] : null;

                var newUserRoles = relationIds != null ? _dataContext.Roles.Where(dep => relationIds.Contains(dep.id)).ToList() : null;

                var differentRoles = newUserRoles != null ? newUserRoles.Where(dep => !relationIds.Contains(dep.id)).ToList() : null;

                if (differentRoles != null && differentRoles.Count() > 0)
                    throw new ExceptionBase($"{requestBase.currentContextUser.email} has tried to: edit user {common.id} roles without proper permissions", 403, _userId: requestBase.currentContextUser.id);

            }
            //An user that doesn't have profile.user-departments.manage permission cannot edit any department
            //Throw unhautorized exception if is trying to edit department
            //get previousDepartments and compare with new ones comming from request
            if (!requestBase.currentContextUser.HasAccess("profile.user-departments.manage")) 
            {
                relationIds = relations.ContainsKey("departments") ? relations["departments"] : null;

                var newUserDepartments = relationIds != null ? _dataContext.Departments.Where(dep => relationIds.Contains(dep.id)).ToList() : null;

                var differentDepartments = newUserDepartments != null ? newUserDepartments.Where(dep => !relationIds.Contains(dep.id)).ToList() : null;

                if ( differentDepartments!= null && differentDepartments.Count() > 0) 
                    throw new ExceptionBase($"{requestBase.currentContextUser.email} has tried to: edit user {common.id} departments without proper permissions", 403, _userId: requestBase.currentContextUser.id);

            }
            //An user that doesn't have profile.users.edit-permissions permission cannot edit any permission
            //Throw unhautorized exception if is trying to edit permissions
            //get previousPermissions and compare with new ones comming from request
            if (!requestBase.currentContextUser.HasAccess("profile.user.edit-permissions"))
            {
                
                string newUserPermissions = common.permissions;

                if (currentUserToEditValues.permissions != newUserPermissions)
                    throw new ExceptionBase($"{requestBase.currentContextUser.email} has tried to: edit user {common.id} permissions without proper permissions", 403, _userId: requestBase.currentContextUser.id);

            }

            //An user that doesn't have profile.users.edit-options permission cannot edit any role
            //Throw unhautorized exception if is trying to edit roles
            //get previousRoles and compare with new ones comming from request
            if (!requestBase.currentContextUser.HasAccess("profile.user.edit-options"))
            {
                
                string? newUserOptions = common.options;

                if (currentUserToEditValues.options != newUserOptions)
                    throw new ExceptionBase($"{requestBase.currentContextUser.email} has tried to: edit user {common.id} options without proper permissions", 403, _userId: requestBase.currentContextUser.id);

            }





        }
        public async Task Initialize(dynamic wichContext)
        {
            await base.Initialize((object)wichContext);
        }
    }
}