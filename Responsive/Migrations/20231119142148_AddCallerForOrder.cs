using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Responsive.Migrations
{
    /// <inheritdoc />
    public partial class AddCallerForOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderCartItems",
                columns: table => new
                {
                    OrderCartItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    BookId = table.Column<int>(type: "int", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCartItems", x => x.OrderCartItemId);
                    table.ForeignKey(
                        name: "FK_OrderCartItems_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId");
                    table.ForeignKey(
                        name: "FK_OrderCartItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderCartItems_BookId",
                table: "OrderCartItems",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCartItems_OrderId",
                table: "OrderCartItems",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderCartItems");
        }
    }
}
