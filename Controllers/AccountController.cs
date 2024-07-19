using AutoMapper;
using HotelListing_Api.Data;
using HotelListing_Api.Models;
using HotelListing_Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Identity.Client;


////namespace HotelListing_Api.Controllers
////{
////    [Route("api/[controller]")]
////    [ApiController]
////    public class AccountController : ControllerBase
////    {
////        // So first thing we need to do here is create a private readonly field to be used to inject our dependencies
////        // We will be creating two new class library fields Courtsey of the Idenitity Core.
////        // The first will be the User Authentication "UserManager" Class field of Type/Context "<ApiUser>"(the ApiUser which was used when setting up Identity. Note also that if we didn't use an inherited class in order to add more fields to the identity, then we would just simply use "Idenitity" here as the context instead),
////        // which we will call "_userManager"
////        private readonly UserManager<ApiUser> _userManager;

////        // The second is the User Loggin "SignInManager" Class field of Type/Context "<ApiUser>" which we will call "_signInManager"
////        private readonly SignInManager<ApiUser> _signInManager;

////        // the UserManager and SingnInManager and Roles Manager(not included here for this example) Class libraries gives us a bunch of suite that allows us to Manage, SignIn, Retrive User Information, Add Users
////        // so basically we do not need to write any custom code to be adding, nor do we need to put in any unit of work functions for user table interractions or role table interractions
////        // all of these things are implemented already in the UserManager and SingnInManager Class libraries

////        // the next two fields will be for the logger and the mapper dependencies
////        private readonly ILogger<AccountController> _logger;

////        private readonly IMapper _mapper;

////        // Now we can go on to inject the dependencies via the constructor
////        public AccountController(UserManager<ApiUser> userManager,
////            SignInManager<ApiUser> signInManager,
////            ILogger<AccountController> logger,
////            IMapper mapper)
////        {
////            _userManager = userManager;
////            _signInManager = signInManager;
////            _logger = logger;
////            _mapper = mapper;
////        }

////        // So now that we have included our dependencies, we can start writting our endpoints
////        // the first endpoint will be for Registration.
////        // now since our registration will require sensitive information to be passed, then we will need to
////        // make this a "HttpPost" verb operation. A post for example is seen when submitting a for online, during the submission the information
////        // passed into the fields is not seen in the url. So all you see is that the information was submitted in one page and the next page that appears
////        // could represnt it by saying it has been sent successfully or showing you your details in a format.
////        // but the information is encapsulated the url and hidden from preying eyes, while travelling through the web to the server and back to you.
////        // his is unlike a "HttpGet" verb where we include the information in the parenthesis and so it is not encapsulated.
////        // so here the parameter we are going to include for the "Post" unlike the "Get" is going to be a custom one as seen below.
////        // where we will indicate that the sender needs to send it "from the Body" (and not the url), and then this will be followed
////        // by what we want added which is the UserDTO parameter which we will also need to create to interface with the ApiUsee.cs class.
////        // So basically what happens in this endpoint is that when a request hits the endpoint the body will be checked for the post information, and nt the url
////        // which means that if some information is passed in the url then it will be ignored and only the body will be checked.
////        // So then the body must contain information with fields that match up to the UserDTO. And any information aside from this will be ignored.
////        // And so this is how everything ties in concerning "sanitization of/sanitizing" requests and making decisions based on what is coming across the pipeline.
////        [HttpPost]
////        // now for the execution to differentiate the two [HttpPost] requests (Register and Login), we will need to provide another data anotation/decorator to identify
////        // the "Route" for each request function/implementation
////        // here for the register HttpPost request to hit the destinated function/implementation
////        // we will indicate the route "https://api/Account/register" using the data anotation below
////        [Route("register")]
////        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
////        {
////            // here we will log to the file concerning an incoming Registration attempt
////            _logger.LogInformation($"Registration Attempt For {userDTO.Email}");

