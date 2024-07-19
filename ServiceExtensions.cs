using AspNetCoreRateLimit;
using HotelListing_Api.Data;
using HotelListing_Api.Models;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace HotelListing_Api
{
    // we have to make the class a static class
    public static class ServiceExtensions
    {
        // we will ceate a static void function to take care of the configuration of the Identity
        //service
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            // then we will create the builder variable which is basically an amalgamation of all the services which we define in the 
            var builder = services.AddIdentityCore<ApiUser>(I => I.User.RequireUniqueEmail = true);

            // now we will set the builder variable to hold an instance of the IdentityBuilder inorder to build an object of the defined Identity
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);

            // then we will inform the file on where the builder object should be stored using the AddEntityFrameworkStores of type <DatabaseContext>
            // since we want to store this in the Database, so we made the AddEntityFrameworkStores relative to the databaseContext. and then from this
            // we will chain this with the "AddDefaultTokenProviders()" method as well
            builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();
            // now that we have this method in this class, we can now go over to the program.cs class and there include the Authentication() and ConfigureIdentity() methods

        }


        // here we will add another method to hold the configurations for the JWT which we will call "ConfigureJWT()"
        // and in the method parameters we are taking an argument of type "this IServiceCollection" to give us access to the
        // services in our program.cs file here, and the second argument will be of type IConfiguration which will give us
        // access to the settings in the appsettings.json file here in the "ServiceExtensions.cs" file.
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            // first we will get the jwt settings using the IConfiguration type method "GetSection()"
            var jwtSettings = configuration.GetSection("Jwt");
            // then we will get the jwt key stored in our Systems Environment using the Environment class type
            // which gives us access to our Systems Enviroonment and then we will use the "GetEnvironmentVariable()"
            // method and store it in a variable to get the set JWT key named "KEY" here the ServiceExtension.cs file 
            var key = configuration.GetSection("Jwt:Key").Value;
            //var key = jwtSettings.GetSection("Key").Value;

            // Next we want to add the Authentication configuration to the service
            services.AddAuthentication(opt =>
            {
                // here for the Default Authenticate Scheme, we will need to get the nuget package "Microsoft.AspNetCore.Authentication.JwtBearer"
                // and then set the option's "DefaultAuthenticateScheme" property to the JwtBearer's default
                // Authentication Scheme.
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // and also what ever information comes across the API system, we also want to challenge it with the JwtBearer's default
                // Authentication Scheme
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                // we are also going to chain the defined AddAuthentication configuration here with the
                // AddJwtBearer configuration as done below
            })
            .AddJwtBearer(opt =>
            {
                // there are quite a few Parameters that we can set up along the way, but the one's included here is just
                // for our API example. but in an enterprise setting(situation), you may have other needs than the one's used here.

                // so here we are going to add some of the parameters that this Token is going to use to validate a registered user
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    // we will include the validation of the Token Issuer, since we went through the trouble of adding the "issuer" to
                    // the "jwt" settings. And so with this if the Token coming in with the a user's request do not match our
                    // set issuer "HotelListing_Api" defined in our appsettings.json file, then we will not be granting the request
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // we want to validate the life time of the Token. setting this to true will reject a valid Token once it is expired.
                    ValidateLifetime = true,
                    // another we will like to do is validate the issuer's singn in key (that is the key set in the Environment variable "KEY")
                    ValidateIssuerSigningKey = true,
                    // here we are going to set the valid issuer for any given Jwt Token must be the issuer defined in the "jwt" setting in the
                    // appsettings.json file which we have stored here in the variable "jwtSettings"
                    // for for this we will set the value to be the jwtSettings variable "jwt" settings "issuer" value as done below
                    ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
                    ValidAudience = jwtSettings.GetSection("validIssuer").Value,
                    // here we will encode the Issuer Signing Key by passing in the variable "key" where the JWT key is stored into the parameter below,
                    // and then hashing it again afterwards
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });
        }

        // Global Error Handling
        // we will be creating the configure exception handler function and we will be getting the
        // ApplicationBuilder in it's parameter
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            // the the dotNet app type parameter has it's own exception handler, so what we are doing here is 
            // just an override to say how we really want it to operate
            app.UseExceptionHandler(error => {
                error.Run(async context => {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    // then we will create a variable to hold the context feature
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    // we will check using an if statement if the contextFeature variable is null i.e. if there was an error
                    if (contextFeature != null)
                    {
                        // if it is not null meaning there are errors then we want to log an error message
                        Log.Error($"Something Went Wrong In The {contextFeature.Error}");

                        // then we will generate the error in a new error class
                        await context.Response.WriteAsync(new Error
                        {
                            StatusCode = context.Response.StatusCode.ToString(),
                            Message = "Internal Server Error. Please Try Again Later."
                        }.ToString());
                        // now after writing this here we will navigate to the programs.cs file and register the
                        // ConfigureExceptionHandler there, just directly beneath the "app.UseSwaggerUI();" code
                    }
                });
            });
        }

        // IMPLEMENTING API VERSIONING
        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                // in our AddApiVersioning options body, the first thing we will do is report Api version
                // by setting the property to true
                // this means that there will be a header in our responses stating the current version being used
                opt.ReportApiVersions = true;
                // next we will assume the default version when the version to be used is unspecified by the client
                opt.AssumeDefaultVersionWhenUnspecified = true;
                // here we will set the default version, which in this example we will set to version 1.0
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                // Another way to that we can get the versioning to work is to include the
                // option for "ApiVersionReader" in the "services.AddApiVersioning" options body
                // where we can set the value of this to a HeaderApiVersionReader object and set this to look out for the
                // api-version key passed in the header section in postman when the API is called
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });

            // after we have done this here we can go over to the Programs.cs file and include the configuration method

        }

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
        {
            // here we will include every service for the Cache
            // here just for simplicity, we will include the "builder.Services.AddResponseCaching();"
            services.AddResponseCaching();
            // then we will include the HttpCacheHeaders service
            services.AddHttpCacheHeaders(
            (expirationOpt) =>
            {
                expirationOpt.MaxAge = 120;
                expirationOpt.CacheLocation = CacheLocation.Private;
            },
            (validationOpt) =>
            {
                validationOpt.MustRevalidate = true;
            });
            // now after making these modifications to the "services.AddHttpCacheHeaders()" we can now remove the cache header
            // from the CountryController GetCountries endpoint since we have made it global here.
            // next we will just replace the "builder.Services.AddResponseCaching();" line code with this configuration service
        }

        public static void ConfigureRateLimiting(this IServiceCollection services)
        {
            // we create a variable to hold a list of type "RateLimitRule"
            var rateLimitRules = new List<RateLimitRule>
            {
                // Note that we can add rules for each endpoint where we will specify the endpoint and then
                // the limit call per second/ per minute/ per hour etc rule for calls to that endpoint
                // but here we set a global rule for all endpoints in the application
                new RateLimitRule
                {
                    // here we specify that we want this rule adhered to by all endpoints in the application
                    Endpoint = "*",
                    // and the rule is limited to 1 call for a period of 5 seconds
                    Limit = 1,
                    Period = "5s"
                }
            };
            // here we configure the IpRateLimitOptions and set the GeneralRules to the "rateLimitRules" rule we just defined
            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules = rateLimitRules;
            });
            // And finally here we will include the AddSingletons below which are just bits of code that are required to support the
            // library that we imported "AspNetCoreRateLimit". Note a deferent library may require a diferent support, but these are the once required by this library
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }

        // after we are done with this we will head back to the Programs.cs file and include the service configuration for "builder.Services.ConfigureRateLimiting()" and
        // the servie "builder.Services.AddHttpContextAccessor();"
    }
}
