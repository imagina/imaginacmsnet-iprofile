using Core.Repositories;
using Idata.Data;
using Idata.Data.Entities.Iprofile;
using Idata.Data.Entities.Iprofile;
using Iprofile.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Iprofile.Repositories
{

    public class DepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
    {

        private const string controllerName = "Department";

        public DepartmentRepository() 
        {

        }
        #region oldCode
        //public async Task<List<object>> GetItemsBy(UrlRequestBase? requestBase)
        //{
        //    PaginatedList<object>? resultList = null;

        //    IQueryable<Department> query = _dataContext.Departments;

        //    try
        //    {
        //        //Create the base query for entity
        //        //Add the relation tables
        //        //query = query.Include("translations");

        //        //Get includes and apply them
        //        query = requestBase.getIncludes(query);


        //        //Filter Section
        //        if (requestBase.getFilter("search") != null)
        //        {
        //            query = query.Where($"obj => obj.title.Contains(@0) || obj.id == @0", requestBase.getFilter("search"));
        //        }


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
        //    Department? model = null; //object to be returned
        //    try
        //    {
        //        //Try get the search filter
        //        string field = requestBase.getFilter("field");

        //        //Create base query based on criteria and field
        //        var query = _dataContext.Departments.Where($"obj => obj.{field} == @0", requestBase.criteria);

        //        //TODO inyect the include parameters as given in front
        //        query = requestBase.getIncludes(query);

        //        //get the model with given criteria
        //        model = await query.FirstOrDefaultAsync();

        //        //if model is null (not found) throw exception
        //        if (model == null) throw new ExceptionBase($"Item with {field} {requestBase.criteria} not found", 404);
        //        //if the model is valid return the item
        //        return (object)model;

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionBase.HandleException(ex, $"Error creating {controllerName}");

        //    }
        //    return model;
        //}

        //public async Task<object?> Create(UrlRequestBase? requestBase, BodyRequestBase? bodyRequestBase)
        //{
        //    //Transaction must be declared here because throwing exceptions need to rollback the transaction
        //    Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transaction = null;

        //    Department? common = null;

        //    try
        //    {

        //        //Deserialize object that will be created into a model
        //        common = await TransformerBase.ToClass<Department>(bodyRequestBase._attributes);

        //        //model validations
        //        if (common == null || string.IsNullOrEmpty(common.title))
        //        {
        //            throw new ExceptionBase($"Error parsing object {controllerName}", 404);
        //        }

        //        //Begin the transaction
        //        transaction = await _dataContext.Database.BeginTransactionAsync();

        //        //save the model in database and commit transaction
        //        await _dataContext.Departments.AddAsync(common);

        //        await _dataContext.SaveChangesAsync(CancellationToken.None);


        //        if (common.dynamic_parameters.Count > 0)
        //        {
        //            dynamic? relations = null;

        //            common.dynamic_parameters.TryGetValue("relations", out relations);

        //            if (relations != null)
        //            {

        //                IQueryable<Department> query = _dataContext.Departments.Where(obj => obj.id == common.id);

        //                if (relations != null)
        //                {
        //                    foreach (dynamic relation in relations)
        //                    {
        //                        string relationString = relation.Key.ToString();
        //                        query = query.Include(relationString);
        //                    }
        //                }

        //                //Existen relaciones
        //                Department model = await query.SingleOrDefaultAsync();

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

        //    Department? common = null;

        //    try
        //    {


        //        //Convert the json to the required class without adding relations (EF way to work)
        //        common = await TransformerBase.ToClass<Department?>(bodyRequestBase._attributes);

        //        //Begin the transaction
        //        transaction = await _dataContext.Database.BeginTransactionAsync();

        //        //model validations
        //        if (common == null || string.IsNullOrEmpty(common.title))
        //        {
        //            throw new ExceptionBase($"Error parsing object {controllerName}", 400);
        //        }
        //        //TODO URGENTTTT! TEST THIS FUNCTIONALITY

        //        //Try get the search filter
        //        string field = requestBase.getFilter("field");

        //        //Create base query based on criteria and field
        //        var query = _dataContext.Departments.Where($"obj => obj.{field} == @0", requestBase.criteria);


        //        //get the model that will be updated
        //        var modelToUpdate = await query.FirstOrDefaultAsync();


        //        //if the model is null then throw exception and rollback transaction if not then update
        //        if (modelToUpdate != null)
        //        {
        //            ////mi novia licuando...
        //            //var User = AuthUser();
        //            //if (!User.allPermissions["iprofile.users.change-permissions"])
        //            //{
        //            //    bodyRequestBase.attributes["permissions"] = [];
        //            //}

        //            //set update model values, save changes and commit transaction
        //            _dataContext.Entry(modelToUpdate).CurrentValues.SetValues(common);

        //            //Sync Relations
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
        //        //Try get the search filter
        //        string field = requestBase.getFilter("field");

        //        //Begin the transaction
        //        transaction = await _dataContext.Database.BeginTransactionAsync();

        //        //get the model that will be removed
        //        var modelToRemove = await _dataContext.Departments.Where($"obj => obj.{field} == @0", requestBase.criteria).FirstOrDefaultAsync();

        //        //if the model is null then throw exception and rollback transaction if not then delete
        //        if (modelToRemove != null)
        //        {
        //            //delete the model from DB, save changes and commit transaction
        //            _dataContext.Departments.Remove(modelToRemove);

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
            var model = (Department?)input;

            //List for storing relations given in front.
            List<long?> relationIds;

            //clear all current relations
          
            //new dbContext is necesary for avoid strict object tracking in previous context
            //Already implements IDisposable so when finish invoke gets disposed

            using (var db = new IdataContext())
            {
                //get the model again
                var internalModel = await db.Departments.Where(dep => dep.id == model.id).FirstOrDefaultAsync();

                //Asign relations
                if (relations.ContainsKey("users"))
                {
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