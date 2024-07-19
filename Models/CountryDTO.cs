using System.ComponentModel.DataAnnotations;

//namespace HotelListing_Api.Models
//{
//    // here we include the model for the Country class i.e what the ountry should look like
//    public class CountryDTO
//    {
//        // Normally the DTO models will have the axact same content as the
//        // actual class it will be interracting with.
//        // with the difference being that here in the DTO Model, we can add Validations
//        // as done below.
//        public int Id { get; set; }

//        // we can validate the name field to be a "required" field by including the data anotation "[Required]"
//        // as done below
//        [Required]
//        // we can also include other validations like [Stringlength] which we can use to validate/set the maximum and minimum lenght
//        [StringLength(maximumLength: 50, ErrorMessage = "Country Name Is Too Long")]
//        // So then this basically means that only when the user has successfully met the set validation requirements, will the information then be mapped to the actual Data "Country" class
//        // So the principle here is that the user will never interract with the Data Class directly, and the remote Database will never interract with the class DTO Model directly
//        public string Name { get; set; }

//        [Required]
//        [StringLength(maximumLength: 3, ErrorMessage = "Country ShortName Is Too Long")]
//        public string ShortName { get; set; }
//    }
    
//    // here we will include another class for when Creating a Country
//    public class CreateCountryDTO
//    {
//        // When creating a Country entry we will not need to include the id field as this will be generated automatically

//        // we can validate the name field to be a "required" field by including the data anotation "[Required]"
//        // as done below
//        [Required]
//        // we can also include other validations like [Stringlength] which we can use to validate/set the maximum and minimum lenght
//        [StringLength(maximumLength: 50, ErrorMessage = "Country Name Is Too Long")]
//        // So then this basically means that only when the user has successfully met the set validation requirements, will the information then be mapped to the actual Data "Country" class
//        // So the principle here is that the user will never interract with the Data Class directly, and the remote Database will never interract with the class DTO Model directly
//        public string Name { get; set; }

//        [Required]
//        [StringLength(maximumLength: 3, ErrorMessage = "Country ShortName Is Too Long")]
//        public string ShortName { get; set; }
//    }
//}

// But Note there are a lot of repeatitions in this file and we will like to avoid that
// So the best way to avoid this will be to refactor it as done below 
namespace HotelListing_Api.Models
{
    // here we include the model for the Country class i.e what the ountry should look like
    public class CreateCountryDTO
    {
        // When creating a Country entry we will not need to include the id field as this will be generated automatically

        // we can validate the name field to be a "required" field by including the data anotation "[Required]"
        // as done below
        [Required]
        // we can also include other validations like [Stringlength] which we can use to validate/set the maximum and minimum lenght
        [StringLength(maximumLength: 50, ErrorMessage = "Country Name Is Too Long")]
        // So then this basically means that only when the user has successfully met the set validation requirements, will the information then be mapped to the actual Data "Country" class
        // So the principle here is that the user will never interract with the Data Class directly, and the remote Database will never interract with the class DTO Model directly
        public string Name { get; set; }

        [Required]
        [StringLength(maximumLength: 3, ErrorMessage = "Country ShortName Is Too Long")]
        public string ShortName { get; set; }
    }

    // CONSTRUCTING PUT ENDPOINT TO UPDATE A COUNTRY RECORD OR
    // CREATE THE COUNTRY RECORD IF IT DOES NOT EXIST IN THE DATABASE
    // here, in order to maintain the single responsibility rule and not to have the CreateCountryDTO Type field in the UpdateCountry Method parameter,
    // we will need to create another DTO "UpdateCountryDTO" and simply make it inherit from the "CreateCountryDTO" in order to possess the same members
    public class UpdateCountryDTO : CreateCountryDTO
    {
        // here we will also include a field of IList of Type <CreateHotelDTO> instaed of <HotelDTO> (this is because with HotelDTO we will need to include the id, while with CreateHotelDTO we do not as it is added automatically), to enable us also update a list of hotels
        // along with the country
        public IList<CreateHotelDTO> Hotels { get; set; }
    }

    // here we since both are actually similar, with the difference being that the CountryDTO includes the "id" field,
    // then we can simply just make the CountryDTO inherit from the CreateCountryDTO
    public class CountryDTO : CreateCountryDTO
    {
        public int Id { get; set; }

        // since we only need to include the Hotels in the includes List when querying the database and not when creating the
        // country, then we will need to place the code for holding all the list of Hotels in the Country here in the CountryDTO
        // and also the field here does not need to be virtual as done in the Country.cs file
        public IList<HotelDTO> Hotels { get; set; }
    }

    
}

// We can now go ahaed and do the same for the HotelDTO
