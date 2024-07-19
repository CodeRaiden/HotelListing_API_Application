// Second
// we make sure that the using statement for serilog library is included
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.AspNetCore;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListing_Api.Data;
using Microsoft.EntityFrameworkCore;
using HotelListing_Api.Configurations;
using HotelListing_Api.IRepository;
using HotelListing_Api.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using HotelListing_Api;
using HotelListing_Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.X86;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// First
// include serilog in the http request pipeline
builder.Host.UseSerilog();

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Twelveth
// Now we will go and include the fields for our Country and Hotel classes in the
// Country.cs and Hotel.cs files

// Thirteenth
// we will go to the Package manager console by clicking on "tools", and then clicking on
// "nuget package manager" and then on "package manager console"
// And there is where the EntityFramework.Tools comes in, as this will allow us to scafold/create our
// database in the remote server
// the first thing we need to do in the PMC is to add a Migration name "DatabaseCreated" using the code below
// Add-Migration DatabaseCreated
// when we run this command in the PMC, we will find that a new folder has been created called
// Migrations, which contains a file with a snapshot of what each instance/object(i.e Country and Hotel objects)
// will look like in the database when it is created.
// So next we will create the Database using the code below
// Update-Database
// Now to check the creeated database, we will need to click on "View" above and click on
// "Sql Server Object Explorer"
// then there we will expand the in built "(localdb)\MSSQLLOCALDB" and
// then we will expand the "Databases", and then we expand the created "Hotellisting_db" database


// Fourteenth
// SEEDING DATA INTO THE DATABASE
// To seed data into the database we will navigate to the DatabaseContext.cs file


// Fifteenth
// SET UP SERVICE REPOSITORIES AND DEPENDENCY INJECTION
//To get this started we are going to be creating two new folders "IRepository" and "Repository"
//we will be using the "Separation of Concerns" concept, where we want to make sure that every file is responsible for only a particular task
//to keep things Generic, making sure there is no repeatition of anything

//So in the "IRepository" folder, we will create a public Interface, and we will call it "IGenericRepository"

// Sixteenth
// SETTING UP DTO (DATA TRANSFER OBJECT) AND USING AUTOMAPPER TO AUTOMATE
// THE PROCESS OF LINKING OUR DTO TO THE DOMAIN OBJECTS
// So then we can think of DTO's as a midle layer which will enforce certain validations at the frontend part of the application amongst other things
// So we can use DTO to sanitize our data before it actually gets over to our actual data Class (Country or Hotel and via extension, our database)
// To create the DTO's to interface with our classes, first we need to create a folder called Models which will contain the DTO Models of our actual data Class
//(Country or Hotel and via extension, our database).
//Now Inside of the Models folder, we generally have a number of classes that represents each variation of a request relative to each Domain Object (Country and Hotel)


// Seventeenth
// SECURING YOUR API
// the first security we will be implementing will be for Authentication, which we will be implementing
// in the DatabaseContext.cs file.



// Sixth
// here to configure the CORS (Cross Origin Resourse Sharing), we will first add the
// service, and then in it's parenthesis we will add the policy/configuration on how we
// want it to behave using a lambda expression as done here below
builder.Services.AddCors(cor =>
{
    // the first configuration here will be to build the policy using the ".AddPolicy" method
    // which takes in a customized policy name (in this case "AllowAll") as it's fist parameter , and then
    // a lambda expiression to buld the policy in which we can set the policy to alllow/do a list of
    // things in the properties of the "build" name variable("builder" in this example case) provided
    cor.AddPolicy("AllowAll", builder =>
        // here since we will be developing an API that can be acessed in the web by anybody who
        // needs it, then we will use the "AllowAnyOrigin()" which makes it possible for anybody
        // on the web to access it. and with allowing any origin we will also make sure that the
        // accessing origin can also gain access using every Method/request the API is set to serve.
        // we will do this by chainning the "AllowAnyMethod()" method with the CorsPolicyBuilder class' "AllowAnyOrigin()" method.
        // and also for the API to accept any Method/Request Header, we will also chain the CorsPolicyBuilder class' "AllowAnyHeader()" method
        // to the list of allowed access functionalities as seen below
        builder.AllowAnyOrigin().
        AllowAnyMethod().AllowAnyHeader()
    // but remember that context of the API you are building is what determines how strict you are with the accessibility of your API
    // So after doing all this we will have to go down the file to the "Confiure/app.Use" section, in order
    // to let the application know that it should use the set cors policy "AllowAll".
    );
});


