using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bilby.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPostedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "posted_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "posted_at",
                table: "posts");
        }
    }
}
