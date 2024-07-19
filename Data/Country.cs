namespace HotelListing_Api.Data
{
    public class Country
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        // here we are going to include a way to get the list of Hotels available in a single country
        // Note that this will be needed if it is added/requested in the "includes" list parameter of the "IGenericRepository" member functions
        // which basically means that we can query the database for a Country and include the list of hotels in the Country
        public virtual IList<Hotel> Hotels { get; set; }
    }
}