using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateItemTbCustomFieldsNotRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CustomFields",
                table: "Items",
                type: "json",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "json")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "CustomFields",
                keyValue: null,
                column: "CustomFields",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "CustomFields",
                table: "Items",
                type: "json",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "json",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
