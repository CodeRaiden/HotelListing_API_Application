using HotelListing_Api.Data;
using HotelListing_Api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// Old, so Ignore
//namespace HotelListing_Api.Data
//{
//    public class DatabaseContext : DbContext
//    {
//        // Eighth
//        // then here we will include a constructor for the class which will itake in a parameter
//        // of type "DbContextOptions"
//        // we will also provide an argument to the constructor of the inherited parent "DbContext" class
//        // here in the child class, by passing in the parameter of type "DbContextOptions" also
//        // into it.

//        public DatabaseContext(DbContextOptions options) : base (options)
//        {

//        }

//        // Nineth
//        // It's here we will actually list out what the database should know about when it is being
//        // generated

//        // first we state here below that we want in the remote Database table, a table("DbSet")
//        // of type "Country" and we will set the name of the table to countries
//        public DbSet<Country> Countries { get; set; }

//        public DbSet<Hotel> Hotels { get; set; }

//        // Tenth
//        // After setting the table fields above, we will need to let our "appsettings.json" know
//        // about the connection string that that outlines how the DatabaseContext gets to/connects to
//        // the remote microsoftsql server database
//    }
//}




//// To be done in the program.cs file but is Old, so Ignore
///
//// Since the DatabaseContext will be fed the argument to it's options parameter from the
//// "program.cs", then the "program.cs" file needs to know that when the application starts up,
//// the appliction should be loading it's database configuration/options
//// from "program.cs" file, using the DatabaseContxt.cs file as the bridge.
//builder.Services.AddDbContext<DatabaseContext>(options =>
//    // the first option is to tell the application to use the SqlServer connection String
//    // defined in the appsettings.json file
//    options.UseSqlServer("sqlConnection")
//);
//// then we will move the controller service below to be the last service added
//// builder.Services.AddControllers();



//// DatabaseContext code without authentication
//{
//    public class DatabaseContext : DbContext
//    {
//        // Eighth
//        // then here we will include a constructor for the class which will itake in a parameter
//        // of type "DbContextOptions"
//        // we will also provide an argument to the constructor of the inherited parent "DbContext" class
//        // here in the child class, by passing in the parameter of type "DbContextOptions" also
//        // into it.
//        public DatabaseContext(DbContextOptions options) : base(options)
//        { }

//        // Fourteenth
//        // SEEDING DATA INTO THE DATABASE
//        // what we will do here basically is just hard code some records such that
//        // when we perform a migration afterwards the migration will have instructions
//        // to create these records, update the database with these records, and won't
//        // necessary need to rely on user input.
//        // at least this will provide nice basis for our testing when we start developing
//        // the API endpoint.
//        // So to do all this, first here we will have to override a protected method that is inside of the DbContext called "OnModelCreating()"
//        protected override void OnModelCreating(ModelBuilder builder)
//        {
//            // so here for the data seeding, we will build and Entity of type "Country" in the database using the ModelBuilder class parameter (builder) "Entity" collection and then set the type to "Country"
//            // Note that we are building the Country instead of the Hotel entity, because according to what we specified in our data i.e. we need to have a Country to have a Hotel, and this is why we used the Hotel
//            // as the ForeignKey in the Country table.
//            builder.Entity<Country>().HasData(
//                // The "HasData" block here takes in an array. so we will define as many Country objects/instances as needed
//                new Country
//                {
//                    Id = 1,
//                    Name = "Jamaica",
//                    ShortName = "JAM"
//                },
//                new Country
//                {
//                    Id = 2,
//                    Name = "Bahamas",
//                    ShortName = "BAH"
//                },
//                new Country
//                {
//                    Id = 3,
//                    Name = "Cayman Islands",
//                    ShortName = "CAI"
//                }
//                );

//            // Now that we are done with the seeding of data into the Country table, we can go on
//            // to seed data into the ForeingnKey Table "Hotel"
//            builder.Entity<Hotel>().HasData(
//                // The "HasData" block here takes in an array. so we will define as many Country objects/instances as needed
//                new Hotel
//                {
//                    // Jamaica
//                    Id = 1,
//                    Name = "Sandals Resort and Spa",
//                    Address = "Negril",
//                    CountryId = 1,
//                    Rating = 4.3,
//                },
//                new Hotel
//                {
//                    // Caymen Islands
//                    Id = 2,
//                    Name = "Comfort Suits",
//                    Address = "George Town",
//                    CountryId = 3,
//                    Rating = 4.5,
//                },
//                new Hotel
//                {
//                    // Bahamas
//                    Id = 3,
//                    Name = "Grand Palladium",
//                    Address = "Nassua",
//                    CountryId = 2,
//                    Rating = 4,
//                }
//                );
//            // After seeding in the data the next thing for us to do will be add a Migration in the PMC using the code below
//            // Add-Migration SeedingData
//            // the next thing we need to do is to update the database with the added/set Migrations using the code below
//            // Update-Database
//            // then we can navigate back to our "SQL Server Object Explorer" and there we can navigate to the database tables and right click on each one and click on the "View Data"
//        }

