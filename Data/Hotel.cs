using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListing_Api.Data
{
    public class Hotel
    {
        // the id field is automatically recognized as the primary key
        // which is a unique identitfier for any given table
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public double Rating { get; set; }

        // the foreign key represets a strong reference to another table
        // in the ForeignKey field we will define two fields the first is the int "CountryId" field
        // which will be the strong reference field where we will need to pass in the Id of the country
        // we are looking for inorder to find a country in the database with the matching id.
        // And the second is the "Country" field of type "Country", which will return
        // the Country object.

        // Now To define a foreign key of type "Country" to link the Country
        // table/class here in the Hotel class, we will first include the
        // data anotation block for ForeignKey as done below to describe the
        // field "Country" as the foreign key. and in the data anotation block,
        // we will include the custom name of the ForeignKey or like
        // in this case "nameof(Country)" we are giving the foreign key the same
        // name with the type(class)/table it is referrencing
        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        // we can then go on to build this so we can test it out

    }
}
