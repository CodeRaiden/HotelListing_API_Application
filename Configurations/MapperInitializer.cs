using AutoMapper;
using HotelListing_Api.Data;
using HotelListing_Api.Models;

namespace HotelListing_Api.Configurations
{
    // the MapperInitializer Class will inherit from the Automapper "Profile" Class
    public class MapperInitializer : Profile
    {
        // first thing to do here is to create a constructor which will hold all the mappings to be done
        public MapperInitializer()
        {
            // here first we are going to create a Map that states that the Domain class "Country" is going to Map directly to "CountryDTO"
            // and we will chain this also with the "ReverseMap()" functionality, which allows for the "CountryDTO" to also Map to the Domain class "Country"
            CreateMap<Country, CountryDTO>().ReverseMap();
            CreateMap<Country, CreateCountryDTO>().ReverseMap();
            CreateMap<Hotel, HotelDTO>().ReverseMap();
            CreateMap<Hotel, CreateHotelDTO>().ReverseMap();
            CreateMap<ApiUser, UserDTO>().ReverseMap();
        }
    }
}
// So now that we have the Mapper Initialization set up, we need to go on and include it in the Program.cs file (just beneath the "builder.Services.AddCors" code block) for it to work