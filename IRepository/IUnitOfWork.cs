using HotelListing_Api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing_Api.IRepository
{
    // The "IUnitOfWork" file is going to act as a register for every variation of the "GenericRepository" relative to the class/table/type <T>
    // The IUnitOfWork file must inherit from the IDisposable Interface
    public interface IUnitOfWork : IDisposable
    {
        // So here we will write the unit of work for both class variations (Country and Hotel).
        
        // Unit of work for the "GenericRepository" relative to the "Country" class/type
        IGenericRepository<Country> Countries { get; }

        // Unit of work for the "GenericRepository" relative to the "Hotel" class/type
        IGenericRepository<Hotel> Hotels { get; }

        // then lastly here we will include the functionality to save the CRUD operation "Tasks" in the "UnitOfWork" file (which we will create)
        // instead of including it multiple times in the "GenericRepository" at the end of every operation
        // we can get simply include it here once to be called after the CRUD operations have been staged
        // and then theis will make sure that the "save Tasks" is called only once after every staged operation
        // is pushed to the database.
        Task Save();
    }
}

// So after creating the Interface for the Unit of work, Next we will need to navigate to the "Repository" folder to
// create the concrete class "UnitOfWork" and also provide implementations for the Interface "IUnitOfWork".

