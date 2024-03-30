using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddImageToCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageStorageName",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "ImageStorageName",
                table: "Collections",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Collections",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageStorageName",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Collections");

            migrationBuilder.AddColumn<string>(
                name: "ImageStorageName",
                table: "Items",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
