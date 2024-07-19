using HotelListing_Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing_Api.Entities
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
                 new Hotel
                 {
                     // Jamaica
                     Id = 1,
                     Name = "Sandals Resort and Spa",
                     Address = "Negril",
                     CountryId = 1,
                     Rating = 4.3,
                 },
                new Hotel
                {
                    // Caymen Islands
                    Id = 2,
                    Name = "Comfort Suits",
                    Address = "George Town",
                    CountryId = 3,
                    Rating = 4.5,
                },
                new Hotel
                {
                    // Bahamas
                    Id = 3,
                    Name = "Grand Palladium",
                    Address = "Nassua",
                    CountryId = 2,
                    Rating = 4,
                }
                );
        }
    }
}