//// SQL Connection
//// here we will register the DatabaseContext.cs here in the programs.cs file so as to enable it available application wide via Dependency Injection
//// first for this we will need to get the ConnectionString "sqlConnection" and store it in a variable to be used proide the connection settings for the DatabaseContext
//string? connectionString = builder.Configuration.GetConnectionString("sqlConnection");
//builder.Services.AddDbContext<DatabaseContext>(options => {
//    options.UseSqlServer(connectionString, op =>
//    {
//        op.EnableRetryOnFailure();
//    });
//});

// POSGRESQL Connection
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("posgresqlConnection"));
});

// USING RATE LIMIT(THROTTLING) IN AN API APPLICATION
// To allow the application to use memory cache, which will help the application
// to store and keep track of who requested what, and how many times they have requested it, we will
// need to add memory cache right here in the services
builder.Services.AddMemoryCache();
// next we will navigate to the ServiceExtensions.cs file to make some modifications in the form of including
// the configuration for the RateLimiting
// then we include the defined configuration for the RateLimiting as well as the service "bilder.Services.AddHttpContextAccessor();"
builder.Services.ConfigureRateLimiting();
builder.Services.AddHttpContextAccessor();
// finally we are going to add the middle ware just above the "app.UseRouting();" line code

// IMPLEMENTING CACHING ON AN API ENDPOINT
// we will just replace the "builder.Services.AddResponseCaching();" line code with the cache configuration service defined in the ServiceExtensions.cs
builder.Services.ConfigureHttpCacheHeaders();
// next we will need to register the middle wares fpr both the AddResponseCaching() and the AddHttpCacheHeaders()
// just above the app.UseRouting(); line code

//// SECURING YOUR API
//
//
//// SETTING UP USER IDENTITY CORE
// so now what we need to do here is to configure our Identity sevice to know which class is going
// to be used to infer the tables that will be generated.
// So then basically what we want to happen here is that we are telling the application that from the
// start of the application, we want to use our "Identity" Services
// the identity services will be based on a database connection that connects to a database
// that has tables to facilitate "Identity" related things such as "user storage", "role storage", "claims" etc.
// so what we need to do now is to configure the Identity Services to know which class is going to inform how
// the user's table should look, and if potentally there exists another for roles, then also how the role's table
// should look etc.
// So already included at our disposal for this, is the Class called "IdentityUser", which also allows us to add
// more fields to it's already existing fields depending on the table we want included in the class for our authentication.
// we will also first need to include another Class "ApiUser.cs" inside the "Data" folder. This will be the class to
// inform how the user's table should look.
// Now, after doing creating the ApiUser class and doing the necessary modifications on the DatabaseContext file, we can now
// finally add the Identity services here.
// but in order not to overwhelm the program.cs file with too much infomation, we can simply just create another class file "ServiceExtensions"
// to extend our services.
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
// so we can basically start looking at things this way from now on, where if we are not adding a one liner code like the "builder.Services.ConfigureIdentity();"
// line code above, we can simply extract the configurations method to an external file and then include the single line method call here, there by simplifying our program.cs code file
// So now we need to add a Migration to the database to implement the change (the accomodation of the identity tables) made to the Daytabase via the DatabaseContext file, using the code below inside the PMC
// Add-Migration AddedIdentity
// Next we can update the database using the code below
// Update-Database

// here we will include the configuration for the JWT Service
builder.Services.ConfigureJWT(builder.Configuration);
// or
//var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value;
//var jwtKey = builder.Configuration.GetSection("Jwt:Key").Value;

//builder.Services.AddAuthentication(opt =>
//{
//    // here for the Default Authenticate Scheme, we will need to get the nuget package "Microsoft.AspNetCore.Authentication.JwtBearer"
//    // and then set the option's "DefaultAuthenticateScheme" property to the JwtBearer's default
//    // Authentication Scheme.
//    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    // and also what ever information comes across the API system, we also want to challenge it with the JwtBearer's default
//    // Authentication Scheme
//    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

//    // we are also going to chain the defined AddAuthentication configuration here with the
//    // AddJwtBearer configuration as done below
//})
// .AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = jwtIssuer,
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
//     };
// });


