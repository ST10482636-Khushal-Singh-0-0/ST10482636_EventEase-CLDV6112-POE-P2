using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ST10482636_EventEase.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToBookingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Booking",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Booking");
        }
    }
}
