using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Responsive.Migrations
{
    /// <inheritdoc />
    public partial class testforeignname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BookCategories_BookCategoryCategoryId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_BookCategoryCategoryId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "BookCategoryCategoryId",
                table: "Books");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CategoryId",
                table: "Books",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BookCategories_CategoryId",
                table: "Books",
                column: "CategoryId",
                principalTable: "BookCategories",
                principalColumn: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BookCategories_CategoryId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_CategoryId",
                table: "Books");

            migrationBuilder.AddColumn<int>(
                name: "BookCategoryCategoryId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_BookCategoryCategoryId",
                table: "Books",
                column: "BookCategoryCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BookCategories_BookCategoryCategoryId",
                table: "Books",
                column: "BookCategoryCategoryId",
                principalTable: "BookCategories",
                principalColumn: "CategoryId");
        }
    }
}
