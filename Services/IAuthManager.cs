using HotelListing_Api.Models;

namespace HotelListing_Api.Services
{
    public interface IAuthManager
    {
        // here we want to have a Task that returns a <bool>, and we will call it ValidateUser
        // the Task parameter will take an argument of type LoginUserDTO
        Task<bool> ValidateUser(LoginUserDTO userDTO);

        // we will also need another Task that returns a <string> to create the Token after the user is validated
        Task<string> CreateToken();
    }
}

// Now we can go on to create the concrete class "AuthManager" to provide implementation for the "IAuthManager"