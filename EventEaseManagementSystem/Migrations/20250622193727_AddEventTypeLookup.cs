using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEaseManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddEventTypeLookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Event");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Venue",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    EventTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.EventTypeID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventTypeID",
                table: "Event",
                column: "EventTypeID");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_EventType",
                table: "Event");

            migrationBuilder.DropTable(
                name: "EventTypes");

            migrationBuilder.DropIndex(
                name: "IX_Event_EventTypeID",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Venue");

            migrationBuilder.DropColumn(
                name: "EventTypeID",
                table: "Event");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
