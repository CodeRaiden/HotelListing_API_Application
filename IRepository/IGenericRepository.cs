using HotelListing_Api.Models;
using System.Linq.Expressions;
using X.PagedList;

namespace HotelListing_Api.IRepository
{
    // so what we will do here is that we are going to make teh IGenericRepository take in a Generic Parameter <T>, where T is going to be a class
    // and that is actually the point with generics, as "<T>" signifies that this is going to take/accept any Class/Type passed into it.
    // And this helps us in creating the base operations for different Types
    public interface IGenericRepository<T> where T : class
    {
        // So here We are going to have similar functions for our CRUD Operations

        // GET
        // The first one we will be creating here is the functionality "GetAll", to get a list of all entries of the Table/Type
        Task<IList<T>> GetAll(
            Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<string> includes = null
            );

        // HANDLING PAGINATION
        // Adding another Task GetAll() method of <IPagedList> of Type <T> to handle the RequestParams for the GetCountries() method in the CountryCountroller
        // this will take in the "includes" and the "requstParams" as arguments
        Task<IPagedList<T>> GetPagedList(
            RequestParams requsetParams,
            List<string> includes = null);
        // then we go and implement this in the GenericRepository.cs file

        // We will also include a follow up functionality which will be responsible for retrieving only a single entry from the entered Table/Type
        Task<T> Get(Expression<Func<T, bool>> expression, List<string>? includes = null);

        // CREATE
        Task Insert(T entity);

        // and a follow up functionality for creating a list/range of entries at a time
        Task InsertRange(IEnumerable<T> entities);

        // DELETE
        Task Delete(int id);

        // a follow up Void functionality to delete a set of records
        void DeleteRange(IEnumerable<T> entities);

        // UPDATE
        // the update functionality will also be a Void functionality just like the DeleteRange
        void Update(T entity);

        // So basically this single file will be able to receive different Table/Class/Types and perform CRUD operations on them
        // Now that we have the Interface set up, we then then go on to create the concrete class "GenericRepository"
        // We will create the "GenericRepository" class in the "Repository" folder
    }
}
