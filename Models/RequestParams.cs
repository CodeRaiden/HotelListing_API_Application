using System.Net.NetworkInformation;

namespace HotelListing_Api.Models
{

    //    PAGING IN C#
    //Paging is segmenting the data that is returned
    //So to set this we are going to create a new model called "RequestParams"
    // this would be a module outlining the constraints and parameters the user is allowed to pass in
    // to define the way the data will be retrieved from the database
    public class RequestParams
    {
        // first we will declare a maximum page size of 50 pages
        const int maxPageSize = 50;

        // next field here is a public field for getting and setting the Page Number
        // we will also give this field a default value of "1"
        public int PageNumber { get; set; } = 1;

        // next field is a private field of the pageSize which we will set by default to 10
        private int _PageSize = 10;

        // next field will be the public version of the pageSize
        // with a get and set field where the user can set
        // which the page size but if the page size set by the user is greater
        // than the maxPageValue, then the maxPageValue will be automatically
        // asserted as the _pageSize value.
        public int PageSize
        {
            get { return _PageSize; }
            set { _PageSize = (value > maxPageSize) ? maxPageSize : value; }
        }

        // After setting this here, the next thing we are going to do is modify our GetCountries endpoint in
        // the CountryController.cs not to look into the url query [FromQuery] for
        // the passed in parameter argument
    }
}
 