//        // Nineth
//        // It's here we will actually list out what the database should know about when it is being
//        // generated

//        // first we state here below that we want in the remote Database table, a table("DbSet")
//        // of type "Country" and we will set the name of the table to countries
//        public DbSet<Country> Countries { get; set; }

//        public DbSet<Hotel> Hotels { get; set; }

//    }
//}





//// Seventeenth
//// SECURING YOUR API
//// the first security we will be implementing will be for Authentication, which we will be implementing
//// in the DatabaseContext.cs file
//// in order to implement the authentication we will need to modify our DatabaseContext code here
//// but first we will install the package "Microsoft.AspNetCore.Identity.EntityFrameWorkCore"
namespace HotelListing_Api.Data
{
    // the first update we will need to make in the DatabaseContext is to make the DatabaseContext
    // inherit from the IdentityDbContext class

    // And in returning here after the creation of the ApiUser class, we make sure that the inherited
    // class "IdentityDbContext" is of type "ApiUser", this means the "IdentityDbContex" is relative to the
    // ApiUser class. Now we did this because of the added fields of the IdentityUser child "ApiUser" class
    // But in a situation where we do not want to add extra fields to the IdentityUser aready specified fields,
    // then would not need to add a <ralation> to the "IdentityUser" class the DatabaseContext is inheriting from.
    // But still either way, this extra step is still important to be included as latter on when you feel like including
    // a new table/field in the database, you can always do so.

    // And so after including the ApiUser relation below, we can go back to the program.cs file to configure our "Identity Services"
    public class DatabaseContext : IdentityDbContext<ApiUser>
    {
        // Eighth
        // then here we will include a constructor for the class which will itake in a parameter
        // of type "DbContextOptions"
        // we will also provide an argument to the constructor of the inherited parent "DbContext" class
        // here in the child class, by passing in the parameter of type "DbContextOptions" also
        // into it.
        public DatabaseContext(DbContextOptions options) : base(options)
        { }

        // Fourteenth
        // SEEDING DATA INTO THE DATABASE
        // what we will do here basically is just hard code some records such that
        // when we perform a migration afterwards the migration will have instructions
        // to create these records, update the database with these records, and won't
        // necessary need to rely on user input.
        // at least this will provide nice basis for our testing when we start developing
        // the API endpoint.
        // So to do all this, first here we will have to override a protected method that is inside of the DbContext called "OnModelCreating()"
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // the next update to the DatabaseContext will be here in the overriden OnModelCreating() function in the child "DatabaseContext"
            // here we will also execute the parent "base" OnModelCreating() function in the overriden child class function
            // which will also expect the database builder parameter to be passed in also
            base.OnModelCreating(builder);
            // after we've gotten this done, we can then go over to the "program.cs" file and start making our modifications there


            // so here after creating the CountryConfiguration file in the Entity folder, we can just simply
            // add the Country builder Configuration
            builder.ApplyConfiguration(new CountryConfiguration());
            // we will also refactor the code for seeding the Hotel also

            // Now that we are done with the seeding of data into the Country table, we can go on
            // to seed data into the ForeingnKey Table "Hotel"
            builder.ApplyConfiguration(new HotelConfiguration());

            // SEEDING THE ROLES INTO THE DATABASE
            // in order to seed the data into the database so that it exist prior to the system being used,
            // we will just include the builder Configuration to seed the Roles Entity here in the DatabaseContext.cs file 
            builder.ApplyConfiguration(new RoleConfiguration());
            // This makes the file a whole lot neater
            // we will go on do the same with the Country and Hotel Entities as we did with the Roles Entity
            // i.e including/defining the Entities in Separate files inside the "Entities" folder
            // and then include them their builder configuration here in the Database.cs file

