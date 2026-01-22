using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoFla.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class Updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_RestaurantId_CategoryId",
                table: "MenuItems",
                columns: new[] { "RestaurantId", "CategoryId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MenuItems_RestaurantId_CategoryId",
                table: "MenuItems");
        }
    }
}
