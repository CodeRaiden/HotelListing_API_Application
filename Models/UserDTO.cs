using System.ComponentModel.DataAnnotations;

namespace HotelListing_Api.Models
{
    public class LoginUserDTO
    {
        // we only need the Email address
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        // and the Password for the Login
        [Required]
        [StringLength(15, ErrorMessage = "Your Password Is Limited To {2} to {1} Characters", MinimumLength = 7)]
        public string Password { get; set; }

        // and then after implementing the loginDTO here, we can make the UserDTO below inherit from it
        // since teh UserDTO also contains the two fields and more
    }

    public class UserDTO : LoginUserDTO
    {
        // this will contain optional FirstName and LastName fields
        public string FirstName { get; set; }

        public string LastName { get; set; }

        // optional aslo
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        // we will include the get and set field of ICollection of type "<string>" called "Roles"
        // so we can give the user the opportunity to select which role or roles to have.
        public ICollection<string> Roles { get; set; }

    }
}

// Now we need to make sure that the MapperInitializer.cs file also knows about the UerDTO
