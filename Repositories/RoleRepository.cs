using Core.Repositories;
using Idata.Data;
using Idata.Data.Entities.Iprofile;
using Iprofile.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Iprofile.Repositories
{
    public class RolesRepository : RepositoryBase<Role>, IRolesRepository
    {


        private const string controllerName = "Roles";
        public RolesRepository() 
        {
            //Dependency injection of dataContext and logger

        }
        #region oldCode
        //public async Task<List<object>> GetItemsBy(UrlRequestBase? requestBase)
        //{
        //    PaginatedList<object>? resultList = null;

        //    try
        //    {

        //        //Add the relation tables

        //        //query = query.Include("translations");

        //        //Verify the filters that are present in requestBase
        //        string field = requestBase.getFilter("field");

        //        //Base query of the entity
        //        var query = _dataContext.Roles.Where($"obj => obj.{field}.Contains(@0)", requestBase.criteria);


        //        //Get includes and apply them
        //        query = requestBase.getIncludes(query);

        //        //if (requestBase.InternalFilter() != null)
        //        //{
        //        //    //Try get the search filter
        //        //    var searchFilter = (requestBase.InternalFilter().SelectToken("search"))?.ToString();
        //        //    //update the query for search filter include
        //        //    if (searchFilter != null)
        //        //    {
        //        //        query = query.Where(modelDB => modelDB.slug.Contains(searchFilter));
        //        //        query = query.Where(modelDB => modelDB.name.Contains(searchFilter));
        //        //    }
        //        //}
        //        //Use of PaginatedList to get the items and the meta of them
        //        resultList = await PaginatedList<object>.CreateAsync(query, requestBase.page, requestBase.take);

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionBase.HandleException(ex, $"Error obtaining list of {controllerName}");
        //    }

        //    return resultList;

        //}

        //public async Task<object?> GetItem(UrlRequestBase? requestBase)
        //{
        //    Role? model = null; //object to be returned
        //    try
        //    {



        //        string field = requestBase.getFilter("field");





        //        //Create base query based on criteria and field
        //        var query = _dataContext.Roles.Where($"obj => obj.{field} == @0", requestBase.criteria);

        //        //TODO inyect the include parameters as given in front
        //        query = requestBase.getIncludes(query);

        //        model = await query.FirstOrDefaultAsync();

        //        //if model is null (not found) throw exception
        //        if (model == null) throw new ExceptionBase($"Item with id {field} {requestBase.criteria} not found", 400);
        //        //if the model is valid return the item
        //        return (object)model;

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionBase.HandleException(ex, $"Error obtaining {controllerName}");

        //    }
        //    return model;
        //}

        //public async Task<object?> Create(UrlRequestBase? requestBase, BodyRequestBase? bodyRequestBase)
        //{
        //    //Transaction must be declared here because throwing exceptions need to rollback the transaction
        //    Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transaction = null;

        //    Role? common = null;

        //    try
        //    {

        //        //Convert the dictionary to the required class
        //        common = await TransformerBase.ToClass<Role>(bodyRequestBase._attributes);

        //        //model validations
        //        if (common == null || string.IsNullOrEmpty(common.slug) || string.IsNullOrEmpty(common.name))
        //        {
        //            throw new ExceptionBase($"Error parsing object {controllerName}", 404);
        //        }
        //        //Begin the transaction
        //        transaction = await _dataContext.Database.BeginTransactionAsync();

        //        //save the model in database and commit transaction
        //        await _dataContext.Roles.AddAsync(common);

        //        await _dataContext.SaveChangesAsync(CancellationToken.None);



        //        if (common.dynamic_parameters.Count > 0)
        //        {
        //            dynamic? relations = null;

        //            common.dynamic_parameters.TryGetValue("relations", out relations);

        //            if (relations != null)
        //            {

        //                IQueryable<Role> query = _dataContext.Roles.Where(obj => obj.id == common.id);

        //                if (relations != null)
        //                {
        //                    foreach (dynamic relation in relations)
        //                    {
        //                        string relationString = relation.Key.ToString();
        //                        query = query.Include(relationString);
        //                    }
        //                }

        //                //Existen relaciones
        //                Role model = await query.SingleOrDefaultAsync();

        //                await syncRelations(model, relations, _dataContext);


        //            }


        //        }





        //        await transaction.CommitAsync();


        //        return common;

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionBase.HandleException(ex, $"Error creating {controllerName}", transaction);
        //    }
        //    return common;

        //}

        //public async Task<object?> UpdateBy(UrlRequestBase? requestBase, BodyRequestBase? bodyRequestBase)
        //{
        //    //Transaction must be declared here because throwing exceptions need to rollback the transaction
        //    Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transaction = null;

        //    Role? common = null;

        //    try
        //    {

        //        //Convert the json to the required class without adding relations (EF way to work)
        //        common = await TransformerBase.ToClass<Role?>(bodyRequestBase._attributes);

        //        //model validations
        //        if (common == null || string.IsNullOrEmpty(common.slug) || string.IsNullOrEmpty(common.name))
        //        {
        //            throw new ExceptionBase($"Error parsing object {controllerName}", 404);
        //        }

        //        //Begin the transaction
        //        transaction = await _dataContext.Database.BeginTransactionAsync();


        //        //get the model that will be updated
        //        var modelToUpdate = await _dataContext.Roles.Where(obj => obj.id == common.id).SingleOrDefaultAsync();


        //        //if the model is null then throw exception and rollback transaction if not then update
        //        if (modelToUpdate != null)
        //        {
        //            //set update model values, save changes and commit transaction
        //            _dataContext.Entry(modelToUpdate).CurrentValues.SetValues(common);

        //            if (common.dynamic_parameters.Count > 0)
        //            {
        //                dynamic? relations = null;

        //                common.dynamic_parameters.TryGetValue("relations", out relations);

        //                if (relations != null)
        //                {
        //                    await syncRelations(modelToUpdate, relations, _dataContext);


        //                }


        //            }
        //            await _dataContext.SaveChangesAsync(CancellationToken.None);

        //            await transaction.CommitAsync();
        //        }
        //        else
        //        {
        //            throw new ExceptionBase($"Item with id {common.id} not found", 404);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        ExceptionBase.HandleException(ex, $"Error updating {controllerName}", transaction);

        //    }

        //    return common;

        //}
        //public async Task<object?> DeleteBy(UrlRequestBase? requestBase)
        //{
        //    //Transaction must be declared here because throwing exceptions need to rollback the transaction
        //    Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transaction = null;
        //    try
        //    {
        //        //Begin the transaction
        //        //Try get the search filter
        //        string field = requestBase.getFilter("field");
        //        //Begin the transaction
        //        transaction = await _dataContext.Database.BeginTransactionAsync();
        //        //get the model that will be removed
        //        var modelToRemove = await _dataContext.Roles.Where($"obj => obj.{field} == @0", requestBase.criteria).FirstOrDefaultAsync();
        //        //if the model is null then throw exception and rollback transaction if not then delete
        //        if (modelToRemove != null)
        //        {
        //            //delete the model from DB, save changes and commit transaction
        //            _dataContext.Roles.Remove(modelToRemove);
        //            await _dataContext.SaveChangesAsync(CancellationToken.None);
        //            await transaction.CommitAsync();
        //        }
        //        else
        //        {
        //            throw new ExceptionBase($"Item with id {modelToRemove.id} not found", 404);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionBase.HandleException(ex, $"Error deleting {controllerName}", transaction);
        //    }

        //    return null;

        //} 
        #endregion


        public override async Task SyncRelations(object? input, dynamic relations, dynamic dataContext)
        {
            //First clear current relations 
            var model = (Role?)input;

            //List for storing relations given in front.
            List<long?> relationIds;

           
            //new dbContext is necesary for avoid strict object tracking in previous context
            //Already implements IDisposable so when finish invoke gets disposed

            using (var db = new IdataContext())
            {
                //get the model again
                var internalModel = await db.Roles.Where(rol => rol.id == model.id).FirstOrDefaultAsync();

                //Asign relations
                if (relations.ContainsKey("users"))
                {

                    //clear all current relations
                    model.users.Clear();

                    await dataContext.SaveChangesAsync(CancellationToken.None);

                    relationIds = relations["users"];

                    internalModel.users = _dataContext.Users.Where(dep => relationIds.Contains(dep.id)).ToList();
                }

                //save the changes to the model
                await db.SaveChangesAsync(CancellationToken.None);

            }


        }


        public async Task Initialize(dynamic wichContext)
        {
            await base.Initialize((object)wichContext);
        }

    }
}