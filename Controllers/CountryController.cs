using AutoMapper;
using HotelListing_Api.Data;
using HotelListing_Api.IRepository;
using HotelListing_Api.Models;
using HotelListing_Api.Repository;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace HotelListing_Api.Controllers
{
    // Now here there are basically two types of routing mechanisms that we can use during API development(at least MVC API development)
    // and they are:
    // 1. The Convention Based Routing:
    // In this type of Routing we will have to go to the program.cs file and there we will have to configure in "app.UseEndpoints();" parameter, just beneath the "app.UseAuthorization();" line code
    // But the problem with this here is we will need to be very specific with our route, where we will specify the path to the exact endpoint
    // this will be okay for an MVC application, but for REAST API standard application, this will require the verb
    // to determine what it is we will be doing with the route. And so we will be making use of "Attribute Routing" instead the "Convention Based Routing".

    // Attribute Routing
    // here, in specifying the route we will only need to use the verb as defined in the Route parameter below "api/[controller]" where "controller" would signify the particular controller we are dealing with
    // i.e. "Country" or "Hotel".
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        // so first thing we need to do here is create a private readonly field to be used to inject the workload done in the UnitOfWork here in the CountryController
        private readonly IUnitOfWork _unitOfWork;
        // And then we will need to create an Ilogger field of type CountryControler, so we can write to the logger file here
        private readonly ILogger<CountryController> _logger;
        // To Mapp the CountryController to the CountryDTO, we will need to inject the AutoMapper dependency here
        private readonly IMapper _mapper;

        // Now we can go on to inject the dependencies here in the "CountryController" via the constructor
        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            // then we inject the mapper into the file
            _mapper = mapper;
        }

        // Now here we can go on to include our routes

        // First the Route Function to get all Countries
        [HttpGet]
        // IMPLEMENTING CACHING ON AN API ENDPOINT
        // To start chaching our resources is pretty simple
        // what we need to do is add the response cache attribute to an endpoint, and then we set how long the cache should live for
        // the ResponseCache duration below is set to 60 seconds.
        // [ResponseCache(Duration = 60)]
        // IMPLEMENTING A GLOBAL VALUE FOR THE CACHE DURATION FOR EVERY ENDPOINT IN THE APPLICATION
        // we can use this for all the get endpoints in the application which is where there are needed them inorder to give a user access to get information from the database
        // for a specified period of time
        // After globally implementing the cache headers, we no longer have a need for the ResponseCache data anotation below
        // [ResponseCache(CacheProfileName = "120SecondsDuration")]
        // CUSTOMIZING THE ALREADY SET GLOBAL HEADERS FROM AN ENDPOINT
        // Note that we can still customize the set global headers of any endpoint by using
        // the "HttpCacheExpiration" and HttpCacheValidation data anotation as seen in the below
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        // here we can also indicate the response types which at the same time also Informs swagger of the expected response type
        // using the Data anotations below. so that swagger does not interpret it as undocumented
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams)
        {
                // here we will create a variable "var" to hold the gotten Countries and we will make sure that the execution awaits for the countries to be gotten
                // this is where the unit of work dependency injection comes in handy
                // then we will just pass the requstParams as an argument in the GetAll method parenthesis here
                // but we do not want to get rid of the original GetAll() method and so we will navigate to the IGenericRepository file and there we will create
                // another method to handle the RequestParameters.
                var countries = await _unitOfWork.Countries.GetPagedList(requestParams);

                // we will include a variable here called "result", which will map the gotten Countries to the CountryDTO
                // we will do this by pulling the Map class from the Injected Automapper dependency, and then we will make this map to a list "IList" of type "CountryDTO"
                // and then we will include the variable "countries" as the parameter to be mapped to the CountryDTO.
                var results = _mapper.Map<IList<CountryDTO>>(countries);
                // and if everything goes right we want to return a 200 Ok response for the goetten IList of type CountryDTO store in the "result" variable
                return Ok(results);
        }

        // Next we will create an endpoint for getting a single Country element from the database
        // here the difference is we are going to state to the datanotation decorator that we need to
        // get the country by it's id which should be an integer
        [HttpGet("{id:int}", Name = "GetCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountry(int id)
        {

            // here we will create a variable "var" to hold the gotten Country and we will make sure that the execution awaits for the countries to be gotten
            // And also in the Get function we will include the parameters as defined in the Generic Repository. The first of which is a lamda expression that must
            // return true for the search to be successful, and the other (if added) should be an object of type "Hotels" to be stored in a list of type string note that
            // the "Hotels" object name must match with the name defined class name defined in the IUnitOfWork "Hotel"

            // TESTING THE GLOBAL EXCEPTION HANDLER
            // So now that we have written the global exception handler to override the dotNet app error handler
            // we can now remove all the try catch blocks in the Country and Hotel cotrollers
            // as the app can now handle any exception thrown.
            // we will test this by throwing a delibrate exception here
            // also note that we no longer need the try catch below as the global exception handler will
            // handle this for us

            // the delibrate exception
            throw new Exception();

            var country = await _unitOfWork.Countries.Get(r => r.Id == id, new List<string> { "Hotels" });

            // here we will map a single entity of the CoutryDTO to the country instead of an Ilist
            var result = _mapper.Map<CountryDTO>(country);
            // and if everything goes right we want to return a 200 Ok response for the goetten IList of type CountryDTO store in the "result" variable
            return Ok(result);
        }

        // we can try the Get() method on postman using the link https://localhost:7131/api/Country/1

        // CONSTRUCTING A POST ENDPOINT TO
        // CREATE A COUNTRY
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO countryDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid Post Attempt in {nameof(CreateCountry)}");
                return BadRequest(ModelState);
            }

                // map the coutryDTO to the Country Database Model 
                var country = _mapper.Map<Country>(countryDTO);
                // Insert the country entry into the database
                await _unitOfWork.Countries.Insert(country);
                // comit the change
                await _unitOfWork.Save();
                return CreatedAtRoute("GetCountry", new { id = country.Id }, country);
        }


        // CONSTRUCTING PUT ENDPOINT TO UPDATE A COUNTRY RECORD OR
        // CREATE THE COUNTRY RECORD IF IT DOES NOT EXIST IN THE DATABASE
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO countryDTO)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid Update Attempt in {nameof(UpdateCountry)}");
                return BadRequest(ModelState);
            }

            var country = await _unitOfWork.Countries.Get(r => r.Id == id);

            if (country == null)
            {
                _logger.LogError($"Invalid Post Attempt In {nameof(UpdateCountry)}");
                return BadRequest("Submitted Data Is Invalid");
            }

            // map the change
            _mapper.Map(countryDTO, country);
            // inform the database to track the update made
            _unitOfWork.Countries.Update(country);
            // save/comit the change in the database
            await _unitOfWork.Save();

            return NoContent();
        }

        // CONSTRUCTING DELETE ENDPOINT TO DELETE A COUNTRY RECORD FROM THE DATABASE
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (id < 1)
            {
                _logger.LogError($"Invalid Delete Attempt In {nameof(DeleteCountry)}");
                return BadRequest("Submitted Data Is Invalid");
            }

            
            var country = await _unitOfWork.Countries.Get(r => r.Id == id);

            if (country == null)
            {
                _logger.LogError($"Invalid Delete Attempt In {nameof(DeleteCountry)}");
                return BadRequest("Submitted Data Is Invalid");
            }

            await _unitOfWork.Countries.Delete(id);
            await _unitOfWork.Save();

            return NoContent();
        }


    }


    // Now we can go on to create Hotel API Controller

}

// Now we can go register the Controller in the boot strapper "program.cs" file, just below the "builder.Services.AddAutoMapper(typeof(MapperInitializer));" line code.