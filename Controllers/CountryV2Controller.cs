using AutoMapper;
using HotelListing_Api.Data;
using HotelListing_Api.IRepository;
using HotelListing_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing_Api.Controllers
{
    // note that if both the country and countryV2 are giving the same api route by changing the datanotation below to "[Route("api/Country")]"
    // then we will have to specify above the version inorder to differentiate them using another datanotation [ApiVersion("2.0")]. we will see this here
    [ApiVersion("2.0", Deprecated = true)]
    // changing the route name to "Country" also
    // HOW TO MAKE THE USER SPECIFY THE SPECIFIC API VERSION TO BE USED IN THE API CALL
    // here we will modify the route to accept the version from the user forcefully, between the api and the Country
    // and then in postman we can do this instead "https://localhost:44379/api/2.0/Country"
    [Route("api/country")]
    [ApiController]
    public class CountryV2Controller : ControllerBase
    {
        // here we will change the way this API accesses data, and here instead of making use the unitOfWork via the dependency injection
        // we will make use of the DatabaseContext directly via dependency injection in this file
        // Note that this is not allowed in the real world scenerio as we would not want the api interfacing directly with the database, but for example purposes we will do this here

        // create the dependency field for the databaseContext
        private DatabaseContext _context;

        // then we inject the dependency into the api
        public CountryV2Controller(DatabaseContext context)
        {
            _context = context;
        }

        // Now here we can go on to include our routes

        // First the Route Function to get all Countries
        [HttpGet]
        // here we can also indicate the response types which at the same time also Informs swagger of the expected response type
        // using the Data anotations below. so that swagger does not interpret it as undocumented
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams)
        {
            // all we will just need to do here is to get/return all the countries from the database
             
            return Ok(_context.Countries);
        }
    }
}