////            // then here we will check if the incoming Registration Post request is in a valid state
////            if (!ModelState.IsValid)
////            {
////                // if not valid we will return a bad request and in it we will return the Model state
////                // so that the user can see what went wrong
////                return BadRequest(ModelState);
////            }
////            // otherwise if the request is okay, then we will return a "Created 204" response
////            return Accepted();
////            // lastly here, we will try catch for any exception caught
////            // during the processing of the request in the server
////            try
////            {
////                // so we map the incoming post request Model(in which are the required fields) contained in the "userDTO" to the "ApiUser" class
////                var user = _mapper.Map<ApiUser>(userDTO);
////                // so now that we have the user required fields, we can go on to create the user as done below
////                var result = await _userManager.CreateAsync(user);

////                // we will also provide an if statement to check if the result was was sucessfully gotten
////                if (!result.Succeeded)
////                {
////                    // if the result is unsuccessful then we want to return a bad request
////                    // and since we are dealing with the valid Model state we want to be careful with what we
////                    // want sent back to the user, and so instead we will return a message saying "User Registration Attempt Failed. Try Again Latter."
////                    // because the obvious reason for the failutre of the request here will be from the code implementation.
////                    BadRequest($"User Registration Atempt Failed. Try Again Latter.");
////                }
////            }
////            catch (Exception ex)
////            {
////                _logger.LogInformation(ex, $"Something Went Wrong In The {nameof(Register)}");
////                // then we can return a 500 error response in another way as done below
////                return Problem($"Something Went Wrong In The {nameof(Register)}", statusCode: 500);
////            }
////        }


////        // The next endpoint will be for the Login
////        // this will be pretty much the same as the Register
////        // with the difference being that with login we only need the email and password details
////        // and so we will need to create a separate LoginDTO class to handle this
////        [HttpPost]
////        [Route("login")]
////        // here in the parenthesis below will will indicate that we want the LoginUserDTO Model fields
////        // so that if any information is passed in by the user/client aside from the specified information required(also malicious information) by the
////        // LoginUserDTO, then we want such information to be ignored.
////        public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
////        {
////            // here we will log to the file concerning an incoming Login attempt
////            _logger.LogInformation($"Registration Attempt For {userDTO.Email}");

////            // then here we will check if the incoming Login Post request is in a valid state
////            if (!ModelState.IsValid)
////            {
////                // if not valid we will return a bad request and in it we will return the Model state
////                // so that the user can see what went wrong
////                return BadRequest(ModelState);
////            }
////            //otherwise if the request is okay, then we will try catch for any exception caught
////            // during the processing of the request in the server
////            try
////            {
////                // Note we do not need to inform the MapperInitializer.cs file of the LoginUserDTO since it is included in the
////                // in the UserDTO which is already mapped in the MapperInitializer.cs file
////                // So then we would not need to do any mapping here. Instead we will just do this below
////                // where we use the _signInManager dependency and there we pull the passwordSignInAsync() method which takes in
////                // two overloaded parameters, one of these takes in the ApiUser Class object(which we could use here if we had included the mapper for the LOginUserDTO)
////                // while the other just takes in the UserName, the Password(and this is what we will be using) it also contains a boolean parameter to indicate whether the signin cookie should persist after the browser is closed
////                // but since we do not know what kind of application is calling the API (could be postman, the browser, a mobile application etc), it does not need to persist, so we can simply set this as false.
////                // the last parameter the LockOnFailure parameter, which is also a boolean which indicates locking out the user/client after a failed login attempt.
////                var result = await _signInManager.PasswordSignInAsync(userDTO.Email, userDTO.Email, false, false);

////                // now we can use an if statement to check if the result sign in manager operation was successful
////                if (!result.Succeeded)
////                {
////                    // if the result was unsuccessful then since this is a failed login attempt we want to return an "Unauthorized" response(and not a bad request) for the userDTO
////                    return Unauthorized(userDTO);
////                }
////                // otherwise we will return a "Accepted 202" response
////                // side note: "Created" is a "204" response while "OK" is a "200" response
////                return Accepted();
////            }
////            catch (Exception ex)
////            {
////                _logger.LogInformation(ex, $"Something Went Wrong In The {nameof(Login)}");