// after including the JWT Service here, the next thing we would need to do is to create
// some functionality for the validation and the isuuing of the Token
// And for this we will need to create another folder called "Services"
// for the Extensions (such as the JWT). And in the folder we will create an Interface file called "IAuthManager"
// which will be the Interface for managing the JWT authentification.


// Add the AutoMapper Service of Type "MapperInitializer" for the Mapping between the Domain Classes and the DTO Models
// when we start developing our endpoints we will actually get to see the power of AutoMapper and how the DTO's work and
// how everything relates to the Data Classes
// So getting the configurations of the Domain Classes, The DTO Models and the AutoMapper out of the way, is very important for that.
builder.Services.AddAutoMapper(typeof(MapperInitializer));
// So with this milestoe crossed we can now check in the Migation to Github
// to do this we can click on the "Git Changes" next to "Solutons Explorer"
// or if the option is missing, you can click on "View" and navigate to the "Git Changes" option from there
// and after including the Migration comment in the text box, you can commit the changes by clicking on the
// "Commit All" drop box arrow and then select "Commit All And Sync"
// this when working on a collaboration project with pairs will push up your changes, get the latest ones, and if you and a par modify the same files,
// then this will result to a merge conflict.
// but if you are not modifying the same file as your pair or you are working alone, then this will be a seemless process.

// Registering the Controller
// Here below AddTransient means that every time it is needed, a new instance/object would be created.
// there are also others like AddScoped means that a new instance is created for a period or for a life time of a certain set of requests.
// AddSingleton means that only one instance will exist for the entire duration of the application.
// AddTransient here means that when ever anyone hits the controller, a fresh copy of the object/instance of the "IUnitOfWork" and "UnitOfWork" is generated
// So which ever you pick will be depending on your need.
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
// After registering the Controller, we can test this out on Postman using the "Get" Route Controller path "https://localhost:7131/api/Country"
// but before we do we will need to install a Nuget package "Microsoft.AspNetCore.Mvc.NewtonsoftJson" to handle the retrieving/getting of information from the API. this is because the Country
// table links to the Hotels and the Hotel links to the specific Country and this link continues in a loop. and so as a result of this unending "Reference Loop", the web server is not exactly clear on what it is suposed to be retrieving.
// After installing the "Microsoft.AspNetCore.Mvc.NewtonsoftJson" package, we can then go on to Add it to our "builder.Service.AddControllers();" line code here, just below the "builder.Services.AddSwaggerGen();" line code.

// here we will register the IAuthManager and AuthManager service
// to do this we will use the Builder.Services.AddScoped() method to register both files here, this means that a new instance is created for a period or for a life time of a certain set of requests and Mapping the IAuthManager file to the AuthManager file
builder.Services.AddScoped<IAuthManager, AuthManager>();

builder.Services.AddSwaggerGen();

// IMPLEMENTING API VERSIONING
builder.Services.ConfigureVersioning();
// after registering our ApiVersioning service here, next, we will need to create another controller "ContryV2Controller.cs" and this will be a version 2 of the Country Api

// placing the AddControllers service as the last service added to our AddControllers() method, and then in the lambda operation configuration we will pull the "SerializerSettings" and then from this we will pull the "ReferenceLoopHandling"
// (which is an Enum) and ignore it.
// All this is actually saying is that when the web execution sees a Reference Loop happening, then it should be ignored. we will ignore this by setting the value to the set "Newtonsoft.Json.ReferenceLoopHandling.Ignore" property
// here we will pull the "AddNewtonsoftJson" from our "builder.Services.AddControllers()"
// IMPLEMENTING A GLOBAL VALUE FOR THE CACHE DURATION FOR EVERY ENDPOINT IN THE APPLICATION
// here we will include the lambda expression in the parenthesis in order to add the global cache duration (by specifying the object name name and then creating the object of the "CacheProfile" class in which we specify the duration) to the config "cacheProfile" property
builder.Services.AddControllers(config =>
{
    config.CacheProfiles.Add("120SecondsDuration", new CacheProfile
    {
        Duration = 120,
        // then we can now go on to the "CountryController.cs" file and replace the ResponseCache duration "[ResponseCache(Duration = 60)]" with the specified "CacheProfile" class object/instance name in this case "120SecondsDuration" "[ResponseCache(CacheProfileName = "120SecondsDuration")]"
    });
}).AddNewtonsoftJson(op =>
    op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );
