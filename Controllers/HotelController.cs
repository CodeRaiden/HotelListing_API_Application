using AutoMapper;
using HotelListing_Api.Data;
using HotelListing_Api.IRepository;
using HotelListing_Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;

namespace HotelListing_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        // so first thing we need to do here is create a private readonly field to be used to inject the workload done in the UnitOfWork here in the CountryController
        private readonly IUnitOfWork _unitOfWork;
        // And then we will need to create an Ilogger field of type CountryControler, so we can write to the logger file here
        private readonly ILogger<CountryController> _logger;
        // To Mapp the CountryController to the CountryDTO, we will need to inject the AutoMapper dependency here
        private readonly IMapper _mapper;

        // Now we can go on to inject the dependencies here in the "CountryController" via the constructor
        public HotelController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            // then we inject the mapper into the file
            _mapper = mapper;
        }

        // Now here we can go on to include our routes

        // First the Route Function to get all Hotels
        [HttpGet]
        // here we can also Inform swagger of the the expected response type
        // using the Data anotations below. so that swagger does not interpret it as undocumented
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotels()
        {
            try
            {
                // here we will create a variable "var" to hold the gotten Hotels and we will make sure that the execution awaits for the hotels to be gotten
                // this is where the unit of work dependency injection comes in handy
                var hotels = await _unitOfWork.Hotels.GetAll();

                // we will include a variable here called "result", which will map the gotten Hotels to the HotelsDTO
                // we will do this by pulling the Map class from the Injected Automapper dependency, and then we will make this map to a list "IList" of type "HotelsDTO"
                // and then we will include the variable "hotels" as the parameter to be mapped to the CountryDTO.
                var results = _mapper.Map<IList<HotelDTO>>(hotels);
                // and if everything goes right we want to return a 200 Ok response for the goetten IList of type CountryDTO store in the "result" variable
                return Ok(results);
            }
            catch (Exception ex)
            {
                // here in the catch, this is where the logger becomes very important. In that we can log the error
                // in the logger file.
                // for example we can say "something went wrong with the name of the method of the "GetCountries""
                _logger.LogError(ex, $"Something went wrong with the {nameof(GetHotels)}");
                // Note that the logger file is specificaly for internal information
                // So for the user information, we will return a 500 inetrnal server error
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }

        // Next we will create an endpoint for getting a single Hotel element from the database
        // here the difference is we are going to state to the datanotation decorator that we need to
        // get the country by it's id which should be an integer
        [HttpGet("{id:int}", Name = "GetHotel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotel(int id)
        {
            try
            {
                // here we will create a variable "var" to hold the gotten Hotel and we will make sure that the execution awaits for the hotel to be gotten
                // And also in the Get function we will include the parameters as defined in the Generic Repository. The first of which is a lamda expression that must
                // return true for the search to be successful, and the other (if added) should be an object of type "Country" to be stored in a list of type string note that
                // the "Country" object name must match with the name defined class name defined in the IUnitOfWork "Country"
                var hotel = await _unitOfWork.Hotels.Get(hotel => hotel.Id == id, new List<string> { "Country" });

                // here we will map a single entity of the CoutryDTO to the country instaed of an Ilist
                var result = _mapper.Map<HotelDTO>(hotel);
                // and if everything goes right we want to return a 200 Ok response for the goetten IList of type CountryDTO store in the "result" variable
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong with the {nameof(GetHotel)}");

                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }

        // we can try the Get() method on postman using the link https://localhost:7131/api/Hotel/1


        // CONSTRUCTING A POST ENDPOINT TO
        // CREATE HOTEL
        // So what we are going to be doing is setting up the post functionality for our Hotel, so
        // that when we are creating a hotel, we can ask the user to send over the details for a hotel
        // in the form of a json object and then we pass it down to the database.
        // In order to prevent an Authorized user access to the endpoint we can include the authorized verb
        // Authorize inside the [HttpGet("{id:int}")] data anotation "[HttpGet("{id:int}"), Authorize]", or
        // we can create another data anotation for the Authorize verb "[Authorize]" and that's what we will do.
        // TESTING THE AUTHORIZE ENDPOINT
        // we will also specify that only clients with "Administrator" priviledges/roles can create
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
        {
            // the first thing we need to do is to check if the incoming CreateHotelDTO type model/data state
            // is valid
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid Post Attempt in {nameof(CreateHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                // so when we get the object/data we want to map the object to the Hotel Model inorder for
                // it to be created in the database.
                var hotel = _mapper.Map<Hotel>(hotelDTO);
                // next we will call on the "_unitOfWork" in order to insert our "hotel" object entry into the database
                await _unitOfWork.Hotels.Insert(hotel);
                // next we will need to commit the change made to the database
                await _unitOfWork.Save();
                // next we will need to return a 201 created response to the user,
                // but for this we will return a specific type of 201 created response "CreatedAtRoute"
                // which after the entry has been created, will call the endpoint "GetHotel" with the
                // created "hotel" id required in an object, and the third parameter will be the hotel object itself.
                return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotel);
                // now one last thing here is that we will need to let the CreatedAtRoute operation know that
                // we are using the GetHotel Method by including the specified method name "GetHotel" we
                // included in the "CreatedAtRoute" method name parameter.
                // so for this we will navigate to the GetHotel Method's "[HttpGet("{id:int}")]" verb, and there
                // we will include the specified "CreatedAtRoute" method name "GetHotel". And this modification to the verb should look like this below
                // [HttpGet("{id:int}", Name = "GetHotel")]
            }
            catch (Exception ex)
            {

                // here we will repeat the same code for the caught exception, in order to avoid code
                // repetition, we will refactor the code latter to fix this.
                _logger.LogError(ex, $"Something went wrong with the {nameof(CreateHotel)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }

        }
        // we can go on to test this out on postman by pasting the url "https://localhost:7131/api/Hotel",
        // then clicking on the "body" section, and then the "raw" and then click the drop down arrow next
        // to the "Text" and select "Json" and then we can pass a json object entry as seen below to add a new hotel entry
        // {
        //     "id": 1,
        //     "name": "Moon Palace Jamaica Grande Resort",
        //     "address": "Ocho Rios",
        //     "rating": 5,
        //     "countryId": 1
        // }

        // you will also notice that when you check the "headers", you will find that as a result of the "CreateAtRoute",
        // the location by id to the created hotel is returned.
        // which means also that we can copy this location and do a call on it.



        // CONSTRUCTING PUT/UPDATE ENDPOINT TO UPDATE A HOTEL RECORD OR
        // CREATE THE HOTEL RECORD IF IT DOES NOT EXIST IN THE DATABASE

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // here, in order to maintain the single responsibility rule and not to have the CreateHotelDTO Type field in the UpdateHotel Method parameter,
        // we will need to create another DTO "UpdateHotelDTO" and simply make it inherit from the "CreateHotelDTO" in order to possess the same members
        // So here below we are taking the id in the paameter, and we are also expecting the fields for the update in the body of the request
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDTO)
        {
            // we will start with the Validation check for the model state in the incoming request body
            // so we will check if the model state is not valid and also we will check if the passed in id
            // is less than 1
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid Delete Attempt in {nameof(UpdateHotel)}");
                return BadRequest();
            }

            try
            {
                // first here we will need to querry the database to get the record to be updated an store it in a variable
                // for this we will use a lambda to get the hotel record where the record "Id" is the same as the passed in parameter id
                var hotel = await _unitOfWork.Hotels.Get(r => r.Id == id);
                // here we can check if no hotel with the matching id was found
                if (hotel == null)
                {
                    _logger.LogError($"Invalid Post Attempt In {nameof(UpdateHotel)}");
                    // we will include a message in the BadRequest prameter stating the "Submitted Data Is Invalid"
                    return BadRequest("Submitted Data Is Invalid");
                }

                // So if it is not null then we will go on with updating the record gotten and stored in the hotel
                // variable, with the fields for the update in the body of the request "hotelDTO"
                // we will do this by making use of overloaded mapping parameter which takes in two parameters the "source" (which contains the latest information) and the "destination"(which contains the old information to be updated by the source) as done below
                _mapper.Map(hotelDTO, hotel);
                // since we are not doing any tracking of the changes made here in the database, then we can not just save the changes immediatly here, instead we will have to use the Update() method which we defined to track any change in the hotel record to the database with the new mapped hotel variable object(which has been updated here locally with the hotelDTO model state)
                _unitOfWork.Hotels.Update(hotel);
                // now after tracking the update in the hotel record to the database, we can now finally save/comit the change in the database
                await _unitOfWork.Save();

                // then after we have saved the change to the database, since there is nothing we will wish to return to the user we will return a Status 204 NoContent which means exacty that
                return NoContent();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Something went wrong with the {nameof(UpdateHotel)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }

        // CONSTRUCTING DELETE ENDPOINT TO DELETE A HOTEL RECORD FROM THE DATABASE
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            if (id < 1)
            {
                _logger.LogError($"Invalid Update Attempt in {nameof(DeleteHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotel = await _unitOfWork.Hotels.Get(r => r.Id == id);

                if (hotel == null)
                {
                    _logger.LogError($"Invalid Post Attempt In {nameof(DeleteHotel)}");
                    return BadRequest("Submitted Data Is Invalid");
                }

                // since the delete action is set to be tracked by the database we will await the action
                await _unitOfWork.Hotels.Delete(id);
                // then we save the changes to the database
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Something went wrong with the {nameof(DeleteHotel)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
            
        }
    }
}

// we can go on to commit the progress to Github.
