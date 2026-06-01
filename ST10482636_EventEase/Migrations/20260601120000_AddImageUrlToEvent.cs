using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ST10482636_EventEase.Migrations
{
    public partial class AddImageUrlToEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Event");
        }
    }
}