// Now one last thing we will need to do is to make sure that the CountryController is mapped to the CountryDTO and not the actual Domain Country Class
// So we will need to navigate to the CountryController.cs file to do this

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // Fourth
    // here the "app.UseSwagger" and "app.UseSwaggerUI()" code below means that we will use swagger
    // only when in the development environment.
    // but Swagger is extremely useful also for documentation during the production stage.
    // And so we will take swagger out of this code block to display it also in the production environment
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

// Ensuring that swagger works both on development and production envionment
app.UseSwagger();
app.UseSwaggerUI();

app.ConfigureExceptionHandler();
// we can go ahead to test this by throwing a false exception in the Get

app.UseHttpsRedirection();

// Seventh
// Ensuring that the application knows that it should use the set cors policy "corspolicy"
app.UseCors("AllowAll");

// THE CACHING MIDDLE WARE
app.UseResponseCaching();
app.UseHttpCacheHeaders();


// MIDDLE WARE FOR USING RATE LIMIT(THROTTLING) IN AN API APPLICATION
app.UseIpRateLimiting();

app.UseRouting();

// to create an authorized access to an endpoint, we will need to include the middle ware "app.UseAuthentication();"
app.UseAuthentication();

app.UseAuthorization();

//// Using The Convention Based Routing
//app.UseEndpoints(endpoints => 
//    // here we will define the map controller route
//    endpoints.MapControllerRoute(
//        // first the name
//        name: "default",
//        // then the route pattern
//        // and here we will need to be very specific in our route where we will specify the path to the exact endpoint
//        // this will be okay for an MVC application, but for REAST API standard application, this will require the verb
//        // to determine what it is we will be doing with the route. And so we will be making use of "Attribute Routing"
//        // instead of this("Convention Based Routing").
//        pattern: "{countroller=Home}/{action=Index}/{id?}"
//        )
//);


app.MapControllers();

// Third
// here we will need to modify the main application which is going to initialize our logger
// when the application starts up. And here we are going to put in some test log scenarios right here
// in the main section as our test cases.
// the firstly here is to create the "Log" variable, and we will make it point to the inherited class
// "LoggerConfiguration()" instance/object. This will help us setup some default and expected
// behaviours. we will put a line break for each configuration added just to make things simple
Log.Logger = new LoggerConfiguration()
    // the first configuration would be to confgure where we want the information written to
    // which we will set to write to a file
    .WriteTo.File(
        path: @"C:\Users\HP\Desktop\HotelListings\logs\log-.txt",
        // the next will be the output template, i.e how we want each line to look
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        // the next parameter will be the "rollingInterval", which means at what interval we will want to
        // create a new file. "Note that the interval is represented by the hyphe "-" in the "log-.txt" file
        // where we can set maybe a date as the rollingInterval using "RollingInterval.Day".
        // this makes it easier to keep track of the log file of a specific day to be reviewed latter
        rollingInterval: RollingInterval.Day,
        // next parameter would be the restricted minimum level, which is to set a limit to what is logged
        // in the file. So here we only want to log Event level infomation, so we will set it to that by
        // first including the "Serilog.Event" using statement above
        restrictedToMinimumLevel: LogEventLevel.Information
        // so after including all the configuration necessary here, we can then go ahead to create the
        // logger using the ".CreateLogger();"as seen below
        ).CreateLogger();

// So now that we have the Logger created we can actually start using it
// to do this we are going to wrap the appication "app.run();" in a try catch
try
{
    // but first we are going to log an information to the file stating that the application has
    // started. Just so we can see what time and when the application started
    Log.Information("The Application Is Starting");
    app.Run();
}
catch (Exception ex)
{
    // then we will go on to write the caught exception to the file using Log.Fatal() method
    // which takes in the exception object as well as a string containing a customized message
    Log.Fatal(ex, "Application Failed to Start");
    // so basically what happens here is that when an exception is caught, the Logger will format it
    // according to what we specified in the logger configuration before logging it to the file
}
// we will also include a finally block here to make sure that we close and flush the log session
// after we are done whether there was an exception or not
finally
{
    Log.CloseAndFlush();
}

// Fifth
// we will now take a look at the controller file to see an
// Example of SHOWING HOW LOGGING MESSAGES WITH SWAGGER WORKS
