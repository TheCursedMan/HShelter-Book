using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Responsive.Migrations
{
    /// <inheritdoc />
    public partial class clearuserlink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_AccountId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_AccountId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "CartItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_AccountId",
                table: "CartItems",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_AspNetUsers_AccountId",
                table: "CartItems",
                column: "AccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_AspNetUsers_AccountId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_AccountId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "CartItems");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Reviews",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AccountId",
                table: "Reviews",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_AccountId",
                table: "Reviews",
                column: "AccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
