using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Responsive.Migrations
{
    /// <inheritdoc />
    public partial class addDiscountOnBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "DiscountPer",
                table: "Books",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscount",
                table: "Books",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPer",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "IsDiscount",
                table: "Books");
        }
    }
}
