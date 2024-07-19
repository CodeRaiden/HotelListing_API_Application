using Microsoft.AspNetCore.Identity;

namespace HotelListing_Api.Data
{
    // the ApiUser Class will inherit from the "Microsoft.AspNetCore.Identity.EntityFrameWork" library Class "IdentityUser"
    public class ApiUser : IdentityUser
    {
        // And then here we can include the fields we will need
        // so now, this child "ApiUser" class has inherited all the fields of the parent "IdentityUser" class, and so
        // contain any other fied we specify like the ones below.

        // here we will include a field for the firstname of the user
        public string FirstName { get; set; }

        // here we will include a field for the lastname of the user

        public string LastName { get; set; }
        // Note that we can also include fields for date of birth, country of birth, area code and others, whatever
        // we require to use in order to authenticate our user.

        // Next we can go back to the DatabaseContext class and add the "ApiUser" class as the context/type of IdentityUser
        // class we want the DatabaseContext to inherit from.
    }
}
