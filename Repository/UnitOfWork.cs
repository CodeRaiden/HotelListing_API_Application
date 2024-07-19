using HotelListing_Api.Data;
using HotelListing_Api.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing_Api.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        // fist thing here, we will include a referrence to the database context using dependency injection
        private readonly DatabaseContext _context;

        // next we need to create private IGenericRepository class fields of the Country and Hotel Type
        private IGenericRepository<Country>? _countries;
        private IGenericRepository<Hotel>? _hotels;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }

        // here we are just going to state that if the "_countries" field is null then we want to return a new instance of the GenericRepository class of type "Country" with the "_context" Database reference
        public IGenericRepository<Country> Countries => _countries ??= new GenericRepository<Country>(_context);

        public IGenericRepository<Hotel> Hotels => _hotels ??= new GenericRepository<Hotel>(_context);

        // So we have successfully made this a register, where we have access to every table defined in the database

        // so now we can go on to provide implementation for the "Dispose" function
        // the "Dispose" function is really like a garbage collector where we will need to specify
        // that when the CRUD operations are done, the memory should be freed. 
        public void Dispose()
        {
            // So when the Disposed function is called, first thing we want to do is to dispose the database referrence connection "_context"
            _context.Dispose();

            // next we will need to make sure that the copy of the table stored in memory as well as the instance referrence itself, is Garbage Collected by the Garbage Collector "GC"
            GC.SuppressFinalize(this);
        }

        // To implement the Task Save function first as with every implementation of a "Task" function, we need to include "async" in the method identity definition
        public async Task Save()
        {
            // here we will save all the staged CRUD operations after the changes hae been pushed to the database
            await _context.SaveChangesAsync();
        }
    }
}
