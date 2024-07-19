using HotelListing_Api.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListing_Api.Models
{
    public class CreateHotelDTO
    {
        [Required]
        [StringLength(maximumLength: 150, ErrorMessage = "Hotel Name Is Too Long")]
        public string Name { get; set; }

        [Required]
        [StringLength(maximumLength: 250, ErrorMessage = "Hotel Address Is Too Long")]
        public string Address { get; set; }

        [Required]
        [Range(1,5)]
        public double Rating { get; set; }

        // we will make this not required so as not to require a countryid in the Hotel when adding a Hotel entry from the UpdateCountry
        // [Required]
        public int CountryId { get; set; }

        // Note that here we do not need provide a field for the foreignkey object since the user does not need to interract with it 
    }

    public class HotelDTO : CreateHotelDTO
    {
        public int Id { get; set; }

        // To include the CountryDTO object here which the user will now need to have access to, when a Hotel is created,
        // we will need to create a field to hold the object of type ContryDTO as done below
        public CountryDTO Country { get; set; }
    }

    // CONSTRUCTING PUT ENDPOINT TO UPDATE A HOTEL RECORD OR
    // CREATE THE HOTEL RECORD IF IT DOES NOT EXIST IN THE DATABASE
    // here, in order to maintain the single responsibility rule and not to have the CreateHotelDTO Type field in the UpdateHotel Method parameter,
    // we will need to create another DTO "UpdateHotelDTO" and simply make it inherit from the "CreateHotelDTO" in order to possess the same members
    public class UpdateHotelDTO : CreateHotelDTO
    {

    }

}

// Note also that for the Mapping to work Seemlessly, the DTO models field names must match the field names in the actual Data Classes
// So with all this done, now we can go on to install the AutoMapper because that's what we will use to Map the DTO Models to the actual Classes
// we will right click on the project in the Solutions folder, and then navigate to the "manage Nuget Packages" and then we will search for "AutoMapper.Extensions.Microsoft.DependencyInjection"
// as this version is optimized for .NetCore

// After Installing Automapper, we will need to Set up iur Initialization file
// So for this we will create a folder called "Configurations"
// and in the "Configurations" folder we will create a class called "MapperInitializer.cs" and then the class will inherit from the Automapper "Profile" Class

