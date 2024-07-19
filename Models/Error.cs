using Newtonsoft.Json;

namespace HotelListing_Api.Models
{
    public class Error
    {
        // here in the Error class we will have three fields
        // first is the status code
        public string? StatusCode { get; set; }
        // next is the error message 
        public string? Message { get; set; }
        // next we will have a field that returns the message as a json object
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}

// next we will need to navigate to the ServiceExtension.cs file to configure our ServiceExtension error pipeline