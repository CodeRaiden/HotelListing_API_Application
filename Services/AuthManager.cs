using HotelListing_Api.Data;
using HotelListing_Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing_Api.Services
{
    public class AuthManager : IAuthManager
    {

        // Now before we start inplementing the Interface members here, we will need to make sure
        // we register our IAuthManager and AuthManager service inside the program.cs file.
        // so we will navigate to the program.cs file to register this just below the .

        // after we have gotten that out of the way,
        // Next we will need to inject two things into this Task function

        // the first is the "UserManager<ApiUser>" dependency
        private readonly UserManager<ApiUser> _userManager;
        // the second is the IConfiguration which contains our appsettings
        private readonly IConfiguration _configuration;
        // we will also create a private variable _user of type ApiUser here so we can use this for both the Claims and ValidateUser() method
        private ApiUser _user;

        // we inject the dependencies
        public AuthManager (UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // implement the CreateToken()
        public async Task<string> CreateToken()
        {

            // for the create token function we would like to do a few things to 
            // create some signin credentials, get claims, then we want to add these
            // to the token options
            // Note that we will have to explicitly create the methods for each operation and store them
            // here in the variables

            // first we get the signin credentials
            var signingCredentials = GetSigningCredentials();

            // get claims
            var claims = await GetClaims();

            // create token options by generating the token options by passing the signingCredentials
            // and claims variable as parameters
            var token = GenerateTokenOptions(signingCredentials, claims);

            // and in the end we want to return a new instance of the JWTSecurityTokenHandler and chain this
            // class instance with it's inbuilt "WriteToken()" method and we will pass in the "tokenOptions"
            // to it.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // creating the private GetSigningCredentials() method
        private SigningCredentials GetSigningCredentials()
        {
            // first we will get the JWT KEY
            //var key = _configuration.GetSection("Jwt:Key").Value;
            var key = _configuration.GetSection("Jwt:Key").Value;

            // then we encode the gotten key value and store it in a variiable "secret"
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            // then we will return the new Signing Credentials which will include the security
            // key as well as the security algorithm used for this, which is "HmacSha256"
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        // creating the private Task of List of type Claims "GetClaims()" method
        private async Task<List<Claim>> GetClaims()
        {
            // claims are the bits and pieces of information that really tells who the user claims to be, or what
            // the user claims he can do. so these are the things we want to make sure are included in our Token

            // the claim for the name
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.UserName)
            };

            // the claim for the user roles
            var roles = await _userManager.GetRolesAsync(_user);

            // we will then use a foreach loop to loop through the gotten list of roles and then
            // we include each one in the claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // again depending on what you want, you can add as many claims as needed here to the list of Claims.

            //lastly here we will return the claims
            return claims;
        }

        // creating the private GenerateTokenOptions() method
        // here the return type will be of "JWTSecurityToken"
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            // so here to generate the token options, we will have to get the jwt settings from the appsettigs.json
            var jwtSettings = _configuration.GetSection("Jwt");
            // expiraton should be of type "DataTime"
            // then we will set the expiration date, but for this we will modify the jwt settings in the appsettings.json file
            // and then we will include the "lifetime" key here.
            // and since the value here is return a double, then we will need to cast/convert this to an Int 
            var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("Jwt:lifetime").Value)); 
            // then we will need to create an instance/object for the JwtSecurityToken and it's passed in fields
            // and store this in a variable "token"
            var token = new JwtSecurityToken(
                // we will set the issuer as the jwtsetting's defind "ValidIssuer" value
                issuer: jwtSettings.GetSection("validIssuer").Value,
                // we will set the claims to the defined claims
                claims: claims,
                // then we will set the expires to the defined expiration variable
                expires: expiration,
                // then the signingCredentials as the passed in signingCredentials
                signingCredentials: signingCredentials
                );

            // And then finally we will return the token
            return token;
        }

        public async Task<bool> ValidateUser(LoginUserDTO userDTO)
        {
            // we will check if we have the user by waitng for the user to be found
            // in the database where we are getti ng the user by the user name
            // which in the record we set to be the same as the userEmail
            _user = await _userManager.FindByNameAsync(userDTO.Email);

            // so if here we will check to return a bool
            // if the user is not null and the checked user password matches a user password stored in the User database, then this should return true, else false
            return (_user != null && await _userManager.CheckPasswordAsync(_user, userDTO.Password));
        }
    }
}
