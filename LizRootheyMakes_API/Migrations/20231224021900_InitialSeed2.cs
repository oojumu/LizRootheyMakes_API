using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LizRootheyMakesAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialSeed2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Category", "Description", "Name" },
                values: new object[] { "Hat", "Black hat with Feathers", "Black hat" });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Category", "Description", "Name" },
                values: new object[] { "Hat", "Pink hat with Feathers", "Pink hat" });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "Category", "CreatedBy", "DateCreated", "Description", "Image", "Name", "Price", "SpecialTag" },
                values: new object[] { 3, "Hat", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Purple hat with Feathers", "https://dotnetmasteryimages.images.blob.core.windows.net/redmango/idli.jpg", "Purple hat", 8.9900000000000002, "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Category", "Description", "Name" },
                values: new object[] { "Appetizer", "Fusc totenitum", "Spring Roll" });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Category", "Description", "Name" },
                values: new object[] { "Appetizer", "Fusc Votenitum vnt ollum jheut", "Idli" });
        }
    }
}
