using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEaseManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddEventTypeSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EventTypes", // <--- ***CRITICAL CHANGE: USE "EventTypes" (PLURAL) HERE***
                columns: new[] { "EventTypeID", "Name" },
                values: new object[,]
                {
                    { 1, "Wedding" },
                    { 2, "Conference" },
                    { 3, "Concert" },
                    { 4, "Seminar" },
                    { 5, "Workshop" },
                    { 6, "Charity" },
                    { 7, "Expos" },
                    { 8, "Fair" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventTypes", // <--- ***CRITICAL CHANGE: USE "EventTypes" (PLURAL) HERE***
                keyColumn: "EventTypeID",
                keyValues: new object[] { 1 });
            migrationBuilder.DeleteData(
                table: "EventTypes", // <--- And for all other DeleteData entries
                keyColumn: "EventTypeId",
                keyValues: new object[] { 2 });
            // ... continue for 3 through 8
            migrationBuilder.DeleteData(
                table: "EventTypes", // <--- And the last one
                keyColumn: "EventTypeId",
                keyValues: new object[] { 8 });
        }
    }
}