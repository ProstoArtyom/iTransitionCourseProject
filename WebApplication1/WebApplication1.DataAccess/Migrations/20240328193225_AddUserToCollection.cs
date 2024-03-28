using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Collections",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Collections",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_ApplicationUserId",
                table: "Collections",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Collections_AspNetUsers_ApplicationUserId",
                table: "Collections",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collections_AspNetUsers_ApplicationUserId",
                table: "Collections");

            migrationBuilder.DropIndex(
                name: "IX_Collections_ApplicationUserId",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Collections");

            migrationBuilder.InsertData(
                table: "Collections",
                columns: new[] { "Id", "Description", "Name", "ThemeId" },
                values: new object[] { 1, "It's a description for collection №1", "Collection №1", 2 });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Cool_Collection" });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "CollectionId", "CustomFields", "Name" },
                values: new object[] { 1, 1, "{\"Book\":\"Librarian of Basra\",\"Author\":\"Jeanette Winter\",\"Published\":\"1939-12-04\"}", "Book 1" });
        }
    }
}
