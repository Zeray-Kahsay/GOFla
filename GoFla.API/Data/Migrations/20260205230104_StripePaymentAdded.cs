using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoFla.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class StripePaymentAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_Restaurants_RestaurantId",
                table: "MenuItems");

            migrationBuilder.RenameColumn(
                name: "PaymentIntentId",
                table: "orders",
                newName: "ExternalPaymentId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmedAt",
                table: "orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentExpiresAt",
                table: "orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodId",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentProvider",
                table: "orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "Carts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "CartItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CartItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Carts_RestaurantId",
                table: "Carts",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Restaurants_RestaurantId",
                table: "Carts",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_Restaurants_RestaurantId",
                table: "MenuItems",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Restaurants_RestaurantId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_Restaurants_RestaurantId",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_Carts_RestaurantId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ConfirmedAt",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "PaymentExpiresAt",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "PaymentProvider",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "ExternalPaymentId",
                table: "orders",
                newName: "PaymentIntentId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_Restaurants_RestaurantId",
                table: "MenuItems",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
