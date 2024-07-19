using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelListing_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedDefaultRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "58d72a7a-2e29-4d71-afd4-adf24d63d74a", "322c8e5d-fc55-466d-a91c-977656d2823f", "Administrator", "ADMINISTRATOR" },
                    { "f346d40c-91d1-42ee-8aad-d8256ab5e2b4", "9c399449-1486-407e-8928-d625e9ff55b4", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "58d72a7a-2e29-4d71-afd4-adf24d63d74a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f346d40c-91d1-42ee-8aad-d8256ab5e2b4");
        }
    }
}