////                return Problem($"Something Went Wrong In The {nameof(Login)}", statusCode: 500);
////            }
////        }


////    }
////}







// With the commented out implementation above, What is wrong is the use of the SignInManager dependency which has implementations for cookies to keep track of a user which is really useful for a web application.
// But in the case of an API, we do not necesssarily need this since we do not know who is calling the API and for how long. And it's also the user will not be lingering inside the API for one or few calls
// since the API is simply just going to accept your request give you a response and it's done. 
// So what we really want here is to make use of "Tokens"


// so to implement the use of tokens, we will remove all reference to the SignInManager dependency here
namespace HotelListing_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        // So first thing we need to do here is create a private readonly field to be used to inject our dependencies
        // We will be creating a class library field Courtsey of the Idenitity Core.
        // This will be the User Authentication "UserManager" Class field of Type/Context "<ApiUser>"(the ApiUser which was used when setting up Identity. Note also that if we didn't use an inherited class in order to add more fields to the identity, then we would just simply use "Idenitity" here as the context instead),
        // which we will call "_userManager"
        private readonly UserManager<ApiUser> _userManager;

        // the next two fields will be for the logger and the mapper dependencies
        private readonly ILogger<AccountController> _logger;

        private readonly IMapper _mapper;

        // injecting the AuthManager Service
        private readonly IAuthManager _authManager;

        // Now we can go on to inject the dependencies via the constructor
        public AccountController(UserManager<ApiUser> userManager,
            ILogger<AccountController> logger,
            IMapper mapper, IAuthManager authManager)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _authManager = authManager;
        }

        // So now that we have included our dependencies, we can start writting our endpoints
        // the first endpoint will be for Registration.
        // now since our registration will require sensitive information to be passed, then we will need to
        // make this a "HttpPost" verb operation. A post for example is seen when submitting a for online, during the submission the information
        // passed into the fields is not seen in the url. So all you see is that the information was submitted in one page and the next page that appears
        // could represnt it by saying it has been sent successfully or showing you your details in a format.
        // but the information is encapsulated the url and hidden from preying eyes, while travelling through the web to the server and back to you.
        // his is unlike a "HttpGet" verb where we include the information in the parenthesis and so it is not encapsulated.
        // so here the parameter we are going to include for the "Post" unlike the "Get" is going to be a custom one as seen below.
        // where we will indicate that the sender needs to send it "from the Body" (and not the url), and then this will be followed
        // by what we want added which is the UserDTO parameter which we will also need to create to interface with the ApiUsee.cs class.
        // So basically what happens in this endpoint is that when a request hits the endpoint the body will be checked for the post information, and nt the url
        // which means that if some information is passed in the url then it will be ignored and only the body will be checked.
        // So then the body must contain information with fields that match up to the UserDTO. And any information aside from this will be ignored.
        // And so this is how everything ties in concerning "sanitization of/sanitizing" requests and making decisions based on what is coming across the pipeline.
        [HttpPost]
        // now for the execution to differentiate the two [HttpPost] requests (Register and Login), we will need to provide another data anotation/decorator to identify
        // the "Route" for each request function/implementation
        // here for the register HttpPost request to hit the destinated function/implementation
        // we will indicate the route "https://api/Account/register" using the data anotation below
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            // here we will log to the file concerning an incoming Registration attempt
            _logger.LogInformation($"Registration Attempt For {userDTO.Email}");

            // then here we will check if the incoming Registration Post request is in a valid state
            if (!ModelState.IsValid)
            {
                // if not valid we will return a bad request and in it we will return the Model state
                // so that the user can see what went wrong
                return BadRequest(ModelState);
            }
            // lastly here, we will try catch for any exception caught
            // during the processing of the request in the server
            try
            {
                // so we map the incoming post request Model(in which are the required fields) contained in the "userDTO" to the "ApiUser" class
                var user = _mapper.Map<ApiUser>(userDTO);
                // and since userName and Password are a required fields by the Identity "ApiUser", we will also provide the "UserName" which should
                // be the same as the User.Email as done below
                user.UserName = userDTO.Email;

                // so now that we have the user required fields, we can go on to create the user as done below
                // Note: The UserManager.CreateAsync() method has two overloaded parameters, the first is for creating a User without a Password record. so [this only takes in a single parameter i.e. the Mapped variable "user".
                // While the other is for creating a User along with a Password.
                // where we will need to pass in the mapped userDTO model object variable "user" in the first parameter, and the "user" model password userDTO.Password in the second parameter
                // so here below we will use the second overloaded parameter.
                // this ensures that when the user is created the password gets hashed and is stored in the record.
                var result = await _userManager.CreateAsync(user, userDTO.Password);

                // we will also provide an if statement to check if the result was was sucessfully gotten
                if (!result.Succeeded)
                {
                    // So in relations to some of the error messages that we want to send back, so what we are
                    // going to do here is add all the result errors using the foreach loop
                    foreach (var error in result.Errors)
                    {
                        // So for each error in result.Errors, we will add the ModelError to the ModelState by pulling the "AddModelError()" from the "ModelState"
                        // and then in the AddModelError() method parameter, we will add the error name/key as the "error.Code" and
                        // the second parameter i.e. the error value will be the "errorDescription"
                        // now this may reveal sensitive information but we will take the risk for now.
                        ModelState.AddModelError(error.Code, error.Description);
                    }


                    // if the result is unsuccessful then we want to return a bad request for the ModelState
                    return BadRequest(ModelState);
                }

                // And then after the check for whether the entered user model is correct, we will then add the user Role to the successful user model just before returning the "202 Accepted" success response as done below
                await _userManager.AddToRolesAsync(user, userDTO.Roles);

                // otherwise if the result is okay, then we will return an "Accepted 202" response
                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"Something Went Wrong In The {nameof(Register)}");
                // then we can return a 500 error response in another way as done below
                return Problem($"Something Went Wrong In The {nameof(Register)}", statusCode: 500);
            }
        }


        // So for the Login endpoint here below we will comment this out, since we are not creating a login for a web application,
        // since we will not be providing accessibilty to the API for a period of time but only for an API call. so then we will
        // no longer be creating a Login endpoint
        // Instead we will be using a token validation check to check if the user is a registered user


        // The next endpoint will be for the Login
        // this will be pretty much the same as the Register
        // with the difference being that with login we only need the email and password details
        // and so we will need to create a separate LoginDTO class to handle this
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // here in the parenthesis below will will indicate that we want the LoginUserDTO Model fields
        // so that if any information is passed in by the user/client aside from the specified information required(also malicious information) by the
        // LoginUserDTO, then we want such information to be ignored.
        public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
        {
            // here we will log to the file concerning an incoming Login attempt
            _logger.LogInformation($"Login Attempt For {userDTO.Email}");

            // then here we will check if the incoming Login Post request is in a valid state
            if (!ModelState.IsValid)
            {
                // if not valid we will return a bad request and in it we will return the Model state
                // so that the user can see what went wrong
                return BadRequest(ModelState);
            }
            //otherwise if the request is okay, then we will try if the entered Model is in sync with a
            // valid user record in the database
            try
            {
                if (!await _authManager.ValidateUser(userDTO))
                {
                    // if there is no match then we want to return an Unauthorized response back to the user
                    return Unauthorized();
                }
                //else we will return Accepted along with a new Token object
                return Accepted(new { Token = await _authManager.CreateToken() });
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"Something Went Wrong In The {nameof(Login)}");

                return Problem($"Something Went Wrong In The {nameof(Login)}", statusCode: 500);
            }
        }


    }
}
