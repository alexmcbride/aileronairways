using Microsoft.EntityFrameworkCore.Migrations;

namespace AileronAirwaysWeb.Data.Migrations
{
    public partial class AddedEventsCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventsCount",
                table: "Timelines",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventsCount",
                table: "Timelines");
        }
    }
}
