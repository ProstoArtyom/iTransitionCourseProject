using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddItemToComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ItemId",
                table: "Comments",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Items_ItemId",
                table: "Comments",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Items_ItemId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ItemId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Comments");
        }
    }
}
