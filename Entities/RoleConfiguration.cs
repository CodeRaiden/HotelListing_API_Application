using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing_Api.Entities
{
    // this will inherit from the "IEntityTypeConfiguration" and this will be of "IdentityRole" type/context
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        // then here we will need to provide the implementation for the "Configure()" method
        // which is going to pass over the same "builder" entity, that is in use in our DatabaseContext.cs
        // "OnModulesCreating()" method
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            // so we can actually replecate the code here to seed in our roles
            builder.HasData(
                // we will create our role objects/intances
                // we are not going to give it an id, as it will automatically add an id on it's own
                new IdentityRole
                {
                    // Role "Name"
                    Name = "User",
                    // Role "NormalizedName", which is really just the Capitalization of the "Name"
                    NormalizedName = "USER"
                },
                new IdentityRole
                {
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"
                }
                // you can add as many Roles here as we want, but this would be okay for this example
                // The most important thing now would be seeding this into the DatabaseContext.cs file
                // so then we will need to navigate to the DatabaseContext.cs file
                );

        }
    }
}
