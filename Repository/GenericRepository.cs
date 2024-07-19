using HotelListing_Api.Data;
using HotelListing_Api.IRepository;
using HotelListing_Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;
using X.PagedList;

namespace HotelListing_Api.Repository
{
    // The "GenericRepository" Class will inherit from the "IGenericRepository" and provide implementations for it's members
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        // So here basically we are going to do some amount of Dependency Injection by creating two private readonly fields (since they only need to be read/executed here in the GenericRepository folder)
        // The first is of type DatabaseContext which we will call "_context" what this does is that it will help 'reference' the link to the instantiated remote Database table which already exists inside the DatabaseContext.cs file here.
        private readonly DatabaseContext? _context;

        // The second will be of type DbSet, which we will call "_db". and this will help create/encapsulate the queried database Set from the remote Database Table/Class/Type(i.e Country or Hotel depending on which is being querried), making it available here in the GenericRepository.cs file so that we can perform CRUD operations on the Table/Class/Type Set
        private readonly DbSet<T>? _db;

        // Now after creating the readonly fields, we can go on to create the dependency injection Constructor in order to inject the dependencies from the "DatabaseContext.cs" file (i.e the database instantiation reference parameter "context", and the "db" parameter for holding the querried dataset we will get from the database Type/Class/Table value present in the "context" parameter)
        public GenericRepository (DatabaseContext context)
        {
            // so now here we will inject the intantiated database which is defined in the "DatabaseContext.cs" file here in the "GenericRepository.cs" file, through the DatabaseContext.cs dependency "context" parameter value.
            // By "Injecting" the "context" parameter into the "GenericRepository.cs" defined DatabaseContext field "_context"
            _context = context;
            // so now that we have successfully injected the DatabaseContext dependency here in the "GenericRepository.cs" file,
            // the next thing we will need to do is to make a copy of the remote database Set and store it in our local memory to be referrenced by the "_db" field of type "DbSet<>" during the querying of the database when performing the CRUD operations
            // we will do this by simply saying, "from the referenced remote database link stored in the "_context" field, we want a "Set" of the entered database Type/Class/Table (i.e Country or Hotel depending on which is being querried)", as done in the code below
            _db = _context.Set<T>();
        }

        // when providing implementations for "Task" memebers, Note that these use Asynchronous programming
        // and so we will always need to specify the "async"(which signifies that this begins the execution of this block in another thread while the current execution is given to the UI) in the method's identity definition
        public async Task Delete(int id)
        {
            // here we will make the program/application line code execution to await until the record/entry with the id is found
            //var entity = _db != null ? await _db.FindAsync(id) : null;
            var entity = await _db.FindAsync(id);
            // and then when the awaited line code is done executing, then the program/application line code execution can progress to the next lne of code below.
            // then here below we will remove the record/entry from the db if found
            _db.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            // this is pretty straight forward here, we willl just need to remove the entered list of entities from the database
            _db.RemoveRange(entities);
        }

        public async Task<T> Get(Expression<Func<T, bool>> expression, List<string>? includes = null)
        {
            // here we will create an IQueryable list to hold all the class/type/table "<T>" (Hotel or Country) records in a list to be queried
            IQueryable<T> query = _db;

            // then we will query the list by checking if the "includes" (which is used to add the foreignkey e.g "CountryId", This can helps us to query the details in both tables at the same time. Thereby allowing us to make two database calls in a single query) is not null,
            // then we want to include each included properties in the list also
            // Note that this "if" code block will run according to the number of foreign keys added in the includes parameter
            if (includes != null)
            {
                foreach (var includedProperty in includes)
                {
                    query = query.Include(includedProperty);
                }
                    
            }

            // the next thing we want to do here is return the all gotten queried information as "NoTracking", now this is simply because the
            // any record retrieved here is not being checked/tracked for changes/updates.
            // this is because a copy is simply taken from the database and sent into memory and then finally to the client. The database and entity frame work dont
            // really care about it.
            // and then we want to get firstordefaultasync by the expression 
            // return await query.AsNoTracking().FirstOrDefaultAsync(expression);
            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<string> includes = null)
        {
            // here we will create a list to hold the class/type/table (Hotel or Country) in a list to be queried
            IQueryable<T> query = _db;

            // next thing we will like to do here is to first filter the query first before we even check the "includes"
            // we will do this by checking if the expression(which represents a lambda expression e.g "hotel => CountryId = 1" i.e. a list of hotels where countryId is 1 etc) is not null 
            if (expression != null)
            {
                // So then if the expresion is present, then we want to filter the collection "query"
                // to be only those which contain expressions
                query = query.Where(expression);
            }

            // then we will query the list by checking if the "includes" (which is used to add the foreignkey e.g CountryId This can helps us to query the details in both tables at the same time) is not null,
            // then we want to include each included properties in the list also
            // Note that this "if" code block will run according to the number of foreign keys added in the includes parameter
            if (includes != null)
            {
                foreach (var includedProperty in includes)
                {
                    query = query.Include(includedProperty);
                }

            }

            // next here we will need to implement the order in which the information is requested in if specified
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // the next thing we want to do here is return the all gotten queried information as "NoTracking", now this is simply because the
            // any record retrieved here is not being checked/tracked for changes/updates.
            // this is because a copy is simply taken from the database and sent into memory and then finally to the client. The database and entity frame work dont
            // really care about it.
            // but here in this case really we do not want to use the "firstordefaultasync" but instead we want to make it go to a list using the TolistAsync() method
            return await query.AsNoTracking().ToListAsync();
        }

        // HANDLING PAGINATION
        // Adding another Task GetAll() method of <IPagedList> of Type <T> (which we will call "GetPagedList") to handle the RequestParams for the GetCountries() method in the CountryCountroller
        // this will take in the "includes" and the "requstParams" as arguments

        // to take care of the red squiddly lines beneath the "IPagedList", we will need to install the Nuget package
        // "X.PagedList.Mvc.Core"
        public async Task<IPagedList<T>> GetPagedList(RequestParams requestParams, List<string> includes = null)
        {
            IQueryable<T> query = _db;

            // This is similar to the GetAll() method but without the expression and the orderBy codes

            if (includes != null)
            {
                foreach (var includedProperty in includes)
                {
                    query = query.Include(includedProperty);
                }

            }

            // here the IpagedList allows us to get the data in a PagedList by using the ToPagedListAsync() as seen below
            // the "ToPagedListAsync()" takes in two parameters; the first is the pageNumber, and the second is the pageSize
            // and these two are located in the RequestParams Class/Type
            return await query.AsNoTracking().ToPagedListAsync(requestParams.PageNumber, requestParams.PageSize);
        }

        public async Task Insert(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            // here we are going to do two things
            // the first is we are going to instruct the execution to check/track the class type entity against the gotten table/class/type Set "_db" to see if there is any difference between the updated entity and the gotten table Set, by making use of the Attach() method
            _db.Attach(entity);
            // then if a difference exists, we will update the State of the record/table/class in the remote Database with the passed in entity
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}

// So now that we have gotten majority of the work out of the way by creating the Repository files
// we will need to create the "IUnitOfWork" file inside the "IRepository" folder
// The "IUnitOfWork" file is going to act as a register for every variation of the generic repository relative to the class/table/type <T>