            // After seeding in the data the next thing for us to do will be to add a Migration in the PMC using the code below
            // Add-Migration AddedDefaultRoles
            // the next thing we need to do is to update the database with the added/set Migrations using the code below
            // Update-Database
            // then we can navigate back to our "SQL Server Object Explorer" and there we can navigate to the database tables and right click on each one and click on the "View Data"
            // So once we have included the Migration for the Roles, there are two more changes to be made:
            // the first is that we need to navigate to the UserDTO.cs file and there we will need to include a get and set field for ICollection of type "<string>" called "Roles"
            // so we can give the user the opportunity to select which role or roles to have.
            // The second change would be to navigate to the AccountController.cs file, and then after
            // the check for whether the entered user model is correct, we will then add the user Role to the successful user model just before returning the "202 Accepted" success response
        }

        // Nineth
        // It's here we will actually list out what the database should know about when it is being
        // generated

        // first we state here below that we want in the remote Database table, a table("DbSet")
        // of type "Country" and we will set the name of the table to countries
        public DbSet<Country> Countries { get; set; }

        public DbSet<Hotel> Hotels { get; set; }

    }
}





//namespace HotelListing_Api.Data
//{
//    public class DatabaseContext : DbContext
//    {
//        // here we will declare the Configuration field of type "IConfiguration"
//        protected readonly IConfiguration _configuration;

//        // then here we will include a constructor for the class which will take in a parameter
//        // of type "IConfiguration"
//        // we will also provide an argument to the constructor of the inherited parent "DbContext" class
//        // here in the child class, by passing in the parameter of type "DbContextOptions" also
//        // into it.
//        public DatabaseContext(IConfiguration configuration)
//        {
//            // here we will set the Configuration field value as the passed in configuration parameter
//            _configuration = configuration;
//        }

//        protected override void OnConfiguring(DbContextOptionsBuilder options)
//        {
//            // connect to sql server with connection string from app settings
//            options.UseSqlServer(_configuration.GetConnectionString("sqlConnection"));
//        }

//        // Fourteenth
//        // SEEDING DATA INTO THE DATABASE
//        // what we will do here basically is just hard code some records such that
//        // when we perform a migration afterwards the migration will have instructions
//        // to create these records, update the database with these records, and won't
//        // necessary need to rely on user input.
//        // at least this will provide nice basis for our testing when we start developing
//        // the API endpoint.
//        // So to do all this, first here we will have to override a protected method that is inside of the DbContext called "OnModelCreating()"
//        protected override void OnModelCreating (ModelBuilder builder)
//        {
//            // so here for the data seeding, we will build and Entity of type "Country" in the database using the ModelBuilder class parameter (builder) "Entity" collection and then set the type to "Country"
//            // Note that we are building the Country instead of the Hotel entity, because according to what we specified in our data i.e. we need to have a Country to have a Hotel, and this is why we used the Hotel
//            // as the ForeignKey in the Country table.
//            builder.Entity<Country>().HasData(
//                // The "HasData" block here takes in an array. so we will define as many Country objects/instances as needed
//                new Country
//                {
//                    Id = 1,
//                    Name = "Jamaica",
//                    ShortName = "JAM"
//                },
//                new Country 
//                { 
//                    Id =2,
//                    Name = "Bahamas",
//                    ShortName = "BAH"
//                },
//                new Country 
//                { 
//                    Id = 3,
//                    Name = "Cayman Islands",
//                    ShortName = "CAI"
//                }
//                ) ;

//            // Now that we are done with the seeding of data into the Country table, we can go on
//            // to seed data into the ForeingnKey Table "Hotel"
//            builder.Entity<Hotel>().HasData(
//                // The "HasData" block here takes in an array. so we will define as many Country objects/instances as needed
//                new Hotel
//                {
//                    // Jamaica
//                    Id = 1,
//                    Name = "Sandals Resort and Spa",
//                    Address = "Negril",
//                    CountryId = 3,
//                    Rating = 4.3,
//                },
//                new Hotel
//                {
//                    // Caymen Islands
//                    Id = 2,
//                    Name = "Comfort Suits",
//                    Address = "George Town",
//                    CountryId = 3,
//                    Rating = 4.5,
//                },
//                new Hotel
//                {
//                    // Bahamas
//                    Id = 3,
//                    Name = "Grand Palladium",
//                    Address = "Nassua",
//                    CountryId = 2,
//                    Rating = 4,
//                }
//                );
//            // After seeding in the data the next thing for us to do will be add a Migration in the PMC using the code below
//            // Add-Migration SeedingData
//            // the next thing we need to do is to update the database with the added/set Migrations using the code below
//            // Update-Database
//            // then we can navigate back to our "SQL Server Object Explorer" and there we can navigate to the database tables and right click on each one and click on the "View Data"
//        }
//        public DbSet<Country> Countries { get; set; }



//        public DbSet<Hotel> Hotels { get; set; }
//    }
//}
